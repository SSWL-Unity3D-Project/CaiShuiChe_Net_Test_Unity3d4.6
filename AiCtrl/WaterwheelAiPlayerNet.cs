using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class WaterwheelAiPlayerNet : MonoBehaviour {

	public GameObject EmptyObj;
	public GameObject PlayerBoxColObj;
	public GameObject DingShenWater;
	public GameObject HuanWeiFuTeXiao;
	public GameObject WaterParticle;
	
	Vector3 PositionCur;
	Quaternion RotationCur;

	GameObject PlayerObj;
	Transform PlayerTran;
	Rigidbody RigObj;

	int mGameTime = 1000;
	float mSpeed;
	public float mThrottleForce;
	public float currentEnginePower = 300f;

	public float AimMarkSpeed;
	public Transform AimMarkTran;
	public Transform AiPathCtrlTran;
	
	Vector3 mBakeTimePointPos;
	Vector3 mBakeTimePointRot;
	
	bool IsActiveXieTong;
	const float XieTongSpeed = 30f;
	
	bool IsActiveShenXingMode;
	const float ShenXingSpeed = 50f;
	
	bool IsActiveBingLu;
	const float BingLuSpeed = 50f;
	
	bool IsActiveJuLiFu;
	bool IsActiveHuanYingFu;
	
	bool IsHandlePlayer;
	NetworkView netView;
	PlayerAutoFire AutoFireScript;

	bool IsRunToEndPoint;
	PlayerAimMarkData AimMarkDataScript;
	int RankNoAi;

	bool IsCheckAiSpeed;
	public float AiSpeedChangeVal;

	float TimeCheckAiPos;
	Vector3 AiPlayerOldPos;
	ChuanShenCtrl ChuanShenScript;
	ZhuJiaoNan ZhuJiaoNanScript;
	
	float TimeApplyAddGravityVal;
	float TimeDownHitVal;
	static int TestPosCount;
	public int StartMarkCount = 0;
	public bool IsActiveHuanWeiPos;
	int CountSpeed;

	// Use this for initialization
	void Start()
	{
		ChuanShenScript = GetComponentInChildren<ChuanShenCtrl>();
		ZhuJiaoNanScript = GetComponentInChildren<ZhuJiaoNan>();

		netView = GetComponent<NetworkView>();
		if (AimMarkDataScript == null) {			
			AimMarkDataScript = gameObject.AddComponent<PlayerAimMarkData>();
		}

		PlayerObj = gameObject;
		PlayerTran = transform;
		RigObj = GetComponent<Rigidbody>();
		RigObj.isKinematic = true;

		SetRankPlayerArray();
		
		mGameTime = 1000;
		CreatePlayerNeedObj();
		
		AiPathCtrlTran = GameCtrlXK.GetInstance().AiPathCtrlTran.GetChild(0);
		AimMarkTran = AiPathCtrlTran.GetChild(0);
		if (AimMarkTran != null)
		{
			AiMark markAimScript = AimMarkTran.GetComponent<AiMark>();
			AiPathCtrl pathScript = AiPathCtrlTran.GetComponent<AiPathCtrl>();
			SetPlayerAimMarkData(pathScript.PathIndex, markAimScript.getMarkCount(), PlayerTran.name);
		}
		
		mBakeTimePointPos = AimMarkTran.position;
		mBakeTimePointPos += Vector3.up * 0.5f;
		mBakeTimePointRot = AimMarkTran.forward;
		
		if (GameCtrlXK.GetInstance().PlayerMarkTest != null)
		{
			AiPathCtrlTran = GameCtrlXK.GetInstance().PlayerMarkTest.parent;
			AiMark markScript = GameCtrlXK.GetInstance().PlayerMarkTest.GetComponent<AiMark>();
			AimMarkTran = markScript.mNextMark;
			if (AimMarkTran != null)
			{
				AiMark markAimScript = AimMarkTran.GetComponent<AiMark>();
				AiPathCtrl pathScript = AiPathCtrlTran.GetComponent<AiPathCtrl>();
				SetPlayerAimMarkData(pathScript.PathIndex, markAimScript.getMarkCount(), PlayerTran.name);
			}
			TestPosCount++;

			Transform posTran = GameCtrlXK.GetInstance().PlayerMarkTest;
			mBakeTimePointRot = posTran.forward;
			mBakeTimePointRot.y = 0f;
			switch(TestPosCount) {
			case 1:
				mBakeTimePointPos = posTran.position - posTran.right * 9f + mBakeTimePointRot;
				break;

			case 2:
				mBakeTimePointPos = posTran.position + posTran.right * 9f + mBakeTimePointRot;
				break;

			case 3:
				mBakeTimePointPos = posTran.position - posTran.right * 6f + mBakeTimePointRot * 4f;
				break;

			case 4:
				mBakeTimePointPos = posTran.position + posTran.right * 6f + mBakeTimePointRot * 4f;
				break;

			case 5:
				mBakeTimePointPos = posTran.position - posTran.right * 3f + mBakeTimePointRot * 7f;
				break;

			case 6:
				mBakeTimePointPos = posTran.position + posTran.right * 3f + mBakeTimePointRot * 7f;
				break;

			case 7:
				mBakeTimePointPos = posTran.position + mBakeTimePointRot * 10f;
				TestPosCount = 0;
				break;
			}
			mBakeTimePointPos += Vector3.up * 0.5f;
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

		if (GameCtrlXK.IsStopMoveAiPlayer) {
			return;
		}

		if (Network.peerType != NetworkPeerType.Disconnected) {
			if (PositionCur != PlayerTran.position) {
				PositionCur = PlayerTran.position;
				netView.RPC("SendAiPlayerPosToOther", RPCMode.OthersBuffered, PositionCur, mSpeed);
			}
			
			if (RotationCur != PlayerTran.rotation) {
				RotationCur = PlayerTran.rotation;
				netView.RPC("SendAiPlayerRotToOther", RPCMode.OthersBuffered, RotationCur);
			}
		}
	}

	public void SetIsRunMoveAiPlayer(bool isRun)
	{
		if (!IsHandlePlayer) {
			return;
		}

		iTween itweenScript = GetComponent<iTween>();
		if (itweenScript == null) {
			return;
		}

		if (itweenScript.isRunning == isRun) {
			return;
		}

		itweenScript.isRunning = isRun;
	}

	public void SetHuanWeiFuActiveInfo(int countPath, int countMark)
	{
		if (IsActiveHuanWeiPos) {
			return;
		}
		IsActiveHuanWeiPos = true;
		AiPathCtrlTran = AiPathGroupCtrl.FindAiPathTran(countPath);
		StartMarkCount = countMark;
		
		if (AiPathCtrlTran != null) {
			AimMarkTran = AiPathCtrlTran.GetChild(StartMarkCount);
			
			AiMark markAimScript = AimMarkTran.GetComponent<AiMark>();
			AiPathCtrl pathScript = AiPathCtrlTran.GetComponent<AiPathCtrl>();
			SetPlayerAimMarkData(pathScript.PathIndex, markAimScript.getMarkCount(), PlayerTran.name);
		}
		
		iTween itweenScript = GetComponent<iTween>();
		if (itweenScript != null) {
			itweenScript.isRunning = false;
			Destroy(itweenScript);
		}
		Invoke("MoveAiPlayerByItween", 0.05f);
	}

	void MoveAiPlayerByItween()
	{
		List<Transform> nodes = new List<Transform>(AiPathCtrlTran.GetComponentsInChildren<Transform>()){};
		nodes.Remove(AiPathCtrlTran);
		if (IsActiveHuanWeiPos) {
			IsActiveHuanWeiPos = false;
			int maxCount = AiPathCtrlTran.childCount;
			if (maxCount > 1 && StartMarkCount > 0 && StartMarkCount < maxCount) {
				for (int i = 0; i < StartMarkCount; i++) {
					nodes.Remove(AiPathCtrlTran.GetChild(i));
				}
			}
		}
		nodes.Insert(0, PlayerTran);

		int max = nodes.Count;
		Vector3[] posArray = new Vector3[max];
		posArray[0] = PlayerTran.position;
		AimMarkSpeed = CheckAiPlayerMoveSpeed();
		mSpeed = AimMarkSpeed;

		float RandVal_x = 0f;
		float RandVal_y = 0f;
		float RandVal_z = 0f;
		for (int i = 1; i < max; i++) {
			RandVal_x = (Random.Range(0, 10000) % 3 - 1) * 0.4f * nodes[i].localScale.x;
			RandVal_z = (Random.Range(0, 10000) % 3 - 1) * 0.4f * nodes[i].localScale.z;

			posArray[i] = nodes[i].position
							+ RandVal_x * nodes[i].right
							+ RandVal_z * nodes[i].forward
							+ RandVal_y * nodes[i].up;
		}
		iTween.MoveTo(PlayerObj, iTween.Hash("speed", AimMarkSpeed,
		                                     "orienttopath", true,
		                                     "easeType", iTween.EaseType.linear,
		                                     "path", posArray,
		                                     "oncomplete", "OnFinishMoveAiPlayerByPath"));
	}

	float CheckAiPlayerMoveSpeed()
	{
		float speed = 0f;
		if (CountSpeed == 0) {
			speed = Random.Range(0.1f * WaterwheelPlayerNetCtrl.mMaxVelocityFoot, 0.3f * WaterwheelPlayerNetCtrl.mMaxVelocityFoot);
		}
		else {
			int PlayerOneNo = PlayerRankData.PlayerOneDt.RankNo;
			float speedA = 0f;
			float speedB = 0f;
			if (RankNoAi < PlayerOneNo) {
				speedA = GameNetCtrlXK.GetInstance().AiMinMoveSpeedA * WaterwheelPlayerNetCtrl.mMaxVelocityFoot;
				speedB = GameNetCtrlXK.GetInstance().AiMinMoveSpeedB * WaterwheelPlayerNetCtrl.mMaxVelocityFoot;
			}
			else {
				speedA = GameNetCtrlXK.GetInstance().AiMaxMoveSpeedA * WaterwheelPlayerNetCtrl.mMaxVelocityFoot;
				speedB = GameNetCtrlXK.GetInstance().AiMaxMoveSpeedB * WaterwheelPlayerNetCtrl.mMaxVelocityFoot;
			}
			speed = Random.Range(speedA, speedB);
		}
		speed = Mathf.Floor(speed);

		if (speed < 10f) {
			speed = 10f;
		}
		CountSpeed++;
		return speed;
	}

	void OnFinishMoveAiPlayerByPath()
	{
		AiPathCtrl pathScript = AimMarkTran.parent.GetComponent<AiPathCtrl>();
		int rv = pathScript.GetNextPathNum();
		if (rv > 0) {
			int key = 0;
			Transform trNextPath = null;
			while (trNextPath == null) {
				int randVal = Random.Range(0, 1000) % 3;
				switch(randVal) {
				case 0:
					trNextPath = pathScript.mNextPath1;
					break;
					
				case 1:
					trNextPath = pathScript.mNextPath2;
					break;
					
				case 2:
					trNextPath = pathScript.mNextPath3;
					break;
				}
				
				key++;
				if (key > 2) {
					if (pathScript.mNextPath1 != null) {
						trNextPath = pathScript.mNextPath1;
					}
					else if (pathScript.mNextPath2 != null) {
						trNextPath = pathScript.mNextPath2;
					}
					else if (pathScript.mNextPath3 != null) {
						trNextPath = pathScript.mNextPath3;
					}
					break;
				}
			}
			
			if (trNextPath != null) {
				AimMarkTran = trNextPath.GetChild(0);
			}
		}
		else {
			Invoke("MoveAiPlayerToEndPos", 0.05f);
			return;
		}
		SetPlayerPathCount();

		AiPathCtrlTran = AimMarkTran.parent;

		if (AimMarkTran != null) {
			AiMark markAimScript = AimMarkTran.GetComponent<AiMark>();
			pathScript = AiPathCtrlTran.GetComponent<AiPathCtrl>();
			SetPlayerAimMarkData(pathScript.PathIndex, markAimScript.getMarkCount(), PlayerTran.name);
			UpdateAiPlayerAction();
		}
		Invoke("MoveAiPlayerByItween", 0.05f);
	}

	void MoveAiPlayerToEndPos()
	{
		Vector3 endPos = Vector3.zero;
		for (int i = 0; i < 7; i++) {
			if (GameNetCtrlXK.GetInstance().AiPlayerEndPos[i].activeSelf) {
				GameNetCtrlXK.GetInstance().AiPlayerEndPos[i].SetActive(false);
				endPos = GameNetCtrlXK.GetInstance().AiPlayerEndPos[i].transform.position;
				break;
			}
		}

		Vector3[] posArray = new Vector3[2];
		posArray[0] = PlayerTran.position;
		posArray[1] = endPos;
		iTween.MoveTo(PlayerObj, iTween.Hash("speed", AimMarkSpeed,
		                                     "orienttopath", true,
		                                     "easeType", iTween.EaseType.linear,
		                                     "path", posArray,
		                                     "oncomplete", "OnAiMoveToEndPos"));
	}

	void OnAiMoveToEndPos()
	{
		this.enabled = false;
		IsRunToEndPoint = true;
		CloseWaterParticle();

		ShowRankPlayerQiZhi();
		AimMarkDataScript.SetIsRunToEndPoint();
		AimMarkDataScript.SetIsGameOver();
	}
	
	void ShowRankPlayerQiZhi()
	{
		RankingCtrl.GetInstance().ShowRankPlayerQiZhi();
		if (Network.peerType != NetworkPeerType.Disconnected) {
			netView.RPC("SendAiPlayerShowRankPlayerQiZhi", RPCMode.OthersBuffered);
		}
	}
	
	[RPC]
	void SendAiPlayerShowRankPlayerQiZhi()
	{
		RankingCtrl.GetInstance().ShowRankPlayerQiZhi();
	}

	void Update()
	{
		HandleOtherPortGameOverInfo();
		if (!IsHandlePlayer) {
			UpdateAiPlayerOtherPortPos();
		}
		
		if (DaoJiShiCtrl.GetInstance().GetIsStopCheckAddSpeed() || GameCtrlXK.IsStopMoveAiPlayer) {
			if (mGameTime != 0) {
				mGameTime = 0;
				StopCheckIsAddSpeed();
			}
			return;
		}
		
		if (mGameTime == 0) {
			mGameTime = 100;
		}
		checkPlayerMoveDir();
	}

	[RPC]
	void SendAiPlayerRotToOther(Quaternion rot)
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
	void SendAiPlayerPosToOther(Vector3 pos, float speedVal)
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

	void UpdateAiPlayerOtherPortPos()
	{
		if (PositionCur == Vector3.zero) {
			return;
		}
		PlayerTran.position = Vector3.Lerp(PlayerTran.position, PositionCur, (mSpeed * Time.deltaTime) / 3.6f);
	}
	
	void SetRankPlayerArray()
	{
		RankingCtrl.GetInstance().SetRankPlayerArray(gameObject);
	}
	
	public void SetIsHandlePlayer()
	{
		if (IsHandlePlayer) {
			return;
		}
		IsHandlePlayer = true;

		if (netView == null) {
			netView = GetComponent<NetworkView>();
		}
		
		if (Network.isServer && NetworkRpcMsgCtrl.MaxLinkServerCount == NetworkRpcMsgCtrl.NoLinkClientCount) {
			SendShowAllCamera();
		}
		InvokeRepeating("LoopCheckMoveAiPlayer", 0.1f, 0.1f);
	}

	void LoopCheckMoveAiPlayer()
	{
		if (!MoveCameraByPath.IsMovePlayer) {
			return;
		}

		if (AimMarkTran == null) {
			return;
		}
		CancelInvoke("LoopCheckMoveAiPlayer");
		Invoke("MoveAiPlayerByItween", 1f);
	}
	
	[RPC]
	void HandlePlayerRpcInfo()
	{
		AutoFireScript = GetComponent<PlayerAutoFire>();
		AutoFireScript.enabled = false;
	}
	
	public void SendShowAllCamera()
	{
		NetworkRpcMsgCtrl.MaxLinkServerCount = 100;
		
		if (Network.peerType != NetworkPeerType.Disconnected
		    && MoveCameraByPath.IsMovePlayer
		    && GlobalData.GetInstance().gameLeve == GameLeve.WaterwheelNet) {
			netView.RPC("SendOtherLinkerShowAllCamera", RPCMode.OthersBuffered);
		}
		GameCtrlXK.GetInstance().ShowAllCameras();
	}
	
	[RPC]
	void SendOtherLinkerShowAllCamera()
	{
		NetworkRpcMsgCtrl.MaxLinkServerCount = 100;
		GameCtrlXK.GetInstance().ShowAllCameras();
	}
	
	public void RemovePlayerNetObj()
	{
		if (Network.peerType != NetworkPeerType.Disconnected
		    && MoveCameraByPath.IsMovePlayer
		    && GlobalData.GetInstance().gameLeve == GameLeve.WaterwheelNet) {
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
		PlayerBoxColObj.layer = LayerMask.NameToLayer(GameCtrlXK.WaterLayer);
	}

	void UpdateAiPlayerAction()
	{
		if (AimMarkTran == null) {
			return;
		}

		Vector3 aimForward = AimMarkTran.position - PlayerTran.position;
		aimForward = aimForward.normalized;
		float dx = aimForward.x - PlayerTran.forward.x;
		if (dx > 0f) {
			ChuanShenScript.UpdateChuanShenAction(false, true);
			ZhuJiaoNanScript.UpdateZhuJiaoNanAction(false, true);
		}
		else if (dx < 0f) {
			ChuanShenScript.UpdateChuanShenAction(true, false);
			ZhuJiaoNanScript.UpdateZhuJiaoNanAction(true, false);
		}

		if (!IsInvoking("ResetAiPlayerAction")) {
			Invoke("ResetAiPlayerAction", 1f);
		}
	}

	void ResetAiPlayerAction()
	{
		ChuanShenScript.UpdateChuanShenAction(false, false);
		ZhuJiaoNanScript.UpdateZhuJiaoNanAction(false, false);
	}

	public void CloseWaterParticle()
	{
		if (!WaterParticle.activeSelf) {
			return;
		}

		WaterParticle.SetActive(false);
		if (Network.peerType != NetworkPeerType.Disconnected
		    && !GameCtrlXK.IsStopMoveAiPlayer
		    && MoveCameraByPath.IsMovePlayer) {
			netView.RPC("SendCloseWaterParticle", RPCMode.OthersBuffered);
		}
	}
	
	[RPC]
	void SendCloseWaterParticle()
	{
		WaterParticle.SetActive(false);
	}
	
	public void ActiveWaterParticle()
	{
		WaterParticle.SetActive(true);
		if (Network.peerType != NetworkPeerType.Disconnected
		    && MoveCameraByPath.IsMovePlayer
		    && GlobalData.GetInstance().gameLeve == GameLeve.WaterwheelNet) {
			netView.RPC("SendActiveWaterParticle", RPCMode.OthersBuffered);
		}
	}
	
	[RPC]
	void SendActiveWaterParticle()
	{
		WaterParticle.SetActive(true);
	}

	void CheckIsOpenWaterParticle()
	{
		if (mSpeed <= 5f || IsRunToEndPoint || GameCtrlXK.IsStopMoveAiPlayer) {
			CloseWaterParticle();
			return;
		}

		if (GameCtrlXK.IsStopMoveAiPlayer) {
			return;
		}
		ActiveWaterParticle();
	}
	
	public bool GetIsHandlePlayer()
	{
		return IsHandlePlayer;
	}
	
	public float GetMoveSpeed()
	{
		return mSpeed;
	}

	void SetPlayerPathCount()
	{
		RankingCtrl.GetInstance().SetPlayerPathCount(PlayerTran.name);
		if (Network.peerType != NetworkPeerType.Disconnected
		    && MoveCameraByPath.IsMovePlayer
		    && GlobalData.GetInstance().gameLeve == GameLeve.WaterwheelNet) {
			netView.RPC("SendPlayerPathCountToOther", RPCMode.OthersBuffered, PlayerTran.name);
		}
	}
	
	[RPC]
	void SendPlayerPathCountToOther(string playerName)
	{
		RankingCtrl.GetInstance().SetPlayerPathCount(playerName);
	}
	
	void SetPlayerAimMarkData(int aimPathId, int markId, string playerName)
	{
		RankingCtrl.GetInstance().SetPlayerAimMark(aimPathId, markId, playerName);
		
		if (Network.peerType != NetworkPeerType.Disconnected
		    && MoveCameraByPath.IsMovePlayer
		    && GlobalData.GetInstance().gameLeve == GameLeve.WaterwheelNet) {
			netView.RPC("SendAiPlayerAimMarkToOther", RPCMode.OthersBuffered, aimPathId, markId, playerName);

			AiPathCtrl pathScript = AiPathCtrlTran.GetComponent<AiPathCtrl>();
			netView.RPC("SendAiPlayerPathMark", RPCMode.OthersBuffered, pathScript.PathIndex, markId);
		}
	}
	
	[RPC]
	void SendAiPlayerAimMarkToOther(int aimPathId, int markId, string playerName)
	{
		RankingCtrl.GetInstance().SetPlayerAimMark(aimPathId, markId, playerName);
	}

	[RPC]
	void SendAiPlayerPathMark(int countPath, int countMark)
	{
		Transform tmpTran = AiPathGroupCtrl.FindAiPathTran(countPath);
		if (tmpTran == null) {
			return;
		}
		AiPathCtrlTran = tmpTran;
		AimMarkTran = AiPathCtrlTran.GetChild(countMark);
	}

	void checkPlayerMoveDir()
	{
		if (IsActiveHuanWeiPos) {
			return;
		}

		if (AimMarkTran == null) {
			return;
		}
		
		Vector3 posA = AimMarkTran.position;
		Vector3 posB = PlayerTran.position;
		posA.y = posB.y = 0f;
		Vector3 vecB = posA - posB;
		if (Vector3.Distance(posA, posB) > 18f) {
			return;
		}
		
		Vector3 vecA = AimMarkTran.forward;
		vecA.y = vecB.y = 0f;
		float cosAB = Vector3.Dot(vecA, vecB);
		if (cosAB <= 0f) {
			AiMark markScript = AimMarkTran.GetComponent<AiMark>();
			if (markScript == null) {
				return;
			}
			int conutTmp = AimMarkTran.parent.childCount - 1;
			int markCount = markScript.getMarkCount();
			
			if (markCount != conutTmp) {
				AimMarkTran = markScript.mNextMark;
			}
			
			if (AimMarkTran != null) {
				AiMark markAimScript = AimMarkTran.GetComponent<AiMark>();
				AiPathCtrl pathScript = AiPathCtrlTran.GetComponent<AiPathCtrl>();
				SetPlayerAimMarkData(pathScript.PathIndex, markAimScript.getMarkCount(), PlayerTran.name);
				UpdateAiPlayerAction();
			}
		}
	}

	public Transform GetAimMarkTran()
	{
		return AimMarkTran;
	}

	public Transform GetAiPathCtrlTran()
	{
		return AiPathCtrlTran;
	}

	void StopCheckIsAddSpeed()
	{
		if (!IsCheckAiSpeed) {
			return;
		}
		IsCheckAiSpeed = false;
		CancelInvoke("RandCheckIsAddSpeed");

		ResetAiSpeedChangeVal();
	}

	void InitCheckIsAddSpeed()
	{
		if (IsCheckAiSpeed) {
			return;
		}
		IsCheckAiSpeed = true;
		
		Invoke("RandCheckIsAddSpeed", Random.Range(5, 8));
	}

	void WaitPlayerOne()
	{
		AiSpeedChangeVal = Random.Range(30f, 55f);
		CancelInvoke("ResetAiSpeedChangeVal");
		Invoke("ResetAiSpeedChangeVal", Random.Range(2f, 5f));
	}

	void RunAfterPlayerOne()
	{
		AiSpeedChangeVal = Random.Range(95f, 120f);
		CancelInvoke("ResetAiSpeedChangeVal");
		Invoke("ResetAiSpeedChangeVal", Random.Range(5f, 8f));
	}

	void ResetAiSpeedChangeVal()
	{
		AiSpeedChangeVal = 0f;
	}

	void RandCheckIsAddSpeed()
	{
		if (!IsCheckAiSpeed) {
			return;
		}

		int PlayerOneNo = PlayerRankData.PlayerOneDt.RankNo;
		if (RankNoAi < PlayerOneNo) {
			//Wait playerOne, sub aiplayer speed.
			if (Random.Range(0, 100) % 3 == 0)
			{
				CancelInvoke("ResetAiSpeedChangeVal");
				WaitPlayerOne();
			}
		}
		else {
			//run after playerOne, add aiplayer speed.
			if (Random.Range(0, 100) % 3 > 0)
			{
				//Debug.Log("run after ** " + RigObj.name + ", plsyer " + PlayerRankData.PlayerOneDt.Name);
				CancelInvoke("ResetAiSpeedChangeVal");
				RunAfterPlayerOne();
			}
		}

		Invoke("RandCheckIsAddSpeed", Random.Range(3, 7));
	}
	
	public void SetRankNoAi(int val)
	{
		RankNoAi = val;
	}

	void HandleOtherPortGameOverInfo()
	{
		if (!GameCtrlXK.IsStopMoveAiPlayer) {
			return;
		}

		if (WaterParticle.activeSelf) {
			WaterParticle.SetActive(false);
		}
	}
}