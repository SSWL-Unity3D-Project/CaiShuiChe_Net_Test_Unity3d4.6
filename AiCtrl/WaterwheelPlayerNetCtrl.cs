using UnityEngine;
using System.Collections;

public class WaterwheelPlayerNetCtrl : MonoBehaviour {

	public GameObject EmptyObj;
	public GameObject FlyNpcAimCube_1; //Fire point
	public GameObject FlyNpcAimCube_2; //aim point
	
	public GameObject PlayerBoxColObj;
	WaterwheelPlayerData WaterPlayerDt;

	GameObject JuLiFuObjTeXiao;
	GameObject HuanYingFuTeXiao;
	public GameObject HuanYingFengXiaoObj;
	
	public Transform CamPointBackFar;
	public Transform CamPointBackNear;
	public Transform CamAimPoint;

	Transform JuLiFuTeXiaoTran;
	public Transform CamPointFirst;
	public Transform CamPointForward;
	public Transform CamPointRight;
	public Transform CamPointUp;
	public Transform ScreenWaterParticle;

	Vector3 PositionCur;
	Quaternion RotationCur;

	Transform PlayerTran;
	Rigidbody RigObj;
	public static float PlayerZhuanXiangVal = 90f;
	float mSteer;
	float OldSteer;
	float mSteerTimeCur;
	const float maxSteerTime = 5f;
	
	bool bIsTurnLeft;
	bool bIsTurnRight;
	
	int mGameTime;
	
	static bool bIsMouseDown_P1;
	static bool bIsMouseDown_P2;
	static float mMouseDownTime_P1;
	static float mMouseDownTime_P2;
	static float MaxMouseDownCount_P1 = 9500f;
	
	public float mSpeed;
	float mThrottleForce;
	float currentEnginePower;
	float mMaxMouseDownCount;
	static float MouseDownCountP_1;
	static float MouseDownCountP_2;
	
	bool IsHitFuBingObj;
	int KillFireNpcNum;
	public static int mMaxVelocityFoot = 90;
	Transform AimMarkTran;
	Transform AiPathCtrlTran;

	bool IsCheckMoreNextPath;
	Transform ParentPath;
	AiPathCtrl AiParentPathScript;
	
	Vector3 mBakeTimePointPos;
	Vector3 mBakeTimePointRot;
	
	bool IsActiveXieTong;
	const float XieTongSpeed = 30f;
	
	bool IsActiveShenXingMode;
	const float ShenXingSpeed = 100f;
	
	bool IsActiveBingLu;
	const float BingLuSpeed = 80f;

	bool IsHandlePlayer;
	NetworkView netView;
	PlayerAutoFire AutoFireScript;

	bool IsXuanYunState;
	float XunYunSpeed = 10f;
	bool IsRunToEndPoint;

	PlayerAimMarkData AimMarkDataScript;
	bool IsActiveJuLiFu;
	bool IsActiveHuanYingFu;
	bool IsActiveDingShenFu;
	bool IsActiveHuanWeiFu;
	bool IsActiveDianDaoFu;
	bool IsStopMovePlayer;
	
	GameObject DianDaoFuSprite;
	static Vector3 JuLiFuSpritePosOffset = new Vector3(0f, 1f, 0f);
	
	bool IsDonotTurnRight;
	bool IsDonotTurnLeft;
	ChuanShenCtrl ChuanShenScript;
	ChuanLunZiCtrl ChuanLunZiScript;
	ZhuJiaoNan ZhuJiaoNanScript;
	float TimeCheckServerPort;
	bool IsSetServerPortClientCount;
	public static WaterwheelPlayerNetCtrl DamagePlayer;
	bool IsDelayActiveGuanWeiFu;

	public static WaterwheelPlayerNetCtrl _Instance;
	public static WaterwheelPlayerNetCtrl GetInstance()
	{
		return _Instance;
	}

	// Use this for initialization
	void Start() 	
	{
		//_Instance = this;
		pcvr.DongGanState = 1;
		DamagePlayer = null;
		ChuanLunZiScript = GetComponentInChildren<ChuanLunZiCtrl>();
		ChuanShenScript = GetComponentInChildren<ChuanShenCtrl>();
		ZhuJiaoNanScript = GetComponentInChildren<ZhuJiaoNan>();
		PlayerZhuanXiangVal = GameCtrlXK.PlayerZhuanXiangPTVal;

		WaterPlayerDt = GetComponentInChildren<WaterwheelPlayerData>();
		if (WaterPlayerDt != null) {
			DianDaoFuSprite = WaterPlayerDt.DianDaoFuSprite;
			JuLiFuObjTeXiao = WaterPlayerDt.JuLiFuObjTeXiao;
			HuanYingFuTeXiao = WaterPlayerDt.HuanYingFuTeXiao;
		}

		if (DianDaoFuSprite == null) {
			ScreenLog.LogError("*****************DianDaoFuSprite is null, name -> "+gameObject.name);
		}
		else {
			if (DianDaoFuSprite != null) {
				DianDaoFuSprite.SetActive(false);
			}
		}

		if (JuLiFuObjTeXiao != null) {
			JuLiFuObjTeXiao.SetActive(false);
		}
		ScreenWaterParticle.gameObject.SetActive(false);

		netView = GetComponent<NetworkView>();
		if (AimMarkDataScript == null) {			
			AimMarkDataScript = gameObject.AddComponent<PlayerAimMarkData>();
		}
		AimMarkDataScript.SetIsPlayer();
		if (AutoFireScript == null) {
			AutoFireScript = GetComponent<PlayerAutoFire>();
		}

		PlayerTran = transform;
		RigObj = GetComponent<Rigidbody>();
		RigObj.isKinematic = true;

		//AniWaterwheelPlayer = GetComponent<Animator>();
		if (GetComponent<Animator>() != null) {
			Debug.LogWarning("Player Animator should be remove");
			PlayerTran = null;
			PlayerTran.name = "null";
		}
		SetRankPlayerArray();
		
		mGameTime = 1000;
		//SetCamAimInfo();
		CreatePlayerNeedObj();
		
		AiPathCtrlTran = GameCtrlXK.GetInstance().AiPathCtrlTran.GetChild(0);
		AutoFireScript.SetPathKeyState( AiPathCtrlTran.GetComponent<AiPathCtrl>() );
		AimMarkTran = AiPathCtrlTran.GetChild(0);
		if (AimMarkTran != null)
		{
			AiMark markAimScript = AimMarkTran.GetComponent<AiMark>();
			AiPathCtrl pathScript = AiPathCtrlTran.GetComponent<AiPathCtrl>();
			SetPlayerAimMarkData(pathScript.PathIndex, markAimScript.getMarkCount(), PlayerTran.name);
		}

		mBakeTimePointPos = AimMarkTran.position;
		mBakeTimePointRot = AimMarkTran.forward;
		AutoFireScript.SetPlayerPreMark(AimMarkTran);

		CreateFlyNpcAimCube();
		
		if (GameCtrlXK.GetInstance().PlayerMarkTest != null)
		{
			AiPathCtrlTran = GameCtrlXK.GetInstance().PlayerMarkTest.parent;
			AutoFireScript.SetPathKeyState( AiPathCtrlTran.GetComponent<AiPathCtrl>() );
			AimMarkTran = GameCtrlXK.GetInstance().PlayerMarkTest;
			mBakeTimePointPos = AimMarkTran.position;
			mBakeTimePointRot = AimMarkTran.forward;
			AutoFireScript.SetPlayerPreMark(AimMarkTran);
			
			PlayerTran.position = mBakeTimePointPos;
			PlayerTran.forward = mBakeTimePointRot;
		}
	}
	
	void FixedUpdate()
	{
		if (!IsHandlePlayer) {
			return;
		}
		
		if (!MoveCameraByPath.IsMovePlayer) {
			return;
		}
		//CheckWaterwheelPlayerSpeed();

		if (AimMarkDataScript != null && AimMarkDataScript.GetIsActiveDingShen()) {
			//DingShenTeXiao ChuLi
			if (!RigObj.isKinematic) {
				RigObj.isKinematic = true;
			}
			return;
		}
		
		if (mGameTime > 0) {
			setBikeMouseDown();
			
			GetInput();
			
			CalculateEnginePower();
			
			ApplyThrottle();
			if (Network.peerType != NetworkPeerType.Disconnected) {
				if (PositionCur != PlayerTran.position) {
					PositionCur = PlayerTran.position;
					netView.RPC("SendPlayerPosToOther", RPCMode.OthersBuffered, PositionCur, mSpeed);
				}
				
				if (RotationCur != PlayerTran.rotation) {
					RotationCur = PlayerTran.rotation;
					netView.RPC("SendPlayerRotToOther", RPCMode.OthersBuffered, RotationCur);
				}
			}
		}
	}
	
	void Update()
	{
		if (!IsHandlePlayer) {
			UpdatePlayerOtherPortPos();
			return;
		}

		if (!StartBtCtrl.GetInstanceP1().CheckIsActivePlayer() || IsStopMovePlayer) {
			if (mGameTime != 0) {
				mGameTime = 0;
				StopMovePlayer();
				ResetPlayerInfo();
				CloseHuanYingFuState();
				PlayerAutoFire.HandlePlayerCloseHuanYingFu();
			}
			return;
		}
		
		if (mGameTime == 0) {
			mGameTime = 100;
		}
		checkPlayerMoveDir();

		if (Time.timeScale != 1f) {
			//CheckWaterwheelPlayerSpeed();

			GetInput();
			
			CalculateEnginePower();
			
			ApplyThrottle();
		}
		CheckWaterwheelPlayerSpeed();
		
		if (AutoFireScript.CheckIsBackPlayerOutWater()) {
			//Debug.Log("ResetPlayerPos*************");
			ResetPlayerPos();
		}

		if (IsActiveHuanWeiFu) {
			CheckIsActiveHuanWeiFu();
		}
		else if (IsActiveDingShenFu) {
			CheckIsActiveDingShenFu();
		}
		else if (IsActiveDianDaoFu) {
			CheckIsActiveDianDaoFu();
		}
	}
	
	void OnTriggerEnter(Collider other)
	{
		if (!IsHandlePlayer) {
			return;
		}
		HandleHitShootObj(other.gameObject, 0);
	}

	public WaterwheelPlayerData GetWaterPlayerDt()
	{
		return WaterPlayerDt;
	}

	public void ShowDianDaoFuSprite()
	{
		if (DianDaoFuSprite == null) {
			ScreenLog.LogError("*****************DianDaoFuSprite is null, name -> "+gameObject.name);
		}
		else {
			if (DianDaoFuSprite.activeSelf) {
				return;
			}
			DianDaoFuSprite.SetActive(true);
			
			if (netView != null && Network.peerType != NetworkPeerType.Disconnected) {
				netView.RPC("SendShowPlayerDianDaoFuSprite", RPCMode.OthersBuffered);
			}
		}
	}

	[RPC]
	void SendShowPlayerDianDaoFuSprite()
	{
		if (DianDaoFuSprite == null) {
			ScreenLog.LogError("*****************DianDaoFuSprite is null, name -> "+gameObject.name);
		}
		else {
			if (DianDaoFuSprite.activeSelf) {
				return;
			}
			DianDaoFuSprite.SetActive(true);
		}
	}

	public void HiddenDianDaoFuSprite()
	{
		if (DianDaoFuSprite == null) {
			ScreenLog.LogError("*****************DianDaoFuSprite is null, name -> "+gameObject.name);
		}
		else {
			if (!DianDaoFuSprite.activeSelf) {
				return;
			}
			DianDaoFuSprite.SetActive(false);
		}

		if (netView != null && Network.peerType != NetworkPeerType.Disconnected) {
			netView.RPC("SendHiddenPlayerDianDaoFuSprite", RPCMode.OthersBuffered);
		}
	}

	[RPC]
	void SendHiddenPlayerDianDaoFuSprite()
	{
		if (DianDaoFuSprite == null) {
			ScreenLog.LogError("*****************DianDaoFuSprite is null, name -> "+gameObject.name);
		}
		else {
			if (!DianDaoFuSprite.activeSelf) {
				return;
			}
			DianDaoFuSprite.SetActive(false);
		}
	}

	public void ResetPlayerInfo()
	{
		IsActiveDingShenFu = false;
		IsActiveHuanWeiFu = false;
		IsActiveDianDaoFu = false;
		HiddenJuLiFuSprite();
		HiddenDianDaoFuSprite();
	}

	public void StopMovePlayer()
	{
		IsStopMovePlayer = true;
		AutoFireScript.CloseWaterParticle();

		RankingCtrl.GetInstance().StopCheckPlayerRank();
		InsertCoinCtrl.GetInstanceP2().HiddenInsertCoin();
		StartBtCtrl.GetInstanceP2().CloseStartBtCartoon();
		HeadCtrlPlayer.GetInstanceP1().SetHeadColor();
		HeadCtrlPlayer.GetInstanceP2().SetHeadColor();

		StartBtCtrl.GetInstanceP1().ResetIsActivePlayer();
		StartBtCtrl.GetInstanceP2().ResetIsActivePlayer();
	}

	public int GetPlayerRankNo()
	{
		return AimMarkDataScript.RankNo + 1;
	}

	public void SetChuanLunZiAction(float val)
	{
		netView.RPC("SendChuanLunZiAction", RPCMode.OthersBuffered, val);
	}

	[RPC]
	void SendChuanLunZiAction(float val)
	{
		if (ChuanLunZiScript == null) {
			return;
		}
		ChuanLunZiScript.UpdateChuanLunZiAction(val);
	}

	public void PlayZhuJiaoNanWinAction()
	{
		ZhuJiaoNanScript.PlayWinAction();
	}

	public void PlayZhuJiaoNanFailAction()
	{
		ZhuJiaoNanScript.PlayFailAction();
	}

	public void PlayZhuJiaoNanAction(string action)
	{
		netView.RPC("SendPlayZhuJiaoNanAction", RPCMode.OthersBuffered, action);
	}
	
	[RPC]
	void SendPlayZhuJiaoNanAction(string action)
	{
		ZhuJiaoNanScript.PlayZhuJiaoNanAction(action);
	}

	public void SetZhuJiaoNanAction(int turnLeftVal, int turnRightVal)
	{
		netView.RPC("SendZhuJiaoNanAction", RPCMode.OthersBuffered, turnLeftVal, turnRightVal);
	}

	[RPC]
	void SendZhuJiaoNanAction(int turnLeftVal, int turnRightVal)
	{
		ZhuJiaoNanScript.SetZhuJiaoNanAction(turnLeftVal, turnRightVal);
	}

	[RPC]
	void SendPlayerRotToOther(Quaternion rot)
	{
		if (PlayerTran != null) {
			PlayerTran.rotation = rot;
		}
		else {
			PlayerTran = transform;
			PlayerTran.rotation = rot;
		}
	}
	
	[RPC]
	void SendPlayerPosToOther(Vector3 pos, float speedVal)
	{
		if (PlayerTran != null) {
			PositionCur = pos;
			mSpeed = speedVal;
		}
		else {
			PlayerTran = transform;
			mSpeed = speedVal;
		}
	}

	void UpdatePlayerOtherPortPos()
	{
		if (!MoveCameraByPath.IsMovePlayer) {
			return;
		}
		PlayerTran.position = Vector3.Lerp(PlayerTran.position, PositionCur, (mSpeed * Time.deltaTime) / 3.6f);
	}

	void SetRankPlayerArray()
	{
		RankingCtrl.GetInstance().SetRankPlayerArray(gameObject);
	}

	public void CheckServerPortPlayerLoop()
	{
		TimeCheckServerPort = Time.realtimeSinceStartup;
		StartCoroutine(CheckServerPortPlayer());
	}

	IEnumerator CheckServerPortPlayer()
	{
		while (NetworkRpcMsgCtrl.MaxLinkServerCount != NetworkRpcMsgCtrl.NoLinkClientCount) {

			if (Time.realtimeSinceStartup - TimeCheckServerPort > 5f) {
				Debug.Log("Time over CheckServerPortPlayer*********");
				break;
			}

			Debug.Log("MaxLinkServerCount " + NetworkRpcMsgCtrl.MaxLinkServerCount
			          + ", NoLinkClientCount " + NetworkRpcMsgCtrl.NoLinkClientCount);
			yield return new WaitForSeconds(0.5f);
		}
		Debug.Log("MaxLinkServerCount " + NetworkRpcMsgCtrl.MaxLinkServerCount
		          + ", NoLinkClientCount " + NetworkRpcMsgCtrl.NoLinkClientCount);
		SendShowAllCamera();
	}

	[RPC]
	void SetServerPortClientCount()
	{
		if (!Network.isServer) {
			return;
		}

		if (!IsSetServerPortClientCount) {
			IsSetServerPortClientCount = false;
			NetworkRpcMsgCtrl.NoLinkClientCount = 0;
		}
		NetworkRpcMsgCtrl.NoLinkClientCount++;
	}

	public void SetIsHandlePlayer()
	{
		_Instance = this;
		IsHandlePlayer = true;

		SetCamAimInfo();
		if (netView == null) {
			netView = GetComponent<NetworkView>();
		}

		if (Network.isServer) {
			CheckServerPortPlayerLoop();
		}
		else {
			netView.RPC("SetServerPortClientCount", RPCMode.OthersBuffered);
		}

		
		PlayerNameCtrl nameCtrlScript = GetComponentInChildren<PlayerNameCtrl>();
		if (nameCtrlScript != null) {
			nameCtrlScript.gameObject.SetActive(false);
		}
		StartCoroutine( RankingCtrl.GetInstance().SetRankListUISpriteColor(gameObject.name) );
	}

	[RPC]
	void HandlePlayerRpcInfo()
	{
		AutoFireScript = GetComponent<PlayerAutoFire>();
		AutoFireScript.enabled = false;
	}

	public void SetPlayerNetworkPlayer(NetworkPlayer netPlayer)
	{
		if (netView == null) {
			netView = GetComponent<NetworkView>();
		}

		if (Network.peerType != NetworkPeerType.Disconnected) {
			netView.RPC("SendPlayerNetworkPlayer", RPCMode.OthersBuffered, netPlayer);
		}
	}

	[RPC]
	void SendPlayerNetworkPlayer(NetworkPlayer netPlayer)
	{
		if (netPlayer != Network.player) {
			return;
		}
		SetIsHandlePlayer();
	}

	public void SendShowAllCamera()
	{
		NetworkRpcMsgCtrl.MaxLinkServerCount = 100;
		NetworkRpcMsgCtrl.NoLinkClientCount = 10;

		if (Network.peerType != NetworkPeerType.Disconnected) {
			netView.RPC("SendOtherLinkerShowAllCamera", RPCMode.OthersBuffered);
		}
		GameCtrlXK.GetInstance().ShowAllCameras();
	}
	
	[RPC]
	void SendOtherLinkerShowAllCamera()
	{
		NetworkRpcMsgCtrl.MaxLinkServerCount = 100;
		NetworkRpcMsgCtrl.NoLinkClientCount = 10;
		GameCtrlXK.GetInstance().ShowAllCameras();
	}

	public void RemovePlayerNetObj()
	{
		if (Network.peerType != NetworkPeerType.Disconnected) {
			netView.RPC("SendRemovePlayerNetObj", RPCMode.OthersBuffered);
		}
		Network.Destroy(gameObject);
	}

	[RPC]
	void SendRemovePlayerNetObj()
	{
		if (gameObject == null) {
			return;
		}
		Network.Destroy(gameObject);
	}

	public void CloseXuanYunState()
	{
		IsXuanYunState = false;
		DianDaoFuSprite.SetActive(false);
		BeiGongJiInfoCtrl.GetInstance().HiddenGongJiInfo();
	}

	public void ActiveXuanYunState()
	{
		if (IsXuanYunState) {
			return;
		}

		DianDaoFuSprite.SetActive(true);
		IsXuanYunState = true;
		CancelInvoke("CloseXuanYunState");
		Invoke("CloseXuanYunState", 3f);
		if (Network.peerType != NetworkPeerType.Disconnected) {
			netView.RPC("SendPlayerActiveXuanYun", RPCMode.OthersBuffered);
		}
	}
	
	[RPC]
	void SendPlayerActiveXuanYun()
	{
		DianDaoFuSprite.SetActive(true);
		IsXuanYunState = true;
		CancelInvoke("CloseXuanYunState");
		Invoke("CloseXuanYunState", 3f);

		if (IsHandlePlayer) {
			XunYunSpeed = 0.1f * mSpeed;
			if (XunYunSpeed < 10f) {
				XunYunSpeed = 10f;
			}
			currentEnginePower = 0.05f * currentEnginePower;

			BeiGongJiInfoCtrl.GetInstance().ShowGongJiInfo();
		}
	}

	public void ShootingDeadObj(GameObject obj)
	{
		HandleHitShootObj(obj, 1);
	}

	void TestActiveDianDaoFu()
	{
		ActiveDaJuCtrl.SetTypeDaoJuList((int)DaoJuTypeIndex.dianDaoFu, 1);
		GameCtrlXK.GetInstance().ActivePlayerDaoJuType((int)DaoJuTypeIndex.dianDaoFu);
		HitDianDaoFuObj();
	}

	///<summary>
	/// player hit obj key -> 0, player shooting obj key -> 1
	///</summary>
	void HandleHitShootObj(GameObject obj, int key)
	{
		switch(obj.tag)
		{
		case "TengManObj":
			if(key == 0)
			{
				TengManInfoCtrl.GetInstance().ShowTengManInfo();
			}
			break;
			
		case "FuBingObj":
			if(key == 0)
			{
				ActiveIsHitFuBingObj();
			}
			break;

		case "IntoBingLu":
			if(key == 0)
			{
				ActiveBingLuTrigger();
			}
			break;
			
		case "OutBingLu":
			if(key == 0)
			{
				CloseBingLuTrigger();
			}
			break;
			
		case "HuanWeiFuObj":
			ActiveDaJuCtrl.SetTypeDaoJuList((int)DaoJuTypeIndex.huanWeiFu, 1);
			GameCtrlXK.GetInstance().ActivePlayerDaoJuType((int)DaoJuTypeIndex.huanWeiFu);
			ShowDaoJuExplosion(obj);
			HitHuanWeiFuObj();
			break;
			
		case "HuanYingFuObj":
			ActiveDaJuCtrl.SetTypeDaoJuList((int)DaoJuTypeIndex.huanYingFu, 1);
			GameCtrlXK.GetInstance().ActivePlayerDaoJuType((int)DaoJuTypeIndex.huanYingFu);
			ShowDaoJuExplosion(obj);
			NengLiangQuanCtrl.GetInstance().MoveNengLiangQuanToEnd(DaoJuTypeIndex.huanYingFu);
			DaoJuTiShiCtrl.GetInstance().ShowDaoJuTiShi(DaoJuState.HuanYingFu);
			PlayerZhuanXiangVal = GameCtrlXK.PlayerZhuanXiangJSVal;
			break;
			
		case "JuLiFuObj":
			ActiveDaJuCtrl.SetTypeDaoJuList((int)DaoJuTypeIndex.juLiFu, 1);
			GameCtrlXK.GetInstance().ActivePlayerDaoJuType((int)DaoJuTypeIndex.juLiFu);
			ShowDaoJuExplosion(obj);
			ActiveJuLiFuState();
			break;
			
		case "DianDaoFuObj":
			ActiveDaJuCtrl.SetTypeDaoJuList((int)DaoJuTypeIndex.dianDaoFu, 1);
			GameCtrlXK.GetInstance().ActivePlayerDaoJuType((int)DaoJuTypeIndex.dianDaoFu);
			ShowDaoJuExplosion(obj);
			HitDianDaoFuObj();
			break;
			
		case "DingShenFuObj":
			ActiveDaJuCtrl.SetTypeDaoJuList((int)DaoJuTypeIndex.dingShenFu, 1);
			GameCtrlXK.GetInstance().ActivePlayerDaoJuType((int)DaoJuTypeIndex.dingShenFu);
			ShowDaoJuExplosion(obj);
			HitDingShenFuObj();
			DaoJuTiShiCtrl.GetInstance().ShowDaoJuTiShi(DaoJuState.DingShenFu);
			break;
		}
	}
	
	void ShowDaoJuExplosion(GameObject daoJuObj)
	{
		if (daoJuObj == null) {
			return;
		}
		
		Transform daoJuTran = daoJuObj.transform;
		Instantiate(GameNetCtrlXK.GetInstance().DaoJuExplosionObj, daoJuTran.position, daoJuTran.rotation);
		NpcHealthCtrl healthScript = daoJuObj.GetComponent<NpcHealthCtrl>();
		if (healthScript != null) {
			healthScript.RemoveGameObj();
		}
	}
	
	void HitHuanWeiFuObj()
	{
		ActiveHuanWeiFuState();
	}

	void ActiveHuanWeiFuState()
	{
		if (IsActiveHuanWeiFu) {
			return;
		}
		IsActiveHuanWeiFu = true;
		ResetDaoJuState(DaoJuState.HuanWeiFu);
	}

	void CloseHuanWeiFuState()
	{
		if (!IsActiveHuanWeiFu) {
			return;
		}
		IsActiveHuanWeiFu = false;
		
		AudioListCtrl.PlayAudio(AudioListCtrl.GetInstance().AudioActiveDaoJu);
		ActiveDaJuCtrl.SetTypeDaoJuList((int)DaoJuTypeIndex.huanWeiFu, 0);
		GameCtrlXK.GetInstance().ActivePlayerDaoJuType((int)DaoJuTypeIndex.Close);
	}

	void CheckIsActiveHuanWeiFu()
	{
		if (!IsActiveHuanWeiFu) {
			return;
		}

		PlayerAimMarkData aimMarkScript = GetPlayerAimMarkScript(0);
		if (aimMarkScript != null && !IsDelayActiveGuanWeiFu) {
			IsDelayActiveGuanWeiFu = true;
			aimMarkScript.OpenHuanWeiFuTeXiao();
			AimMarkDataScript.OpenHuanWeiFuTeXiao();
			StartCoroutine(DelayActiveHuanWeiFu(aimMarkScript));
		}
	}

	IEnumerator DelayActiveHuanWeiFu(PlayerAimMarkData aimMarkScript)
	{
		yield return new WaitForSeconds(0.5f);

		IsDelayActiveGuanWeiFu = false;
		if (aimMarkScript != null) {
			float offsetPy = 1.3f;
			Vector3 forVal = Vector3.zero;
			Vector3 posTmp = aimMarkScript.transform.position;
			forVal = aimMarkScript.transform.forward;
			AiPathCtrl pathScript = AiPathCtrlTran.GetComponent<AiPathCtrl>();
			AiMark markScript = AimMarkTran.GetComponent<AiMark>();
			
			Transform pathTranTmp = null;
			Transform markTranTmp = null;
			if (aimMarkScript.AiNetScript != null) {
				pathTranTmp = aimMarkScript.AiNetScript.GetAiPathCtrlTran();
				markTranTmp = aimMarkScript.AiNetScript.GetAimMarkTran();
			}
			else if (aimMarkScript.playerNetScript) {
				pathTranTmp = aimMarkScript.playerNetScript.GetAiPathCtrlTran();
				markTranTmp = aimMarkScript.playerNetScript.GetAimMarkTran();
			}

			Vector3 playerPosTmp = PlayerTran.position;
			playerPosTmp.y += offsetPy;
			aimMarkScript.ActiveHuanWeiState(PlayerTran.position, pathScript.PathIndex, markScript.getMarkCount());
			
			//HuanWeiTeXiao ChuLi
			if (pathTranTmp != null && markTranTmp != null) {
				AiPathCtrlTran = pathTranTmp;
				AimMarkTran = markTranTmp;
				
				AiMark markAimScript = AimMarkTran.GetComponent<AiMark>();
				pathScript = AiPathCtrlTran.GetComponent<AiPathCtrl>();
				SetPlayerAimMarkData(pathScript.PathIndex, markAimScript.getMarkCount(), PlayerTran.name);
			}
			
			posTmp.y += offsetPy;
			PlayerTran.position = posTmp;
			if (AimMarkTran != null) {
				forVal = AimMarkTran.position - posTmp;
				forVal.y = 0f;
				forVal = forVal.normalized;
			}
			PlayerTran.forward = forVal;
			CloseHuanWeiFuState();

			if (Network.peerType != NetworkPeerType.Disconnected) {
				RotationCur = PlayerTran.rotation;
				PositionCur = PlayerTran.position;
				netView.RPC("SendBackOtherPortTranform", RPCMode.OthersBuffered, PositionCur, RotationCur);
			}
		}
	}
	
	void HitDingShenFuObj()
	{
		ActiveDingShenFuState();
	}

	void CheckIsActiveDingShenFu()
	{
		if (!IsActiveDingShenFu) {
			return;
		}

		PlayerAimMarkData aimMarkScript = GetPlayerAimMarkScript(0);
		if (aimMarkScript != null) {
			aimMarkScript.ActiveDingShenState();
			CloseDingShenFuState();
		}
	}

	/// <summary>
	/// Gets the player aim mark script. if keyState == 1 is ActiveDianDaoFu, else is other.
	/// </summary>
	/// <returns>The player aim mark script.</returns>
	/// <param name="keyState">Key.</param>
	PlayerAimMarkData GetPlayerAimMarkScript(int keyState)
	{
		int rankNo = AimMarkDataScript.RankNo;
		if (rankNo < 1) {
			return null;
		}

		float dis = 0f;
		Transform parTran = null;
		Vector3 playerPos = PlayerTran.position;
		Vector3 parPos = playerPos;
		playerPos.y = 0f;
		
		Vector3 vecA = PlayerTran.forward;
		Vector3 vecB = parPos - playerPos;
		
		float angle = 45f;
		float key = angle / 180f;
		float cosAng = Mathf.Cos(Mathf.PI * key);
		float cosAB = 0f;

		bool isFindObj = false;
		for (int i = rankNo - 1; i >= 0; i--) {
			if (RankingCtrl.mRankPlayer[i].Player == null) {
				continue;
			}

			if (RankingCtrl.mRankPlayer[i].Player == gameObject) {
				continue;
			}

			if (IsActiveDianDaoFu
			    && keyState == 1
			    && !RankingCtrl.mRankPlayer[i].IsPlayer) {
				//Debug.Log("test ********* name " + RankingCtrl.mRankPlayer[i].Name);
				continue;
			}
			
			parTran = RankingCtrl.mRankPlayer[i].Player.transform;
			parPos = parTran.position;
			parPos.y = 0f;
			dis = Vector3.Distance(parPos, playerPos);
			if (IsActiveHuanWeiFu) {
				if (dis > 100 || dis < 50) {
					continue;
				}
			}
			else {
				if (dis > GlobalData.GetInstance().MinDistancePlayer || dis < 20) {
					continue;
				}
			}
			
			vecB = parPos - playerPos;
			vecB = vecB.normalized;
			cosAB = Vector3.Dot(vecA, vecB);
			if (cosAB >= cosAng) {
				isFindObj = true;
				break;
			}
		}

		if (parTran != null && isFindObj) {
			return parTran.GetComponent<PlayerAimMarkData>();
		}
		return null;
	}

	void ActiveDingShenFuState()
	{
		if (IsActiveDingShenFu) {
			return;
		}
		IsActiveDingShenFu = true;
		ResetDaoJuState(DaoJuState.DingShenFu);
	}

	void CloseDingShenFuState()
	{
		if (!IsActiveDingShenFu) {
			return;
		}
		IsActiveDingShenFu = false;
		
		AudioListCtrl.PlayAudio(AudioListCtrl.GetInstance().AudioActiveDaoJu);
		ActiveDaJuCtrl.SetTypeDaoJuList((int)DaoJuTypeIndex.dingShenFu, 0);
		GameCtrlXK.GetInstance().ActivePlayerDaoJuType((int)DaoJuTypeIndex.Close);
	}

	void HitDianDaoFuObj()
	{
		ActiveDianDaoFuState();
	}

	void ActiveDianDaoFuState()
	{
		if ( (Network.peerType == NetworkPeerType.Server && Network.connections.Length < RankingCtrl.ServerPlayerRankNum)
		    || Network.peerType == NetworkPeerType.Disconnected ) {
			return;
		}

		if (IsActiveDianDaoFu) {
			return;
		}
		IsActiveDianDaoFu = true;
		ResetDaoJuState(DaoJuState.DianDaoFu);
	}

	void CloseDianDaoFuState()
	{
		if (!IsActiveDianDaoFu) {
			return;
		}
		IsActiveDianDaoFu = false;
		
		AudioListCtrl.PlayAudio(AudioListCtrl.GetInstance().AudioActiveDaoJu);
		ActiveDaJuCtrl.SetTypeDaoJuList((int)DaoJuTypeIndex.dianDaoFu, 0);
		GameCtrlXK.GetInstance().ActivePlayerDaoJuType((int)DaoJuTypeIndex.Close);
	}
	
	void CheckIsActiveDianDaoFu()
	{
		if (!IsActiveDianDaoFu) {
			return;
		}

		PlayerAimMarkData aimMarkScript = GetPlayerAimMarkScript(1);
		if (aimMarkScript != null) {
			aimMarkScript.ActiveDianDaoState();
			CloseDianDaoFuState();
		}
	}

	public void HiddenJuLiFuSprite()
	{
		if (!JuLiFuObjTeXiao.activeSelf) {
			return;
		}
		JuLiFuObjTeXiao.SetActive(false);
		
		if (netView != null && Network.peerType != NetworkPeerType.Disconnected) {
			netView.RPC("SendHiddenPlayerJuLiFuSprite", RPCMode.OthersBuffered);
		}
	}
	
	[RPC]
	void SendHiddenPlayerJuLiFuSprite()
	{
		if (!JuLiFuObjTeXiao.activeSelf) {
			return;
		}
		JuLiFuObjTeXiao.SetActive(false);
	}

	void OpenJuLiFuTeXiao()
	{
		if (JuLiFuObjTeXiao.activeSelf) {
			return;
		}

		if (JuLiFuTeXiaoTran == null) {
			JuLiFuTeXiaoTran = JuLiFuObjTeXiao.transform;
			JuLiFuTeXiaoTran.parent = GameCtrlXK.MissionCleanup;
		}
		JuLiFuTeXiaoTran.position = PlayerTran.position + JuLiFuSpritePosOffset;
		JuLiFuObjTeXiao.SetActive(true);
	}

	void ActiveJuLiFuState()
	{
		if(IsActiveJuLiFu)
		{
			return;
		}
		IsActiveJuLiFu = true;

		OpenJuLiFuTeXiao();
		AudioListCtrl.PlayAudio(AudioListCtrl.GetInstance().AudioJuLiFu);
		GlobalData.GetInstance().IsActiveJuLiFu = true;
		Invoke("CloseJuLiFuState", GameCtrlXK.GetInstance().ActiveJuLiFuTime);

		if (Network.peerType != NetworkPeerType.Disconnected) {
			netView.RPC("SendOpenPlayerJuLiFuTeXiao", RPCMode.OthersBuffered);
		}
	}

	[RPC]
	void SendOpenPlayerJuLiFuTeXiao()
	{
		OpenJuLiFuTeXiao();
	}

	[RPC]
	void SendClosePlayerJuLiFuTeXiao()
	{
		if (!JuLiFuObjTeXiao.activeSelf) {
			return;
		}
		JuLiFuObjTeXiao.SetActive(false);
	}

	void CloseJuLiFuState()
	{
		if(!IsActiveJuLiFu)
		{
			return;
		}
		IsActiveJuLiFu = false;
		JuLiFuObjTeXiao.SetActive(false);
		GlobalData.GetInstance().IsActiveJuLiFu = false;
		ActiveDaJuCtrl.SetTypeDaoJuList((int)DaoJuTypeIndex.juLiFu, 0);
		GameCtrlXK.GetInstance().ActivePlayerDaoJuType((int)DaoJuTypeIndex.Close);

		if (Network.peerType != NetworkPeerType.Disconnected) {
			netView.RPC("SendClosePlayerJuLiFuTeXiao", RPCMode.OthersBuffered);
		}
	}

	void UpdateJuLiFuObjTeXiao()
	{
		if (!JuLiFuObjTeXiao.activeSelf) {
			return;
		}

		if (JuLiFuTeXiaoTran == null) {
			JuLiFuTeXiaoTran = JuLiFuObjTeXiao.transform;
		}

		JuLiFuTeXiaoTran.position = PlayerTran.position + JuLiFuSpritePosOffset;
		JuLiFuTeXiaoTran.forward = Camera.main.transform.forward;
	}
	
	public void ActiveHuanYingFuState()
	{
		if (IsActiveHuanYingFu) {
			return;
		}
		IsActiveHuanYingFu = true;
		ResetDaoJuState(DaoJuState.HuanYingFu);
		AudioListCtrl.PlayAudio(AudioListCtrl.GetInstance().AudioHuanYingFu);
		//CameraShake.GetInstance().SetRadialBlurActive(true, CameraShake.BlurStrengthHuanYingFu);
		CameraShake.GetInstance().SetIsActiveHuanYingFu(true);
		HuanYingFuTeXiao.SetActive(true);
		HuanYingFengXiaoObj.SetActive(true);
		if (Network.peerType != NetworkPeerType.Disconnected) {
			netView.RPC("SendPlayerHuanYingFuOpen", RPCMode.OthersBuffered);
		}

		XingXingCtrl.IsPlayerCanHitNPC = false;
		PlayerAutoFire.IsActivePlayerForwardHit = true;
		PlayerBoxColObj.layer = LayerMask.NameToLayer("TransparentFX");
		Invoke("CloseHuanYingFuState", GameCtrlXK.GetInstance().JiaSuWuDiTime);
	}

	[RPC]
	void SendPlayerHuanYingFuOpen()
	{
		HuanYingFuTeXiao.SetActive(true);
		HuanYingFengXiaoObj.SetActive(true);
		Invoke("HiddenHuanYingFuTeXiao", GameCtrlXK.GetInstance().JiaSuWuDiTime);
	}

	void HiddenHuanYingFuTeXiao()
	{
		HuanYingFuTeXiao.SetActive(false);
		HuanYingFengXiaoObj.SetActive(false);
	}
	
	public void SetPlayerBoxColliderState(bool isActive)
	{
		if (!IsHandlePlayer) {
			return;
		}
		AutoFireScript.SetPlayerBoxColliderState(isActive);
	}

	public Transform GetHuanYingFuTeXiaoTran()
	{
		return HuanYingFuTeXiao.transform;
	}

	void CloseHuanYingFuState()
	{
		PlayerZhuanXiangVal = GameCtrlXK.PlayerZhuanXiangPTVal;
		if(!IsActiveHuanYingFu)
		{
			return;
		}
		CancelInvoke("CloseHuanYingFuState");
		NengLiangQuanCtrl.GetInstance().MoveNengLiangQuanToStart(DaoJuTypeIndex.huanYingFu);
		CameraShake.GetInstance().SetRadialBlurActive(false, CameraShake.BlurStrengthHuanYingFu);
		CameraShake.GetInstance().SetIsActiveHuanYingFu(false);

		if (!IsInvoking("CloseShenXingState")) {
			XingXingCtrl.IsPlayerCanHitNPC = true;
			PlayerAutoFire.IsActivePlayerForwardHit = false;
			PlayerBoxColObj.layer = LayerMask.NameToLayer("Default");
		}
		HuanYingFuTeXiao.SetActive(false);
		HuanYingFengXiaoObj.SetActive(false);
		IsActiveHuanYingFu = false;
		ActiveDaJuCtrl.SetTypeDaoJuList((int)DaoJuTypeIndex.huanYingFu, 0);
		GameCtrlXK.GetInstance().ActivePlayerDaoJuType((int)DaoJuTypeIndex.Close);
	}
	
	void ActiveXieTongMode()
	{
		if(!StartBtCtrl.GetInstanceP2().CheckIsActivePlayer())
		{
			return;
		}
		
		if(IsActiveXieTong)
		{
			return;
		}
		IsActiveXieTong = true;
		XieTongInfoCtrl.GetInstance().ShowXieTongInfo();
	}
	
	void CloseXieTongMode()
	{
		if(!IsActiveXieTong)
		{
			return;
		}
		IsActiveXieTong = false;
		XieTongInfoCtrl.GetInstance().HiddenXieTongInfo();
	}
	
	void CreateFlyNpcAimCube()
	{
		GameObject ObjTmp;
		ObjTmp = (GameObject)Instantiate(FlyNpcAimCube_1);
		ObjTmp.transform.parent = transform;
		ObjTmp.transform.localPosition = FlyNpcAimCube_1.transform.position;
		ObjTmp.transform.localRotation = FlyNpcAimCube_1.transform.rotation;
		FlyNpcAimCube_1 = ObjTmp;
		FlyNpcAimCube_1.name = "FlyNpcAimCube_1";
		
		ObjTmp = (GameObject)Instantiate(FlyNpcAimCube_2);
		ObjTmp.transform.parent = transform;
		ObjTmp.transform.localPosition = FlyNpcAimCube_2.transform.position;
		ObjTmp.transform.localRotation = FlyNpcAimCube_2.transform.rotation;
		FlyNpcAimCube_2 = ObjTmp;
		FlyNpcAimCube_2.name = "FlyNpcAimCube_2";
	}
	
	void ActiveIsHitFuBingObj()
	{
		CancelInvoke("CloseIsHitFuBingObj");
		Invoke("CloseIsHitFuBingObj", 3f);
		IsHitFuBingObj = true;
	}
	
	void CloseIsHitFuBingObj()
	{
		IsHitFuBingObj = false;
	}

	void CreatePlayerNeedObj()
	{
		GameObject obj = (GameObject)Instantiate(PlayerBoxColObj);
		Transform objTran = obj.transform;
		objTran.parent = transform;
		objTran.localPosition = PlayerBoxColObj.transform.position;
		objTran.localScale = PlayerBoxColObj.transform.localScale;
		objTran.localEulerAngles = PlayerBoxColObj.transform.eulerAngles;
		objTran.name = "PlayerBoxColObj";
		PlayerBoxColObj = obj;
		PlayerBoxColObj.tag = "Player";
	}

	void SetCamAimInfo()
	{
		WaterwheelCameraNetCtrl.GetInstance().setAimPlayerInfo();
		CameraShake.GetInstance().SetCameraPointInfo();
	}
	
	public static void setBikeMouseDown()
	{
		float dTime = 0f;
		if( (!pcvr.bIsHardWare && InputEventCtrl.VerticalVal > 0f || pcvr.IsGetValByKey)
		   || (pcvr.bIsHardWare && pcvr.bPlayerHitTaBan_P1))
		{
			bIsMouseDown_P1 = true;
			if(!pcvr.bIsHardWare || pcvr.IsGetValByKey)
			{
				dTime = Time.time - mMouseDownTime_P1;
				MouseDownCountP_1 = Mathf.FloorToInt((190f / dTime) + 0.5f);
				mMouseDownTime_P1 = Time.time;
			}
			else
			{
				MouseDownCountP_1 = pcvr.TanBanDownCount_P1;
			}
			
			if(MouseDownCountP_1 < 3f)
			{
				MouseDownCountP_1 = 0f;
			}
			else if(MaxMouseDownCount_P1 < MouseDownCountP_1)
			{
				MaxMouseDownCount_P1 = MouseDownCountP_1;
			}
		}
		else
		{
			if(!pcvr.bIsHardWare || pcvr.IsGetValByKey)
			{
				if(bIsMouseDown_P1)
				{
					bIsMouseDown_P1 = false;
				}
				else if (MouseDownCountP_1 > 0f)
				{
					dTime = Time.time - mMouseDownTime_P1;
					if(dTime > 0.1f)
					{
						MouseDownCountP_1 = 0f;
					}
				}
			}
			else
			{
				if(bIsMouseDown_P1)
				{
					bIsMouseDown_P1 = false;
				}
				MouseDownCountP_1 = 0f;
			}
		}
	}

	void GetInput()
	{
		float steerTmp = 0f;
		if (mGameTime > 0) {
			steerTmp = pcvr.mGetSteer;
		}
		else {
			MouseDownCountP_1 = 0f;
			MouseDownCountP_2 = 0f;
		}

		if (IsActiveBingLu) {
			steerTmp *= 3f;
			if (steerTmp > 1f) {
				steerTmp = 1f;
			}
		}
		mSteer = steerTmp;
		
		if(Mathf.Abs(mSteer) < 0.1f)
		{
			mSteer = 0f;
		}
		
		float rotSpeed = PlayerZhuanXiangVal * mSteer * Time.smoothDeltaTime;
		if(mSteer > 0f && !IsDonotTurnRight)
		{
			if (mSteer - OldSteer > 0f) {
				mSteerTimeCur += GameCtrlXK.GetInstance().PlayerSteerKey * maxSteerTime;
				if (mSteerTimeCur > maxSteerTime) {
					mSteerTimeCur = maxSteerTime;
				}
			}
			else if (mSteer - OldSteer < 0f) {
				mSteerTimeCur -= GameCtrlXK.GetInstance().PlayerSteerKey * maxSteerTime;
				if (mSteerTimeCur < 0f) {
					mSteerTimeCur = 0f;
				}
			}
			OldSteer = mSteer;

			if(TengManInfoCtrl.GetInstance().GetIsActiveTengManInfo()
			   || (AimMarkDataScript != null && AimMarkDataScript.GetIsActiveDianDao()) ) {
			   //|| IsXuanYunState) {
				PlayerTran.Rotate(0, -rotSpeed, 0);
			}
			else
			{
				PlayerTran.Rotate(0, rotSpeed, 0);
			}
			
			bIsTurnLeft = false;
			if(!bIsTurnRight)
			{
				bIsTurnRight = true;
				PlayerAutoFire.ActiveIsTurnRight();
				if (mSpeed > 15f && !pcvr.IsPlayerHitShake) {
					pcvr.OpenQiNangZuo();
				}
			}

			if(Mathf.Abs( mSteer ) < 0.4f)
			{
				bIsTurnRight = false;
				if (!pcvr.IsPlayerHitShake) {
					pcvr.CloseQiNangZuo();
				}
			}
		}
		else if(mSteer < 0f && !IsDonotTurnLeft)
		{
			if (mSteer - OldSteer > 0f) {
				mSteerTimeCur += GameCtrlXK.GetInstance().PlayerSteerKey * maxSteerTime;
				if (mSteerTimeCur > 0f) {
					mSteerTimeCur = 0f;
				}
			}
			else if (mSteer - OldSteer < 0f) {
				mSteerTimeCur -= GameCtrlXK.GetInstance().PlayerSteerKey * maxSteerTime;
				if (mSteerTimeCur < -maxSteerTime) {
					mSteerTimeCur = -maxSteerTime;
				}
			}
			OldSteer = mSteer;

			if(TengManInfoCtrl.GetInstance().GetIsActiveTengManInfo()
			   || (AimMarkDataScript != null && AimMarkDataScript.GetIsActiveDianDao()) ) {
			   //|| IsXuanYunState) {
				PlayerTran.Rotate(0, -rotSpeed, 0);
			}
			else
			{
				PlayerTran.Rotate(0, rotSpeed, 0);
			}
			
			bIsTurnRight = false;
			if(!bIsTurnLeft)
			{
				bIsTurnLeft = true;
				PlayerAutoFire.ActiveIsTurnLeft();
				if (mSpeed > 15f && !pcvr.IsPlayerHitShake) {
					pcvr.OpenQiNangYou();
				}
			}

			if(Mathf.Abs( mSteer ) < 0.4f)
			{
				bIsTurnLeft = false;
				if (!pcvr.IsPlayerHitShake) {
					pcvr.CloseQiNangYou();
				}
			}
		}
		else
		{
			OldSteer = 0f;
			mSteerTimeCur = 0f;
			
			PlayerAutoFire.ResetIsTurn();
			bIsTurnLeft = false;
			bIsTurnRight = false;
		}
		ChuanShenScript.UpdateChuanShenAction(bIsTurnLeft, bIsTurnRight);
		ZhuJiaoNanScript.UpdateZhuJiaoNanAction(bIsTurnLeft, bIsTurnRight);

		float maxAngle = 15f;
		Vector3 rotationA = PlayerTran.localEulerAngles;
		if (AutoFireScript.CheckPlayerDownIsHit()) {
			rotationA = PlayerTran.localEulerAngles;
		}

		float angleZ = -(mSteerTimeCur * maxAngle) / maxSteerTime;
		if(angleZ < -maxAngle)
		{
			angleZ = -maxAngle;
		}
		else if(angleZ > maxAngle)
		{
			angleZ = maxAngle;
		}
		rotationA.z = angleZ;
		PlayerTran.localEulerAngles = rotationA;
	}
	
	void CheckWaterwheelPlayerSpeed()
	{
		float speedTmp = 0f;
		speedTmp = rigidbody.velocity.magnitude * 3.6f * Time.timeScale;
		speedTmp *= 0.9f;
		speedTmp = Mathf.FloorToInt(speedTmp);
		
		float mouseDownCountTmp = MouseDownCountP_1 + MouseDownCountP_2;
		float dVal = mSpeed - speedTmp;
		if (dVal > PlayerAutoFire.DisSpeedVal && mouseDownCountTmp > 0f) {
			CameraShake.GetInstance().SetCameraShakeImpulseValue();
			PlayerAutoFire.AddPlayerHitZhangAiNum();
			pcvr.GetInstance().OnPlayerHitShake();
		}
		mSpeed = speedTmp;

		if (!pcvr.IsPlayerHitShake) {
			if (mSpeed > 25f) {
				pcvr.OpenQiNangQian();
			}
			else {
				pcvr.CloseQiNangQian();
			}
		}
		GameCtrlXK.GetInstance().SetPlayerMvSpeedSpriteInfo(speedTmp / (0.65f * mMaxVelocityFoot));

		if (IsRunToEndPoint) {
			speedTmp = 0f;
		}
		ChuanLunZiScript.UpdateChuanLunZiAction(speedTmp);
		SetChuanLunZiAction(speedTmp);

		if (IsStopMovePlayer) {
			AutoFireScript.SetPlayerMvSpeed(0f);
		}
		else {
			AutoFireScript.SetPlayerMvSpeed(mSpeed);
		}
	}

	public bool GetIsHandlePlayer()
	{
		return IsHandlePlayer;
	}

	public float GetMoveSpeed()
	{
		return mSpeed;
	}
	
	void ActiveBingLuTrigger()
	{
		if (IsActiveBingLu) {
			return;
		}
		IsActiveBingLu = true;
	}
	
	void CloseBingLuTrigger()
	{
		if (!IsActiveBingLu) {
			return;
		}
		IsActiveBingLu = false;
	}
	
	void CalculateEnginePower()
	{
		float timeAdd = 200f;
		float timeSub = 10f * timeAdd;
		float dTime = Time.deltaTime;
		
		float mouseDownCountTmp = MouseDownCountP_1 + MouseDownCountP_2;
		if ( mouseDownCountTmp > 0
		    || IsActiveXieTong
		    || IsActiveHuanYingFu
		    || IsXuanYunState
		    || IntoPuBuCtrl.IsIntoPuBu
		    || IsActiveBingLu ) {
			
			float maxVelocity = 0f;
			if(mMaxMouseDownCount < MouseDownCountP_1)
			{
				mMaxMouseDownCount = MouseDownCountP_1;
			}
			
			if(mMaxMouseDownCount < MouseDownCountP_2)
			{
				mMaxMouseDownCount = MouseDownCountP_2;
			}
			
			if(mouseDownCountTmp > mMaxMouseDownCount)
			{
				mouseDownCountTmp = mMaxMouseDownCount;
			}
			
			if (PlayerYueJieCtrl.GetInstance().CheckIsShowPlayerYueJie()) {
				maxVelocity = PlayerYueJieCtrl.YueJieSpeed;
				CloseHuanYingFuState();
			}
			else if (IntoPuBuCtrl.IsIntoPuBu)
			{
				if (mSpeed < IntoPuBuCtrl.PlayerMvSpeed) {
					Vector3 forwardVal = PlayerTran.forward;
					forwardVal.y = 0f;
					RigObj.AddForce(forwardVal * 90000f);
				}
				maxVelocity = IntoPuBuCtrl.PlayerMvSpeed;
			}
			else if (IsXuanYunState && mouseDownCountTmp > 0)
			{
				maxVelocity = XunYunSpeed;
			}
			else if(IsActiveHuanYingFu)
			{
				if (mSpeed < PlayerAutoFire.HuanYingSpeed) {
					RigObj.AddForce(PlayerTran.forward * 90000f);
				}
				maxVelocity = PlayerAutoFire.HuanYingSpeed;
			}
			else if(IsActiveXieTong)
			{
				maxVelocity = XieTongSpeed;
			}
			else if(IsActiveBingLu)
			{
				if (mSpeed < BingLuSpeed) {
					RigObj.AddForce(PlayerTran.forward * 9000f);
				}
				maxVelocity = BingLuSpeed;
			}
			else
			{
				float velocityTmp = (mouseDownCountTmp / mMaxMouseDownCount) * mMaxVelocityFoot;
				if(mouseDownCountTmp > 0)
				{
					maxVelocity = velocityTmp;
				}
				
				if(maxVelocity > mMaxVelocityFoot)
				{
					maxVelocity = mMaxVelocityFoot;
				}
			}
			
			if(mSpeed >= maxVelocity)
			{
				currentEnginePower -= dTime * timeAdd;
			}
			else
			{
				currentEnginePower += dTime * timeAdd;
				if (currentEnginePower < PlayerAutoFire.MinPowerPlayer) {
					currentEnginePower = PlayerAutoFire.MinPowerPlayer;
				}
			}
		}
		else
		{
			if(currentEnginePower < 0)
			{
				currentEnginePower = 0;
			}
			else if(currentEnginePower > 0)
			{
				currentEnginePower -= PlayerAutoFire.SubPlayerEnginePower * dTime * timeSub;
				
				if(currentEnginePower < 0)
				{
					currentEnginePower = 0;
				}
			}
		}
		
		float maxPower = GlobalData.PlayerPowerShipMax;
		if(currentEnginePower > maxPower)
		{
			currentEnginePower = maxPower;
		}
		
		float power = currentEnginePower;
		mThrottleForce = power * rigidbody.mass;
	}

	public void SetcurrentEnginePower(float val)
	{
		currentEnginePower = val;
	}

	void ApplyThrottle()
	{
		if(mThrottleForce <= 0f && mSpeed <= 3f)
		{
			if(!RigObj.isKinematic && !PlayerAutoFire.IsIntoSky && !PlayerAutoFire.IsRestartMove)
			{
				RigObj.isKinematic = true;
			}
			return;
		}
		
		if(RigObj.isKinematic)
		{
			RigObj.isKinematic = false;
		}
		
		if(IsHitFuBingObj)
		{
			mThrottleForce = 300f * RigObj.mass;
		}
		RigObj.AddForce(PlayerTran.forward * Time.deltaTime * mThrottleForce);
	}
	
	public void AddKillFireNpcNum()
	{
		KillFireNpcNum++;
		if(KillFireNpcNum >= 20)
		{
			KillFireNpcNum = 0;
			//GameTimeCtrl.GetInstance().AddGameTime(10); //add time 10s
		}
	}
	
	public void ActiveShenXingState()
	{
		if(AimMarkTran == null)
		{
			return;
		}
		
		if(IsActiveShenXingMode)
		{
			return;
		}
		IsActiveShenXingMode = true;

		XingXingCtrl.IsPlayerCanHitNPC = false;
		PlayerAutoFire.IsActivePlayerForwardHit = true;
		PlayerBoxColObj.layer = LayerMask.NameToLayer("TransparentFX");
		ShenXingInfoCtrl.GetInstance().ShowShenXingInfo();
		Invoke("CloseShenXingState", 6f);
	}
	
	public void ShouldCloseShenXingState()
	{
		CancelInvoke("CloseShenXingState");
		CloseShenXingState();
	}

	void CloseShenXingState()
	{
		if(!IsActiveShenXingMode)
		{
			return;
		}
		IsActiveShenXingMode = false;

		if (!IsInvoking("CloseHuanYingFuState")) {
			XingXingCtrl.IsPlayerCanHitNPC = true;
			PlayerAutoFire.IsActivePlayerForwardHit = false;
			PlayerBoxColObj.layer = LayerMask.NameToLayer("Default");
		}
		ShenXingInfoCtrl.GetInstance().HiddenShenXingInfo();
	}
	
	void SetIsDirWrong(bool isWrong)
	{
		if (isWrong == DirectionInfoCtrl.GetInstance().GetIsActiveDirection())
		{
			return;
		}
		
		if(isWrong)
		{
			CloseHuanYingFuState();
			PlayerAutoFire.ResetIsIntoPuBu();
			PlayerAutoFire.HandlePlayerCloseHuanYingFu();
			DirectionInfoCtrl.GetInstance().ShowDirWrongInfo();
		}
		else
		{
			DirectionInfoCtrl.GetInstance().HiddenDirWrong();
		}
	}
	
	public void ResetPlayerPos()
	{
		IsDonotTurnRight = false;
		IsDonotTurnLeft = false;

		Vector3 posTmp = mBakeTimePointPos;
		posTmp.y += 0.8f;
		PlayerTran.position = mBakeTimePointPos;
		PlayerTran.forward = mBakeTimePointRot;
		PlayerAutoFire.PlayerThrottleForce = mThrottleForce;
		PlayerAutoFire.PlayerCurrentEnginePower = currentEnginePower;
		AutoFireScript.ResetPlayerCameraPos();

		if (Network.peerType != NetworkPeerType.Disconnected) {
			RotationCur = PlayerTran.rotation;
			PositionCur = PlayerTran.position;
			netView.RPC("SendBackOtherPortTranform", RPCMode.OthersBuffered, PositionCur, RotationCur);
		}
	}

	[RPC]
	void SendBackOtherPortTranform(Vector3 pos, Quaternion qua)
	{
		PositionCur = pos;
		RotationCur = qua;
		PlayerTran.position = pos;
		PlayerTran.rotation = qua;
	}

	void SetPlayerPathCount()
	{
		RankingCtrl.GetInstance().SetPlayerPathCount(PlayerTran.name);
		
		if (Network.peerType == NetworkPeerType.Disconnected) {
			return;
		}
		
		netView.RPC("SendPlayerPathCountToOther", RPCMode.OthersBuffered, PlayerTran.name);
	}

	[RPC]
	void SendPlayerPathCountToOther(string playerName)
	{
		RankingCtrl.GetInstance().SetPlayerPathCount(playerName);
	}

	void SetPlayerAimMarkData(int aimPathId, int markId, string playerName)
	{
		RankingCtrl.GetInstance().SetPlayerAimMark(aimPathId, markId, playerName);

		if (Network.peerType == NetworkPeerType.Disconnected) {
			return;
		}
		netView.RPC("SendPlayerAimMarkToOther", RPCMode.OthersBuffered, aimPathId, markId, playerName);
		
		AiPathCtrl pathScript = AiPathCtrlTran.GetComponent<AiPathCtrl>();
		netView.RPC("SendPlayerPathMark", RPCMode.OthersBuffered, pathScript.PathIndex, markId);
	}

	[RPC]
	void SendPlayerAimMarkToOther(int aimPathId, int markId, string playerName)
	{
		RankingCtrl.GetInstance().SetPlayerAimMark(aimPathId, markId, playerName);
	}
		
	[RPC]
	void SendPlayerPathMark(int countPath, int countMark)
	{
		Transform tmpTran = AiPathGroupCtrl.FindAiPathTran(countPath);
		if (tmpTran == null) {
			return;
		}
		AiPathCtrlTran = tmpTran;
		AimMarkTran = AiPathCtrlTran.GetChild(countMark);
	}

	void CheckMoreNextPathDir()
	{
		if (AiParentPathScript == null) {
			return;
		}
		
		bool isDirRight = false;
		//int [] DirTag = new int[3]; //DirTag: 0 -> dir is wrong, 1 -> dir is right.
		Transform [] aimMarkTranArray = new Transform[3];
		aimMarkTranArray[0] = AiParentPathScript.mNextPath1;
		aimMarkTranArray[1] = AiParentPathScript.mNextPath2;
		aimMarkTranArray[2] = AiParentPathScript.mNextPath3;
		
		Transform aimMarkTran = null;
		Vector3 vecA = Vector3.zero;
		Vector3 vecB = Vector3.zero;
		Vector3 vecC = Vector3.zero;
		float cosAC = 0f;
		float cosAB = 0f;
		float cosBC = 0f;
		for (int i = 0; i < 3; i++) {
			if (aimMarkTranArray[i] == null) {
				continue;
			}
			
			aimMarkTran = aimMarkTranArray[i].GetChild(0);
			if(aimMarkTran == null)
			{
				continue;
			}
			
			vecA = aimMarkTran.forward;
			vecB = aimMarkTran.position - PlayerTran.position;
			vecC = PlayerTran.forward;
			vecA.y = vecB.y = vecC.y = 0f;
			cosAC = Vector3.Dot(vecA, vecC);
			cosAB = Vector3.Dot(vecA, vecB);
			cosBC = Vector3.Dot(vecB, vecC);
			
			if(cosAC < 0f && cosBC < 0f) {
				//dir is wrong.
			}
			else {
				
				isDirRight = true;
				float disAB = Vector3.Distance(vecB, Vector3.zero);
				if (cosAB <= 0f && disAB < 15f) {
					IsCheckMoreNextPath = false;
					ParentPath = aimMarkTran.parent;
					AimMarkTran = aimMarkTran;
					
					AiMark markScript = AimMarkTran.GetComponent<AiMark>();
					if(markScript == null)
					{
						continue;
					}
					mBakeTimePointPos = AimMarkTran.position;
					mBakeTimePointRot = AimMarkTran.forward;
					AutoFireScript.SetPlayerPreMark(AimMarkTran);
					
					int conutTmp = AimMarkTran.parent.childCount - 1;
					int markCount = markScript.getMarkCount();
					AimMarkTran = markScript.mNextMark;
					AiPathCtrlTran = AimMarkTran.parent;
					AutoFireScript.SetPathKeyState( AiPathCtrlTran.GetComponent<AiPathCtrl>() );
					
					if(markCount == conutTmp)
					{
						AiPathCtrlTran = AimMarkTran.parent; //next path
						AutoFireScript.SetPathKeyState( AiPathCtrlTran.GetComponent<AiPathCtrl>() );
						
						if (ParentPath != null) {
							AiParentPathScript = ParentPath.GetComponent<AiPathCtrl>();
							if (AiParentPathScript.GetNextPathNum() > 1) {
								IsCheckMoreNextPath = true;
							}
						}
					}
					return;
				}
			}
		}
		
		if (isDirRight && !AutoFireScript.CheckPlayerIsMoveDirWrong()) {
			IsDonotTurnRight = false;
			IsDonotTurnLeft = false;
			SetIsDirWrong(false);
		}
		else {
			if ( !IsDonotTurnRight && !IsDonotTurnLeft
			    && ( AutoFireScript.PathKeyState == 0
			    || IntoPuBuCtrl.IsIntoPuBu
			    || PlayerAutoFire.IsRestartMove) ) {
				if (bIsTurnRight) {
					IsDonotTurnRight = true;
					IsDonotTurnLeft = false;
				}
				else if (bIsTurnLeft) {
					IsDonotTurnRight = false;
					IsDonotTurnLeft = true;
				}
			}
			SetIsDirWrong(true);
		}
	}

	void checkPlayerMoveDir()
	{
		if (IsCheckMoreNextPath) {
			CheckMoreNextPathDir();
			return;
		}

		if(AimMarkTran == null)
		{
			return;
		}
		
		Vector3 vecA = AimMarkTran.forward;
		Vector3 vecB = AimMarkTran.position - PlayerTran.position;
		Vector3 vecC = PlayerTran.forward;
		vecA.y = vecB.y = vecC.y = 0f;
		float cosAC = Vector3.Dot(vecA, vecC);
		float cosAB = Vector3.Dot(vecA, vecB);
		float cosBC = Vector3.Dot(vecB, vecC);
		
		if(cosAC < 0f && cosBC < 0f)
		{
			if ( !IsDonotTurnRight && !IsDonotTurnLeft
			    && ( AutoFireScript.PathKeyState == 0
			    || IntoPuBuCtrl.IsIntoPuBu
			    || PlayerAutoFire.IsRestartMove) ) {
				if (bIsTurnRight) {
					IsDonotTurnRight = true;
					IsDonotTurnLeft = false;
				}
				else if (bIsTurnLeft) {
					IsDonotTurnRight = false;
					IsDonotTurnLeft = true;
				}
			}
			SetIsDirWrong(true);
		}
		else
		{
			IsDonotTurnRight = false;
			IsDonotTurnLeft = false;

			SetIsDirWrong(false);
			if(cosBC <= 0f && cosAB <= 0f)
			{
				ParentPath = AimMarkTran.parent; //next path
				AiMark markScript = AimMarkTran.GetComponent<AiMark>();
				if(markScript == null)
				{
					return;
				}
				
				mBakeTimePointPos = AimMarkTran.position;
				mBakeTimePointRot = AimMarkTran.forward;
				AutoFireScript.SetPlayerPreMark(AimMarkTran);
				
				int conutTmp = AimMarkTran.parent.childCount - 1;
				int markCount = markScript.getMarkCount();
				AimMarkTran = markScript.mNextMark;
				if(markCount == conutTmp)
				{
					SetPlayerPathCount();

					if(AimMarkTran == null)
					{
						//player run to end
						this.enabled = false;
						IsRunToEndPoint = true;
						//IsStopMovePlayer = true;
						StopMovePlayer();
						ResetPlayerInfo();

						AimMarkDataScript.SetIsRunToEndPoint();
						AimMarkDataScript.SetIsGameOver();
						ShowRankPlayerQiZhi();
						
						//DaoJiShiCtrl.GetInstance().StopDaoJiShi();
						//GameOverCtrl.GetInstance().HiddenGameOver();
						if (Network.peerType == NetworkPeerType.Disconnected
						    || (Network.peerType == NetworkPeerType.Server && Network.connections.Length == 0)) {
							
							RankingCtrl.GetInstance().StopCheckPlayerRank();
							GameTimeCtrl.GetInstance().StopRunGameTime();
							//FinishPanelCtrl.GetInstance().ShowFinishPanel();
							AutoFireScript.CloseWaterParticle();

							if (WaterwheelPlayerNetCtrl.GetInstance().GetPlayerRankNo() == 1) {
								FinishPanelCtrl.GetInstance().ShowFinishPanel();
							}
							else {
								FinishPanelCtrl.GetInstancePlayer().ShowFinishPanel();
							}
						}
						else {
							
							GameTimeCtrl.GetInstance().StopRunGameTime();
							if (!DaoJiShiCtrl.GetInstance().CheckIsPlayDaoJiShi()) {
								ShowPlayerNetDaoJiShi();
							}
						}
						
						AutoFireScript.SetPlayerMvSpeed(0f);
						return;
					}
					AiPathCtrlTran = AimMarkTran.parent;
					AutoFireScript.SetPathKeyState( AiPathCtrlTran.GetComponent<AiPathCtrl>() );

					if (ParentPath != null) {
						AiParentPathScript = ParentPath.GetComponent<AiPathCtrl>();
						if (AiParentPathScript.GetNextPathNum() > 1) {
							IsCheckMoreNextPath = true;
						}
					}
				}

				if (AimMarkTran != null)
				{
					AiMark markAimScript = AimMarkTran.GetComponent<AiMark>();
					AiPathCtrl pathScript = AiPathCtrlTran.GetComponent<AiPathCtrl>();
					SetPlayerAimMarkData(pathScript.PathIndex, markAimScript.getMarkCount(), PlayerTran.name);
				}
			}
		}
	}

	void ShowRankPlayerQiZhi()
	{
		PlayerRankNumCtrl.GetInstance().ShowPlayerRankNum(GetPlayerRankNo());
		RankingCtrl.GetInstance().ShowRankPlayerQiZhi();
		if (Network.peerType != NetworkPeerType.Disconnected) {
			netView.RPC("SendPlayerShowRankPlayerQiZhi", RPCMode.OthersBuffered);
		}
	}

	[RPC]
	void SendPlayerShowRankPlayerQiZhi()
	{
		RankingCtrl.GetInstance().ShowRankPlayerQiZhi();
	}

	public bool GetIsRunToEndPoint()
	{
		return IsRunToEndPoint;
	}

	void ShowPlayerNetDaoJiShi()
	{
		DaoJiShiCtrl.GetInstance().InitPlayDaoJiShi();
		if (Network.peerType != NetworkPeerType.Disconnected) {
			netView.RPC("SendShowPlayerNetDaoJiShi", RPCMode.OthersBuffered);
		}
	}

	[RPC]
	void SendShowPlayerNetDaoJiShi()
	{
		if (!DaoJiShiCtrl.GetInstance().CheckIsPlayDaoJiShi()) {
			DaoJiShiCtrl.GetInstance().InitPlayDaoJiShi();
		}
		GameTimeCtrl.GetInstance().StopRunGameTimeNet();
	}
	
	void ActiveCamPointFirst()
	{
		WaterwheelCameraNetCtrl.GetInstance().EnableCamPointFirst();
	}
	
	void CloseCamPointFirst()
	{
		WaterwheelCameraNetCtrl.GetInstance().EnableCamPointBack();
	}
	
	public Transform GetCamPointFirst()
	{
		return CamPointFirst;
	}

	public Transform GetCamPointRight()
	{
		return CamPointRight;
	}
	
	public Transform GetCamPointForward()
	{
		return CamPointForward;
	}
	
	public Transform GetCamPointUp()
	{
		return CamPointUp;
	}
	
	public Transform GetCamPointBackNear()
	{
		return CamPointBackNear;
	}
	
	public Transform GetCamPointBackFar()
	{
		return CamPointBackFar;
	}
	
	public Transform GetCamAimPoint()
	{
		return CamAimPoint;
	}
	
	public Transform GetAimMarkTran()
	{
		return AimMarkTran;
	}

	public Transform GetAiPathCtrlTran()
	{
		return AiPathCtrlTran;
	}

	public void SetActivePlayerGunWaterObj(int key)
	{
		if (key == 1) {
			DamagePlayer = this;
		}
		else {
			DamagePlayer = null;
		}

		if (Network.peerType != NetworkPeerType.Disconnected) {
			netView.RPC("SendActivePlayerGunWaterObj", RPCMode.OthersBuffered, key);
		}
	}

	[RPC]
	void SendActivePlayerGunWaterObj(int key)
	{
		if (!IsHandlePlayer) {
			return;
		}
		bool isActive = key == 0 ? false : true;
		WaterwheelCameraNetCtrl.GetInstance().SetPlayerGunWaterObjActive(isActive);
	}
	
	void ResetDaoJuState(DaoJuState key)
	{
		switch (key) {
		case DaoJuState.DianDaoFu:
			IsActiveDingShenFu = false;
			IsActiveHuanWeiFu = false;
			break;
			
		case DaoJuState.DingShenFu:
			IsActiveHuanWeiFu = false;
			IsActiveDianDaoFu = false;
			break;
			
		case DaoJuState.HuanWeiFu:
			IsActiveDingShenFu = false;
			IsActiveDianDaoFu = false;
			break;

		case DaoJuState.HuanYingFu:
			IsActiveHuanWeiFu = false;
			IsActiveDingShenFu = false;
			IsActiveDianDaoFu = false;
			break;
		}
	}
}