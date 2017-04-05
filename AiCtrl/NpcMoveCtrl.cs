using UnityEngine;
using System.Collections;
using Frederick.ProjectAircraft;

public enum NPC_STATE
{
	DI_MIAN_NPC,
	FEI_XING_NPC,
	ZAI_TI_NPC,
	SHUI_QIANG_NPC,
	BOSS_NPC,
	DaEYu_ZXiong_NPC
}

public enum AudioNpcState
{
	NULL,
	Xiong,
	ShiZi,
	LaoHu,
	ChangJingLu
}

public class NpcMoveCtrl : MonoBehaviour {

	public AudioNpcState AudioState = AudioNpcState.NULL;
	AudioSource AudioSourceNpc;

	public NPC_STATE NPC_Type = NPC_STATE.DI_MIAN_NPC;
	public bool IsDanCheLvNpc;
	public bool IsDonotAimPlayerFire;

	public GameObject BuWaWaObj;
	public GameObject ExplodeObj;

	public GameObject NpcSimpleBulletObj;
	public GameObject NpcSimpleBulletObjFire_2;
	public Transform SpawnBulletTran;
	public Transform SpawnBulletTranFire_2;
	public ZaiTiNpcCtrl []ZaiTiScript;

	[Range(-5f, 5f)] public float HighOffset;
	public bool IsTeShuZiDanHaiDao = false;
	public bool IsTeShuZiDanShuiQiang = false;
	[Range(1f, 10000f)] public float BuWaWaPowerVal = 500f;
	public GameObject AudioHitNpcObj;
	public GameObject AudioNpcFireObj_1;
	public GameObject AudioNpcFireObj_2;

	public AudioClip AudioHitNpc;
	public AudioClip AudioNpcFire_1;
	public AudioClip AudioNpcFire_2;
	public GameObject SpawnPointObj;
	
	float ShuiQiangNpcFlyTime = 2.5f;
	bool IsFireNpc;
	bool IsMoveNpc;

	int NextMarker;
	float MoveSpeed = 1f;
	float OldMoveSpeed = 1f;
	public static float MinDistance = 0.5f;
	float FireDistance = 10f;

	Animator AnimatorNpc;
	Transform NpcPathTran;
	Transform WaterwheelPlayer;

	bool IsDoRootAction;
	bool IsPlayRun_3;
	bool IsDeadNpc;

	float RootTimeVal;
	float RunAniSpeed;
	NpcMark MarkScripte;
	NpcRunState RunStateVal;

	Transform ZhangAiTranAimVal;
	Transform ZhangAiTranObjVal;
	
	int PreMarker;
	bool IsLoopMovePath;
	bool IsBackStartPoint;

	bool IsGetMarkPos;
	Vector3 AimMarkPos;

	bool IsDoFireAction;
	bool IsMoveFlyNpcToCam;
	bool IsStartMoveFlyNpcToCam;

	float RandVal_x;
	float RandVal_y;
	float RandVal_z;
	Transform AimCuBeTranFly;
	
	bool IsMoveByITween;
	bool IsNiXingAiPath;
	bool IsMoveToAiPath;
	const float LerpForwardVal = 5f;
	Transform NpcAiMarkTran;
	Transform NpcAiPathTran;

	Transform NpcTran;
	int ZaiTiNpcDeadNum;

	float SpawnTime;
	const int MaxZaiTiZhengXing = 5;
	const int MaxZaiTiNiXing = 5;
	public static GameObject [] ZaiTiZhengXingObj;
	public static GameObject [] ZaiTiNiXingObj;
	
	bool IsCheckPlayerPos = true;
	
	bool IsHandleNpc;
	NetworkView netView;
	Vector3 PositionCur;
	Quaternion RotationCur;

	bool IsMakeEYuMvToplayer;
	public static Transform TranCamera;
	AudioSource AudioSourceBoss;
	Rigidbody RigObj;

	bool IsDiLeiNpc;
	bool IsMoveLeft;
	bool IsMoveRight;
	bool IsPlayerKillNpc;
	bool IsMoveToEndPoint;
	
	float TimeZaiTiPos;
	Vector3 ZaiTiOldPos;
	
	float BoxColSizeYVal = 2f;
	bool IsPlayerFireNpc;
	bool IsMoveZaiTiNpcItween;

	public static bool CheckIsSpawnZhengXingZaiTiNpc()
	{
		int objNum = 0;
		if (ZaiTiZhengXingObj == null) {
			ZaiTiZhengXingObj = new GameObject[MaxZaiTiZhengXing];
		}

		int max = ZaiTiZhengXingObj.Length;
		for (int i = 0; i < max; i++) {
			if(ZaiTiZhengXingObj[i] != null)
			{
				objNum++;
			}
		}

		if(objNum < MaxZaiTiZhengXing)
		{
			return true;
		}
		return false;
	}

	void FillZaiTiZhengXingObj()
	{
		int max = ZaiTiZhengXingObj.Length;
		for (int i = 0; i < max; i++) {
			if(ZaiTiZhengXingObj[i] == null)
			{
				ZaiTiZhengXingObj[i] = gameObject;
				return;
			}
		}
	}

	public static bool CheckIsSpawnNiXingZaiTiNpc()
	{
		int objNum = 0;
		if (ZaiTiNiXingObj == null) {
			ZaiTiNiXingObj = new GameObject[MaxZaiTiNiXing];
		}
		
		int max = ZaiTiNiXingObj.Length;
		for (int i = 0; i < max; i++) {
			if(ZaiTiNiXingObj[i] != null)
			{
				objNum++;
			}
		}
		
		if(objNum < MaxZaiTiNiXing)
		{
			return true;
		}
		return false;
	}
	
	void FillZaiTiNiXingObj()
	{
		int max = ZaiTiNiXingObj.Length;
		for (int i = 0; i < max; i++) {
			if(ZaiTiNiXingObj[i] == null)
			{
				ZaiTiNiXingObj[i] = gameObject;
				return;
			}
		}
	}

	void Awake()
	{
		SetIsShuiLeiNpc();
		if (TranCamera == null) {
			TranCamera = Camera.main.transform;
		}

		SpawnTime = Time.time;
		if (GlobalData.GetInstance().gameMode == GameMode.OnlineMode) {
			netView = GetComponent<NetworkView>();
			netView.stateSynchronization = NetworkStateSynchronization.Off;
		}
		RigObj = rigidbody;

		if(ZaiTiZhengXingObj == null)
		{
			ZaiTiZhengXingObj = new GameObject[MaxZaiTiZhengXing];
		}

		if(ZaiTiNiXingObj == null)
		{
			ZaiTiNiXingObj = new GameObject[MaxZaiTiNiXing];
		}

		if(NPC_Type == NPC_STATE.ZAI_TI_NPC || NPC_Type == NPC_STATE.BOSS_NPC)
		{
			if (Random.Range(0, 100) % 2 == 0) {
				IsMoveLeft = true;
			}
			else {
				IsMoveRight = true;
			}

			CreateAudioSourceBoss();
			NPC_Type = NPC_STATE.ZAI_TI_NPC;
			BuWaWaObj = null;

			BoxCollider[] boxColArray = GetComponents<BoxCollider>();
			for (int i = 0; i < boxColArray.Length; i++) {
				Vector3 centerPos = boxColArray[i].center;
				centerPos.y = -HighOffset + 0.2f;
				boxColArray[i].center = centerPos;
			}
		}
		else {
			BoxColSizeYVal = GetComponent<BoxCollider>().size.y;
		}
		NpcTran = transform;
		NpcTran.parent = GameCtrlXK.MissionCleanup;
		
		if(BuWaWaObj != null)
		{
			BuWaWaObj.SetActive(false);
			Rigidbody rigObj = BuWaWaObj.GetComponent<Rigidbody> ();
			if (rigObj == null) {
				rigObj.name = "null";
			}
		}
		CreateAudioSourceNpc();

		if (SpawnBulletTran == null && NPC_Type != NPC_STATE.ZAI_TI_NPC) {
			Debug.LogError("SpawnBulletTran is null");
			SpawnBulletTran.name = "null";
		}
		AnimatorNpc = GetComponent<Animator>();
		ResetActionInfo();

		if (IsDanCheLvNpc) {
			IsDonotAimPlayerFire = true;
		}

		if (IsTeShuZiDanShuiQiang) {
			NpcSimpleBulletObj.SetActive(false);
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (!IsHandleNpc){
			return;
		}
		HandleHitShootedPlayer(other.gameObject, 0);
	}

	void CheckNpcDownHit()
	{
		if (NPC_Type != NPC_STATE.ZAI_TI_NPC) {
			return;
		}
		RaycastHit hitInfo;
		Vector3 startPos = NpcTran.position + Vector3.up * 10f;
		Physics.Raycast(startPos, Vector3.down, out hitInfo, 10f, GameCtrlXK.GetInstance().WaterLayerMask);
		if (hitInfo.collider != null
		    && !hitInfo.collider.isTrigger
		    && hitInfo.collider.GetComponent<BoxCollider>() != null){
			Vector3 posTmp = hitInfo.point + new Vector3(0f, HighOffset, 0f) + new Vector3(0f, 0.1f, 0f);
			NpcTran.position = posTmp;
		}
	}

	// Update is called once per frame
	void Update()
	{
		CheckDistancePlayerCross();
		if (!StartBtCtrl.GetInstanceP1().CheckIsActivePlayer()) {
			return;
		}

		if (!IsHandleNpc) {
			return;
		}

		if (FinishPanelCtrl.GetInstance().CheckIsActiveFinish()) {
			RemoveNpcObj(false);
			return;
		}
		
		if (IsPlayerKillNpc) {
			CheckNpcDisCamera();
			return;
		}
		CheckNpcDownHit();
		
		if (IsDeadNpc) {
			CheckNpcDisCamera();
			CheckIsFireNpcAction();

			if (IsMoveToAiPath) {
				HandleMoveToAiPathNpc();
				CheckPlayerPos();
			}
			return;
		}
		
		if (Time.time >= SpawnTime + 30f) {
			//Debug.Log("*****************Do root");
			IsDeadNpc = true;
			if (NPC_Type != NPC_STATE.ZAI_TI_NPC) {
				if (!IsDoFireAction) {
					RandomDoRootAction();
				}
			}
			else {
				if (IsDanCheLvNpc) {
					for (int i = 0; i < ZaiTiScript.Length; i++) {
						ZaiTiScript[i].CloseRootAction();
					}
				}
			}
			return;
		}
		
		if (NPC_Type == NPC_STATE.FEI_XING_NPC) {
			HandleMoveFlyNpc();
			return;
		}
		
		if (!IsMoveNpc) {
			if (IsFireNpc && NPC_Type != NPC_STATE.ZAI_TI_NPC && !IsDonotAimPlayerFire) {
				NpcTran.LookAt(WaterwheelPlayer.position);
			}

			if(IsFireNpc && !IsDoFireAction && IsMoveToEndPoint)
			{
				float disPlayer = Vector3.Distance(NpcTran.position, WaterwheelPlayer.position);
				if(disPlayer <= FireDistance)
				{
					if (NPC_Type == NPC_STATE.DaEYu_ZXiong_NPC) {
						MakeNpcMoveToPlayer();
					}
					else {
						PlayFireAction();
					}
				}
			}
			return;
		}
		
		if (IsMoveByITween) {
			return;
		}
		
		if (IsFireNpc) {
			float disPlayer = Vector3.Distance(transform.position, WaterwheelPlayer.position);
			if (disPlayer <= FireDistance) {
				if (NPC_Type == NPC_STATE.DaEYu_ZXiong_NPC) {
					MakeNpcMoveToPlayer();
				}
				else {
					PlayFireAction();
				}

				if(NPC_Type == NPC_STATE.ZAI_TI_NPC || NPC_Type == NPC_STATE.BOSS_NPC)
				{
				}
				else
				{
					return;
				}
			}
		}
		
		if (IsLoopMovePath) {
			HandleLoopMoveNPC();
			return;
		}
		
		if (IsDoRootAction) {
			RootTimeVal += Time.deltaTime;
			if (RootTimeVal < MarkScripte.RootAniTime) {
				return;
			}
			
			IsDoRootAction = false;
			RootTimeVal = 0f;
			if (AnimatorNpc != null) {
				AnimatorNpc.speed = RunAniSpeed;
				CloseRootAction();
			}
			RandPlayRunAction();
		}

		Vector3 markPos = Vector3.zero;
		if (!IsMoveToAiPath) {
			markPos = GetAimMarkPos( NpcPathTran.GetChild(NextMarker) );
		}

		Vector3 vecA = NpcTran.position;
		Vector3 vecB = markPos;
		vecA.y = vecB.y = 0f;
		float dis = Vector3.Distance(vecA, vecB);

		if (IsMoveToAiPath) {
			if (IsDanCheLvNpc && !IsDoFireAction) {
				return;
			}
			HandleMoveToAiPathNpc();
			CheckPlayerPos();
			return;
		}
		
		if (dis < (MinDistance + 1f) && NextMarker < NpcPathTran.childCount) {
			Transform markTran = NpcPathTran.GetChild(NextMarker);
			ResetIsGetMarkPos();
			NextMarker++;
			
			MarkScripte = markTran.GetComponent<NpcMark>();
			
			if(NextMarker < NpcPathTran.childCount && NPC_Type == NPC_STATE.SHUI_QIANG_NPC)
			{
				Transform nextMarkTmp = NpcPathTran.GetChild(NextMarker);
				NpcMark nextMarkScript = nextMarkTmp != null ? nextMarkTmp.GetComponent<NpcMark>() : null;
				if(nextMarkScript.GetFlyNodes().Count > 0)
				{
					IsMoveByITween = true;
					MoveNpcByITween( nextMarkTmp );
					PlayActionRun_2();
					return;
				}
			}
			
			if (MarkScripte.IsDoRoot) {
				IsDoRootAction = true;
				if (AnimatorNpc) {
					AnimatorNpc.speed = 1f;
					RandomDoRootAction();
				}
			}
			else
			{
				RandPlayRunAction();
			}
		}
		
		if (NextMarker >= NpcPathTran.childCount) {
			if (NpcAiMarkTran != null) {
				AiMark aiMarkScript = NpcAiMarkTran.GetComponent<AiMark>();
				NextMarker = aiMarkScript.getMarkCount();
				PreMarker = NextMarker;
				IsMoveToAiPath = true;
				if (IsDanCheLvNpc) {
					MakeDanCheLvNpcDoRootAction();
				}
			}
			else
			{
				IsMoveNpc = false;
				IsMoveToEndPoint = true;
				if (AnimatorNpc != null) {
					AnimatorNpc.speed = 1f;
					RandomDoRootAction();
				}
			}
		}
		else if (!IsDoRootAction) {
			Vector3 mv = markPos - NpcTran.position;
			mv.y = 0f;
			NpcTran.forward = Vector3.Lerp(NpcTran.forward, mv, MoveSpeed * Time.deltaTime);

			mv = mv.normalized * MoveSpeed * Time.deltaTime;
			NpcTran.Translate(mv.x, 0f, mv.z, Space.World);
			ApplyGravity();
		}

		if (Camera.main != null
		    && Camera.main.enabled
		    && GlobalData.GetInstance().gameMode == GameMode.OnlineMode
		    && Network.peerType != NetworkPeerType.Disconnected) {
			
			if (PositionCur != NpcTran.position) {
				PositionCur = NpcTran.position;
				netView.RPC("SendNpcPosToOther", RPCMode.OthersBuffered, PositionCur);
			}
			
			if (RotationCur != NpcTran.rotation) {
				RotationCur = NpcTran.rotation;
				netView.RPC("SendNpcRotToOther", RPCMode.OthersBuffered, RotationCur);
			}
		}
	}

	void MakeDanCheLvNpcDoRootAction()
	{
		if (!IsDanCheLvNpc) {
			return;
		}
		
		if (!RigObj.isKinematic) {
			RigObj.isKinematic = true;
		}

		for (int i = 0; i < ZaiTiScript.Length; i++) {
			ZaiTiScript[i].RandomDoRootAction();
		}
	}

	void OnBecameInvisible()
	{
		if (!IsDeadNpc) {
			return;
		}

		if (NPC_Type != NPC_STATE.ZAI_TI_NPC) {
			return;
		}
		
		if (NPC_Type == NPC_STATE.ZAI_TI_NPC) {
			int max = ZaiTiScript.Length;
			for(int i = 0; i < max; i++)
			{
				if(ZaiTiScript[i] != null)
				{
					Destroy(ZaiTiScript[i].gameObject);
				}
			}
		}
		HiddenNpcObj();
	}

	void DelayActiveNpcSimpleBulletObj()
	{
		NpcSimpleBulletObj.SetActive(true);
	}

	void OnMouseFireActive()
	{
		if (NPC_Type == NPC_STATE.DaEYu_ZXiong_NPC || IsTeShuZiDanShuiQiang) {
			if (AnimatorNpc.GetBool("IsFire_1")) {
				PlayNpcAudio(AudioNpcFireObj_1);
			}
			else {
				PlayNpcAudio(AudioNpcFireObj_2);
			}
			return;
		}

		Vector3 vecA = Camera.main.transform.position - NpcTran.position;
		Vector3 vecB = Camera.main.transform.forward;
		vecA.y = vecB.y = 0f;
		if (Vector3.Dot(vecA, vecB) > 0f) {
			return;
		}
		
		Vector3 startPos = SpawnBulletTran.position;
		Vector3 endPos = Camera.main.transform.position;
		vecA = NpcTran.position - WaterwheelPlayer.position;
		vecA.y = vecB.y = 0f;
		if(Random.Range(0, 100) > 50 || Vector3.Dot(vecA, vecB) < 0.866f) {
			endPos = WaterwheelPlayer.position + Vector3.up * 1.5f;
		}
		
		float distance = Vector3.Distance(startPos, endPos);
		Vector3 forwardVec = endPos - startPos;
		forwardVec = forwardVec.normalized;
		
		RaycastHit hitInfo;
		Physics.Raycast(startPos, forwardVec, out hitInfo, distance, GameCtrlXK.GetInstance().NpcAmmoHitLayer.value);
		if (hitInfo.collider != null && hitInfo.collider.gameObject != gameObject){
			endPos = hitInfo.point;
		}

		GameObject ammo = null;
		if (AnimatorNpc.GetBool("IsFire_1")) {
			ammo = (GameObject)Instantiate(NpcSimpleBulletObj);
			PlayNpcAudio(AudioNpcFireObj_1);
		}
		else {
			ammo = (GameObject)Instantiate(NpcSimpleBulletObjFire_2);
			PlayNpcAudio(AudioNpcFireObj_2);
		}
		Transform ammoTran = ammo.transform;
		ammoTran.parent = GameCtrlXK.MissionCleanup;
		ammoTran.position = startPos;
		ammoTran.forward = forwardVec;
		
		NpcSimpleBullet bulletScript = ammo.GetComponent<NpcSimpleBullet>();
		bulletScript.dist = Vector3.Distance(startPos, endPos);
	}

	void MakeNpcMoveToPlayer()
	{
		if (IsMakeEYuMvToplayer) {
			return;
		}
		IsMakeEYuMvToplayer = true;
		IsMoveByITween = true;

		AnimatorNpc.speed = 1.0f;
		CloseRootAction();
		CloseRunAction(); //kongZhongJingZhenAction
		RandomDoFireAction();

		Vector3 [] nodesArray = new Vector3[3];
		nodesArray[0] = NpcTran.position;

		Transform attackTran = WaterwheelPlayerCtrl.GetInstance().EYuXiongAttackTran;
		Vector3 offset = Vector3.zero;
		offset.x = (Random.Range(0, 10000) % 3 - 1) * 0.4f * attackTran.localScale.x;
		offset.y = (Random.Range(0, 10000) % 3 - 1) * 0.4f * attackTran.localScale.y;
		offset.z = 0f;
		
		nodesArray[1] = attackTran.position + offset.x * attackTran.right + offset.z * attackTran.forward
			+ offset.y * attackTran.up;

		Vector3 forVec = nodesArray[1] - nodesArray[0];
		forVec.y = 0f;
		offset = nodesArray[1] + forVec.normalized * Random.Range(2f, 3.5f);
		offset.y -= Random.Range(3f, 4.5f);
		nodesArray[2] = offset;

		iTween.MoveTo(gameObject, iTween.Hash("path", nodesArray,
		                                      "time", 0.8f,
		                                      "orienttopath", true,
		                                      "looktime", 0.3f,
		                                      "easeType", iTween.EaseType.linear,
		                                      "oncomplete", "EYuNpcOnCompelteITween"));
	}

	void EYuNpcOnCompelteITween()
	{
		HiddenNpcObj();
	}

	void ApplyGravity ()
	{
		if (NPC_Type == NPC_STATE.FEI_XING_NPC) {
			return;
		}

		if (NPC_Type == NPC_STATE.ZAI_TI_NPC) {
			
			RaycastHit hitInfo;
			Vector3 startPos = NpcTran.position + Vector3.up * 2f;
			Vector3 forwardVal = Vector3.down;
			Physics.Raycast(startPos, forwardVal, out hitInfo, 200f, GameCtrlXK.GetInstance().NpcVertHitLayer.value);
			if (hitInfo.collider != null){
				
				Vector3 posTmp = hitInfo.point;
				GameObject objHit = hitInfo.collider.gameObject;
				if (objHit.layer == LayerMask.NameToLayer(GameCtrlXK.WaterLayer)) {
					posTmp = hitInfo.point + new Vector3(0f, HighOffset, 0f) + new Vector3(0f, 0.1f, 0f); //Hit water
					if (Vector3.Distance(NpcTran.position, posTmp) > 0.001f) {
						NpcTran.position = posTmp;
					}
				}
			}
		}
		else {
			
			RaycastHit hitInfo;
			Vector3 startPos = NpcTran.position + Vector3.up * 5f;
			Vector3 forwardVal = Vector3.down;
			Physics.Raycast(startPos, forwardVal, out hitInfo, 30f, GameCtrlXK.GetInstance().NpcVertHitLayer.value);
			if (hitInfo.collider != null){
				
				Vector3 posTmp = hitInfo.point;
				GameObject objHit = hitInfo.collider.gameObject;
				if (objHit.layer == LayerMask.NameToLayer(GameCtrlXK.WaterLayer) && NPC_Type != NPC_STATE.SHUI_QIANG_NPC) {
					posTmp = hitInfo.point + new Vector3(0f, HighOffset, 0f); //Hit water
				}

				if (Vector3.Distance(NpcTran.position, posTmp) > 0.001f) {
					NpcTran.position = posTmp;
				}
			}
		}
	}

	/// <summary>
	/// Sets the npc animator action. if val == 0 is false, else is true.
	/// </summary>
	/// <param name="action">Action.</param>
	/// <param name="val">Value.</param>
	/// <param name="speedAction">Speed action.</param>
	void SetNpcAnimatorAction(string action, int val, float speedAction)
	{
		bool isPlay = val == 0 ? false : true;
		if (AnimatorNpc.GetBool(action) == isPlay) {
			return;
		}

		AnimatorNpc.speed = speedAction;
		AnimatorNpc.SetBool(action, isPlay);
		if (Network.peerType != NetworkPeerType.Disconnected) {
			netView.RPC("SendSetNpcAnimatorAction", RPCMode.OthersBuffered, action, val, speedAction);
		}
	}
	
	[RPC]
	void SendSetNpcAnimatorAction(string action, int val, float speedAction)
	{
		bool isPlay = val == 0 ? false : true;
		AnimatorNpc.speed = speedAction;
		AnimatorNpc.SetBool(action, isPlay);
	}

	public void SetIsHandleNpc()
	{
		IsHandleNpc = true;
	}

	[RPC]
	void SendNpcRotToOther(Quaternion rot)
	{
		if (NpcTran != null) {
			NpcTran.rotation = rot;
		}
		else {
			NpcTran = transform;
			NpcTran.rotation = rot;
		}
	}
	
	[RPC]
	void SendNpcPosToOther(Vector3 pos)
	{
		if (NpcTran != null) {
			NpcTran.position = pos;
		}
		else {
			NpcTran = transform;
			NpcTran.position = pos;
		}
	}

	public void ShootedByPlayer()
	{
		HandleHitShootedPlayer(WaterwheelPlayer.gameObject, 1);
	}

	///<summary>
	/// npc hit player key -> 0, player shooting obj key -> 1
	///</summary>
	void HandleHitShootedPlayer(GameObject obj, int key)
	{
		switch(obj.tag)
		{
		case "NpcPlayer":
			if (key == 1) {
				return;
			}

			NpcMoveCtrl npcScript = obj.GetComponent<NpcMoveCtrl>();
			if (npcScript == null) {
				return;
			}

			if (npcScript.NPC_Type != NPC_STATE.ZAI_TI_NPC || NPC_Type != NPC_STATE.ZAI_TI_NPC) {
				return;
			}

			CheckZaiTiNpcPos(obj, 0);
			break;

		case "Player":
			if (!XingXingCtrl.IsPlayerCanHitNPC && 0 == key) {
				return;
			}

			if (0 == key) {
				AudioListCtrl.PlayAudio(AudioListCtrl.GetInstance().AudioShipHit_2);
				if (PlayerAutoFire.PlayerMvSpeed > GameCtrlXK.NpcHitPlayerShakeSpeed
				    && NPC_Type == NPC_STATE.ZAI_TI_NPC
				    && Random.Range(0, 1000) % GameCtrlXK.NpcShakeCamVal == 0) {
					CameraShake.GetInstance().SetCameraShakeImpulseValue();
				}
			}

			if(NPC_Type == NPC_STATE.ZAI_TI_NPC || NPC_Type == NPC_STATE.BOSS_NPC)
			{
				if (0 == key && !IsPlayerKillNpc) {

					Vector3 vecA = WaterwheelPlayer.forward;
					Vector3 vecB = WaterwheelPlayer.position - NpcTran.position;
					vecA.y = vecB.y = 0f;
					if (Vector3.Dot(vecA, vecB) < 0f) {
						if (PlayerAutoFire.PlayerMvSpeed > 1f || !RigObj.isKinematic) {
							IsDeadNpc = true;
							IsPlayerKillNpc = true;
							StopAudioSourceBoss();
							for (int i = 0; i < ZaiTiScript.Length; i++) {
								if (ZaiTiScript[i] != null) {
									ZaiTiScript[i].ShootedByPlayer(1);
								}
							}
						}
					}

					if (IsDanCheLvNpc || IsDiLeiNpc) {
						Invoke("HiddenNpcObj", 2f);
					}
				}
				return;
			}

			if(IsPlayerKillNpc)
			{
				return;
			}

			if(IsFireNpc)
			{
				WaterwheelPlayerCtrl.GetInstance().AddKillFireNpcNum();
			}
			PlayNpcAudio(AudioHitNpcObj);

			if (1 == key || (key == 0 && !StartBtCtrl.GetInstanceP2().CheckIsActivePlayer())) {
				WaterwheelCameraCtrl.GetInstance().SpawnPlayerNengLiangLiZi(NpcTran.position);
			}
			OnHitWaterwheelPlayer();

			IsDeadNpc = true;
			IsPlayerKillNpc = true;
			break;
		}
	}

	public void SetFlyNpcAimCubeTran()
	{
		if(IsFireNpc)
		{
			AimCuBeTranFly = WaterwheelPlayerCtrl.GetInstance().FlyNpcAimCube_1.transform;
		}
		else
		{
			AimCuBeTranFly = WaterwheelPlayerCtrl.GetInstance().FlyNpcAimCube_2.transform;
		}
	}

	void EnableIsStartMoveFlyNpcToCam()
	{
		IsStartMoveFlyNpcToCam = true;
	}

	void MoveFlyNpcToCam()
	{
		if (!IsMoveFlyNpcToCam || !IsStartMoveFlyNpcToCam) {
			return;
		}

		Vector3 AimPos = GetAimMarkPos(Camera.main.transform);
		float dis = Vector3.Distance(AimPos, transform.position);
		if (dis < MinDistance) {
			Destroy(gameObject);
			return;
		}
		
		NpcTran.forward = Vector3.Lerp(NpcTran.forward, AimPos - transform.position, LerpForwardVal * MoveSpeed * Time.deltaTime);
		NpcTran.Translate(Vector3.forward * Time.deltaTime * MoveSpeed);
	}

	void HandleMoveFlyNpc()
	{
		if (!IsMoveNpc) {
			return;
		}

		if (IsMoveFlyNpcToCam) {
			MoveFlyNpcToCam();
			return;
		}

		Vector3 AimPos = GetAimMarkPos(AimCuBeTranFly);
		float dis = Vector3.Distance(AimPos, NpcTran.position);
		if (dis < (MinDistance + 3f)) {
			ResetIsGetMarkPos();
			IsMoveFlyNpcToCam = true;
			if (IsFireNpc && !IsDoFireAction) {
				EnableIsStartMoveFlyNpcToCam();
				IsDoFireAction = true;

				AnimatorNpc.speed = 1.0f;
				CloseRunAction();
				RandomDoFireAction();
			}
			else
			{
				EnableIsStartMoveFlyNpcToCam();
			}
			return;
		}

		NpcTran.forward = Vector3.Lerp(NpcTran.forward, AimPos - transform.position, LerpForwardVal * MoveSpeed * Time.deltaTime);
		NpcTran.Translate(Vector3.forward * Time.deltaTime * MoveSpeed);
	}

	void HandleNiXingAiPathNpc()
	{
		Vector3 markPos = GetAimMarkPos( NpcAiPathTran.GetChild(PreMarker) );
		Vector3 posA = NpcTran.position;
		Vector3 posB = markPos;
		posA.y = posB.y = 0f;
		float dis = Vector3.Distance(posA, posB);
		if (dis < (MinDistance + 1f) && PreMarker > -1) {
			ResetIsGetMarkPos();
			PreMarker--;
			
			RandPlayRunAction();
		}
		
		if (PreMarker <= -1) {
			AiPathCtrl aiPathScript = NpcAiPathTran.GetComponent<AiPathCtrl>();
			int preNum = aiPathScript.GetPrePathNum();
			if (preNum <= 0) {
				RemoveNpcObj(false);
				return;
			}

			NpcAiPathTran = aiPathScript.GetPrePathTran(1) != null ? aiPathScript.GetPrePathTran(1) : null;
			if(NpcAiPathTran == null)
			{
				NpcAiPathTran = aiPathScript.GetPrePathTran(2) != null ? aiPathScript.GetPrePathTran(2) : null;
			}
			
			if(NpcAiPathTran == null)
			{
				NpcAiPathTran = aiPathScript.GetPrePathTran(3) != null ? aiPathScript.GetPrePathTran(3) : null;
			}
			PreMarker = NpcAiPathTran.childCount - 1;
		}
		else
		{
			posB = WaterwheelPlayer.position;
			posA.y = posB.y = 0f;
			if (RigObj.isKinematic) {
				RigObj.isKinematic = false;
			}
			
			Vector3 forwardVal = markPos - NpcTran.position;
			forwardVal.y = 0f;
			float disVal = Vector3.Distance(forwardVal, Vector3.zero);

			forwardVal = forwardVal.normalized;
			//Debug.DrawLine(NpcTran.position, markPos, Color.red);

			if (Vector3.Dot(forwardVal, NpcTran.forward) < 0.998f && disVal > 2f) {
				NpcTran.forward = Vector3.Lerp(NpcTran.forward, forwardVal, MoveSpeed * Time.deltaTime);
			}
			
			ApplyGravity();
			forwardVal = NpcTran.forward;
			forwardVal.y = 0f;
			RigObj.velocity = Vector3.Lerp(RigObj.velocity, forwardVal * MoveSpeed, Time.deltaTime * 10f);
		}
	}

	void HandleMoveToAiPathNpc()
	{
		if (NextMarker >= NpcAiPathTran.childCount) {
			return;
		}

		if(IsNiXingAiPath)
		{
			HandleNiXingAiPathNpc();
			return;
		}
		Vector3 markPos = GetAimMarkPos( NpcAiPathTran.GetChild(NextMarker) );

		Vector3 posA = NpcTran.position;
		Vector3 posB = markPos;
		posA.y = posB.y = 0f;
		float dis = Vector3.Distance(posA, posB);
		if(dis < (MinDistance + 1f) && NextMarker < NpcAiPathTran.childCount)
		{
			ResetIsGetMarkPos();
			NextMarker++;

			RandPlayRunAction();
		}
		
		if (NextMarker >= NpcAiPathTran.childCount)
		{
			AiPathCtrl aiPathScript = NpcAiPathTran.GetComponent<AiPathCtrl>();
			if(aiPathScript.mNextPath1 == null && aiPathScript.mNextPath2 == null && aiPathScript.mNextPath3 == null)
			{
				//RemoveNpcObj();
				IsDeadNpc = true;
				StopAudioSourceBoss();
				return;
			}
			else
			{
				NpcAiPathTran = aiPathScript.mNextPath1 != null ? aiPathScript.mNextPath1 : null;
				if(NpcAiPathTran == null)
				{
					NpcAiPathTran = aiPathScript.mNextPath2 != null ? aiPathScript.mNextPath2 : null;
				}

				if(NpcAiPathTran == null)
				{
					NpcAiPathTran = aiPathScript.mNextPath3 != null ? aiPathScript.mNextPath3 : null;
				}
				NextMarker = 0;
			}
		}
		else
		{
			if (IsMoveZaiTiNpcItween) {
				return;
			}

			if (RigObj.isKinematic) {
				RigObj.isKinematic = false;
			}

			Vector3 forwardVal = markPos - NpcTran.position;
			float dy = Mathf.Abs( forwardVal.y );
			if (dy < 30f) {
				Vector3 offsetVal = NpcTran.forward * 2.5f;
				offsetVal.y = 0f;
				posA = NpcTran.position + Vector3.up * (1f + Mathf.Abs(HighOffset)) + offsetVal;
				posB = NpcTran.position + Vector3.up * (1f + Mathf.Abs(HighOffset));
				
				RaycastHit hitInfo;
				Vector3 startPos = posA;
				Physics.Raycast(startPos, Vector3.down, out hitInfo, 10f, GameCtrlXK.GetInstance().NpcVertHitLayer.value);
				if (hitInfo.collider != null){
					GameObject objHit = hitInfo.collider.gameObject;
					if (objHit.layer == LayerMask.NameToLayer(GameCtrlXK.WaterLayer)) {
						posA = hitInfo.point;
						//Debug.DrawLine(startPos, posA, Color.red);
						
						startPos = posB;
						Physics.Raycast(startPos, Vector3.down, out hitInfo, 10f,
						                GameCtrlXK.GetInstance().NpcVertHitLayer.value);
						if (hitInfo.collider != null){
							objHit = hitInfo.collider.gameObject;
							if (objHit.layer == LayerMask.NameToLayer(GameCtrlXK.WaterLayer)) {
								posB = hitInfo.point;
								//Debug.DrawLine(startPos, posB, Color.blue);
								
								Vector3 vecTmp = posA - posB;
								float disVal = Vector3.Distance(forwardVal, Vector3.zero);
								
								forwardVal = forwardVal.normalized;
								forwardVal.y = vecTmp.normalized.y;
								forwardVal = forwardVal.normalized;
								if (Vector3.Dot(forwardVal, NpcTran.forward) < 0.998f && disVal > 2f) {
									NpcTran.forward = Vector3.Lerp(NpcTran.forward, forwardVal, MoveSpeed * Time.deltaTime);
								}
							}
						}
					}
				}
			}
			else {
				//NpcTran.position = markPos;
				MakeZaiTiNpcMoveByItween();
				return;
			}

			Vector3 mv = NpcTran.forward * 1.5f * Time.deltaTime;
			mv.y = 0f;
			NpcTran.Translate(mv.x, mv.y, mv.z, Space.World);

			CheckZaiTiNpcPos();
			ApplyGravity();

			forwardVal = NpcTran.forward;
			forwardVal.y = 0f;
			RigObj.velocity = Vector3.Lerp(RigObj.velocity, forwardVal * MoveSpeed, Time.deltaTime * 10f);
		}
	}

	void MakeZaiTiNpcMoveByItween()
	{
		if (IsMoveZaiTiNpcItween) {
			return;
		}
		IsMoveZaiTiNpcItween = true;
		RigObj.isKinematic = true;
		
		Vector3 [] nodesArray = new Vector3[2];
		nodesArray[0] = NpcTran.position;
		Vector3 markPos = GetAimMarkPos( NpcAiPathTran.GetChild(NextMarker) );
		markPos.y += 1.5f;
		nodesArray[1] = markPos;
		
		iTween.MoveTo(gameObject, iTween.Hash("path", nodesArray,
		                                      "time", Random.Range(2f, 5f),
		                                      "orienttopath", true,
		                                      "looktime", 1.5f,
		                                      "easeType", iTween.EaseType.linear,
		                                      "oncomplete", "EYuNpcOnCompelteITween"));
	}

	void ZaiTiNpcOnCompelteITween()
	{
		IsMoveZaiTiNpcItween = false;
	}

	void CheckZaiTiNpcPos()
	{
		if (Time.realtimeSinceStartup - TimeZaiTiPos < 2f) {
			return;
		}

		Vector3 posA = ZaiTiOldPos;
		Vector3 posB = NpcTran.position;
		if (Vector3.Distance(posA, posB) < 3f) {
			NpcTran.position = posB + new Vector3(0f, 0.3f, 0f);
		}

		TimeZaiTiPos = Time.realtimeSinceStartup;
		ZaiTiOldPos = NpcTran.position;

		posA = WaterwheelPlayer.position;
		if (Vector3.Distance(posA, posB) > 350f) {
			//Debug.Log("CheckZaiTiNpcPos ************** delete " + gameObject.name);
			if (!IsDeadNpc) {
				return;
			}
			
			if (NPC_Type != NPC_STATE.ZAI_TI_NPC) {
				return;
			}
			
			if (NPC_Type == NPC_STATE.ZAI_TI_NPC) {
				int max = ZaiTiScript.Length;
				for (int i = 0; i < max; i++) {
					if (ZaiTiScript[i] != null) {
						Destroy(ZaiTiScript[i].gameObject);
					}
				}
			}
			HiddenNpcObj();
		}
	}

	Vector3 GetAimMarkPos(Transform tranVal)
	{
		if(IsGetMarkPos)
		{
			if(NPC_Type == NPC_STATE.FEI_XING_NPC)
			{
				AimMarkPos = tranVal.position + RandVal_x * tranVal.right + RandVal_z * tranVal.forward
									+ RandVal_y * tranVal.up;
			}
			return AimMarkPos;
		}
		IsGetMarkPos = true;

		float perDis = Random.Range(0f, 40f) / 100f;
		RandVal_x = (Random.Range(0, 10000) % 3 - 1) * perDis * tranVal.localScale.x;
		RandVal_y = 0f;
		RandVal_z = (Random.Range(0, 10000) % 3 - 1) * perDis * tranVal.localScale.z;

		switch (NPC_Type) {
		case NPC_STATE.ZAI_TI_NPC:
			if(IsMoveLeft)
			{
				RandVal_x = (Random.Range(0, 10000) % 2 - 1) * perDis * tranVal.localScale.x;
				RandVal_y = 0f;
				RandVal_z = 0f;
			}
			else if (IsMoveRight)
			{
				RandVal_x = (Random.Range(0, 10000) % 2) * perDis * tranVal.localScale.x;
				RandVal_y = 0f;
				RandVal_z = 0f;
			}
			break;

		case NPC_STATE.FEI_XING_NPC:
			if(IsMoveFlyNpcToCam)
			{
				RandVal_x = (Random.Range(0, 10000) % 3 - 1) * perDis;
				RandVal_y = 25f;
				RandVal_z = -40f;
			}
			else
			{
				RandVal_y = (Random.Range(0, 10000) % 3 - 1) * perDis * tranVal.localScale.y;
				RandVal_z = 0f;
			}
			break;
		}

		AimMarkPos = tranVal.position + RandVal_x * tranVal.right + RandVal_z * tranVal.forward
							+ RandVal_y * tranVal.up;
		return AimMarkPos;
	}

	void ResetIsGetMarkPos()
	{
		IsGetMarkPos = false;
	}

	void RandomDoRootAction()
	{
		CloseRunAction();
		if (Random.Range(0, 100) % 2 == 0) {
			SetNpcAnimatorAction("IsRoot_2", 0, AnimatorNpc.speed);
			SetNpcAnimatorAction("IsRoot_1", 1, AnimatorNpc.speed);
		}
		else {
			SetNpcAnimatorAction("IsRoot_1", 0, AnimatorNpc.speed);
			SetNpcAnimatorAction("IsRoot_2", 1, AnimatorNpc.speed);
		}
		PlayNpcRootAudio();

		if (NPC_Type == NPC_STATE.SHUI_QIANG_NPC) {
			
			RaycastHit hitInfo;
			Vector3 startPos = NpcTran.position + Vector3.up * 5f;
			Vector3 forwardVal = Vector3.down;
			Physics.Raycast(startPos, forwardVal, out hitInfo, 30f, GameCtrlXK.GetInstance().NpcVertHitLayer.value);
			if (hitInfo.collider != null){
				
				Vector3 posTmp = hitInfo.point;
				GameObject objHit = hitInfo.collider.gameObject;
				if (objHit.layer == LayerMask.NameToLayer(GameCtrlXK.WaterLayer)) {
					posTmp = hitInfo.point + new Vector3(0f, HighOffset, 0f); //Hit water
				}
				NpcTran.position = posTmp;
			}
		}
	}

	void CloseRootAction()
	{
		SetNpcAnimatorAction("IsRoot_1", 0, AnimatorNpc.speed);
		SetNpcAnimatorAction("IsRoot_2", 0, AnimatorNpc.speed);
		StopNpcRootAudio();
	}

	void MoveNpcToEndPoint()
	{
		Vector3 markPos = NpcPathTran.GetChild(NextMarker).position;
		markPos = GetAimMarkPos( NpcPathTran.GetChild(NextMarker) );

		Vector3 vecA = NpcTran.position;
		Vector3 vecB = markPos;
		vecA.y = vecB.y = 0f;
		float dis = Vector3.Distance(vecA, vecB);
		if(dis < MinDistance && NextMarker < NpcPathTran.childCount)
		{
			Transform markTran = NpcPathTran.GetChild(NextMarker);
			ResetIsGetMarkPos();
			NextMarker++;

			MarkScripte = markTran.GetComponent<NpcMark>();
			if(MarkScripte.IsDoRoot)
			{
				IsDoRootAction = true;
				AnimatorNpc.speed = 1f;
				//SetNpcAnimatorAction("IsRun_2", 0, AnimatorNpc.speed);
				RandomDoRootAction();
			}
			else
			{
				RandPlayRunAction();
			}
		}
		
		if(NextMarker >= NpcPathTran.childCount)
		{
			IsBackStartPoint = true;
			PreMarker = NextMarker - 2;
			NextMarker = 1;
		}
		else
		{
			if(IsDoRootAction)
			{
				return;
			}

			Vector3 mv = markPos - NpcTran.position;
			mv.y = 0f;
			NpcTran.forward = Vector3.Lerp(NpcTran.forward, mv, MoveSpeed * Time.deltaTime);
			
			mv = mv.normalized * MoveSpeed * Time.deltaTime;
			NpcTran.Translate(mv.x, 0f, mv.z, Space.World);

			ApplyGravity();
		}
	}

	void MoveNpcToStartPoint()
	{
		Vector3 markPos = NpcPathTran.GetChild(PreMarker).position;
		markPos = GetAimMarkPos( NpcPathTran.GetChild(PreMarker) );

		Vector3 vecA = NpcTran.position;
		Vector3 vecB = markPos;
		vecA.y = vecB.y = 0f;
		float dis = Vector3.Distance(vecA, vecB);
		if(dis < MinDistance && PreMarker > -1)
		{
			Transform markTran = NpcPathTran.GetChild(PreMarker);
			ResetIsGetMarkPos();
			PreMarker--;

			MarkScripte = markTran.GetComponent<NpcMark>();
			if(MarkScripte.IsDoRoot)
			{
				IsDoRootAction = true;
				AnimatorNpc.speed = 1f;
				//SetNpcAnimatorAction("IsRun_2", 0, AnimatorNpc.speed);
				RandomDoRootAction();
			}
			else
			{
				RandPlayRunAction();
			}
		}
		
		if(PreMarker <= -1)
		{
			IsBackStartPoint = false;
			PreMarker = 0;
		}
		else
		{
			if(IsDoRootAction)
			{
				return;
			}

			Vector3 mv = markPos - NpcTran.position;
			mv.y = 0f;
			NpcTran.forward = Vector3.Lerp(NpcTran.forward, mv, MoveSpeed * Time.deltaTime);
			
			mv = mv.normalized * MoveSpeed * Time.deltaTime;
			NpcTran.Translate(mv.x, 0f, mv.z, Space.World);
			
			ApplyGravity();
		}
	}
	
	void HandleLoopMoveNPC()
	{
		if(!IsLoopMovePath)
		{
			return;
		}
		
		if(IsDoRootAction)
		{
			RootTimeVal += Time.deltaTime;
			if(RootTimeVal < MarkScripte.RootAniTime)
			{
				return;
			}
			
			IsDoRootAction = false;
			RootTimeVal = 0f;
			AnimatorNpc.speed = RunAniSpeed;
			CloseRootAction();
			
			RandPlayRunAction();
		}

		if(!IsBackStartPoint)
		{
			MoveNpcToEndPoint();
		}
		else
		{
			MoveNpcToStartPoint();
		}
	}

	public void RemoveNpcObj(bool isSpawnNpcPoint)
	{
		if(NPC_Type == NPC_STATE.ZAI_TI_NPC || NPC_Type == NPC_STATE.BOSS_NPC)
		{
			if(NpcTran != null)
			{
				if (isSpawnNpcPoint) {
					IsDeadNpc = true;
				}
				else {
					int max = ZaiTiScript.Length;
					for(int i = 0; i < max; i++)
					{
						if(ZaiTiScript[i] != null)
						{
							Destroy(ZaiTiScript[i].gameObject);
						}
					}
					Destroy(gameObject);
				}
			}
		}
		else
		{
			HiddenNpcObj();
		}
	}

	void HiddenNpcObj()
	{
		if (gameObject == null) {
			return;
		}

		if (GlobalData.GetInstance().gameMode == GameMode.SoloMode) {
			Destroy(gameObject);
		}
		else {
			if (Network.peerType != NetworkPeerType.Disconnected) {
				netView.RPC("SendHiddenNetNpcObj", RPCMode.OthersBuffered);
			}
			gameObject.SetActive(false);
		}
	}

	[RPC]
	void SendHiddenNetNpcObj()
	{
		gameObject.SetActive(false);
	}

	void CheckHitZhangAiObj()
	{
		if(NPC_Type != NPC_STATE.ZAI_TI_NPC)
		{
			return;
		}

		RaycastHit hit;
		Vector3 startPos = NpcTran.position + (Vector3.up * NpcTran.localScale.y * 0.4f) + (NpcTran.forward * NpcTran.localScale.z * 0.55f);
		startPos += NpcTran.right * NpcTran.localScale.x * (Random.Range(-0.5f, 0.5f));

		if (Physics.Raycast(startPos, NpcTran.forward, out hit, 20.0f))
		{
			Transform colTran = hit.collider.transform;
			switch (hit.collider.tag) {
			case "NpcPlayer":
				NpcMoveCtrl npcScript = colTran.GetComponent<NpcMoveCtrl>();
				if(npcScript == null)
				{
					return;
				}
				
				if(npcScript.NPC_Type != NPC_STATE.ZAI_TI_NPC)
				{
					return;
				}
				return;
			}
		}
	}

	void ActiveBuWaWa()
	{
		if(BuWaWaObj == null || BuWaWaObj.activeSelf || AnimatorNpc == null || !AnimatorNpc.enabled)
		{
			return;
		}

		AnimatorNpc.enabled = false;
		if (IsMoveByITween && NpcTran.GetComponent<iTween>() != null) {
			//Debug.LogError("***************test " + NpcTran.name);
			Vector3 [] nodesArray = new Vector3[2];
			nodesArray[0] = NpcTran.position;
			
			Transform attackTran = WaterwheelPlayer;
			Vector3 offset = Vector3.zero;
			offset.x = Random.Range(-3f, 3f);
			offset.y = Random.Range(0.5f, 1.5f);
			offset.z = Random.Range(17f, 26f);
			
			nodesArray[1] = attackTran.position + offset.x * attackTran.right + offset.z * attackTran.forward
				+ offset.y * attackTran.up;

			if (!IsInvoking("DelayActiveBuWaWa")) {
				Invoke("DelayActiveBuWaWa", 0.1f);
			}
			iTween.MoveTo(gameObject, iTween.Hash("path", nodesArray,
			                                      "time", 0.6f,
			                                      "orienttopath", false,
			                                      "easeType", iTween.EaseType.linear));
		}
		else {
			if (!IsInvoking("DelayActiveBuWaWa")) {
				Invoke("DelayActiveBuWaWa", 0.1f);
			}

			Vector3 [] nodesArray = new Vector3[2];
			nodesArray[0] = NpcTran.position;
			nodesArray[1] = NpcTran.position + Vector3.up * 0.5f;
			iTween.MoveTo(gameObject, iTween.Hash("path", nodesArray,
			                                      "time", 0.3f,
			                                      "orienttopath", false,
			                                      "easeType", iTween.EaseType.linear));
		}
	}

	void DelayActiveBuWaWa()
	{
		BuWaWaObj.SetActive(true);
		Rigidbody rigObj = BuWaWaObj.GetComponent<Rigidbody>();
		
		Vector3 vecDir = Vector3.Lerp(-NpcTran.forward, Vector3.up, GameCtrlXK.GetInstance().NpcBuWaWaVal);
		if (GameCtrlXK.PlayerTran != null) {
			Vector3 vecA = NpcTran.position - GameCtrlXK.PlayerTran.position;
			vecA.y = 0f;
			vecDir = Vector3.Lerp(vecA.normalized, Vector3.up, GameCtrlXK.GetInstance().NpcBuWaWaVal);
		}
		rigObj.AddForce(vecDir * BuWaWaPowerVal, ForceMode.Impulse);
	}

	void PlayActionRun_2()
	{
		if (NPC_Type != NPC_STATE.SHUI_QIANG_NPC) {
			return;
		}
		CloseRootAction();
		SetNpcAnimatorAction("IsRun_3", 0, AnimatorNpc.speed);
		SetNpcAnimatorAction("IsRun_2", 1, AnimatorNpc.speed);
	}

	void PlayActionRun_3()
	{
		if (NPC_Type != NPC_STATE.SHUI_QIANG_NPC) {
			return;
		}
		CloseRootAction();
		CloseRunAction();
		IsPlayRun_3 = true;
		SetNpcAnimatorAction("IsRun_3", 1, AnimatorNpc.speed);
	}

	void CloseRunAction()
	{
		if (NPC_Type == NPC_STATE.SHUI_QIANG_NPC) {
			SetNpcAnimatorAction("IsRun_3", 0, AnimatorNpc.speed);
		}
		SetNpcAnimatorAction("IsRun_2", 0, AnimatorNpc.speed);
	}

	void RandPlayRunAction()
	{
		if(AnimatorNpc == null)
		{
			return;
		}

		if (IsPlayRun_3 && NPC_Type == NPC_STATE.SHUI_QIANG_NPC) {
			PlayActionRun_3();
			return;
		}

		if(Random.Range(0, 10000) % 2 == 0)
		{
			CloseRunAction();
		}
		else
		{
			SetNpcAnimatorAction("IsRun_2", 1, AnimatorNpc.speed);
		}
	}

	public void SetShuiQiangNpcFlyTime(float val)
	{
		ShuiQiangNpcFlyTime = val;
	}

	public void SetNpcPathTran(Transform tranNpcPathVal, float speedVal, bool isLoop, float fireDisVal,
	                           NpcRunState runStateVal, GameObject objAiMarkVal, bool isNiXingVal)
	{
		SetIsShuiLeiNpc();
		if(tranNpcPathVal == null && NPC_Type == NPC_STATE.DI_MIAN_NPC)
		{
			return;
		}

		if (speedVal < 1f) {
			speedVal = 1f;
		}

		if(NPC_Type == NPC_STATE.SHUI_QIANG_NPC)
		{
			runStateVal = NpcRunState.RUN_1;
			objAiMarkVal = null;
			isNiXingVal = false;
			isLoop = false;
		}
		MoveSpeed = speedVal;
		NpcPathTran = tranNpcPathVal;
		NpcAiMarkTran = objAiMarkVal != null ? objAiMarkVal.transform : null;
		NpcAiPathTran = NpcAiMarkTran != null ? NpcAiMarkTran.parent : null;
		IsNiXingAiPath = isNiXingVal;

		if(AnimatorNpc)
		{
			RunAniSpeed = 1f;
			AnimatorNpc.speed = 1f;

			switch(runStateVal)
			{
			case NpcRunState.RUN_1:
				CloseRunAction();
				break;

			case NpcRunState.RUN_2:
				SetNpcAnimatorAction("IsRun_2", 1, AnimatorNpc.speed);
				break;

			default:
				RandPlayRunAction();
				break;
			}
		}

		if (GlobalData.GetInstance().gameMode == GameMode.SoloMode) {
			WaterwheelPlayer = WaterwheelPlayerCtrl.GetInstance().transform;
		}
		else {
			WaterwheelPlayer = WaterwheelPlayerNetCtrl.GetInstance().transform;
		}

		FireDistance = fireDisVal;
		if (fireDisVal > 0f) {
			IsFireNpc = true;
		}

		if(NPC_Type == NPC_STATE.FEI_XING_NPC)
		{
			SetFlyNpcAimCubeTran();
			isLoop = false;
		}
		else if (NPC_Type == NPC_STATE.ZAI_TI_NPC)
		{
			isLoop = false;
			if (NpcPathTran == null) {
				IsMoveToAiPath = true;
				AiMark aiMarkScript = NpcAiMarkTran.GetComponent<AiMark>();
				NextMarker = aiMarkScript.getMarkCount();
				PreMarker = NextMarker;
				
				if (IsDanCheLvNpc) {
					MakeDanCheLvNpcDoRootAction();
				}
			}

			if (IsFireNpc)
			{
				int max = ZaiTiScript.Length;
				for (int i = 0; i < max; i++) {
					if (ZaiTiScript[i] != null) {
						ZaiTiScript[i].SetIsFireNpc();
					}
				}
			}

			if (!IsNiXingAiPath) {
				if (!IsDiLeiNpc && !IsDanCheLvNpc) {					
					MoveSpeed = Random.Range(WaterwheelPlayerCtrl.mMaxVelocityFootMS * 0.3f, WaterwheelPlayerCtrl.mMaxVelocityFootMS * 0.5f);
				}
				FillZaiTiZhengXingObj();
			}
			else {
				FillZaiTiNiXingObj();
			}
		}

		IsLoopMovePath = isLoop;
		IsMoveNpc = true;
	}

	void PlayFireAction()
	{
		if(NPC_Type == NPC_STATE.ZAI_TI_NPC || NPC_Type == NPC_STATE.BOSS_NPC)
		{
			if (NpcAiPathTran != null) {
				IsMoveNpc = true;
			}

			if(!IsDoFireAction)
			{
				int max = ZaiTiScript.Length;
				for(int i = 0; i < max; i++)
				{
					if(ZaiTiScript[i] != null)
					{
						ZaiTiScript[i].PlayZaiNpcFireAction();
					}
				}
			}
			IsDoFireAction = true;
			return;
		}
		else
		{
			IsDoFireAction = true;
			IsMoveNpc = false;
		}

		if (!IsDonotAimPlayerFire) {
			transform.LookAt(WaterwheelPlayer);
		}

		AnimatorNpc.speed = 1f;
		CloseRunAction();
		CloseRootAction();
		RandomDoFireAction();
	}

	void RandomDoFireAction()
	{
		if (Random.Range(0, 100) % 2 == 0) {
			SetNpcAnimatorAction("IsFire_2", 0, AnimatorNpc.speed);
			SetNpcAnimatorAction("IsFire_1", 1, AnimatorNpc.speed);
		}
		else {
			SetNpcAnimatorAction("IsFire_1", 0, AnimatorNpc.speed);
			SetNpcAnimatorAction("IsFire_2", 1, AnimatorNpc.speed);
		}

		if (IsTeShuZiDanShuiQiang) {
			NpcSimpleBulletObj.SetActive(true);
		}

		if (NPC_Type == NPC_STATE.SHUI_QIANG_NPC) {
			
			RaycastHit hitInfo;
			Vector3 startPos = NpcTran.position + Vector3.up * 5f;
			Vector3 forwardVal = Vector3.down;
			Physics.Raycast(startPos, forwardVal, out hitInfo, 30f, GameCtrlXK.GetInstance().NpcVertHitLayer.value);
			if (hitInfo.collider != null){
				
				Vector3 posTmp = hitInfo.point;
				GameObject objHit = hitInfo.collider.gameObject;
				if (objHit.layer == LayerMask.NameToLayer(GameCtrlXK.WaterLayer)) {
					posTmp = hitInfo.point + new Vector3(0f, HighOffset, 0f); //Hit water
				}
				NpcTran.position = posTmp;
			}
		}
	}

	public void OnHitWaterwheelPlayer()
	{
		IsMoveNpc = false;
		ActiveBuWaWa();
		Invoke("DelaySpawnExplode", GameCtrlXK.GetInstance().TimeNpcSpawnExplode);
	}

	void DelaySpawnExplode()
	{
		Transform tran = BuWaWaObj.transform;
		Instantiate(ExplodeObj, tran.position, tran.rotation);
		Destroy(gameObject, 0.1f);
	}

	public void MoveNpcByITween(Transform npcMarkTranVal)
	{
		if (npcMarkTranVal == null) {
			return;
		}

		NpcMark markScript = npcMarkTranVal.GetComponent<NpcMark>();
		if (markScript == null) {
			return;
		}

		iTween.MoveTo(gameObject, iTween.Hash("path", markScript.GetFlyNodes().ToArray(),
		                                      "time", ShuiQiangNpcFlyTime,
		                                      "orienttopath", true,
		                                      "looktime", 0.6f,
		                                      "easeType", iTween.EaseType.linear,
		                                      "oncomplete", "ShuiQiangNpcOnCompelteITween"));
	}

	void ShuiQiangNpcOnCompelteITween()
	{
		NextMarker++;
		PlayActionRun_3();
		AudioListCtrl.PlayAudio(AudioListCtrl.GetInstance().AudioShuiQiangIntoWater);
		IsMoveByITween = false;
	}

	public void OnZaiTiNpcDead()
	{
		int max = ZaiTiScript.Length;
		ZaiTiNpcDeadNum++;
		if (ZaiTiNpcDeadNum >= max) {
			IsDeadNpc = true;
		}
		else
		{
			int numTmp = 0;
			for (int i = 0; i < max; i++) {
				if (ZaiTiScript[i] != null) {
					numTmp++;
				}
			}

			if (numTmp <= 1) {
				IsDeadNpc = true;
			}
		}

		if (IsDeadNpc) {
			StopAudioSourceBoss();
			if (IsDanCheLvNpc || IsDiLeiNpc) {
				Invoke("HiddenNpcObj", 2f);
			}
		}
	}

	void CheckZaiTiNpcPos(GameObject colObj, int key)
	{
		if (key != 0) {
			return;
		}
		NpcMoveCtrl colObjScript = colObj.GetComponent<NpcMoveCtrl>();

		if (!colObjScript.GetIsCheckPlayerPos() || !IsCheckPlayerPos) {
			return;
		}

		if (Random.Range(0, 100) % 2 == 0) {
			CloseCheckPlayerPos();
		}
		else {
			colObjScript.CloseCheckPlayerPos();
		}
	}

	public bool GetIsCheckPlayerPos()
	{
		return IsCheckPlayerPos;
	}

	void CloseCheckPlayerPos()
	{
		if (!IsCheckPlayerPos) {
			return;
		}
		//Debug.Log("CloseCheckPlayerPos*********");
		IsCheckPlayerPos = false;
		OldMoveSpeed = MoveSpeed;
		MoveSpeed = 0f;
		Invoke("OpenCheckPlayerPos", 1f);
	}

	void OpenCheckPlayerPos()
	{
		IsCheckPlayerPos = true;
		MoveSpeed = OldMoveSpeed;
	}

	void SetIsShuiLeiNpc()
	{
		if (NPC_Type == NPC_STATE.ZAI_TI_NPC || NPC_Type == NPC_STATE.BOSS_NPC) {
			for (int i = 0; i < ZaiTiScript.Length; i++) {
				if (ZaiTiScript[i].IsDiLeiNpc || IsDanCheLvNpc) {
					if (IsDanCheLvNpc) {
						ZaiTiScript[i].SetIsDanCheLvNpc();
					}
					IsDiLeiNpc = true;
					break;
				}
			}
		}
	}

	void CheckPlayerPos()
	{
		if (NPC_Type != NPC_STATE.ZAI_TI_NPC
		    || IsDanCheLvNpc
		    || IsDiLeiNpc) {
			return;
		}

		if (IsNiXingAiPath || !IsMoveToAiPath) {
			return;
		}

		if (!IsCheckPlayerPos) {
			return;
		}

		if (IsDeadNpc) {
			MoveSpeed = Random.Range(WaterwheelPlayerCtrl.mMaxVelocityFootMS * 0.2f, WaterwheelPlayerCtrl.mMaxVelocityFootMS * 0.5f);
			return;
		}

		if (NextMarker >= NpcAiPathTran.childCount) {
			return;
		}

		if (CameraShake.GetInstance().GetIsActiveHuanYingFu()
		    || IntoPuBuCtrl.IsIntoPuBu) {
			if (MoveSpeed >= 20f) {
				MoveSpeed = Random.Range(WaterwheelPlayerCtrl.mMaxVelocityFootMS * 0.2f, WaterwheelPlayerCtrl.mMaxVelocityFootMS * 0.5f);
			}
			return;
		}

		Transform npcAimMark = NpcAiPathTran.GetChild(NextMarker);
		Transform playerAimMark = WaterwheelPlayerCtrl.GetInstance().GetAimMarkTran();
		if (playerAimMark == null) {
			return;
		}

		AiPathCtrl npcAiPathScript = NpcAiPathTran.GetComponent<AiPathCtrl>();
		AiPathCtrl playerAiPathScript = playerAimMark.parent.GetComponent<AiPathCtrl>();
		int npcPathKey = npcAiPathScript.KeyState;
		int playerPathKey = playerAiPathScript.KeyState;

		int AiPathIdP = playerAimMark.parent.GetInstanceID();
		int pathId = npcAimMark.parent.GetInstanceID();
		if (AiPathIdP == pathId) {
			AiMark markScript = npcAimMark.GetComponent<AiMark>();
			AiMark markScriptP = playerAimMark.GetComponent<AiMark>();
			int markCount = markScript.getMarkCount();
			int markCountP = markScriptP.getMarkCount();
			if (markCount <= markCountP) {
				//Npc should add speed
				ChangeNpcMoveSpeed(true);
				return;
			}
			else if (markCount > markCountP) {
				ChangeNpcMoveSpeed(false);
				return;
			}
		}
		else
		{
			if (npcPathKey <= playerPathKey) {
				//Npc should add speed
				ChangeNpcMoveSpeed(true);
				return;
			}
			else if (npcPathKey > playerPathKey) {
				ChangeNpcMoveSpeed(false);
				return;
			}
		}
	}

	public float GetMoveSpeed()
	{
		return MoveSpeed;
	}

	void ChangeNpcMoveSpeed(bool isAddSpeed)
	{
		float playerSpeed = PlayerAutoFire.PlayerMvSpeed;
		if (isAddSpeed) {
			if (MoveSpeed <= playerSpeed) {
				if (playerSpeed > 5) {
					MoveSpeed = Random.Range(WaterwheelPlayerCtrl.mMaxVelocityFootMS * 0.5f, WaterwheelPlayerCtrl.mMaxVelocityFootMS * 1.3f);
				}
				else
				{
					MoveSpeed = WaterwheelPlayerCtrl.mMaxVelocityFootMS * 0.3f;
				}
				//Debug.Log("ChangeNpcMoveSpeed: isAddSpeed " + isAddSpeed + ", MoveSpeed " + MoveSpeed);
			}
		}
		else
		{
			if (MoveSpeed >= playerSpeed) {
				MoveSpeed = Random.Range(WaterwheelPlayerCtrl.mMaxVelocityFootMS * 0.2f, WaterwheelPlayerCtrl.mMaxVelocityFootMS * 0.5f);
				//Debug.Log("ChangeNpcMoveSpeed: isAddSpeed " + isAddSpeed + ", MoveSpeed " + MoveSpeed);
			}
		}
	}
	
	void CheckNpcDisCamera()
	{
		Vector3 posA = TranCamera.position;
		Vector3 posB = NpcTran.position;
		posA.y = 0f;
		posB.y = 0f;
		if (Vector3.Distance(posA, posB) < 5f) {
			return;
		}
		
		Vector3 vecA = TranCamera.forward;
		Vector3 vecB = posB - posA;
		vecA.y = 0f;
		vecB.y = 0f;
		if (Vector3.Dot(vecA, vecB) < 0f) {
			//Debug.Log("CheckNpcDisCamera ***** " + gameObject.name);
			if (NPC_Type == NPC_STATE.ZAI_TI_NPC) {
				OnBecameInvisible();
			}
			else {
				HiddenNpcObj();
			}
		}
	}

	void CreateAudioSourceNpc()
	{
		if (AudioState == AudioNpcState.NULL) {
			return;
		}
		
		AudioSourceNpc = gameObject.AddComponent<AudioSource>();
		AudioSourceNpc.loop = false;
		AudioSourceNpc.playOnAwake = false;
		AudioSourceNpc.Stop();
		switch (AudioState) {
		case AudioNpcState.Xiong:
			AudioSourceNpc.clip = AudioListCtrl.GetInstance().AudioXiong;
			break;
		case AudioNpcState.ShiZi:
			AudioSourceNpc.clip = AudioListCtrl.GetInstance().AudioShiZi;
			break;
		case AudioNpcState.LaoHu:
			AudioSourceNpc.clip = AudioListCtrl.GetInstance().AudioLaoHu;
			break;
		case AudioNpcState.ChangJingLu:
			AudioSourceNpc.clip = AudioListCtrl.GetInstance().AudioChangJingLu;
			break;
		}
	}
	
	void PlayNpcRootAudio()
	{
		if (AudioState == AudioNpcState.NULL) {
			return;
		}
		
		if (!AudioSourceNpc.isPlaying) {
			AudioSourceNpc.Play();
		}
	}
	
	void StopNpcRootAudio()
	{
		if (AudioState == AudioNpcState.NULL) {
			return;
		}
		
		if (AudioSourceNpc.isPlaying) {
			AudioSourceNpc.Stop();
		}
	}
	
	void CreateAudioSourceBoss()
	{
		if (NPC_Type != NPC_STATE.BOSS_NPC) {
			return;
		}
		AudioSourceBoss = gameObject.AddComponent<AudioSource>();
		AudioSourceBoss.clip = AudioListCtrl.GetInstance().AudioBossShip;
		AudioSourceBoss.loop = true;
		AudioSourceBoss.Play();
	}
	
	void StopAudioSourceBoss()
	{
		if (RigObj.isKinematic) {
			RigObj.isKinematic = false;
		}

		if (AudioSourceBoss == null) {
			return;
		}
		AudioSourceBoss.Stop();
	}
	
	void CheckIsFireNpcAction()
	{
		if (!IsMoveNpc) {
			if (IsFireNpc && NPC_Type != NPC_STATE.ZAI_TI_NPC && !IsDonotAimPlayerFire) {
				transform.LookAt(WaterwheelPlayer.position);
			}
			return;
		}
		
		if (IsMoveByITween) {
			return;
		}
		
		if (IsFireNpc) {
			float disPlayer = Vector3.Distance(transform.position, WaterwheelPlayer.position);
			if(disPlayer <= FireDistance)
			{
				if (NPC_Type == NPC_STATE.DaEYu_ZXiong_NPC) {
					MakeNpcMoveToPlayer();
				}
				else {
					PlayFireAction();
				}
			}
		}
	}
	
	void ResetActionInfo()
	{
		if (AnimatorNpc == null) {
			return;
		}
		SetNpcAnimatorAction("IsRoot_1", 0, AnimatorNpc.speed);
		SetNpcAnimatorAction("IsRoot_2", 0, AnimatorNpc.speed);
		SetNpcAnimatorAction("IsRun_2", 0, AnimatorNpc.speed);
		if (NPC_Type == NPC_STATE.SHUI_QIANG_NPC) {
			SetNpcAnimatorAction("IsRun_3", 0, AnimatorNpc.speed);
		}
		SetNpcAnimatorAction("IsFire_1", 0, AnimatorNpc.speed);
		SetNpcAnimatorAction("IsFire_2", 0, AnimatorNpc.speed);
	}
	
	void CheckDistancePlayerCross()
	{	
		if (!PlayerAutoFire.IsPlayerFire) {
			return;
		}
		
		if (IsPlayerFireNpc) {
			return;
		}

		Vector3 startPos = NpcTran.position;
		Vector3 endPos = Camera.main.transform.position;
		Vector3 forVal = endPos - startPos;
		forVal= forVal.normalized;
		startPos += forVal * 2f;
		startPos.y += 1.2f;
		float disVal = Vector3.Distance(startPos, endPos);
		if (GlobalData.GetInstance().IsActiveJuLiFu) {
			if (disVal > 90f) {
				return;
			}
		}
		else {
			if (disVal > 50f) {
				return;
			}
		}
		//Debug.DrawLine(startPos, endPos, Color.red); //test

		RaycastHit hitInfo;
		Physics.Raycast(startPos, forVal, out hitInfo, disVal, GameCtrlXK.GetInstance().NpcAmmoHitLayer.value);
		if (hitInfo.collider != null
		    && hitInfo.collider.gameObject != WaterwheelPlayer.gameObject
		    && hitInfo.collider.gameObject != gameObject){
			//Debug.DrawLine(startPos, hitInfo.point, Color.blue); //test
			return;
		}

		Vector3 vecA = Camera.main.transform.forward;
		Vector3 vecB = NpcTran.position - Camera.main.transform.position;
		vecA.y = vecB.y = 0f;
		if (Vector3.Dot(vecA, vecB) < 0f) {
			return;
		}

		Vector3 posTmp = NpcTran.position;
		Vector3 posA = pcvr.CrossPosition;
		posTmp += 0.6f * NpcTran.up * BoxColSizeYVal;
		Vector3 posB = Camera.main.WorldToScreenPoint(posTmp);
		posB.y = 768f - posB.y;
		posB.z = 0f;
		
		posA.y = 768f - posA.y;
		
		float dx = Mathf.Abs(posA.x - posB.x);
		float dy = Mathf.Abs(posA.y - posB.y);
		if (GlobalData.GetInstance().IsActiveJuLiFu) {
			float crossY = 198f;
			if (dx < (0.7f * crossY) && dy < crossY) {
				IsPlayerFireNpc = true;
				HandleHitShootedPlayer(WaterwheelPlayer.gameObject, 1);
			}
		}
		else {
			float crossY = 98f;
			if (dx < (0.7f * crossY) && dy < crossY) {
				IsPlayerFireNpc = true;
				HandleHitShootedPlayer(WaterwheelPlayer.gameObject, 1);
			}
		}
	}
	
	void PlayNpcAudio(GameObject audioObj)
	{
		if (audioObj == null) {
			return;
		}

		if (!audioObj.activeSelf) {
			audioObj.SetActive(true);
		}
		
		AudioSource ad = audioObj.GetComponent<AudioSource>();
		ad.Play();
	}
}