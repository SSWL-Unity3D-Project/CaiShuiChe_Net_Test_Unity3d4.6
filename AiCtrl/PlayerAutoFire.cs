using UnityEngine;
using System.Collections;

public class PlayerAutoFire : MonoBehaviour
{
	public GameObject bulletPrefab;
	public Transform spawnPoint;
	static public float frequency = 10f;
	
	public LayerMask TerrainLayer;
	public LayerMask AmmoHitLayer;

	public BoxCollider PlayerBoxCollider;
	public GameObject WaterParticle;
	public int PathKeyState;
	bool firing = false;
	float forcePerSecond = 20.0f;
	float lastFireTime = -1;
	
	Transform PlayerTran;
	bool IsMoveingWater = true;
	float TimeMoveOutWater;
	float TimeOutWaterVal;
	bool IsBackPlayer;
	bool IsRecordTimeMoveOut;

	public static float SubPlayerEnginePower = 0.3f;
	public static float DisSpeedVal = 20f;
	public static float PlayerMvSpeed;
	NetworkView NetView;

	Transform PlayerPreMark;
	float MaxDirCosAB;

	public static float HuanYingSpeed = 150f;
	bool IsAddGravity;

	Rigidbody RigObj;
	public static bool IsRestartMove;
	public static float PlayerRestartTime;
	public static bool IsIntoSky;
	public static float GravityValMax = -30f;
	
	public static float MinPowerPlayer = 900f;
	public static float PlayerThrottleForce;
	public static float PlayerCurrentEnginePower;

	AudioSource AudioSourceShipMove;
	bool IsInitPlayerAutoFire;
	ZhuJiaoNv ZhuJiaoNvScript;

	public static bool IsTurnLeft;
	public static bool IsTurnRight;
	public static bool IsActivePlayerForwardHit;

	bool FireLeft;
	bool FireRight;
	float CenterPerPx;
	float TimeCheckForwardVal;
	
	public static int PlayerShootNpcNum = 0;
	public static int PlayerHitZhangAiNum = 0;
	public static bool IsPlayerFire = false;

	void Start()
	{
		if (WaterwheelPlayerCtrl.IsTestShootCartoon) {
			return;
		}
		IsPlayerFire = false;
		IsActivePlayerForwardHit = false;
		InitPlayerAutoFire();
	}

	void InitPlayerAutoFire()
	{
		if (IsInitPlayerAutoFire) {
			return;
		}
		IsInitPlayerAutoFire = true;
		CenterPerPx = GameCtrlXK.GetInstance().ZhuJiaoNvCenterPerPx;
		NetView = GetComponent<NetworkView>();
		PlayerTran = transform;
		RigObj = rigidbody;
		IsIntoSky = false;
		ResetIsTurn();

		SetMaxDirCosAB();
		if (spawnPoint == null)
		{
			spawnPoint = transform;
		}
		CloseWaterParticle();
		SetPlayerBoxColliderState(false);
		ZhuJiaoNvScript = GetComponentInChildren<ZhuJiaoNv>();
		
		InputEventCtrl.GetInstance().ClickFireBtEvent += ClickFireBtEvent;
	}

	void Update()
	{
		if (WaterwheelPlayerCtrl.IsTestShootCartoon) {
			return;
		}
		CheckPlayerForwardHit();
		CheckPlayerForwardVal();
		AddPlayerMoveSpeed();
		if (firing) {
			if (!StartBtCtrl.GetInstanceP2().CheckIsActivePlayer()) {
				OnStopFire();
				return;
			}

			if(Time.time > lastFireTime + 1 / frequency)
			{
				if (Network.peerType != NetworkPeerType.Disconnected) {

					int PosX_1 = (int)(608f * (1f - CenterPerPx));
					int PosX_2 = (int)(608f * (1f + CenterPerPx));
					float crossPx = pcvr.CrossPosition.x;
					if (crossPx < PosX_1) {
						if (!FireLeft) {
							FireLeft = true;
							FireRight = false;
							NetView.RPC("SendUpdateZhuJiaoNvFireAction", RPCMode.OthersBuffered, pcvr.CrossPosition.x);
						}
					}
					else if (crossPx > PosX_2) {
						if (!FireRight) {
							FireLeft = false;
							FireRight = true;
							NetView.RPC("SendUpdateZhuJiaoNvFireAction", RPCMode.OthersBuffered, pcvr.CrossPosition.x);
						}
					}
					else {
						if (FireRight || FireLeft) {
							FireLeft = false;
							FireRight = false;
							NetView.RPC("SendUpdateZhuJiaoNvFireAction", RPCMode.OthersBuffered, pcvr.CrossPosition.x);
						}
					}
				}

				// Spawn visual bullet
				Vector3 mousePosInput = Input.mousePosition;
				if (pcvr.bIsHardWare) {
					mousePosInput = pcvr.CrossPosition;
				}

				Vector3 mousePos = mousePosInput + Vector3.forward * 30f;
				Vector3 posTmp = Camera.main.ScreenToWorldPoint(mousePos);
				Vector3 AmmoSpawnPos = spawnPoint.position;
				Vector3 AmmoForward = Vector3.Normalize( posTmp - AmmoSpawnPos );
				Ray ray = Camera.main.ScreenPointToRay(mousePosInput);
				RaycastHit hit;
				if(Physics.Raycast(ray, out hit, 500.0f, TerrainLayer.value))
				{
					AmmoForward = Vector3.Normalize( hit.point - AmmoSpawnPos );
				}

				Vector3 forwardVal = AmmoForward;
				GameObject go = null;
				if (GlobalData.GetInstance().IsActiveJuLiFu) {
					go = (GameObject)Spawner.Spawn(bulletPrefab, AmmoSpawnPos, forwardVal);
					pcvr.ShuiBengState = PcvrShuiBengState.Level_1;
				}
				else {
					go = (GameObject)Spawner.Spawn(bulletPrefab, AmmoSpawnPos, forwardVal);
					pcvr.ShuiBengState = PcvrShuiBengState.Level_1;
				}
				PlayerSimpleBullet bullet = go.GetComponent<PlayerSimpleBullet> ();
				bullet.SetIsHandleBullet();
				lastFireTime = Time.time;
				
				// Find the object hit by the raycast
				RaycastHit hitInfo = GetHitInfo(AmmoSpawnPos, forwardVal);
				if(hitInfo.transform != null)
				{
					// Get the health component of the target if any
					NpcHealthCtrl targetHealth = hitInfo.transform.GetComponent<NpcHealthCtrl>();
					if (hitInfo.distance < 500f) {
						if (targetHealth) {
							// Apply damage
							if (!GlobalData.GetInstance().IsActiveJuLiFu) {
								targetHealth.OnDamage(2f / frequency);
							}
							else {
								targetHealth.OnDamage(1f / frequency);
							}
						}
					}
					bullet.dist = hitInfo.distance;

					// Get the rigidbody if any
					if (hitInfo.rigidbody != null && !hitInfo.rigidbody.isKinematic
					    && hitInfo.rigidbody.useGravity
					    && targetHealth != null)
					{
						// Apply force to the target object at the position of the hit point
						Vector3 force = PlayerTran.forward * (forcePerSecond / frequency);
						hitInfo.rigidbody.AddForceAtPosition(force, hitInfo.point, ForceMode.Impulse);
						bullet.dist = hitInfo.distance;
					}
				}
				else
				{
					bullet.dist = 1000;
				}
			}
		}
	}

	void CheckPlayerForwardVal()
	{
		if (PlayerTran == null) {
			return;
		}

		if (Mathf.Abs( PlayerTran.forward.y ) < 0.8f) {
			TimeCheckForwardVal = Time.realtimeSinceStartup;
			return;
		}
		
		if (Time.realtimeSinceStartup - TimeCheckForwardVal > 3f) {
			if (GlobalData.GetInstance().gameMode == GameMode.OnlineMode) {
				WaterwheelPlayerNetCtrl.GetInstance().ResetPlayerPos();
			}
			else {
				WaterwheelPlayerCtrl.GetInstance().ResetPlayerPos();
			}
		}
	}

	public void ResetPlayerCameraPos()
	{
		//Debug.Log("ResetPlayerCameraPos*********");
		IsRestartMove = true;
		PlayerRestartTime = Time.realtimeSinceStartup;
		PlayerMvSpeed = 0f;
		Time.timeScale = 1.0f;

		if (GlobalData.GetInstance().gameMode == GameMode.SoloMode) {
			WaterwheelCameraCtrl.GetInstance().ResetPlayerCameraPos();
		}
		else {
			WaterwheelCameraNetCtrl.GetInstance().ResetPlayerCameraPos();
		}
		CameraShake.IsActiveCamOtherPoint = false;
		IntoPuBuCtrl.IsIntoPuBu = false;
		GameCtrlXK.GetInstance().DelayClosePlayerBoxCollider();
	}

	void SetMaxDirCosAB()
	{
		MaxDirCosAB = Mathf.Cos((155f / 180f) * Mathf.PI);
	}

	public void SetPlayerPreMark(Transform tranVal)
	{
		PlayerPreMark = tranVal;
	}

	public static void ResetIsIntoPuBu()
	{
		if (!IntoPuBuCtrl.IsIntoPuBu) {
			return;
		}
		IntoPuBuCtrl.IsIntoPuBu = false;
		PlayerAutoFire.ResetIsRestartMove();
		
		GameCtrlXK.GetInstance().InitDelayClosePlayerBoxCollider();
		CameraShake.GetInstance().SetRadialBlurActive(false, CameraShake.BlurStrengthPubu);
		CameraShake.GetInstance().SetActiveCamOtherPoint(false, CamDirPos.FIRST, null);
	}

	public bool CheckPlayerIsMoveDirWrong()
	{
		if (PlayerPreMark == null) {
			return false;
		}

		Vector3 vecA = PlayerPreMark.forward;
		Vector3 vecB = PlayerTran.forward;
		vecA.y = vecB.y = 0f;

		float cosAB = Vector3.Dot( vecA.normalized, vecB.normalized );
		if (cosAB < MaxDirCosAB) {
			return true;
		}
		return false;
	}

	public void SetPathKeyState(AiPathCtrl pathScript){
		PathKeyState = pathScript.KeyState;
	}

	public void CloseWaterParticle()
	{
		WaterParticle.SetActive(false);
		if (Network.peerType != NetworkPeerType.Disconnected) {
			NetView.RPC("SendCloseWaterParticle", RPCMode.OthersBuffered);
		}
	}

	[RPC]
	void SendCloseWaterParticle()
	{
		if (!WaterParticle.activeSelf) {
			return;
		}
		WaterParticle.SetActive(false);
	}

	public void ActiveWaterParticle()
	{
		WaterParticle.SetActive(true);
		if (Network.peerType != NetworkPeerType.Disconnected) {
			NetView.RPC("SendActiveWaterParticle", RPCMode.OthersBuffered);
		}
	}

	[RPC]
	void SendActiveWaterParticle()
	{
		if (WaterParticle.activeSelf) {
			return;
		}
		WaterParticle.SetActive(true);
	}

	void CreateAudioSourceShipMove()
	{
		AudioSourceShipMove = gameObject.AddComponent<AudioSource>();
		AudioSourceShipMove.playOnAwake = false;
		AudioSourceShipMove.Stop();

		AudioSourceShipMove.clip = AudioListCtrl.GetInstance().AudioShipMove;
		AudioSourceShipMove.loop = true;
	}

	public static void ActiveIsTurnLeft()
	{
		IsTurnLeft = true;
		IsTurnRight = false;
	}

	public static void ActiveIsTurnRight()
	{
		IsTurnLeft = false;
		IsTurnRight = true;
	}

	public static void ResetIsTurn()
	{
		IsTurnLeft = false;
		IsTurnRight = false;
	}

	public void SetPlayerMvSpeed(float val)
	{
		if (AudioSourceShipMove == null) {
			CreateAudioSourceShipMove();
		}

		if (!StartBtCtrl.GetInstanceP1().CheckIsActivePlayer()) {
			val = 0f;
		}
		PlayerMvSpeed = val;

		//CameraShake.GetInstance().SetPlayerSpeedRadialBlur(val);
		if (val > 1f) {
			if (!AudioSourceShipMove.isPlaying) {
				AudioSourceShipMove.Play();
			}
			else {
				AudioSourceShipMove.volume = val / 80f;
			}
		}
		else {
			AudioSourceShipMove.Stop();
		}

		if (val <= 5 && WaterParticle.activeSelf) {
			CloseWaterParticle();
		}
	}

	public static void ResetIsRestartMove()
	{
		IsRestartMove = true;
		PlayerRestartTime = Time.realtimeSinceStartup;
	}

	void AddPlayerMoveSpeed()
	{
		if (IsRestartMove && MoveCameraByPath.IsMovePlayer) {
			if (Time.realtimeSinceStartup - PlayerRestartTime <= 2f) {
				if (RigObj.isKinematic) {
					RigObj.isKinematic = false;
				}
				RigObj.AddForce(PlayerTran.forward * PlayerThrottleForce * Time.deltaTime);
			}
			else {
				IsRestartMove = false;
				if (GlobalData.GetInstance().gameMode == GameMode.SoloMode) {
					WaterwheelPlayerCtrl.GetInstance().SetcurrentEnginePower(PlayerCurrentEnginePower);
				}
				else {
					WaterwheelPlayerNetCtrl.GetInstance().SetcurrentEnginePower(PlayerCurrentEnginePower);
				}
				PlayerThrottleForce = 0f;
				PlayerCurrentEnginePower = 0f;
			}
		}
	}

	public static void HandlePlayerOutPubuEvent()
	{
		float power = 2000f;
		if (GlobalData.GetInstance().gameMode == GameMode.SoloMode) {
			WaterwheelPlayerCtrl.GetInstance().SetcurrentEnginePower(power);
		}
		else {
			WaterwheelPlayerNetCtrl.GetInstance().SetcurrentEnginePower(power);
		}
	}

	public static void HandlePlayerHitShuiLei()
	{
		float power = 50f;
		if (GlobalData.GetInstance().gameMode == GameMode.SoloMode) {
			WaterwheelPlayerCtrl.GetInstance().SetcurrentEnginePower(power);
		}
		else {
			WaterwheelPlayerNetCtrl.GetInstance().SetcurrentEnginePower(power);
		}
	}

	public static void HandlePlayerCloseShenXingState()
	{
		if (GlobalData.GetInstance().gameMode == GameMode.SoloMode) {
			WaterwheelPlayerCtrl.GetInstance().ShouldCloseShenXingState();
		}
		else {
			WaterwheelPlayerNetCtrl.GetInstance().ShouldCloseShenXingState();
		}
	}

	public static void HandlePlayerCloseHuanYingFu()
	{
		float power = 0f;
		if (GlobalData.GetInstance().gameMode == GameMode.SoloMode) {
			WaterwheelPlayerCtrl.GetInstance().SetcurrentEnginePower(power);
		}
		else {
			WaterwheelPlayerNetCtrl.GetInstance().SetcurrentEnginePower(power);
		}
	}

	public bool CheckPlayerDownIsHit()
	{
		bool isHitA = false;
		bool isHitB = false;
		RaycastHit hitInfo;
		Vector3 vecPosA = Vector3.zero;
		Vector3 vecPosB = Vector3.zero;
		
		Vector3 startPos = PlayerTran.position + Vector3.up * 2f + PlayerTran.forward;
		Vector3 forwardVal = Vector3.down;
		Physics.Raycast(startPos, forwardVal, out hitInfo, 5f, GameCtrlXK.GetInstance().PlayerVertHitLayer.value);
		if (hitInfo.collider != null && hitInfo.collider.GetComponent<CamNotCheckHitObj>() == null) {
			isHitA = true;
			vecPosA = hitInfo.point;
		}
		
		startPos = PlayerTran.position + Vector3.up * 2f - PlayerTran.forward;
		Physics.Raycast(startPos, forwardVal, out hitInfo, 5f, GameCtrlXK.GetInstance().PlayerVertHitLayer.value);
		if (hitInfo.collider != null && hitInfo.collider.GetComponent<CamNotCheckHitObj>() == null) {
			vecPosB = hitInfo.point;
			isHitB = true;

			if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer(GameCtrlXK.WaterLayer)) {
				if (PlayerMvSpeed > 5f) {
					ActiveWaterParticle();
				}
				else {
					CloseWaterParticle();
				}
			}
			else {
				CloseWaterParticle();
			}
		}
		else {
			CloseWaterParticle();
		}

		if (!isHitA || !isHitB) {
			IsIntoSky = true;
		}

		if (isHitA && isHitB) {
			Vector3 vecFor = vecPosA - vecPosB;
			//PlayerTran.forward = vecFor.normalized;
			PlayerTran.forward = Vector3.Slerp(PlayerTran.forward, vecFor.normalized, 0.1f);
			
			if (IsAddGravity) {
				ChangePhysicsGravity(false);
			}
			IsIntoSky = false;
			return true;
		}
		else if (IntoPuBuCtrl.IsIntoPuBu) {
			float valTime = Time.timeScale != 0f ? 0.1f / Time.timeScale : 1f;
			
			forwardVal = PlayerTran.forward;
			Vector3 forwardEndVal = forwardVal;
			forwardEndVal.y = IntoPuBuCtrl.PlayerForward.y;
			Vector3 pos = PlayerTran.position;
			float dy = 0.1f * Time.deltaTime * IntoPuBuCtrl.MvCount;
			IntoPuBuCtrl.MvCount++;
			pos.y -= dy;
			PlayerTran.position = pos;
			PlayerTran.forward = Vector3.Slerp(forwardVal, forwardEndVal, valTime);

			if (!IsAddGravity) {
				ChangePhysicsGravity(true);
			}
			return true;
		}
		else if (CameraShake.IsActiveCamOtherPoint) {
			float valTime = Time.timeScale != 0f ? 0.1f / Time.timeScale : 1f;
			
			forwardVal = PlayerTran.forward;
			Vector3 forwardEndVal = forwardVal;
			forwardEndVal.y = TriggerIntoChangeCam.PlayerForward.y;
			PlayerTran.forward = Vector3.Slerp(forwardVal, forwardEndVal, valTime);
			
			if (!IsAddGravity) {
				ChangePhysicsGravity(true);
			}
			return true;
		}

		if (!IsAddGravity) {
			ChangePhysicsGravity(true);
		}
		return false;
	}

	void ChangePhysicsGravity(bool isAdd)
	{
		if (IsAddGravity == isAdd) {
			return;
		}
		IsAddGravity = isAdd;

		if (IsAddGravity) {
			Physics.gravity = new Vector3(0f, GravityValMax, 0f);
		}
		else {
			Physics.gravity = new Vector3(0f, -9.81f, 0f);
		}
	}

	public bool CheckIsBackPlayerOutWater()
	{
		IsBackPlayer = false;
		CheckPlayerVerticalHit();
		return IsBackPlayer;
	}

	void CheckPlayerVerticalHit()
	{
		bool isMoveInWater = false;

		RaycastHit hitInfo;
		Vector3 startPos = PlayerTran.position + Vector3.up * 5f;
		Vector3 forwardVal = Vector3.down;
		Physics.Raycast(startPos, forwardVal, out hitInfo, 30f, GameCtrlXK.GetInstance().PlayerVertHitLayer.value);
		if (hitInfo.collider != null) {
			//Debug.DrawLine(startPos, hitInfo.point, Color.red);
			GameObject objHit = hitInfo.collider.gameObject;
			if (objHit.layer == LayerMask.NameToLayer(GameCtrlXK.WaterLayer)) {
				isMoveInWater = true;
			}
		}

		if (IsMoveingWater != isMoveInWater){
			IsMoveingWater = isMoveInWater;
			if (!IsMoveingWater) {
				IsRecordTimeMoveOut = false;
				TimeOutWaterVal = Time.realtimeSinceStartup;
				TimeMoveOutWater = Time.realtimeSinceStartup;
			}
		}
		
		bool isCloseYueJie = false;
		if (!IsMoveingWater) {
			if (Time.realtimeSinceStartup - TimeOutWaterVal > 0.3f && !IsRecordTimeMoveOut) {
				IsRecordTimeMoveOut = true;
				TimeMoveOutWater = Time.realtimeSinceStartup;
				PlayerYueJieCtrl.GetInstance().ShowPlayerYueJie(); //Show yueJieInfo
			}

			if (Time.realtimeSinceStartup - TimeMoveOutWater > 2f) {
				TimeMoveOutWater = Time.realtimeSinceStartup; //back player, close yueJieInfo
				IsBackPlayer = true;
				isCloseYueJie = true;
				IsMoveingWater = true;
			}
		}
		else {
			if (Time.realtimeSinceStartup - TimeMoveOutWater > 0.3f) {
				isCloseYueJie = true;
			}
		}
		
		if (isCloseYueJie) {
			PlayerYueJieCtrl.GetInstance().ClosePlayerYueJie(); //close yueJieInfo
		}
	}

	public RaycastHit GetHitInfo(Vector3 startPos, Vector3 forwardVal)
	{
		RaycastHit hitInfo;
		Physics.Raycast(startPos, forwardVal, out hitInfo, 500f, AmmoHitLayer.value);
		return hitInfo;
	}

	void ClickFireBtEvent(ButtonState state)
	{
		if(this == null)
		{
			InputEventCtrl.GetInstance().ClickFireBtEvent -= ClickFireBtEvent;
			//Debug.LogWarning("ClickFireBtEvent -> PlayerAutoFire is null");
			return;
		}
		
		if (GlobalData.GetInstance().gameMode == GameMode.OnlineMode) {
			
			WaterwheelPlayerNetCtrl netPlayerScript = GetComponent<WaterwheelPlayerNetCtrl>();
			if (netPlayerScript != null && !netPlayerScript.GetIsHandlePlayer()) {

				this.enabled = false;
				InputEventCtrl.GetInstance().ClickFireBtEvent -= ClickFireBtEvent;
				return;
			}
		}
		GlobalData.GetInstance().PlayerAmmoFrequency = frequency;

		if(state == ButtonState.DOWN && StartBtCtrl.GetInstanceP2().CheckIsActivePlayer())
		{
			Spawner.HiddenCacheObj(bulletPrefab);
			OnStartFire();
		}
		else
		{
			OnStopFire();
		}
	}

	[RPC]
	void SendUpdateZhuJiaoNvFireAction(float crossPx)
	{
		ZhuJiaoNvScript.UpdateZhuJiaoNvFireAction(crossPx);
	}

	[RPC]
	void SendZhuJiaoNvOpenFire()
	{
		ZhuJiaoNvScript.OpenFireOtherPort();
	}

	[RPC]
	void SendZhuJiaoNvCloseFire()
	{
		ZhuJiaoNvScript.CloseFireOtherPort();
	}

	void OnStartFire()
	{
		if (Time.timeScale == 0 || firing)
		{
			return;
		}
		pcvr.IsOpenShuiBeng = true;
		pcvr.ShuiBengState = PcvrShuiBengState.Level_1;
		firing = true;
		IsPlayerFire = true;
		ZhuJiaoNvScript.OpenFire();
		if (Network.peerType != NetworkPeerType.Disconnected) {
			NetView.RPC("SendZhuJiaoNvOpenFire", RPCMode.OthersBuffered);
		}
		
		if(audio != null)
		{
			audio.Play();
		}
	}
	
	void OnStopFire()
	{
		if (!firing) {
			return;
		}
		pcvr.IsOpenShuiBeng = false;
		firing = false;
		IsPlayerFire = false;
		ZhuJiaoNvScript.CloseFire();
		if (Network.peerType != NetworkPeerType.Disconnected) {
			NetView.RPC("SendZhuJiaoNvCloseFire", RPCMode.OthersBuffered);
		}
		
		if (audio != null) {
			audio.Stop();
		}

		if (WaterwheelPlayerNetCtrl.DamagePlayer != null) {
			WaterwheelPlayerNetCtrl.DamagePlayer.SetActivePlayerGunWaterObj(0);
		}
	}

	public void SetPlayerBoxColliderState(bool isActive)
	{
		if (IntoPuBuCtrl.IsIntoPuBu || CameraShake.IsActiveCamOtherPoint) {
			PlayerBoxCollider.enabled = true;
			return;
		}

		if (PlayerBoxCollider.enabled == isActive) {
			return;
		}
		PlayerBoxCollider.enabled = isActive;
	}

	[RPC]
	void SendZhuJiaoNvPlayFailAction()
	{
		ZhuJiaoNvScript.PlayFailAction();
	}

	public void ZhuJiaoNvPlayFailAction()
	{
		ZhuJiaoNvScript.PlayFailAction();
		if (Network.peerType != NetworkPeerType.Disconnected) {
			NetView.RPC("SendZhuJiaoNvPlayFailAction", RPCMode.OthersBuffered);
		}
	}

	[RPC]
	void SendZhuJiaoNvPlayWinAction(int key)
	{
		ZhuJiaoNvScript.PlayWinAction(key);
	}
	
	public void ZhuJiaoNvPlayWinAction(int key)
	{
		ZhuJiaoNvScript.PlayWinAction(key);
		if (Network.peerType != NetworkPeerType.Disconnected) {
			NetView.RPC("SendZhuJiaoNvPlayWinAction", RPCMode.OthersBuffered, key);
		}
	}

	public static void AddPlayerShootNpcNum()
	{
		PlayerShootNpcNum++;
	}

	public static void ResetPlayerShootNpcNum()
	{
		PlayerShootNpcNum = 0;
	}

	public static int GetPlayerShootLevelStar()
	{
		int starNum = 1;
		int val_1 = GameCtrlXK.GetInstance().PlayerShootNpc_1;
		int val_2 = GameCtrlXK.GetInstance().PlayerShootNpc_2;
		if (PlayerShootNpcNum > val_2) {
			starNum = 3;
		}
		else if (PlayerShootNpcNum <= val_2 && PlayerShootNpcNum > val_1) {
			starNum = 2;
		}
		return starNum;
	}

	public static void AddPlayerHitZhangAiNum()
	{
		PlayerHitZhangAiNum++;
	}

	public static void ResetPlayerHitZhangAiNum()
	{
		PlayerHitZhangAiNum = 0;
	}

	public static int GetPlayerJiaShiLevelStar()
	{
		int starNum = 3;
		int val_1 = GameCtrlXK.GetInstance().PlayerHitZhangAi_1;
		int val_2 = GameCtrlXK.GetInstance().PlayerHitZhangAi_2;
		if (PlayerHitZhangAiNum > val_2) {
			starNum = 1;
		}
		else if (PlayerHitZhangAiNum <= val_2 && PlayerHitZhangAiNum > val_1) {
			starNum = 2;
		}
		return starNum;
	}

	void CheckPlayerForwardHit()
	{
		RaycastHit hitInfo;
		Vector3 startPos = PlayerTran.position + Vector3.up * 10f;
		//Vector3 endPos = PlayerTran.position;
		//Debug.DrawLine(startPos, endPos, Color.red);
		Physics.Raycast(startPos, Vector3.down, out hitInfo, 10f, GameCtrlXK.GetInstance().WaterLayerMask);
		if (hitInfo.collider != null
		    && !hitInfo.collider.isTrigger
		    && (hitInfo.collider.GetComponent<BoxCollider>() != null
		    || (hitInfo.collider.GetComponent<MeshCollider>() && hitInfo.collider.tag == "WaterPath"))){
			PlayerTran.position = hitInfo.point;
		}

		if (!IsActivePlayerForwardHit) {
			return;
		}

		startPos = PlayerTran.position + Vector3.up * 1.5f;
		//Vector3 endPos = startPos + PlayerTran.forward * 10f;
		//Debug.DrawLine(startPos, endPos, Color.red);
		Physics.Raycast(startPos, PlayerTran.forward, out hitInfo, 10f, GameCtrlXK.GetInstance().WaterLayerMask);
		if (hitInfo.collider != null
		    && hitInfo.collider.GetComponent<MeshCollider>()
		    && hitInfo.collider.tag != "WaterPath"){
			hitInfo.collider.isTrigger = true;
		}
	}
}