using UnityEngine;
using System.Collections;
using System;

public class SetPanelUiRoot : MonoBehaviour
{
	/**
	 * YouMenSt == YouMenTaBanEnum.JiaoTaBan   -> 机台采用脚踏板来控制运动.
	 * YouMenSt == YouMenTaBanEnum.YouMenTaBan -> 机台采用油门踏板来控制运动.
	 */
	public static YouMenTaBanEnum YouMenSt = YouMenTaBanEnum.YouMenTaBan;
	public UILabel CoinStartLabel;
	public UISprite DuiGouDiffLow;
	public UISprite DuiGouDiffMiddle;
	public UISprite DuiGouDiffHigh;
	
	public UISprite DuiGouYunYingMode;
	public UISprite DuiGouFreeMode;
	
	public GameObject SetPanelObj;
	public GameObject JiaoZhunPanelObj;
	public GameObject CeShiPanelObj;
	public Transform StarTran;

	public GameObject DirTestPanelObj;
	public GameObject PedalTestPanelObj;
	public GameObject GunTestPanelObj;
	public GameObject QiNangTestPanelObj;

	public Transform GunCrossTran;
	
	public UISprite SpriteDirTestInfo;
	public UILabel LabelSpeedTestInfo;
	public GameObject HitAimObjInfo;
	
	public GameObject DirAdjustObj;
	public GameObject PedalAdjustObj;
	public UISprite TaBanAdjustSprite;
	public GameObject GunAdjustObj;
	public UISprite SpriteAdjustDir;
	public UISprite SpriteAdjustGunCross;
	public UITexture TextureAdjustGunCross;

	GameObject StarObj;
	
	enum PanelState
	{
		SetPanel,
		JiaoYanPanel,
		CeShiPanel
	}
	PanelState PanelStVal = PanelState.SetPanel;
	
	int StarMoveCount;
	int GameDiffState;
	bool IsFreeGameMode;
	
	string fileName = "";
	HandleJson handleJsonObj;
	public UILabel GameAudioVolumeLB;
	int GameAudioVolume;

	Vector3 [] SetPanelStarPos = new Vector3[9];
	Vector3 [] JZCSStarPos = new Vector3[5];

	enum SelectSetPanelDate
	{
		CoinStart = 1,
		GameDiff,
		GameMode,
		Adjust,
		HardwareTest,
		ResetFactory,
		GameAudioSet,
		GameAudioReset,
		Exit,
	}
	//SelectSetPanelDate SelSetPanelDt = SelectSetPanelDate.CoinStart;
	
	enum SelectJiaoZhunDate
	{
		DirAdjust = 1,
		PedalAdjust,
		GunAdjust,
		Exit
	}
	//SelectJiaoZhunDate SelJiaoZhunDt = SelectJiaoZhunDate.DirAdjust;
	
	enum SelectCeShiDate
	{
		DirTest = 1,
		PedalTest,
		GunTest,
		QiNangTest,
		Exit
	}
	//SelectCeShiDate SelCeShiDt = SelectCeShiDate.DirTest;
	string startCoinInfo = "";

	enum AdjustDirState
	{
		DirectionRight = 0,
		DirectionCenter,
		DirectionLeft
	}
	AdjustDirState AdjustDirSt = AdjustDirState.DirectionRight;

	AdjustGunDrossState AdjustGunDrossSt = AdjustGunDrossState.GunCrossLU;
	
	bool IsMoveStar = true;
	enum QiNangTestEnum
	{
		Null = -1,
		QNLF_CQ, //左前气囊充气.
		QNLF_FQ, //左前气囊放气.
		QNRF_CQ, //右前气囊充气.
		QNRF_FQ, //右前气囊放气.
		QNLM_CQ, //左中气囊充气.
		QNLM_FQ, //左中气囊放气.
		QNRM_CQ, //右中气囊充气.
		QNRM_FQ, //右中气囊放气.
		QNLB_CQ, //左后气囊充气.
		QNLB_FQ, //左后气囊放气.
		QNRB_CQ, //右后气囊充气.
		QNRB_FQ, //右后气囊放气.
		QNEixt, //退出气囊测试.
	}
	QiNangTestEnum QiNangTestState = QiNangTestEnum.Null;
	
	Vector3[] QiNangTestStarPos = {
		new Vector3(-338f, 214f, 0f),
		new Vector3(-338f, 185f, 0f),
		new Vector3(-338f, 156f, 0f),
		new Vector3(-338f, 125f, 0f),
		new Vector3(-338f, 94f, 0f),
		new Vector3(-338f, 64f, 0f),
		new Vector3(-338f, 34f, 0f),
		new Vector3(-338f, 1f, 0f),
		new Vector3(-338f, -25f, 0f),
		new Vector3(-338f, -54f, 0f),
		new Vector3(-338f, -87f, 0f),
		new Vector3(-338f, -118f, 0f),
		new Vector3(-338f, -151f, 0f),
	};

	public static SetPanelUiRoot _Instance;
	public static SetPanelUiRoot GetInstance()
	{
		return _Instance;
	}

	// Use this for initialization
	void Start()
	{
		_Instance = this;
		pcvr.DongGanState = 1;
		XKShuiQiangCrossCtrl.GetInstance().SetPlayerCrossTr(GunCrossTran);

		SetPanelStarPos[0] = new Vector3(-350f, 243f, 0f);
		SetPanelStarPos[1] = new Vector3(-350f, 154f, 0f);
		SetPanelStarPos[2] = new Vector3(-350f, 65f, 0f);
		SetPanelStarPos[3] = new Vector3(-350f, -24f, 0f);
		SetPanelStarPos[4] = new Vector3(-350f, -112f, 0f);
		SetPanelStarPos[5] = new Vector3(-350f, -200f, 0f);
		SetPanelStarPos[6] = new Vector3(-350f, -288f, 0f);
		SetPanelStarPos[7] = new Vector3(-140f, -325f, 0f);
		SetPanelStarPos[8] = new Vector3(130f, -288f, 0f);

		JZCSStarPos[0] = new Vector3(-305f, 57f, 0f);
		JZCSStarPos[1] = new Vector3(-305f, -12f, 0f);
		JZCSStarPos[2] = new Vector3(-305f, -83f, 0f);
		JZCSStarPos[3] = new Vector3(-305f, -154f, 0f);
		JZCSStarPos[4] = new Vector3(-305f, -225f, 0f);

		StarObj = StarTran.gameObject;
		SetStarObjActive(true);

		InitHandleJson();
		
		InitStarImgPos();
		InitCoinStartLabel();
		InitGameDiffDuiGou();
		InitGameModeDuiGou();
		InitGameAudioValue();
		CancelInvoke("HandleGunJiaoZhunUI");
		Invoke("HandleGunJiaoZhunUI", 2f);

		InputEventCtrl.GetInstance().ClickSetEnterBtEvent += ClickSetEnterBtEvent;
		InputEventCtrl.GetInstance().ClickSetMoveBtEvent += ClickSetMoveBtEvent;
		InputEventCtrl.GetInstance().ClickFireBtEvent += ClickFireBtEvent;
		InputEventCtrl.GetInstance().ClickStartBtOneEvent += ClickStartBtEventP1;
		InputEventCtrl.GetInstance().ClickStartBtTwoEvent += ClickStartBtEventP2;
	}

	void HandleGunJiaoZhunUI()
	{
		if (pcvr.IsUseZhunXingJZ_36 || pcvr.IsUseLineHitCross) {
			TextureAdjustGunCross.gameObject.SetActive(true);
			SpriteAdjustGunCross.gameObject.SetActive(false);
		}
		else {
			TextureAdjustGunCross.gameObject.SetActive(false);
			SpriteAdjustGunCross.gameObject.SetActive(true);
		}
	}

	void Update()
	{
		if (SetBtSt == ButtonState.DOWN && Time.time - TimeSetMoveBt > 1f && Time.frameCount % 50 == 0) {
			MoveStarImg();
		}

		SetLabelSpeedTestInfo( InputEventCtrl.VerticalVal );
		if (pcvr.bIsHardWare) {
			SetSpriteDirTestInfo( pcvr.mGetSteer );

			GlobalData.GetInstance().Icoin = pcvr.GetInstance().CoinNumCurrent;
			SetCoinStartLabelInfo();
			return;
		}

		if (Input.GetKeyUp(KeyCode.T)) {
			GlobalData.GetInstance().Icoin++;
			SetCoinStartLabelInfo();
		}
		SetSpriteDirTestInfo( InputEventCtrl.GetInstance().GetHorVal() );
	}

	void ClickSetEnterBtEvent(ButtonState val)
	{
		if(val == ButtonState.DOWN)
		{
			return;
		}
		//BackMovieScene(); //test
		HanldeClickEnterBtEvent();
	}
	
	float TimeSetMoveBt;
	ButtonState SetBtSt = ButtonState.UP;
	void ClickSetMoveBtEvent(ButtonState val)
	{
		SetBtSt = val;
		if (val == ButtonState.DOWN) {
			TimeSetMoveBt = Time.time;
			return;
		}

		if (Time.time - TimeSetMoveBt > 1f) {
			return;
		}
		MoveStarImg();
	}

	void ClickFireBtEvent(ButtonState val)
	{
		if (val == ButtonState.DOWN) {
			if (GunAdjustObj.activeSelf) {
				pcvr.IsOpenShuiBeng = true;
				pcvr.ShuiBengState = PcvrShuiBengState.Level_1;
			}
			return;
		}
		pcvr.IsOpenShuiBeng = false;
		HanldeClickFireBtEvent();
	}

	void ClickStartBtEventP1(ButtonState val)
	{
		if(val == ButtonState.DOWN)
		{
			return;
		}
		HandleClickStartBtEventP1();
	}

	void HandleClickStartBtEventP1()
	{
		if (PanelStVal == PanelState.JiaoYanPanel) {
			SelectJiaoZhunDate DtEnum = (SelectJiaoZhunDate) StarMoveCount;
			switch (DtEnum) {
			case SelectJiaoZhunDate.DirAdjust:
			case SelectJiaoZhunDate.PedalAdjust:
			case SelectJiaoZhunDate.GunAdjust:
				OpenJiaoYanPanelObj(DtEnum);
				break;
			}
		}
	}

	void ClickStartBtEventP2(ButtonState val)
	{
		if(val == ButtonState.DOWN)
		{
			return;
		}
		HandleClickStartBtEventP2();
	}
	
	void HandleClickStartBtEventP2()
	{
		if (PanelStVal == PanelState.JiaoYanPanel) {
			SelectJiaoZhunDate DtEnum = (SelectJiaoZhunDate) StarMoveCount;
			switch (DtEnum) {
			case SelectJiaoZhunDate.DirAdjust:
			case SelectJiaoZhunDate.PedalAdjust:
			case SelectJiaoZhunDate.GunAdjust:
				OpenJiaoYanPanelObj(DtEnum);
				break;
			}
		}
	}

	void SetStarObjActive(bool isActive)
	{
		StarObj.SetActive(isActive);
	}

	void InitCoinStartLabel()
	{
		startCoinInfo = handleJsonObj.ReadFromFileXml(fileName, "START_COIN");
		if(startCoinInfo == null || startCoinInfo == "")
		{
			startCoinInfo = "1";
			handleJsonObj.WriteToFileXml(fileName, "START_COIN", startCoinInfo);
		}
		GlobalData.GetInstance().XUTOUBI = Convert.ToInt32( startCoinInfo );

		SetCoinStartLabelInfo();
	}

	public void SetCoinStartLabelInfo()
	{
		handleJsonObj.WriteToFileXml(fileName, "START_COIN", GlobalData.GetInstance().XUTOUBI.ToString());
		CoinStartLabel.text = "Start Coin " + GlobalData.GetInstance().XUTOUBI
									+ ", Curent Coin " + GlobalData.GetInstance().Icoin;
	}

	void InitHandleJson()
	{
		GlobalData.GetInstance();
		fileName = GlobalData.fileName;
		handleJsonObj = GlobalData.handleJsonObj;
	}

	void InitGameDiffDuiGou()
	{
		string diffStr = handleJsonObj.ReadFromFileXml(fileName, "GAME_DIFFICULTY");
		if(diffStr == null || diffStr == "")
		{
			diffStr = "1";
			handleJsonObj.WriteToFileXml(fileName, "GAME_DIFFICULTY", diffStr);
		}
		GlobalData.GetInstance().GameDiff = diffStr;

		SetGameDiffState();
	}

	void SetGameDiffState()
	{
		switch (GlobalData.GetInstance().GameDiff)
		{
		case "0":
			DuiGouDiffLow.enabled = true;
			DuiGouDiffMiddle.enabled = false;
			DuiGouDiffHigh.enabled = false;
			GameDiffState = 0;
			break;
			
		case "1":
			DuiGouDiffLow.enabled = false;
			DuiGouDiffMiddle.enabled = true;
			DuiGouDiffHigh.enabled = false;
			GameDiffState = 1;
			break;
			
		case "2":
			DuiGouDiffLow.enabled = false;
			DuiGouDiffMiddle.enabled = false;
			DuiGouDiffHigh.enabled = true;
			GameDiffState = 2;
			break;

		default:
			GlobalData.GetInstance().GameDiff = "1";
			DuiGouDiffLow.enabled = false;
			DuiGouDiffMiddle.enabled = true;
			DuiGouDiffHigh.enabled = false;
			GameDiffState = 1;
			break;
		}
		handleJsonObj.WriteToFileXml(fileName, "GAME_DIFFICULTY", GlobalData.GetInstance().GameDiff);
		GameDiffState++;
	}

	void InitGameModeDuiGou()
	{
		bool isFreeModeTmp = false;
		string modeGame = handleJsonObj.ReadFromFileXml(fileName, "GAME_MODE");
		if(modeGame == null || modeGame == "")
		{
			modeGame = "1";
			handleJsonObj.WriteToFileXml(fileName, "GAME_MODE", modeGame);
		}
		
		if(modeGame == "0")
		{
			isFreeModeTmp = true;
		}
		GlobalData.GetInstance().IsFreeMode = isFreeModeTmp;
		
		SetGameModeState();
	}

	void SetGameModeState()
	{
		string modeGame = "";
		if (GlobalData.GetInstance().IsFreeMode) {
			modeGame = "0";
		}
		else {
			modeGame = "1";
		}

		DuiGouYunYingMode.enabled = !GlobalData.GetInstance().IsFreeMode;
		DuiGouFreeMode.enabled = GlobalData.GetInstance().IsFreeMode;
		IsFreeGameMode = GlobalData.GetInstance().IsFreeMode;
		handleJsonObj.WriteToFileXml(fileName, "GAME_MODE", modeGame);
	}

	void HanldeClickEnterBtEvent()
	{
		if (PanelStVal == PanelState.SetPanel) {
			SelectSetPanelDate DtEnum = (SelectSetPanelDate) StarMoveCount;
			switch (DtEnum) {
			case SelectSetPanelDate.CoinStart:
				if (GlobalData.GetInstance().XUTOUBI >= 10) {
					GlobalData.GetInstance().XUTOUBI = 0;
				}
				GlobalData.GetInstance().XUTOUBI++;

				SetCoinStartLabelInfo();
				break;

			case SelectSetPanelDate.GameDiff:
				if (GameDiffState >= 3) {
					GameDiffState = 0;
				}
				GlobalData.GetInstance().GameDiff = GameDiffState.ToString();
				SetGameDiffState();
				break;

			case SelectSetPanelDate.GameMode:
				IsFreeGameMode = !IsFreeGameMode;
				GlobalData.GetInstance().IsFreeMode = IsFreeGameMode;
				SetGameModeState();
				break;

			case SelectSetPanelDate.Adjust:
			case SelectSetPanelDate.HardwareTest:
				ChangeGuiPanel();
				break;

			case SelectSetPanelDate.ResetFactory:
				ResetFactoryInfo();
				break;
				
			case SelectSetPanelDate.GameAudioSet:
				GameAudioVolume++;
				if (GameAudioVolume > 10) {
					GameAudioVolume = 0;
				}
				GameAudioVolumeLB.text = GameAudioVolume.ToString();
				handleJsonObj.WriteToFileXml(fileName, "GameAudioVolume", GameAudioVolume.ToString());
				GlobalData.GameAudioVolume = GameAudioVolume;
				break;
				
			case SelectSetPanelDate.GameAudioReset:
				GameAudioVolume = 7;
				GameAudioVolumeLB.text = GameAudioVolume.ToString();
				handleJsonObj.WriteToFileXml(fileName, "GameAudioVolume", "7");
				GlobalData.GameAudioVolume = GameAudioVolume;
				break;

			case SelectSetPanelDate.Exit:
				ExitSetPanle();
				break;
			}
		}
		else if (PanelStVal == PanelState.JiaoYanPanel) {
			SelectJiaoZhunDate DtEnum = (SelectJiaoZhunDate) StarMoveCount;
			switch (DtEnum) {
			case SelectJiaoZhunDate.DirAdjust:
			case SelectJiaoZhunDate.PedalAdjust:
			case SelectJiaoZhunDate.GunAdjust:
				OpenJiaoYanPanelObj(DtEnum);
				break;

			case SelectJiaoZhunDate.Exit:
				OpenCeShiPanel();
				SetPanelGunCrossCtrl.GetInstance().SetGunCrossActive(false);
				break;
			}
		}
		else if (PanelStVal == PanelState.CeShiPanel) {
			SelectCeShiDate DtEnum = (SelectCeShiDate) StarMoveCount;
			switch (DtEnum) {
			case SelectCeShiDate.DirTest:
			case SelectCeShiDate.PedalTest:
			case SelectCeShiDate.GunTest:
			case SelectCeShiDate.QiNangTest:
				if (QiNangTestPanelObj.activeSelf) {
					OnClickQiNangTestEnvent();
				}
				else {
					OpenTestPanelObj(DtEnum);
				}
				break;
				
			case SelectCeShiDate.Exit:
				OpenSetPanel();
				break;
			}
		}
	}

	void HanldeClickFireBtEvent()
	{
		if (GunAdjustObj.activeSelf) {
			CloseAllJiaoYanPanel();
		}
	}

	void InitAdjustDir()
	{
		AdjustDirSt = AdjustDirState.DirectionRight;
		ChangeAdjustDirImg();
	}

	void ChangeAdjustDirImg()
	{
		int index = (int)AdjustDirSt;
		//Debug.Log("ChangeAdjustDirImg -> AdjustDirSt " + AdjustDirSt + ", index " + index);
		SpriteAdjustDir.spriteName = "JiaoZhun_" + index.ToString();
	}

	void InitAdjustGunCross()
	{
		AdjustGunDrossSt = AdjustGunDrossState.GunCrossLU;
		ChangeAdjustGunCrossImg();
	}

	void ChangeAdjustGunCrossImg()
	{
		int index = (int)AdjustGunDrossSt;
		SpriteAdjustGunCross.spriteName = "GunJY_" + index.ToString();
	}

	public Texture[] GunJiaoZhunUI;
	public UITexture GunJZUITexture;
	void InitJiaoZhunGunUI()
	{
		GunJZUITexture.mainTexture = GunJiaoZhunUI[0];
	}

	void ChangeJiaoZhunGunUI()
	{
		GunJZUITexture.mainTexture = GunJiaoZhunUI[1];
	}

	void CloseAllJiaoYanPanel()
	{
		if (DirAdjustObj.activeSelf) {
			switch (AdjustDirSt) {
			case AdjustDirState.DirectionRight:
				AdjustDirSt = AdjustDirState.DirectionCenter;
				ChangeAdjustDirImg();
				if (!pcvr.bIsHardWare) {
					pcvr.SaveSteerVal(InputEventCtrl.SteerValCur, PcvrValState.ValMax);
				}
				else {
					pcvr.SaveSteerVal(pcvr.SteerValCur, PcvrValState.ValMax);
				}
				return;

			case AdjustDirState.DirectionCenter:
				AdjustDirSt = AdjustDirState.DirectionLeft;
				ChangeAdjustDirImg();
				if (!pcvr.bIsHardWare) {
					pcvr.SaveSteerVal(InputEventCtrl.SteerValCur, PcvrValState.ValCenter);
				}
				else {
					pcvr.SaveSteerVal(pcvr.SteerValCur, PcvrValState.ValCenter);
				}
				return;

			case AdjustDirState.DirectionLeft:
				if (!pcvr.bIsHardWare) {
					pcvr.SaveSteerVal(InputEventCtrl.SteerValCur, PcvrValState.ValMin);
				}
				else {
					pcvr.SaveSteerVal(pcvr.SteerValCur, PcvrValState.ValMin);
				}
				break;
			}
		}
		else if (PedalAdjustObj.activeSelf) {
			if (!pcvr.bIsHardWare) {
				pcvr.SaveTaBanVal(InputEventCtrl.TaBanValCur, PcvrValState.ValMax);
			}
			else {
				pcvr.SaveTaBanVal(pcvr.TaBanValCur, PcvrValState.ValMax);
			}
		}
		else if (GunAdjustObj.activeSelf) {
			if (pcvr.IsUseZhunXingJZ_36) {
				JiaoZhunZXCount++;
				//Debug.Log("JiaoZhunZXCount "+JiaoZhunZXCount);
				pcvr.SaveCrossPosInfo(AdjustGunDrossSt, JiaoZhunZXCount);
				if (JiaoZhunZXCount >= (int)pcvr.JZPoint) {
					SetPanelGunCrossCtrl.GetInstance().SetAimObjArrayActive(false);
					SetPanelGunCrossCtrl.GetInstance().SetGunCrossActive(true);
				}
				else {
					return;
				}
			}
			else {
				pcvr.SaveCrossPosInfo(AdjustGunDrossSt);
				switch (AdjustGunDrossSt) {
				case AdjustGunDrossState.GunCrossLU:
					AdjustGunDrossSt = AdjustGunDrossState.GunCrossRU;
					ChangeAdjustGunCrossImg();
					return;
					
				case AdjustGunDrossState.GunCrossRU:
					AdjustGunDrossSt = AdjustGunDrossState.GunCrossRD;
					ChangeAdjustGunCrossImg();
					return;
					
				case AdjustGunDrossState.GunCrossRD:
					AdjustGunDrossSt = AdjustGunDrossState.GunCrossLD;
					ChangeAdjustGunCrossImg();
					return;
					
				case AdjustGunDrossState.GunCrossLD:
					if (!pcvr.IsUseLineHitCross) {
						AdjustGunDrossSt = AdjustGunDrossState.GunCrossOver;
						SetPanelGunCrossCtrl.GetInstance().SetAimObjArrayActive(false);
						SetPanelGunCrossCtrl.GetInstance().SetGunCrossActive(true);
					}
					else {
						ChangeJiaoZhunGunUI();
						AdjustGunDrossSt = AdjustGunDrossState.GunCrossCen;
						return;
					}
					break;

				case AdjustGunDrossState.GunCrossCen:
					AdjustGunDrossSt = AdjustGunDrossState.GunCrossOver;
					SetPanelGunCrossCtrl.GetInstance().SetAimObjArrayActive(false);
					SetPanelGunCrossCtrl.GetInstance().SetGunCrossActive(true);
					break;
				}
			}
		}

		DirAdjustObj.SetActive(false);
		PedalAdjustObj.SetActive(false);
		GunAdjustObj.SetActive(false);
		IsJiaoZhunCross = false;
		if (!pcvr.bIsHardWare) {
			Screen.showCursor = false;
		}
		
		IsMoveStar = true;
		StarObj.SetActive(true);
	}
	static int JiaoZhunZXCount;

	void OpenJiaoYanPanelObj(SelectJiaoZhunDate selectVal)
	{
		if (DirAdjustObj.activeSelf || PedalAdjustObj.activeSelf || GunAdjustObj.activeSelf) {
			if (!GunAdjustObj.activeSelf) {
				CloseAllJiaoYanPanel();
			}
			return;
		}

		IsMoveStar = false;
		StarObj.SetActive(false);
		switch (selectVal) {
		case SelectJiaoZhunDate.DirAdjust:
			InitAdjustDir();
			DirAdjustObj.SetActive(true);
			PedalAdjustObj.SetActive(false);
			GunAdjustObj.SetActive(false);
			IsJiaoZhunCross = false;
			SetPanelGunCrossCtrl.GetInstance().SetGunCrossActive(false);
			break;

		case SelectJiaoZhunDate.PedalAdjust:
			if (YouMenSt == YouMenTaBanEnum.JiaoTaBan) {
				TaBanAdjustSprite.spriteName = "JiaoZhunTB";
			}

			if (YouMenSt == YouMenTaBanEnum.YouMenTaBan) {
				TaBanAdjustSprite.spriteName = "JiaoZhunYMTB";
				pcvr.InitUpdateYouMenMinVal();
			}
			DirAdjustObj.SetActive(false);
			PedalAdjustObj.SetActive(true);
			GunAdjustObj.SetActive(false);
			IsJiaoZhunCross = false;
			SetPanelGunCrossCtrl.GetInstance().SetGunCrossActive(false);
			break;

		case SelectJiaoZhunDate.GunAdjust:
			InitAdjustGunCross();
			InitJiaoZhunGunUI();
			JiaoZhunZXCount = 0;
			DirAdjustObj.SetActive(false);
			PedalAdjustObj.SetActive(false);
			GunAdjustObj.SetActive(true);
			IsJiaoZhunCross = true;
			if (!pcvr.bIsHardWare) {
				Screen.showCursor = true;
			}
			SetPanelJiaoZhunDianCtrl.GetInstance().OpenJiaoZhunDian();
			break;
		}
	}
	public static bool IsJiaoZhunCross;

	public void SetHitAimObjInfoActive(bool isActive)
	{
		if (isActive == HitAimObjInfo.activeSelf) {
			return;
		}
		HitAimObjInfo.SetActive(isActive);
	}

	void SetSpriteDirTestInfo(float horizontalVal)
	{
		float valTmp = 0.1f;
		if (Mathf.Abs(horizontalVal) <= valTmp){
			if (SpriteDirTestInfo.enabled) {
				SpriteDirTestInfo.enabled = false;
			}
		}
		else {
			if (horizontalVal > valTmp) {
				SpriteDirTestInfo.spriteName = "Right";
			}
			else if (horizontalVal < valTmp) {
				SpriteDirTestInfo.spriteName = "Left";
			}

			if (!SpriteDirTestInfo.enabled) {
				SpriteDirTestInfo.enabled = true;
			}
		}
	}

	void SetLabelSpeedTestInfo(float val)
	{
		if (val <= 0) {
			LabelSpeedTestInfo.text = "0";
		}
		else  {
			int speed = (int)(val * 90);
			if (pcvr.bIsHardWare) {
				//((bikeTaBanNum - TanBanCenterNum) / (TaBanValMax - TanBanCenterNum)) * PcvrTanBanValTmp
				float tmp = val / pcvr.PcvrTanBanValTmp;
				speed = (int)(tmp * 120f);
				if (speed < 1f) {
					speed = 0;
				}
			}
			LabelSpeedTestInfo.text = speed.ToString();
		}
	}

	void OpenTestPanelObj(SelectCeShiDate SelCeShiDt)
	{
		if (DirTestPanelObj.activeSelf || PedalTestPanelObj.activeSelf || GunTestPanelObj.activeSelf) {
			CloseAllTestPanel();
			return;
		}

		switch (SelCeShiDt) {
		case SelectCeShiDate.DirTest:
			StarObj.SetActive(false);
			DirTestPanelObj.SetActive(true);
			PedalTestPanelObj.SetActive(false);
			GunTestPanelObj.SetActive(false);
			QiNangTestPanelObj.SetActive(false);
			break;
		
		case SelectCeShiDate.PedalTest:
			StarObj.SetActive(false);
			DirTestPanelObj.SetActive(false);
			PedalTestPanelObj.SetActive(true);
			GunTestPanelObj.SetActive(false);
			QiNangTestPanelObj.SetActive(false);
			break;
		
		case SelectCeShiDate.GunTest:
			StarObj.SetActive(false);
			DirTestPanelObj.SetActive(false);
			PedalTestPanelObj.SetActive(false);
			GunTestPanelObj.SetActive(true);
			QiNangTestPanelObj.SetActive(false);
			SetPanelGunCrossCtrl.GetInstance().SetGunCrossActive(true);
			SetPanelGunCrossCtrl.GetInstance().SetAimObjArrayActive(true);
			break;
			
		case SelectCeShiDate.QiNangTest:
			StarObj.SetActive(true);
			DirTestPanelObj.SetActive(false);
			PedalTestPanelObj.SetActive(false);
			GunTestPanelObj.SetActive(false);
			QiNangTestPanelObj.SetActive(true);
			QiNangTestState = QiNangTestEnum.Null;
			SetQiNangTestStarPosition();
			break;
		}
	}

	void SetQiNangTestStarPosition()
	{
		int num = (int)QiNangTestState;
		num = num >= (int)QiNangTestEnum.QNEixt ? 0 : (num + 1);
		StarTran.localPosition = QiNangTestStarPos[num];
		QiNangTestState = (QiNangTestEnum)num;
	}

	void OnClickQiNangTestEnvent()
	{
		//Debug.Log("OnClickQiNangTestEnvent "+QiNangTestState);
		switch (QiNangTestState) {
		case QiNangTestEnum.QNLF_CQ:
			pcvr.QiNangArray[0] = 1;
			break;
			
		case QiNangTestEnum.QNLF_FQ:
			pcvr.QiNangArray[0] = 0;
			break;
			
		case QiNangTestEnum.QNRF_CQ:
			pcvr.QiNangArray[1] = 1;
			break;
			
		case QiNangTestEnum.QNRF_FQ:
			pcvr.QiNangArray[1] = 0;
			break;

		case QiNangTestEnum.QNLM_CQ:
			pcvr.QiNangArray[3] = 1;
			break;
			
		case QiNangTestEnum.QNLM_FQ:
			pcvr.QiNangArray[3] = 0;
			break;
			
		case QiNangTestEnum.QNRM_CQ:
			pcvr.QiNangArray[2] = 1;
			break;
			
		case QiNangTestEnum.QNRM_FQ:
			pcvr.QiNangArray[2] = 0;
			break;
			
		case QiNangTestEnum.QNLB_CQ:
			pcvr.QiNangArray[4] = 1;
			break;
			
		case QiNangTestEnum.QNLB_FQ:
			pcvr.QiNangArray[4] = 0;
			break;

		case QiNangTestEnum.QNRB_CQ:
			pcvr.QiNangArray[5] = 1;
			break;
			
		case QiNangTestEnum.QNRB_FQ:
			pcvr.QiNangArray[5] = 0;
			break;

		case QiNangTestEnum.QNEixt:
			pcvr.CloseAllQiNangArray();
			CloseAllTestPanel();
			break;
		}
	}

	void CloseAllTestPanel()
	{
		DirTestPanelObj.SetActive(false);
		PedalTestPanelObj.SetActive(false);
		GunTestPanelObj.SetActive(false);
		QiNangTestPanelObj.SetActive(false);
		SetPanelGunCrossCtrl.GetInstance().SetGunCrossActive(false);
		StarObj.SetActive(true);
		StarMoveCount = StarMoveCount > 0 ? (StarMoveCount - 1) : 0;
		MoveStarImg();
	}

	void ChangeGuiPanel()
	{
		PanelState stValTmp = PanelStVal;
		switch (PanelStVal) {
		case PanelState.SetPanel:
			SelectSetPanelDate SetPanelDt = (SelectSetPanelDate) StarMoveCount;
			if (SetPanelDt == SelectSetPanelDate.Adjust) {
				JiaoZhunPanelObj.SetActive(true);
				CeShiPanelObj.SetActive(false);
				PanelStVal = PanelState.JiaoYanPanel;
			}
			else if (SetPanelDt == SelectSetPanelDate.HardwareTest) {
				JiaoZhunPanelObj.SetActive(false);
				CeShiPanelObj.SetActive(true);
				PanelStVal = PanelState.CeShiPanel;
			}
			SetPanelObj.SetActive(false);
			break;
			
		case PanelState.JiaoYanPanel:
		case PanelState.CeShiPanel:
			JiaoZhunPanelObj.SetActive(false);
			CeShiPanelObj.SetActive(false);
			SetPanelObj.SetActive(true);
			PanelStVal = PanelState.SetPanel;
			break;
		}

		if (stValTmp == PanelState.CeShiPanel) {
			ResetStarImgPos(false);
		}
		else {
			ResetStarImgPos(true);
		}
	}

	void OpenCeShiPanel()
	{
		JiaoZhunPanelObj.SetActive(false);
		CeShiPanelObj.SetActive(true);
		PanelStVal = PanelState.CeShiPanel;
		ResetStarImgPos(true);
	}

	void OpenSetPanel()
	{
		ChangeGuiPanel();
	}

	void ExitSetPanle()
	{
		BackMovieScene();
	}

	void ResetFactoryInfo()
	{
		ResetPlayerCoinCur();
		GlobalData.GetInstance().XUTOUBI = 1;

		GlobalData.GetInstance().GameDiff = "1";
		GlobalData.GetInstance().IsFreeMode = false;

		handleJsonObj.WriteToFileXml(fileName, "START_COIN", GlobalData.GetInstance().XUTOUBI.ToString());
		handleJsonObj.WriteToFileXml(fileName, "GAME_DIFFICULTY", "1");
		handleJsonObj.WriteToFileXml(fileName, "GAME_MODE", "1");

		GameAudioVolume = 7;
		GameAudioVolumeLB.text = GameAudioVolume.ToString();
		handleJsonObj.WriteToFileXml(fileName, "GameAudioVolume", "7");
		GlobalData.GameAudioVolume = GameAudioVolume;

		InitCoinStartLabel();
		InitGameDiffDuiGou();
		InitGameModeDuiGou();
	}

	void InitStarImgPos()
	{
		MoveStarImg();
	}

	void ResetStarImgPos(bool isReset)
	{
		if (isReset) {
			StarMoveCount = 0;
		}
		else {
			StarMoveCount = (int)SelectSetPanelDate.Adjust;
		}
		InitStarImgPos();
	}

	void MoveStarImg()
	{
		if (!StarObj.activeSelf) {
			return;
		}

		Vector3 pos = Vector3.zero;
		switch(PanelStVal)
		{
		case PanelState.SetPanel:
			if (StarMoveCount >= SetPanelStarPos.Length) {
				StarMoveCount = 0;
			}
			pos = SetPanelStarPos[StarMoveCount];
			break;

		case PanelState.JiaoYanPanel:
		case PanelState.CeShiPanel:
			if (QiNangTestPanelObj.activeSelf) {
				SetQiNangTestStarPosition();
				return;
			}

			if (StarMoveCount >= JZCSStarPos.Length) {
				StarMoveCount = 0;
			}

			if (PanelStVal == PanelState.JiaoYanPanel) {
				if (StarMoveCount >= (JZCSStarPos.Length-1)) {
					StarMoveCount = 0;
				}
			}
			pos = JZCSStarPos[StarMoveCount];
			break;
		}

		if (IsMoveStar) {
			StarTran.localPosition = pos;
			StarMoveCount++;
		}
	}

	void ResetPlayerCoinCur()
	{
		if (pcvr.bIsHardWare) {
			pcvr.GetInstance().SubPlayerCoin(GlobalData.GetInstance().Icoin);
		}
		GlobalData.GetInstance().Icoin = 0;
	}

	void BackMovieScene()
	{
		if(Application.loadedLevel != (int)GameLeve.Movie)
		{
			//ResetPlayerCoinCur(); //test
			GlobalData.GetInstance().gameLeve = GameLeve.Movie;
			System.GC.Collect();
			Application.LoadLevel((int)GameLeve.Movie);
		}
	}
	
	void InitGameAudioValue()
	{
		string val = handleJsonObj.ReadFromFileXml(fileName, "GameAudioVolume");
		if (val == null || val == "") {
			val = "7";
			handleJsonObj.WriteToFileXml(fileName, "GameAudioVolume", val);
		}
		GameAudioVolume = Convert.ToInt32(val);
		GameAudioVolumeLB.text = GameAudioVolume.ToString();
	}
}

public enum AdjustGunDrossState
{
	GunCrossLU = 0,
	GunCrossRU,
	GunCrossRD,
	GunCrossLD,
	GunCrossCen,
	GunCrossOver,
}

public enum YouMenTaBanEnum
{
	JiaoTaBan,		//机台采用脚踏板来控制运动.
	YouMenTaBan,	//机台采用油门踏板来控制运动.
}