using UnityEngine;
using System.Collections;

public class WaterwheelPlayerCtrl : MonoBehaviour {
	public GameObject EmptyObj;
	public GameObject FlyNpcAimCube_1; //Fire point
	public GameObject FlyNpcAimCube_2; //aim point

	public GameObject PlayerBoxColObj;
	
	public Transform CamPointbackFar;
	public Transform CamPointBackNear;
	public Transform CamAimPoint;

	public Transform CamPointFirst;
	public Transform CamPointForward;
	public Transform CamPointRight;
	public Transform CamPointUp;
	public Transform ScreenWaterParticle;
	public Transform EYuXiongAttackTran;
	public GameObject HuanYingFuObj;
	public GameObject HuanYingFengXiaoObj;

	Rigidbody rigObj;
	Transform PlayerTran;

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
	
	float mSpeed;
	float mThrottleForce;
	float currentEnginePower;
	float mMaxMouseDownCount;
	static float MouseDownCountP_1;
	static float MouseDownCountP_2;

	bool IsHitFuBingObj;
	int KillFireNpcNum;
	public static int mMaxVelocityFoot = 45;
	public static float mMaxVelocityFootMS = (float)mMaxVelocityFoot / 3.6f;
	
	public Transform AimMarkTran;
	public Transform AiPathCtrlTran;

	public bool IsCheckMoreNextPath;
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

	bool IsActiveJuLiFu;
	bool IsActiveHuanYingFu;
	PlayerAutoFire AutoFireScript;
	
	bool IsDonotTurnRight = false;
	bool IsDonotTurnLeft = false;
	ChuanShenCtrl ChuanShenScript;
	ChuanLunZiCtrl ChuanLunZiScript;
	ZhuJiaoNan ZhuJiaoNanScript;

	public static WaterwheelPlayerCtrl _Instance;
	public static WaterwheelPlayerCtrl GetInstance()
	{
		return _Instance;
	}

	public static bool IsTestShootCartoon = false;
	// Use this for initialization
	void Start()
	{
		_Instance = this;
		pcvr.DongGanState = 1;
		PlayerZhuanXiangVal = GameCtrlXK.PlayerZhuanXiangPTVal;
		if (IsTestShootCartoon) {
			return;
		}
		if (HuanYingFuObj == null) {
			Debug.LogError("HuanYingFuObj is null");
			HuanYingFuObj.name = "null";
		}
		else {
			HuanYingFuObj.SetActive(false);
		}

		PlayerTran = transform;
		rigObj = GetComponent<Rigidbody>();
		if (GetComponent<Animator>() != null) {
			Debug.LogWarning("Player Animator should be remove");
			PlayerTran = null;
			PlayerTran.name = "null";
		}

		AutoFireScript = GetComponent<PlayerAutoFire>();

		ScreenWaterParticle.gameObject.SetActive(false);
		ChuanShenScript = GetComponentInChildren<ChuanShenCtrl>();
		ChuanLunZiScript = GetComponentInChildren<ChuanLunZiCtrl>();
		ZhuJiaoNanScript = GetComponentInChildren<ZhuJiaoNan>();

		mGameTime = 1000;
		SetCamAimInfo();

		CreatePlayerNeedObj();

		AiPathCtrlTran = GameCtrlXK.GetInstance().AiPathCtrlTran.GetChild(0);
		AutoFireScript.SetPathKeyState( AiPathCtrlTran.GetComponent<AiPathCtrl>() );

		AimMarkTran = AiPathCtrlTran.GetChild(0);
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
		if (IsTestShootCartoon) {
			return;
		}
		if (CartoonShootPlayerCtrl.IsActiveRunPlayer) {
			this.enabled = false;
			return;
		}

		if (MoveCameraByPath.IsMovePlayer) {
			CheckWaterwheelPlayerSpeed();
		}

		if(mGameTime > 0 && MoveCameraByPath.IsMovePlayer)
		{
			setBikeMouseDown();

			GetInput();
			
			CalculateEnginePower();

			ApplyThrottle();
		}
	}

	void Update()
	{
		if (IsTestShootCartoon) {
			return;
		}

		checkPlayerMoveDir();
		if (!StartBtCtrl.GetInstanceP1().CheckIsActivePlayer()) {
			if (mGameTime != 0) {
				mGameTime = 0;
				CloseHuanYingFuState();
				PlayerAutoFire.HandlePlayerCloseHuanYingFu();
			}
			return;
		}
		
		if (mGameTime == 0) {
			mGameTime = 100;
		}

		if (Time.timeScale != 1f) {
			CheckWaterwheelPlayerSpeed();

			GetInput();
			
			CalculateEnginePower();
			
			ApplyThrottle();
		}

		if (AutoFireScript.CheckIsBackPlayerOutWater()) {
			//Debug.Log("ResetPlayerPos*************");
			ResetPlayerPos();
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (IsTestShootCartoon) {
			return;
		}
		HandleHitShootObj(other.gameObject, 0);
	}

	public void ShootingDeadObj(GameObject obj)
	{
		HandleHitShootObj(obj, 1);
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
			DaoJuTiShiCtrl.GetInstance().ShowDaoJuTiShi(DaoJuState.JuLiFu);
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
			break;
		}
	}

	void ShowDaoJuExplosion(GameObject daoJuObj)
	{
		if(daoJuObj == null)
		{
			return;
		}

		Transform daoJuTran = daoJuObj.transform;
		if(daoJuTran.childCount <= 0)
		{
			return;
		}
		Transform explosionTran = daoJuTran.GetChild(0);
		explosionTran.parent = null;
		explosionTran.gameObject.SetActive(true);

		Destroy(daoJuObj);
	}

	void HitHuanWeiFuObj()
	{

	}

	void HitDingShenFuObj()
	{

	}

	void HitDianDaoFuObj()
	{

	}

	void ActiveJuLiFuState()
	{
		if(IsActiveJuLiFu)
		{
			return;
		}
		IsActiveJuLiFu = true;
		AudioListCtrl.PlayAudio(AudioListCtrl.GetInstance().AudioJuLiFu);
		GlobalData.GetInstance().IsActiveJuLiFu = true;
		Invoke("CloseJuLiFuState", GameCtrlXK.GetInstance().ActiveJuLiFuTime);
	}

	void CloseJuLiFuState()
	{
		if(!IsActiveJuLiFu)
		{
			return;
		}
		IsActiveJuLiFu = false;
		GlobalData.GetInstance().IsActiveJuLiFu = false;
		ActiveDaJuCtrl.SetTypeDaoJuList((int)DaoJuTypeIndex.juLiFu, 0);
		GameCtrlXK.GetInstance().ActivePlayerDaoJuType((int)DaoJuTypeIndex.Close);
	}

	public void ActiveHuanYingFuState()
	{
		if(IsActiveHuanYingFu)
		{
			return;
		}
		IsActiveHuanYingFu = true;
		AudioListCtrl.PlayAudio(AudioListCtrl.GetInstance().AudioHuanYingFu);
		CameraShake.GetInstance().SetIsActiveHuanYingFu(true);
		PlayerBoxColObj.layer = LayerMask.NameToLayer("TransparentFX");
		HuanYingFuObj.SetActive(true);
		HuanYingFengXiaoObj.SetActive(true);
		XingXingCtrl.IsPlayerCanHitNPC = false;
		PlayerAutoFire.IsActivePlayerForwardHit = true;
		Invoke("CloseHuanYingFuState", GameCtrlXK.GetInstance().JiaSuWuDiTime);
	}
	
	public void SetPlayerBoxColliderState(bool isActive)
	{
		AutoFireScript.SetPlayerBoxColliderState(isActive);
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

		if (!IsActiveShenXingMode) {
			XingXingCtrl.IsPlayerCanHitNPC = true;
			PlayerAutoFire.IsActivePlayerForwardHit = false;
			PlayerBoxColObj.layer = LayerMask.NameToLayer("Default");
			HuanYingFuObj.SetActive(false);
		}
		IsActiveHuanYingFu = false;
		HuanYingFengXiaoObj.SetActive(false);
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
		WaterwheelCameraCtrl.GetInstance().setAimPlayerInfo();
		CameraShake.GetInstance().SetCameraPointInfo();
	}

	public static void setBikeMouseDown()
	{
		float dTime = 0f;
		if( (!pcvr.bIsHardWare && InputEventCtrl.VerticalVal > 0f)
		   || (pcvr.IsGetValByKey  && InputEventCtrl.VerticalVal > 0f)
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
		if(mGameTime > 0)
		{
			steerTmp = pcvr.mGetSteer;
		}
		else
		{
			MouseDownCountP_1 = 0f;
			MouseDownCountP_2 = 0f;
		}
		//Debug.Log("MouseDownCountP_1 " + MouseDownCountP_1 + ", MouseDownCountP_2 " + MouseDownCountP_2);
		
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

			if (TengManInfoCtrl.GetInstance().GetIsActiveTengManInfo()) {
				PlayerTran.Rotate(0, -rotSpeed, 0);
			}
			else {
				PlayerTran.Rotate(0, rotSpeed, 0);
			}
			
			bIsTurnLeft = false;
			if (!bIsTurnRight) {
				bIsTurnRight = true;
				PlayerAutoFire.ActiveIsTurnRight();
				if (mSpeed > 15f && !pcvr.IsPlayerHitShake) {
					pcvr.OpenQiNangZuo();
				}
			}

			if (Mathf.Abs( mSteer ) < 0.4f) {
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

			if (TengManInfoCtrl.GetInstance().GetIsActiveTengManInfo()) {
				PlayerTran.Rotate(0, -rotSpeed, 0);
			}
			else {
				PlayerTran.Rotate(0, rotSpeed, 0);
			}
			
			bIsTurnRight = false;
			if (!bIsTurnLeft) {
				bIsTurnLeft = true;
				PlayerAutoFire.ActiveIsTurnLeft();
				if (mSpeed > 15f && !pcvr.IsPlayerHitShake) {
					pcvr.OpenQiNangYou();
				}
			}

			if (Mathf.Abs( mSteer ) < 0.4f) {
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
		
		if (CartoonShootPlayerCtrl.IsActiveRunPlayer) {
			return;
		}
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
			//Debug.Log("DSpeedVal = " + dVal);
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
		AutoFireScript.SetPlayerMvSpeed(mSpeed);
		ChuanLunZiScript.UpdateChuanLunZiAction(speedTmp);
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
					rigObj.AddForce(forwardVal * 90000f);
				}
				maxVelocity = IntoPuBuCtrl.PlayerMvSpeed;
			}
			else if(IsActiveHuanYingFu)
			{
				if (mSpeed < PlayerAutoFire.HuanYingSpeed) {
					rigObj.AddForce(PlayerTran.forward * 90000f);
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
					rigObj.AddForce(PlayerTran.forward * 9000f);
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
		if (CartoonShootPlayerCtrl.IsActiveRunPlayer) {
			if (!rigObj.isKinematic) {
				rigObj.isKinematic = true;
			}
			return;
		}

		if (mThrottleForce <= 0f && mSpeed <= 3f) {
			if (!rigObj.isKinematic && !PlayerAutoFire.IsIntoSky && !PlayerAutoFire.IsRestartMove) {
				rigObj.isKinematic = true;
			}
			return;
		}

		if (rigObj.isKinematic) {
			rigObj.isKinematic = false;
		}

		if( IsHitFuBingObj) {
			mThrottleForce = 300f * rigObj.mass;
		}

		rigObj.AddForce(PlayerTran.forward * Time.deltaTime * mThrottleForce);
	}

	public void AddKillFireNpcNum()
	{
		KillFireNpcNum++;
		if(KillFireNpcNum >= 20)
		{
			KillFireNpcNum = 0;
		}
	}

	public void ActiveShenXingState()
	{
		if(IsActiveShenXingMode)
		{
			return;
		}

		IsActiveShenXingMode = true;
		ShenXingInfoCtrl.GetInstance().ShowShenXingInfo();
		
		XingXingCtrl.IsPlayerCanHitNPC = false;
		PlayerAutoFire.IsActivePlayerForwardHit = true;
		PlayerBoxColObj.layer = LayerMask.NameToLayer("TransparentFX");
		HuanYingFuObj.SetActive(true);
		//Invoke("CloseShenXingState", 6f);
	}

	public void ShouldCloseShenXingState()
	{
		//CancelInvoke("CloseShenXingState");
		//Close NengLiangTeXiao
		NengLiangTiaoCtrl.IsStopNengLiangTeXiao = true;
		CloseShenXingState();
	}

	public void CloseShenXingState()
	{
		if (!IsActiveShenXingMode) {
			return;
		}
		IsActiveShenXingMode = false;
		
		if (!IsInvoking("CloseHuanYingFuState")) {
			XingXingCtrl.IsPlayerCanHitNPC = true;
			PlayerAutoFire.IsActivePlayerForwardHit = false;
			PlayerBoxColObj.layer = LayerMask.NameToLayer("Default");
			HuanYingFuObj.SetActive(false);
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
		PlayerTran.position = mBakeTimePointPos;
		PlayerTran.forward = mBakeTimePointRot;
		PlayerAutoFire.PlayerThrottleForce = mThrottleForce;
		PlayerAutoFire.PlayerCurrentEnginePower = currentEnginePower;
		AutoFireScript.ResetPlayerCameraPos();
	}

	public void TestAddForce()
	{
		if (mSpeed < 120f) {
			Vector3 forwardVal = PlayerTran.forward;
			forwardVal.y = 0f;
			rigObj.AddForce(forwardVal * 90000f, ForceMode.Acceleration);
		}
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
						if(AimMarkTran == null)
						{
							//player run to end
							this.enabled = false;
							
							DaoJiShiCtrl.GetInstance().StopDaoJiShi();
							GameOverCtrl.GetInstance().HiddenContinueGame();
							FinishPanelCtrl.GetInstance().ShowFinishPanel();
							
							AutoFireScript.SetPlayerMvSpeed(0f);
							//AutoFireScript.CloseWaterParticle();
							return;
						}
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

		if(cosAC < 0f && cosBC < 0f && !GameOverCtrl.GetInstance().CheckIsActiveOver())
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
					if(AimMarkTran == null)
					{
						//player run to end
						this.enabled = false;
//						IsRunToEndPoint = true;
						
						DaoJiShiCtrl.GetInstance().StopDaoJiShi();
						GameOverCtrl.GetInstance().HiddenContinueGame();
						FinishPanelCtrl.GetInstance().ShowFinishPanel();
						
						AutoFireScript.SetPlayerMvSpeed(0f);
						//AutoFireScript.CloseWaterParticle();
						return;
					}
					AiPathCtrlTran = AimMarkTran.parent; //next path
					AutoFireScript.SetPathKeyState( AiPathCtrlTran.GetComponent<AiPathCtrl>() );

					if (ParentPath != null) {
						AiParentPathScript = ParentPath.GetComponent<AiPathCtrl>();
						if (AiParentPathScript.GetNextPathNum() > 1) {
							IsCheckMoreNextPath = true;
						}
					}
				}
			}
		}
	}

	void ActiveCamPointFirst()
	{
		WaterwheelCameraCtrl.GetInstance().EnableCamPointFirst();
	}

	void CloseCamPointFirst()
	{
		WaterwheelCameraCtrl.GetInstance().EnableCamPointBack();
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

	public Transform GetCamPointFirst()
	{
		return CamPointFirst;
	}

	public Transform GetCamPointBackNear()
	{
		return CamPointBackNear;
	}

	public Transform GetCamPointBackFar()
	{
		return CamPointbackFar;
	}

	public Transform GetCamAimPoint()
	{
		return CamAimPoint;
	}

	public Transform GetAimMarkTran()
	{
		return AimMarkTran;
	}

	/*void OnGUI()
	{
		GUI.Label(new Rect(0f, 50f, 200f, 50f), "speed " + mSpeed.ToString());
		GUI.Label(new Rect(0f, 150f, 200f, 50f), "timeScale " + Time.timeScale.ToString());
	}*/
}
