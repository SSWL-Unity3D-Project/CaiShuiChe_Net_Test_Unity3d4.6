#define TEST_INPUT_CROSS_POS
//#define TEST_SHUIQIANG_ZUOBIAO_PINGJUN
#define TEST_SHUIQIANG_ZUOBIAO
#define USE_LINE_HIT_CROSS
//USE_LINE_HIT_CROSS -> 使用射线碰撞屏幕来控制准星位置.
//测试水枪坐标.
//#define USE_ZHUNXING_JZ_36
//USE_ZHUNXING_JZ_36 -> 将屏幕划分为36块进行坐标校准.
using UnityEngine;
using System.Collections;
using SLAB_HID_DEVICE;
using System;
using System.Collections.Generic;

public class pcvr : MonoBehaviour {
	public static bool bIsHardWare = false;
	public static bool IsGetValByKey = false;
	private static int HID_BUF_LEN = 23;
	public static bool IsUseZhunXingJZ_36;
	public static bool IsUseLineHitCross;
	
	private static int openPCVRFlag = 1;
	float lastUpTime = 0.0f;
	public static float mGetSteer = 0f;
	public static Vector3 MousePosition;
	public static uint gOldCoinNum = 0;
	private uint mOldCoinNum = 0;
	public int CoinNumCurrent = 0;

	public static bool bPlayerHitTaBan_P1 = false;
	public static bool bPlayerHitTaBan_P2 = false;
	
	public static float VerticalVal;
	public static float TanBanDownCount_P1 = 0;
	public static float TanBanDownCount_P2 = 0;

	public static LedState StartLightStateP1 = LedState.Mie;
	public static LedState StartLightStateP2 = LedState.Mie;
	public static bool IsOpenStartLightP1 = false;
	public static bool IsOpenStartLightP2 = false;
	int subCoinNum = 0;
	
	static string fileName;
	static HandleJson handleJsonObj;

	public static uint SteerValMax = 999999;
	public static uint SteerValCen = 1765;
	public static uint SteerValMin = 0;
	public static uint SteerValCur;
	public static uint SteerDisVal;
	
	public static uint TaBanValMax;
	public static uint TaBanValMin;
	public static uint TaBanValCur = 30000;
	public static uint TaBanDisVal;
	
	public static uint CrossPosXMax;
	public static uint CrossPosXMin;
	public static uint CrossPosXCur;
	
	public static uint CrossPosYMax;
	public static uint CrossPosYMin;
	public static uint CrossPosYCur;
	
	public static uint CrossPosDisX;
	public static uint CrossPosDisY;
	
	public static Vector3 CrossPosition;

	static uint CrossPx_1;
	static uint CrossPx_2;
	static uint CrossPx_3;
	static uint CrossPx_4;
	
	static uint CrossPy_1;
	static uint CrossPy_2;
	static uint CrossPy_3;
	static uint CrossPy_4;
	
	public static bool IsOpenShuiBeng = false;
	public static PcvrShuiBengState ShuiBengState = PcvrShuiBengState.Level_1;
	static uint TanBanCenterNum = 30000;
	static float SubTaBanCount = 2.0f;
	
	bool IsSubPlayerCoin = false;
	bool IsSubCoinP1 = false;
	bool IsSubCoinP2 = false;
	static public bool IsFireBtDownP2 = false;
	static public bool bPlayerStartKeyDownP1 = false;
	static public bool bPlayerStartKeyDownP2 = false;
	static public bool IsClickDongGanBt = false;
	private bool bSetEnterKeyDown = false;
	static public bool bSetMoveKeyDown = false;
	static bool IsFanZhuangTaBan = false;
	public static uint CoinCurPcvr;
	//9500.0f -> maxSpeed(60km/h)
	public static float PcvrTanBanValTmp = 1.8f * 9500.0f;
	public static bool IsJiaoYanHid;
	public static bool IsSlowLoopCom;
	static private pcvr Instance = null;
	static public pcvr GetInstance()
	{
		if(Instance == null)
		{
			GameObject obj = new GameObject("_PCVR");
			DontDestroyOnLoad(obj);
			Instance = obj.AddComponent<pcvr>();
			if (bIsHardWare) {
				obj.AddComponent<MyCOMDevice>();
			}
			ScreenLog.init();

			#if USE_ZHUNXING_JZ_36
			IsUseZhunXingJZ_36 = true;
			#endif

			#if TEST_SHUIQIANG_ZUOBIAO_PINGJUN
			JZPoint = PcvrJZCrossPoint.Num7;
			#elif USE_ZHUNXING_JZ_36
			JZPoint = PcvrJZCrossPoint.Num49;
			#endif
			
			#if USE_LINE_HIT_CROSS
			JZPoint = PcvrJZCrossPoint.Num4;
			IsUseLineHitCross = true;
			#endif
		}
		return Instance;
	}

	void Start()
	{
		InitJiaoYanMiMa();
		lastUpTime = Time.realtimeSinceStartup;
		InitHandleJsonInfo();
		InitSteerInfo();
		InitTaBanInfo();
		InitCrossPosInfo();
	}

	void FixedUpdate()
	{
		if (MyCOMDevice.ComThreadClass.IsLoadingLevel) {
			return;
		}
		
		if (!MyCOMDevice.ComThreadClass.IsReadComMsg) {
			return;
		}

		UpdatePcvrCrossPos(MyCOMDevice.ComThreadClass.ReadByteMsg);
		CheckCrossPosition();
	}

	// Update is called once per frame
	void Update()
	{
		if (MyCOMDevice.ComThreadClass.IsLoadingLevel) {
			return;
		}

		GetPcvrTaBanVal();
		GetPcvrSteerVal();
		CheckCrossPosition();
		CheckIsCloseJiaoYanIO();
		if (!bIsHardWare) {
			return;
		}

		float dTime = Time.realtimeSinceStartup - lastUpTime;
		if (IsJiaoYanHid) {
			if (dTime < 0.1f) {
				return;
			}
		}
		else {
			if (dTime < 0.03f) {
				return;
			}
		}
		lastUpTime = Time.realtimeSinceStartup;
		
		GetMessage();
		SendMessage();
	}

	static byte WriteHead_1 = 0x02;
	static byte WriteHead_2 = 0x55;
	static byte WriteEnd_1 = 0x0d;
	static byte WriteEnd_2 = 0x0a;
	byte EndRead_1 = 0x41;
	byte EndRead_2 = 0x42;
	/**
****************.显示器.****************
QiNangArray[0]			QiNangArray[1]
QiNangArray[3]			QiNangArray[2]
	 */
	public static byte[] QiNangArray = {0, 0, 0, 0, 0, 0};
	public static void CloseAllQiNangArray()
	{
		for (int i = 0; i < QiNangArray.Length; i++) {
			QiNangArray[i] = 0;
		}
	}

	static bool IsOpenQiNangQian;
	static bool IsOpenQiNangHou;
	static bool IsOpenQiNangZuo;
	static bool IsOpenQiNangYou;
	public static void OpenQiNangQian()
	{
		if (IsOpenQiNangQian) {
			return;
		}
		IsOpenQiNangQian = true;
		QiNangArray[0] = 1;
		QiNangArray[1] = 1;
	}

	public static void CloseQiNangQian()
	{
		if (!IsOpenQiNangQian) {
			return;
		}
		IsOpenQiNangQian = false;
		QiNangArray[0] = 0;
		QiNangArray[1] = 0;
	}
	
	public static void OpenQiNangHou()
	{
		if (IsOpenQiNangHou) {
			return;
		}
		IsOpenQiNangHou = true;
		QiNangArray[2] = 1;
		QiNangArray[3] = 1;
	}
	
	public static void CloseQiNangHou()
	{
		if (!IsOpenQiNangHou) {
			return;
		}
		IsOpenQiNangHou = false;
		QiNangArray[2] = 0;
		QiNangArray[3] = 0;
	}

	public static void OpenQiNangZuo()
	{
		if (IsOpenQiNangZuo) {
			return;
		}
		IsOpenQiNangZuo = true;
		QiNangArray[0] = 1;
		QiNangArray[3] = 1;
	}

	public static void CloseQiNangZuo()
	{
		if (!IsOpenQiNangZuo) {
			return;
		}
		IsOpenQiNangZuo = false;
		QiNangArray[0] = 0;
		QiNangArray[3] = 0;
	}

	public static void OpenQiNangYou()
	{
		if (IsOpenQiNangYou) {
			return;
		}
		IsOpenQiNangYou = true;
		QiNangArray[1] = 1;
		QiNangArray[2] = 1;
	}
	
	public static void CloseQiNangYou()
	{
		if (!IsOpenQiNangYou) {
			return;
		}
		IsOpenQiNangYou = false;
		QiNangArray[1] = 0;
		QiNangArray[2] = 0;
	}

	static void OpenAllQiNang()
	{
		for (int i = 0; i < QiNangArray.Length; i++) {
			QiNangArray[i]  = 1;
		}
	}

	public static bool IsPlayerHitShake;
	public void OnPlayerHitShake()
	{
		if (IsPlayerHitShake) {
			return;
		}
		IsPlayerHitShake = true;
		//ScreenLog.Log("OnPlayerHitShake...");
		StartCoroutine(PcvrQiNangHitShake());
	}

	void ClosePlayerHitShake()
	{
		if (!IsPlayerHitShake) {
			return;
		}
		IsPlayerHitShake = false;
		//ScreenLog.Log("ClosePlayerHitShake...");
	}
	
	IEnumerator PcvrQiNangHitShake()
	{
		bool isHitShake = true;
		int count = 0;
		do {
			if (count % 2 == 0) {
//				OpenQiNangZuo();
//				CloseQiNangYou();
				OpenAllQiNang();
			}
			else {
//				OpenQiNangYou();
//				CloseQiNangZuo();
				CloseAllQiNangArray();
			}
			yield return new WaitForSeconds(0.8f);

			if (count >= 3) {
				isHitShake = false;
				ClosePlayerHitShake();
				yield break;
			}
			count++;
		} while (isHitShake);
	}
	
	void SendMessage()
	{
		if (!MyCOMDevice.IsFindDeviceDt) {
			return;
		}

		byte []buffer;
		buffer = new byte[HID_BUF_LEN];
		buffer[0] = WriteHead_1;
		buffer[1] = WriteHead_2;
		buffer[HID_BUF_LEN - 2] = WriteEnd_1;
		buffer[HID_BUF_LEN - 1] = WriteEnd_2;
		for (int i = 4; i < HID_BUF_LEN - 2; i++) {
			buffer[i] = (byte)(UnityEngine.Random.Range(0, 10000) % 256);
		}
		
		if (IsSubPlayerCoin) {
			buffer[2] = 0xaa;
			buffer[3] = (byte)subCoinNum;
		}
		
		switch (StartLightStateP1) {
		case LedState.Liang:
			buffer[4] |= 0x01;
			break;
			
		case LedState.Shan:
			buffer[4] |= 0x01;
			break;
			
		case LedState.Mie:
			buffer[4] &= 0xfe;
			break;
		}
		
		switch (StartLightStateP2) {
		case LedState.Liang:
			buffer[4] |= 0x02;
			break;
			
		case LedState.Shan:
			buffer[4] |= 0x02;
			break;
			
		case LedState.Mie:
			buffer[4] &= 0xfd;
			break;
		}

		if (DongGanState == 1 || HardwareCheckCtrl.IsTestHardWare) {
			buffer[5] = (byte)(QiNangArray[0] + (QiNangArray[1] << 1) + (QiNangArray[2] << 2) + (QiNangArray[3] << 3));
		}
		else {
			buffer[5] = 0x00;
		}

		if (IsOpenShuiBeng) {
			switch (ShuiBengState) {
			case PcvrShuiBengState.Level_1:
				buffer[5] |= 0x10;
				break;
				
			case PcvrShuiBengState.Level_2:
				buffer[5] |= 0x10;
				break;
			}
		}
		else {
			buffer[5] &= 0x0f;
		}

		buffer[8] = 0x00;
		buffer[9] = 0x00;
		
		if (IsJiaoYanHid) {
			for (int i = 0; i < 4; i++) {
				buffer[i + 10] = JiaoYanMiMa[i];
			}
			
			for (int i = 0; i < 4; i++) {
				buffer[i + 14] = JiaoYanDt[i];
			}
		}
		else {
			RandomJiaoYanMiMaVal();
			for (int i = 0; i < 4; i++) {
				buffer[i + 10] = JiaoYanMiMaRand[i];
			}
			
			//0x41 0x42 0x43 0x44
			for (int i = 15; i < 18; i++) {
				buffer[i] = (byte)UnityEngine.Random.Range(0x00, 0x40);
			}
			buffer[14] = 0x00;
			
			for (int i = 15; i < 18; i++) {
				buffer[14] ^= buffer[i];
			}
		}

		buffer[6] = 0x00;
		for (int i = 2; i <= 11; i++) {
			if (i == 6) {
				continue;
			}
			buffer[6] ^= buffer[i];
		}
		
//		buffer[10] = 0x00;
//		for (int i = 11; i <= 13; i++) {
//			buffer[10] ^= buffer[i];
//		}

//		buffer[14] = 0x00;
//		for (int i = 15; i <= 17; i++) {
//			buffer[14] ^= buffer[i];
//		}

		buffer[19] = 0x00;
		for (int i = 0; i < HID_BUF_LEN; i++) {
			if (i == 19) {
				continue;
			}
			buffer[19] ^= buffer[i];
		}
		MyCOMDevice.ComThreadClass.WriteByteMsg = buffer;
	}

	void GetMessage()
	{
		if (!MyCOMDevice.ComThreadClass.IsReadComMsg) {
			return;
		}
		
		if (MyCOMDevice.ComThreadClass.IsReadMsgComTimeOut) {
			return;
		}
		
		if (MyCOMDevice.ComThreadClass.ReadByteMsg.Length < (MyCOMDevice.ComThreadClass.BufLenRead - MyCOMDevice.ComThreadClass.BufLenReadEnd)) {
			//ScreenLog.Log("ReadBufLen was wrong! len is "+MyCOMDevice.ComThreadClass.ReadByteMsg.Length);
			return;
		}
		
		if (IsJiOuJiaoYanFailed) {
			return;
		}
		
		if ((MyCOMDevice.ComThreadClass.ReadByteMsg[22]&0x01) == 0x01) {
			JiOuJiaoYanCount++;
			if (JiOuJiaoYanCount >= JiOuJiaoYanMax && !IsJiOuJiaoYanFailed) {
				IsJiOuJiaoYanFailed = true;
				//JiOuJiaoYanFailed
			}
		}
//		//IsJiOuJiaoYanFailed = true; //test
//		
		byte tmpVal = 0x00;
		string testA = "";
		for (int i = 2; i < (MyCOMDevice.ComThreadClass.BufLenRead - 4); i++) {
			if (i == 18 || i == 21) {
				continue;
			}
			testA += MyCOMDevice.ComThreadClass.ReadByteMsg[i].ToString("X2") + " ";
			tmpVal ^= MyCOMDevice.ComThreadClass.ReadByteMsg[i];
		}
		tmpVal ^= EndRead_1;
		tmpVal ^= EndRead_2;
		testA += EndRead_1 + " ";
		testA += EndRead_2 + " ";
		
		if (tmpVal != MyCOMDevice.ComThreadClass.ReadByteMsg[21]) {
			if (MyCOMDevice.ComThreadClass.IsStopComTX) {
				return;
			}
			MyCOMDevice.ComThreadClass.IsStopComTX = true;
//			ScreenLog.Log("testA: "+testA);
//			ScreenLog.LogError("tmpVal: "+tmpVal.ToString("X2")+", byte[21] "+MyCOMDevice.ComThreadClass.ReadByteMsg[21].ToString("X2"));
//			ScreenLog.Log("byte21 was wrong!");
			return;
		}
		
		if (IsJiaoYanHid) {
			tmpVal = 0x00;
			//string testStrA = MyCOMDevice.ComThreadClass.ReadByteMsg[30].ToString("X2") + " ";
//			string testStrB = "";
//			string testStrA = "";
//			for (int i = 0; i < MyCOMDevice.ComThreadClass.ReadByteMsg.Length; i++) {
//				testStrA += MyCOMDevice.ComThreadClass.ReadByteMsg[i].ToString("X2") + " ";
//			}
//			ScreenLog.Log("readStr: "+testStrA);
//
//			for (int i = 0; i < JiaoYanDt.Length; i++) {
//				testStrB += JiaoYanDt[i].ToString("X2") + " ";
//			}
//			ScreenLog.Log("GameSendDt: "+testStrB);
//
//			string testStrC = "";
//			for (int i = 0; i < JiaoYanDt.Length; i++) {
//				testStrC += JiaoYanMiMa[i].ToString("X2") + " ";
//			}
//			ScreenLog.Log("GameSendMiMa: "+testStrC);
			
			for (int i = 11; i < 14; i++) {
				tmpVal ^= MyCOMDevice.ComThreadClass.ReadByteMsg[i];
				//testStrA += MyCOMDevice.ComThreadClass.ReadByteMsg[i].ToString("X2") + " ";
			}
			
			if (tmpVal == MyCOMDevice.ComThreadClass.ReadByteMsg[10]) {
				bool isJiaoYanDtSucceed = false;
				tmpVal = 0x00;
				for (int i = 15; i < 18; i++) {
					tmpVal ^= MyCOMDevice.ComThreadClass.ReadByteMsg[i];
				}
				
				//校验2...
				if ( tmpVal == MyCOMDevice.ComThreadClass.ReadByteMsg[14]
				    && (JiaoYanDt[1]&0xef) == MyCOMDevice.ComThreadClass.ReadByteMsg[15]
				    && (JiaoYanDt[2]&0xfe) == MyCOMDevice.ComThreadClass.ReadByteMsg[16]
				    && (JiaoYanDt[3]|0x28) == MyCOMDevice.ComThreadClass.ReadByteMsg[17] ) {
					isJiaoYanDtSucceed = true;
				}

				JiaoYanCheckCount++;
				if (isJiaoYanDtSucceed) {
					//JiaMiJiaoYanSucceed
					OnEndJiaoYanIO(JIAOYANENUM.SUCCEED);
					//ScreenLog.Log("JMJYCG...");
				}
				else {
					if (JiaoYanCheckCount > 0) {
						OnEndJiaoYanIO(JIAOYANENUM.FAILED);
					}
//					testStrA = "";
//					for (int i = 0; i < 23; i++) {
//						testStrA += MyCOMDevice.ComThreadClass.ReadByteMsg[i].ToString("X2") + " ";
//					}
//
//					testStrB = "";
//					for (int i = 14; i < 18; i++) {
//						testStrB += MyCOMDevice.ComThreadClass.ReadByteMsg[i].ToString("X2") + " ";
//					}
//					
//					string testStrD = "";
//					for (int i = 0; i < 4; i++) {
//						testStrD += JiaoYanDt[i].ToString("X2") + " ";
//					}
//					ScreenLog.Log("ReadByte[0 - 22] "+testStrA);
//					ScreenLog.Log("ReadByte[14 - 17] "+testStrB);
//					ScreenLog.Log("SendByte[14 - 17] "+testStrD);
//					ScreenLog.LogError("校验数据错误!");
				}
			}
//			else {
//				testStrB = "byte[10] "+MyCOMDevice.ComThreadClass.ReadByteMsg[10].ToString("X2")+" "
//					+", tmpVal "+tmpVal.ToString("X2");
//				ScreenLog.Log("ReadByte[10 - 13] "+testStrA);
//				ScreenLog.Log(testStrB);
//				ScreenLog.LogError("ReadByte[10] was wrong!");
//			}
		}

		int len = MyCOMDevice.ComThreadClass.ReadByteMsg.Length;
		uint[] readMsg = new uint[len];
		for (int i = 0; i < len; i++) {
			readMsg[i] = MyCOMDevice.ComThreadClass.ReadByteMsg[i];
		}
		keyProcess(readMsg);
		MyCOMDevice.ComThreadClass.IsReadComMsg = false;
	}
	
	static float OffsetPX = 15f;
	static float OffsetPY = 10f;
	public static byte DongGanState = 1;
	void UpdatePcvrCrossPos(byte []buffer)
	{
		Vector2 crossPos = Vector2.zero;
		#if TEST_SHUIQIANG_ZUOBIAO
		crossPos.x = ((buffer[6] & 0x0f) << 8) + buffer[7];
		crossPos.y = ((buffer[8] & 0x0f) << 8) + buffer[9];
		#else
		crossPos.x = ((buffer[2] & 0x0f) << 8) + buffer[3];
		crossPos.y = ((buffer[4] & 0x0f) << 8) + buffer[5];
		#endif

		if (Mathf.Abs(crossPos.x - MousePosition.x) <= OffsetPX) {
			crossPos.x = MousePosition.x;
		}

		if (Mathf.Abs(crossPos.y - MousePosition.y) <= OffsetPY) {
			crossPos.y = MousePosition.y;
		}
		MousePosition.x = crossPos.x;
		MousePosition.y = crossPos.y;
	}

	void keyProcess(uint []buffer)
	{
		SteerValCur = ((buffer[6] & 0x0f) << 8) + buffer[7]; //fangXiang
		TaBanValCur = (buffer[8] << 8) + buffer[9];                                                           

		//game coinInfo
		CoinCurPcvr = buffer[18];
		uint coinP1 = CoinCurPcvr & 0x0f;
		uint coinP2 = (CoinCurPcvr & 0xf0) >> 4;
		bool isCoinPCBOneTest = true; //test
		if (IsSubPlayerCoin) {
			if (isCoinPCBOneTest) {
				IsSubPlayerCoin = false;
//				if (CoinCurPcvr == 0) {
//					IsSubPlayerCoin = false;
//				}
			}
			else {
				if (coinP1 == 0 && IsSubCoinP1) {
					IsSubCoinP1 = false;
					IsSubPlayerCoin = false;
				}

				if (coinP2 == 0 && IsSubCoinP2) {
					IsSubCoinP2 = false;
					IsSubPlayerCoin = false;
				}
			}
		}
		else {
			if (isCoinPCBOneTest) {
				if (CoinCurPcvr > 0 && CoinCurPcvr < 256) {
					mOldCoinNum += CoinCurPcvr;
					CoinNumCurrent = (int)mOldCoinNum;
					//ScreenLog.Log("CoinNumCurrent "+CoinNumCurrent);
					SubPcvrCoin((int)CoinCurPcvr);
				}
			}
			else {
				if (coinP1 > 0 && coinP1 < 256) {
					IsSubCoinP1 = true;
					mOldCoinNum += coinP1;
					CoinNumCurrent = (int)mOldCoinNum;
					SubPcvrCoin((int)(CoinCurPcvr & 0x0f));
				}

				if (coinP2 > 0 && coinP2 < 256) {
					IsSubCoinP2 = true;
					mOldCoinNum += coinP2;
					CoinNumCurrent = (int)mOldCoinNum;
					SubPcvrCoin((int)(CoinCurPcvr & 0xf0));
				}
			}
		}

		//game startBt, hitNpcBt or jiaoZhunBt
		if( !bPlayerStartKeyDownP1 && (buffer[19]&0x04) == 0x04 )
		{
			//ScreenLog.Log("gameP1 startBt down!");
			bPlayerStartKeyDownP1 = true;
			InputEventCtrl.GetInstance().ClickStartBtOne( ButtonState.DOWN );
		}
		else if ( bPlayerStartKeyDownP1 && (buffer[19]&0x04) == 0x00 )
		{
			//ScreenLog.Log("gameP1 startBt up!");
			bPlayerStartKeyDownP1 = false;
			InputEventCtrl.GetInstance().ClickStartBtOne( ButtonState.UP );
		}

		//game startBt, hitNpcBt or jiaoZhunBt
		if( !bPlayerStartKeyDownP2 && (buffer[19]&0x08) == 0x08 )
		{
			//ScreenLog.Log("gameP2 startBt down!");
			bPlayerStartKeyDownP2 = true;
			InputEventCtrl.GetInstance().ClickStartBtTwo( ButtonState.DOWN );
		}
		else if ( bPlayerStartKeyDownP2 && (buffer[19]&0x08) == 0x00 )
		{
			//ScreenLog.Log("gameP2 startBt up!");
			bPlayerStartKeyDownP2 = false;
			InputEventCtrl.GetInstance().ClickStartBtTwo( ButtonState.UP );
		}
		
		//DongGanBt
		if( !IsClickDongGanBt && (buffer[19]&0x10) == 0x10 )
		{
			//ScreenLog.Log("DongGanBt down!");
			IsClickDongGanBt = true;
			InputEventCtrl.GetInstance().ClickStopDongGanBt( ButtonState.DOWN );
		}
		else if ( IsClickDongGanBt && (buffer[19]&0x10) == 0x00 )
		{
			//ScreenLog.Log("DongGanBt up!");
			IsClickDongGanBt = false;
			InputEventCtrl.GetInstance().ClickStopDongGanBt( ButtonState.UP );
		}

		#if TEST_SHUIQIANG_ZUOBIAO
		if( !IsFireBtDownP2 && (buffer[20]&0x01) == 0x01 )
		{
			IsFireBtDownP2 = true;
			InputEventCtrl.GetInstance().ClickFireBt( ButtonState.DOWN );
		}
		else if( IsFireBtDownP2 && (buffer[20]&0x01) == 0x00 )
		{
			IsFireBtDownP2 = false;
			InputEventCtrl.GetInstance().ClickFireBt( ButtonState.UP );
		}
		#else
		if( !IsFireBtDownP2 && (buffer[19]&0x40) == 0x40 )
		{
			IsFireBtDownP2 = true;
			//ScreenLog.Log("game fireBtP2 down!");
			InputEventCtrl.GetInstance().ClickFireBt( ButtonState.DOWN );
		}
		else if( IsFireBtDownP2 && (buffer[19]&0x40) == 0x00 )
		{
			IsFireBtDownP2 = false;
			//ScreenLog.Log("game fireBtP2 up!");
			InputEventCtrl.GetInstance().ClickFireBt( ButtonState.UP );
		}
		#endif

		//setPanel selectBt
		if( !bSetEnterKeyDown && (buffer[19]&0x01) == 0x01 )
		{
			bSetEnterKeyDown = true;
			//ScreenLog.Log("game setEnterBt down!");
			InputEventCtrl.GetInstance().ClickSetEnterBt( ButtonState.DOWN );
		}
		else if ( bSetEnterKeyDown && (buffer[19]&0x01) == 0x00 )
		{
			bSetEnterKeyDown = false;
			//ScreenLog.Log("game setEnterBt up!");
			InputEventCtrl.GetInstance().ClickSetEnterBt( ButtonState.UP );
		}
		
		//setPanel moveBt
		if ( !bSetMoveKeyDown && (buffer[19]&0x02) == 0x02 )
		{
			bSetMoveKeyDown = true;
			//ScreenLog.Log("game setMoveBt down!");
			//FramesPerSecond.GetInstance().ClickSetMoveBtEvent( ButtonState.DOWN );
			InputEventCtrl.GetInstance().ClickSetMoveBt( ButtonState.DOWN );
		}
		else if( bSetMoveKeyDown && (buffer[19]&0x02) == 0x00 )
		{
			bSetMoveKeyDown = false;
			//ScreenLog.Log("game setMoveBt up!");
			//FramesPerSecond.GetInstance().ClickSetMoveBtEvent( ButtonState.UP );
			InputEventCtrl.GetInstance().ClickSetMoveBt( ButtonState.UP );
		}
	}

	static void SubTaBanCountInfo()
	{
		if(TanBanDownCount_P1 > 0)
		{
			TanBanDownCount_P1 -= SubTaBanCount;
			if(TanBanDownCount_P1 < 0)
			{
				TanBanDownCount_P1 = 0;
			}
		}
	}

	void closeDevice()
	{
		if (openPCVRFlag == 1)
		{
			openPCVRFlag = 2;
		}
	}

	void SubPcvrCoin(int subNum)
	{
		if (!bIsHardWare) {
			return;
		}
		IsSubPlayerCoin = true;
		subCoinNum = subNum;
	}

	public void SubPlayerCoin(int subNum)
	{
		if (!bIsHardWare) {
			return;
		}

		if (gOldCoinNum >= subNum) {
			gOldCoinNum = (uint)(gOldCoinNum - subNum);
		}
		else {
			if (mOldCoinNum == 0) {
				return;
			}
			subCoinNum = (int)(subNum - gOldCoinNum);
			
			mOldCoinNum -= (uint)subCoinNum;
			CoinNumCurrent = (int)mOldCoinNum;
			gOldCoinNum = 0;
		}
	}

	public static void InitHandleJsonInfo()
	{
		fileName = GlobalData.fileName;
		handleJsonObj = GlobalData.handleJsonObj;
	}

	public static void InitSteerInfo()
	{
		string strValMax = handleJsonObj.ReadFromFileXml(fileName, "SteerValMax");
		if(strValMax == null || strValMax == "")
		{
			strValMax = "1";
			handleJsonObj.WriteToFileXml(fileName, "SteerValMax", strValMax);
		}
		SteerValMax = Convert.ToUInt32( strValMax );
		
		string strValMin = handleJsonObj.ReadFromFileXml(fileName, "SteerValMin");
		if(strValMin == null || strValMin == "")
		{
			strValMin = "1";
			handleJsonObj.WriteToFileXml(fileName, "SteerValMin", strValMin);
		}
		SteerValMin = Convert.ToUInt32( strValMin );

		string strValCen = handleJsonObj.ReadFromFileXml(fileName, "SteerValCen");
		if(strValCen == null || strValCen == "")
		{
			strValCen = "1";
			handleJsonObj.WriteToFileXml(fileName, "SteerValCen", strValCen);
		}
		SteerValCen = Convert.ToUInt32( strValCen );

		SteerDisVal = (uint)Mathf.Abs((float)SteerValMax - SteerValMin);
		//ScreenLog.Log("SteerDisVal " + SteerDisVal + ", SteerValMax " + SteerValMax + ", SteerValMin " + SteerValMin);
	}

	public static void SaveSteerVal(uint steerVal, PcvrValState key)
	{
		switch (key) {
		case PcvrValState.ValMin:
			SteerValMin = steerVal;
			handleJsonObj.WriteToFileXml(fileName, "SteerValMin", steerVal.ToString());
			break;
			
		case PcvrValState.ValCenter:
			SteerValCen = steerVal;
			handleJsonObj.WriteToFileXml(fileName, "SteerValCen", steerVal.ToString());
			break;

		case PcvrValState.ValMax:
			SteerValMax = steerVal;
			handleJsonObj.WriteToFileXml(fileName, "SteerValMax", steerVal.ToString());
			break;
		}
		SteerDisVal = (uint)Mathf.Abs((float)SteerValMax - SteerValMin);
	}
	
	public static float GetPcvrSteerVal()
	{
		float steerCur = 0f;
		if (IsGetValByKey || !bIsHardWare) {
			if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.001f) {
				steerCur = Input.GetAxis("Horizontal") > 0f ? 1f : -1f;
			}
			else {
				steerCur = 0f;
			}
			mGetSteer = steerCur;
			return mGetSteer;
		}

		if (bIsHardWare && openPCVRFlag == 0) {
			mGetSteer = 0f;
			return mGetSteer;
		}

		if (SteerDisVal == 0) {
			mGetSteer = 0f;
			return mGetSteer;
		}

		//steerCur = (float)SteerValCur;
		uint bikeDir = SteerValCur;
		uint bikeDirLen = SteerValMax - SteerValMin + 1;
		if(SteerValMax < SteerValMin)
		{
			if(bikeDir > SteerValCen)
			{
				bikeDirLen = SteerValMin - SteerValCen + 1;
			}
			else
			{
				bikeDirLen = SteerValCen - SteerValMax + 1;
			}
			
			//check bikeDir
			if(bikeDir < SteerValMax)
			{
				bikeDir = SteerValMax;
			}
			else if(bikeDir > SteerValMin)
			{
				bikeDir = SteerValMin;
			}
		}
		else
		{
			if(bikeDir > SteerValCen)
			{
				bikeDirLen = SteerValMax - SteerValCen + 1;
			}
			else
			{
				bikeDirLen = SteerValCen - SteerValMin + 1;
			}
			
			//check bikeDir
			if(bikeDir < SteerValMin)
			{
				bikeDir = SteerValMin;
			}
			else if(bikeDir > SteerValMax)
			{
				bikeDir = SteerValMax;
			}
		}
		////ScreenLog.Log("bikeDirLen = " + bikeDirLen);
		
		if(bikeDirLen == 0)
		{
			bikeDirLen = 1;
		}
		
		uint bikeDirCur = SteerValMax - bikeDir;
		float bikeDirPer = (float)bikeDirCur / bikeDirLen;
		if(SteerValMax > SteerValMin)
		{
			//ZhengJie FangXiangDianWeiQi
			if(bikeDir > SteerValCen)
			{
				bikeDirCur = bikeDir - SteerValCen;
				bikeDirPer = (float)bikeDirCur / bikeDirLen;
			}
			else
			{
				bikeDirCur = SteerValCen - bikeDir;
				bikeDirPer = - (float)bikeDirCur / bikeDirLen;
			}
		}
		else
		{
			//FanJie DianWeiQi
			if(bikeDir > SteerValCen)
			{
				bikeDirCur = bikeDir - SteerValCen;
				bikeDirPer = - (float)bikeDirCur / bikeDirLen;
			}
			else
			{
				bikeDirCur = SteerValCen - bikeDir;
				bikeDirPer = (float)bikeDirCur / bikeDirLen;
			}
		}
		mGetSteer = bikeDirPer;

		/*TestValStr = tmpVal.ToString() + " *** " + steerCur.ToString() + " * " + Input.GetAxis("Horizontal") + " ** " + mGetSteer;
		ScreenLog.Log(TestValStr);*/
		return mGetSteer;
	}

	public static void InitTaBanInfo()
	{
		string strValMax = handleJsonObj.ReadFromFileXml(fileName, "TaBanValMax");
		if(strValMax == null || strValMax == "")
		{
			strValMax = "2";
			handleJsonObj.WriteToFileXml(fileName, "TaBanValMax", strValMax);
		}
		TaBanValMax = Convert.ToUInt32( strValMax );
		
		string strValMin = handleJsonObj.ReadFromFileXml(fileName, "TaBanValMin");
		if(strValMin == null || strValMin == "")
		{
			strValMin = "1";
			handleJsonObj.WriteToFileXml(fileName, "TaBanValMin", strValMin);
		}
		TaBanValMin = Convert.ToUInt32( strValMin );

		string strIsFanZhuangTaBan = handleJsonObj.ReadFromFileXml(fileName, "IsFanZhuangTaBan");
		if(strIsFanZhuangTaBan == null || strIsFanZhuangTaBan == "")
		{
			strIsFanZhuangTaBan = "0";
			handleJsonObj.WriteToFileXml(fileName, "IsFanZhuangTaBan", strIsFanZhuangTaBan);
		}
		IsFanZhuangTaBan = strIsFanZhuangTaBan == "0" ? false : true;
		TaBanDisVal = (uint)Mathf.Abs((float)TaBanValMax - TaBanValMin);
		//ScreenLog.Log("TaBanDisVal " + TaBanDisVal + ", TaBanValMax " + TaBanValMax + ", TaBanValMin " + TaBanValMin);
	}

	public static void SaveTaBanVal(uint TaBanVal, PcvrValState key)
	{
		switch (key) {
		case PcvrValState.ValMin:
			TaBanValMin = TaBanVal;
			handleJsonObj.WriteToFileXml(fileName, "TaBanValMin", TaBanVal.ToString());
			break;
			
		case PcvrValState.ValMax:
			if (!bIsHardWare) {
				SaveTaBanVal(1, PcvrValState.ValMin);
			}
			else {
				SaveTaBanVal(TanBanCenterNum, PcvrValState.ValMin);

				uint bikeTaBanNum = TaBanVal;
				if(bikeTaBanNum >= TanBanCenterNum)
				{
					IsFanZhuangTaBan = false;
					handleJsonObj.WriteToFileXml(fileName, "IsFanZhuangTaBan", "0");
				}
				else if(bikeTaBanNum < TanBanCenterNum)
				{
					IsFanZhuangTaBan = true;
					handleJsonObj.WriteToFileXml(fileName, "IsFanZhuangTaBan", "1");
				}
			}
			TaBanValMax = TaBanVal;
			handleJsonObj.WriteToFileXml(fileName, "TaBanValMax", TaBanVal.ToString());
			TaBanDisVal = (uint)Mathf.Abs((float)TaBanValMax - TaBanValMin);
			break;
		}
	}

	public static float GetPcvrTaBanVal()
	{
		if (IsGetValByKey) {
			TanBanDownCount_P1 = Input.GetAxis("Vertical");
			return TanBanDownCount_P1;
		}

		if (bIsHardWare && openPCVRFlag == 0) {
			TanBanDownCount_P1 = 0f;
			return TanBanDownCount_P1;
		}

		if (TaBanDisVal == 0) {
			TanBanDownCount_P1 = 0f;
			return TanBanDownCount_P1;
		}

		float tmpVal = 0f;
		float taBanCur = 0;
		if (bIsHardWare && !IsGetValByKey) {
			uint bikeTaBanNum = TaBanValCur;
			//player click jiaoTaBanBt
			if(!IsFanZhuangTaBan)
			{
				if(bikeTaBanNum > TanBanCenterNum)
				{
					bPlayerHitTaBan_P1 = true;
					if(TaBanValMax < bikeTaBanNum)
					{
						TaBanValMax = bikeTaBanNum;
						handleJsonObj.WriteToFileXml(fileName, "TaBanValMax", TaBanValMax.ToString());
					}
					TanBanDownCount_P1 = ((float)(bikeTaBanNum - TanBanCenterNum) / (TaBanValMax - TanBanCenterNum)) * PcvrTanBanValTmp;
				}
				else if(bikeTaBanNum < TanBanCenterNum)
				{
					SubTaBanCountInfo();
					bPlayerHitTaBan_P1 = false;
				}
				else
				{
					SubTaBanCountInfo();
				}
			}
			else
			{
				if(bikeTaBanNum < TanBanCenterNum)
				{
					bPlayerHitTaBan_P1 = true;
					if(TaBanValMax > bikeTaBanNum)
					{
						TaBanValMax = bikeTaBanNum;
						handleJsonObj.WriteToFileXml(fileName, "TaBanValMax", TaBanValMax.ToString());
					}
					TanBanDownCount_P1 = ((float)(TanBanCenterNum - bikeTaBanNum) / (TanBanCenterNum - TaBanValMax)) * PcvrTanBanValTmp;
				}
				else if(bikeTaBanNum > TanBanCenterNum)
				{
					SubTaBanCountInfo();
					bPlayerHitTaBan_P1 = false;
				}
				else
				{
					SubTaBanCountInfo();
				}
			}
		}
		else {
			taBanCur = Input.GetAxis("Vertical") + 1f;
			if ( (TaBanValMin < TaBanValMax && taBanCur >= TaBanValMin)
			    || (TaBanValMin > TaBanValMax && taBanCur <= TaBanValMin) ) {
				//tmpVal = Mathf.Abs((float)taBanCur - TaBanValMin) / TaBanDisVal;
				tmpVal = Mathf.Abs(taBanCur - TaBanValMin) / TaBanDisVal;
				//TanBanDownCount_P1 = (tmpVal - 0.5f) * 2f;
				TanBanDownCount_P1 = tmpVal;
			}
		}
		return TanBanDownCount_P1;
	}
	
	#if USE_ZHUNXING_JZ_36
	public class CrossDt
	{
		public uint CrossPosXMax;
		public uint CrossPosXMin;
		public uint CrossPosYMax;
		public uint CrossPosYMin;
		public uint CrossPosDisX;
		public uint CrossPosDisY;
	}
	static List<CrossDt> CrossDtList;
	#endif

	void InitCrossPosInfo()
	{
		#if USE_ZHUNXING_JZ_36
		if (CrossUnitVal == null) {
			CrossUnitVal = new CrossUnitDt();
		}

		if (CrossDtList == null) {
			CrossDtList = new List<CrossDt>();
		}
		CrossDt crossDtVal = null;

		string strVal = "";
		for (int i = 0; i < 36; i++) {
			crossDtVal = new CrossDt();
			strVal = handleJsonObj.ReadFromFileXml(fileName, "CrossPosXMax"+i);
			if(strVal == null || strVal == "")
			{
				strVal = "2";
				handleJsonObj.WriteToFileXml(fileName, "CrossPosXMax"+i, strVal);
			}
			CrossPosXMax = Convert.ToUInt32( strVal );
			
			strVal = handleJsonObj.ReadFromFileXml(fileName, "CrossPosXMin"+i);
			if(strVal == null || strVal == "")
			{
				strVal = "1";
				handleJsonObj.WriteToFileXml(fileName, "CrossPosXMin"+i, strVal);
			}
			CrossPosXMin = Convert.ToUInt32( strVal );
			
			strVal = handleJsonObj.ReadFromFileXml(fileName, "CrossPosYMax"+i);
			if(strVal == null || strVal == "")
			{
				strVal = "2";
				handleJsonObj.WriteToFileXml(fileName, "CrossPosYMax"+i, strVal);
			}
			CrossPosYMax = Convert.ToUInt32( strVal );
			
			strVal = handleJsonObj.ReadFromFileXml(fileName, "CrossPosYMin"+i);
			if(strVal == null || strVal == "")
			{
				strVal = "1";
				handleJsonObj.WriteToFileXml(fileName, "CrossPosYMin"+i, strVal);
			}
			CrossPosYMin = Convert.ToUInt32( strVal );

			CrossPosDisX = (uint)Mathf.Abs((float)CrossPosXMax - CrossPosXMin) + 1;
			CrossPosDisY = (uint)Mathf.Abs((float)CrossPosYMax - CrossPosYMin) + 1;

			crossDtVal.CrossPosXMax = CrossPosXMax;
			crossDtVal.CrossPosXMin = CrossPosXMin;
			crossDtVal.CrossPosYMax = CrossPosYMax;
			crossDtVal.CrossPosYMin = CrossPosYMin;
			crossDtVal.CrossPosDisX = CrossPosDisX;
			crossDtVal.CrossPosDisY = CrossPosDisY;
			CrossDtList.Add(crossDtVal);
			/*ScreenLog.Log("index " + i);
			ScreenLog.Log("CrossPosDisX " + CrossDtList[i].CrossPosDisX
			          + ", CrossPosDisY " + CrossDtList[i].CrossPosDisY);
			ScreenLog.Log("CrossPosXMax " + CrossDtList[i].CrossPosXMax
			          + ", CrossPosXMin " + CrossDtList[i].CrossPosXMin);
			ScreenLog.Log("CrossPosYMax " + CrossDtList[i].CrossPosYMax
			          + ", CrossPosYMin " + CrossDtList[i].CrossPosYMin);*/
		}

		#if TEST_SHUIQIANG_ZUOBIAO_PINGJUN
		float pingJunValMax = 0f;
		float pingJunValMin = 0f;
		float disVal = 0f;
		int j = 0;
		int maxVal = 6;
		int minVal = 0;
		for (int i = 0; i < 36; i++) {
			j++;
			if (j <= 6) {
				pingJunValMax = pingJunValMax + CrossDtList[i].CrossPosYMax;
				pingJunValMin = pingJunValMin + CrossDtList[i].CrossPosYMin;

				if (j == 6) {
					pingJunValMax /= 6f;
					pingJunValMin /= 6f;
					disVal = Mathf.Abs(pingJunValMax - pingJunValMin) + 1f;
					for (int k = minVal; k < maxVal; k++) {
						CrossDtList[k].CrossPosYMax = (uint)pingJunValMax;
						CrossDtList[k].CrossPosYMin = (uint)pingJunValMin;
						CrossDtList[k].CrossPosDisY = (uint)disVal;
					}
					minVal += 6;
					maxVal += 6;
					
					j = 0;
					pingJunValMax = 0f;
					pingJunValMin = 0f;
				}
			}
		}

		pingJunValMax = 0f;
		pingJunValMin = 0f;
		disVal = 0f;
		j = 0;
		maxVal = 6;
		minVal = 0;
		int tmpIndexValX = 0;
		for (int i = 0; i < 6; i++) {
			for (j = 0; j < 6; j++) {
				tmpIndexValX = (j * 6) + i;
				pingJunValMax = pingJunValMax + CrossDtList[tmpIndexValX].CrossPosXMax;
				pingJunValMin = pingJunValMin + CrossDtList[tmpIndexValX].CrossPosXMin;
					
				if (j == 5) {
					pingJunValMax /= 6f;
					pingJunValMin /= 6f;
					disVal = Mathf.Abs(pingJunValMax - pingJunValMin) + 1f;
					for (int k = minVal; k < maxVal; k++) {
						tmpIndexValX = (k * 6) + i;
						CrossDtList[tmpIndexValX].CrossPosXMax = (uint)pingJunValMax;
						CrossDtList[tmpIndexValX].CrossPosXMin = (uint)pingJunValMin;
						CrossDtList[tmpIndexValX].CrossPosDisX = (uint)disVal;
					}

					pingJunValMax = 0f;
					pingJunValMin = 0f;
				}
			}
		}

		for(int i = 0; i < 36; i++) {
			handleJsonObj.WriteToFileXml(fileName, "CrossPosXMax"+i, CrossDtList[i].CrossPosXMax.ToString());
			handleJsonObj.WriteToFileXml(fileName, "CrossPosXMin"+i, CrossDtList[i].CrossPosXMin.ToString());
			handleJsonObj.WriteToFileXml(fileName, "CrossPosYMax"+i, CrossDtList[i].CrossPosYMax.ToString());
			handleJsonObj.WriteToFileXml(fileName, "CrossPosYMin"+i, CrossDtList[i].CrossPosYMin.ToString());
		}
		#endif

		/*for(int i = 0; i < 36; i++) {
			ScreenLog.Log("--- index " + i + " ---");
			ScreenLog.Log("CrossPosDisX " + CrossDtList[i].CrossPosDisX
			          + ", CrossPosDisY " + CrossDtList[i].CrossPosDisY);
			ScreenLog.Log("CrossPosXMax " + CrossDtList[i].CrossPosXMax
			          + ", CrossPosXMin " + CrossDtList[i].CrossPosXMin);
			ScreenLog.Log("CrossPosYMax " + CrossDtList[i].CrossPosYMax
			          + ", CrossPosYMin " + CrossDtList[i].CrossPosYMin);
		}*/
		#else
		string strValMax = handleJsonObj.ReadFromFileXml(fileName, "CrossPosXMax");
		if(strValMax == null || strValMax == "")
		{
			strValMax = "2";
			handleJsonObj.WriteToFileXml(fileName, "CrossPosXMax", strValMax);
		}
		CrossPosXMax = Convert.ToUInt32( strValMax );
		
		string strValMin = handleJsonObj.ReadFromFileXml(fileName, "CrossPosXMin");
		if(strValMin == null || strValMin == "")
		{
			strValMin = "1";
			handleJsonObj.WriteToFileXml(fileName, "CrossPosXMin", strValMax);
		}
		CrossPosXMin = Convert.ToUInt32( strValMin );
		
		strValMax = handleJsonObj.ReadFromFileXml(fileName, "CrossPosYMax");
		if(strValMax == null || strValMax == "")
		{
			strValMax = "2";
			handleJsonObj.WriteToFileXml(fileName, "CrossPosYMax", strValMax);
		}
		CrossPosYMax = Convert.ToUInt32( strValMax );
		
		strValMin = handleJsonObj.ReadFromFileXml(fileName, "CrossPosYMin");
		if(strValMin == null || strValMin == "")
		{
			strValMin = "1";
			handleJsonObj.WriteToFileXml(fileName, "CrossPosYMin", strValMax);
		}
		CrossPosYMin = Convert.ToUInt32( strValMin );
		
		CrossPosDisX = (uint)Mathf.Abs((float)CrossPosXMax - CrossPosXMin) + 1;
		CrossPosDisY = (uint)Mathf.Abs((float)CrossPosYMax - CrossPosYMin) + 1;
		/*ScreenLog.Log("CrossPosDisX " + CrossPosDisX + ", CrossPosDisY " + CrossPosDisY);
		ScreenLog.Log("CrossPosXMax " + CrossPosXMax + ", CrossPosXMin " + CrossPosXMin);
		ScreenLog.Log("CrossPosYMax " + CrossPosYMax + ", CrossPosYMin " + CrossPosYMin);*/
		#endif
	}
	
	#if USE_ZHUNXING_JZ_36
	public class CrossUnitDt
	{
		public int Point0 = 0;
		public int Point1 = 1;
		public int Point2 = 8;
		public int Point3 = 7;
	}
	/**
	 *  Point0 Point1
	 *  Point3 Point2
	 */
	static CrossUnitDt CrossUnitVal;

	public class CrossPosDt
	{
		public uint PosX;
		public uint PosY;
	}
	static List<CrossPosDt> CrossPosDtList;
	#endif
	public static PcvrJZCrossPoint JZPoint = PcvrJZCrossPoint.Num7;

	public static void SaveCrossPosInfo(AdjustGunDrossState val, int indexPoint = 0)
	{
		Screen.showCursor = !bIsHardWare;
		#if USE_ZHUNXING_JZ_36
		if (CrossPosDtList == null) {
			CrossPosDtList = new List<CrossPosDt>();
			if (JZPoint == PcvrJZCrossPoint.Num7) {
				for (int i = 0; i < (int)PcvrJZCrossPoint.Num49; i++) {
					CrossPosDt crossPosDtValTmp = new CrossPosDt();
					CrossPosDtList.Add(crossPosDtValTmp);
				}
			}
		}

		Vector3 crossPos = Input.mousePosition;
		if (bIsHardWare) {
			crossPos = MousePosition;
		}
		
		if (crossPos.x  < 0f) {
			crossPos.x = 0f;
		}
		
		if (crossPos.y  < 0f) {
			crossPos.y = 0f;
		}

		uint px = (uint)crossPos.x;
		uint py = (uint)crossPos.y;
		switch (JZPoint) {
		case PcvrJZCrossPoint.Num49:
			ScreenLog.Log("px " + px + ", py " + py + ", indexVal " + indexPoint);
			CrossPosDt crossPosDtVal = new CrossPosDt();
			crossPosDtVal.PosX = px;
			crossPosDtVal.PosY = py;
			CrossPosDtList.Add(crossPosDtVal);
			break;

		case PcvrJZCrossPoint.Num7:
			int indexTmp = indexPoint - 1;
			int hStart = indexTmp * 7;
			int hEnd = (indexTmp + 1) * 7;
			for (int i = hStart; i < hEnd; i++) {
//				Debug.Log("indexX "+i);
				CrossPosDtList[i].PosY = py;
			}

			int zVal = 0;
			for (int i = 0; i < 7; i++) {
				zVal = (i * 7) + indexTmp;
//				Debug.Log("indexY "+zVal);
				CrossPosDtList[zVal].PosX = px;
			}
			break;
		}

		if (indexPoint < (int)JZPoint) {
			return;
		}

//		if (JZPoint == PcvrJZCrossPoint.Num7) {
//			for (int i = 0; i < (int)PcvrJZCrossPoint.Num49; i++) {
//				ScreenLog.Log("px " + CrossPosDtList[i].PosX
//				              + ", py " + CrossPosDtList[i].PosY + ", indexVal " + i);
//			}
//		}

		uint pxCenterMin = 0;
		uint pyCenterMin = 0;
		uint pxCenterMax = 0;
		uint pyCenterMax = 0;
		int indexP0 = 0;
		int indexP1 = 0;
		int indexP2 = 0;
		int indexP3 = 0;
		int tmpIndexVal = 0;
		//共有7*7=49个点,6*6=36个单元格.
		for (int i = 0; i < 36; i++) {
			tmpIndexVal = (i % 6) + (i / 6) * 7;
			indexP0 = CrossUnitVal.Point0 + tmpIndexVal;
			indexP1 = CrossUnitVal.Point1 + tmpIndexVal;
			indexP2 = CrossUnitVal.Point2 + tmpIndexVal;
			indexP3 = CrossUnitVal.Point3 + tmpIndexVal;

			CrossPx_1 = CrossPosDtList[indexP0].PosX;
			CrossPy_1 = CrossPosDtList[indexP0].PosY;
			CrossPx_2 = CrossPosDtList[indexP1].PosX;
			CrossPy_2 = CrossPosDtList[indexP1].PosY;
			CrossPx_3 = CrossPosDtList[indexP2].PosX;
			CrossPy_3 = CrossPosDtList[indexP2].PosY;
			CrossPx_4 = CrossPosDtList[indexP3].PosX;
			CrossPy_4 = CrossPosDtList[indexP3].PosY;

			pxCenterMin = (uint)(0.5f * (CrossPx_1 + CrossPx_4));
			pxCenterMax = (uint)(0.5f * (CrossPx_2 + CrossPx_3));
			pyCenterMin = (uint)(0.5f * (CrossPy_3 + CrossPy_4));
			pyCenterMax = (uint)(0.5f * (CrossPy_1 + CrossPy_2));

			if (pxCenterMin < pxCenterMax) {
				CrossPosXMin = CrossPx_1 >= CrossPx_4 ? CrossPx_4 : CrossPx_1;
				px = CrossPosXMin;
				handleJsonObj.WriteToFileXml(fileName, "CrossPosXMin"+i, px.ToString());
				
				CrossPosXMax = CrossPx_2 >= CrossPx_3 ? CrossPx_2 : CrossPx_3;
				px = CrossPosXMax;
				handleJsonObj.WriteToFileXml(fileName, "CrossPosXMax"+i, px.ToString());
			}
			else {
				CrossPosXMin = CrossPx_1 <= CrossPx_4 ? CrossPx_4 : CrossPx_1;
				px = CrossPosXMin;
				handleJsonObj.WriteToFileXml(fileName, "CrossPosXMin"+i, px.ToString());
				
				CrossPosXMax = CrossPx_2 <= CrossPx_3 ? CrossPx_2 : CrossPx_3;
				px = CrossPosXMax;
				handleJsonObj.WriteToFileXml(fileName, "CrossPosXMax"+i, px.ToString());
			}
			
			if (pyCenterMin < pyCenterMax) {
				CrossPosYMin = CrossPy_3 >= CrossPy_4 ? CrossPy_4 : CrossPy_3;
				py = CrossPosYMin;
				handleJsonObj.WriteToFileXml(fileName, "CrossPosYMin"+i, py.ToString());
				
				CrossPosYMax = CrossPy_1 >= CrossPy_2 ? CrossPy_1 : CrossPy_2;
				py = CrossPosYMax;
				handleJsonObj.WriteToFileXml(fileName, "CrossPosYMax"+i, py.ToString());
			}
			else {
				CrossPosYMin = CrossPy_3 <= CrossPy_4 ? CrossPy_4 : CrossPy_3;
				py = CrossPosYMin;
				handleJsonObj.WriteToFileXml(fileName, "CrossPosYMin"+i, py.ToString());
				
				CrossPosYMax = CrossPy_1 <= CrossPy_2 ? CrossPy_1 : CrossPy_2;
				py = CrossPosYMax;
				handleJsonObj.WriteToFileXml(fileName, "CrossPosYMax"+i, py.ToString());
			}
			
			CrossPosDisX = (uint)Mathf.Abs((float)CrossPosXMax - CrossPosXMin) + 1;
			CrossPosDisY = (uint)Mathf.Abs((float)CrossPosYMax - CrossPosYMin) + 1;

			CrossDtList[i].CrossPosXMax = CrossPosXMax;
			CrossDtList[i].CrossPosXMin = CrossPosXMin;
			CrossDtList[i].CrossPosYMax = CrossPosYMax;
			CrossDtList[i].CrossPosYMin = CrossPosYMin;
			CrossDtList[i].CrossPosDisX = CrossPosDisX;
			CrossDtList[i].CrossPosDisY = CrossPosDisY;
			/*ScreenLog.Log("*** indexVal "+i);
			ScreenLog.Log("CrossPosDisX " + CrossDtList[i].CrossPosDisX
			          + ", CrossPosDisY " + CrossDtList[i].CrossPosDisY
			          + ", CrossPosXMax " + CrossDtList[i].CrossPosXMax
			          + ", CrossPosXMin " + CrossDtList[i].CrossPosXMin
			          + ", CrossPosYMax " + CrossDtList[i].CrossPosYMax
			          + ", CrossPosYMin " + CrossDtList[i].CrossPosYMin);*/
		}

		#if TEST_SHUIQIANG_ZUOBIAO_PINGJUN
		float pingJunValMax = 0f;
		float pingJunValMin = 0f;
		float disVal = 0f;
		int j = 0;
		int maxVal = 6;
		int minVal = 0;
		for (int i = 0; i < 36; i++) {
			j++;
			if (j <= 6) {
				pingJunValMax = pingJunValMax + CrossDtList[i].CrossPosYMax;
				pingJunValMin = pingJunValMin + CrossDtList[i].CrossPosYMin;
				
				if (j == 6) {
					pingJunValMax /= 6f;
					pingJunValMin /= 6f;
					disVal = Mathf.Abs(pingJunValMax - pingJunValMin) + 1f;
					for (int k = minVal; k < maxVal; k++) {
						CrossDtList[k].CrossPosYMax = (uint)pingJunValMax;
						CrossDtList[k].CrossPosYMin = (uint)pingJunValMin;
						CrossDtList[k].CrossPosDisY = (uint)disVal;
					}
					minVal += 6;
					maxVal += 6;
					
					j = 0;
					pingJunValMax = 0f;
					pingJunValMin = 0f;
				}
			}
		}
		
		pingJunValMax = 0f;
		pingJunValMin = 0f;
		disVal = 0f;
		j = 0;
		maxVal = 6;
		minVal = 0;
		int tmpIndexValX = 0;
		for (int i = 0; i < 6; i++) {
			for (j = 0; j < 6; j++) {
				tmpIndexValX = (j * 6) + i;
				pingJunValMax = pingJunValMax + CrossDtList[tmpIndexValX].CrossPosXMax;
				pingJunValMin = pingJunValMin + CrossDtList[tmpIndexValX].CrossPosXMin;
				
				if (j == 5) {
					pingJunValMax /= 6f;
					pingJunValMin /= 6f;
					disVal = Mathf.Abs(pingJunValMax - pingJunValMin) + 1f;
					for (int k = minVal; k < maxVal; k++) {
						tmpIndexValX = (k * 6) + i;
						CrossDtList[tmpIndexValX].CrossPosXMax = (uint)pingJunValMax;
						CrossDtList[tmpIndexValX].CrossPosXMin = (uint)pingJunValMin;
						CrossDtList[tmpIndexValX].CrossPosDisX = (uint)disVal;
					}
					
					pingJunValMax = 0f;
					pingJunValMin = 0f;
				}
			}
		}
		
		for(int i = 0; i < 36; i++) {
			handleJsonObj.WriteToFileXml(fileName, "CrossPosXMax"+i, CrossDtList[i].CrossPosXMax.ToString());
			handleJsonObj.WriteToFileXml(fileName, "CrossPosXMin"+i, CrossDtList[i].CrossPosXMin.ToString());
			handleJsonObj.WriteToFileXml(fileName, "CrossPosYMax"+i, CrossDtList[i].CrossPosYMax.ToString());
			handleJsonObj.WriteToFileXml(fileName, "CrossPosYMin"+i, CrossDtList[i].CrossPosYMin.ToString());
		}
		#endif

		#else
		Vector3 crossPos = Input.mousePosition;
		if (bIsHardWare) {
			crossPos = MousePosition;
		}
		
		if (crossPos.x  < 0f) {
			crossPos.x = 0f;
		}
		
		if (crossPos.y  < 0f) {
			crossPos.y = 0f;
		}
		
		uint pxCenterMin = 0;
		uint pyCenterMin = 0;
		uint pxCenterMax = 0;
		uint pyCenterMax = 0;
		
		uint px = (uint)crossPos.x;
		uint py = (uint)crossPos.y;
		//ScreenLog.Log("px " + px + ", py " + py);
		
		switch (val) {
		case AdjustGunDrossState.GunCrossLU:
			CrossPx_1 = px;
			CrossPy_1 = py;
			break;
			
		case AdjustGunDrossState.GunCrossRU:
			CrossPx_2 = px;
			CrossPy_2 = py;
			break;
			
		case AdjustGunDrossState.GunCrossRD:
			CrossPx_3 = px;
			CrossPy_3 = py;
			break;
			
		case AdjustGunDrossState.GunCrossLD:
			CrossPx_4 = px;
			CrossPy_4 = py;
			
			pxCenterMin = (uint)(0.5f * (CrossPx_1 + CrossPx_4));
			pxCenterMax = (uint)(0.5f * (CrossPx_2 + CrossPx_3));
			pyCenterMin = (uint)(0.5f * (CrossPy_3 + CrossPy_4));
			pyCenterMax = (uint)(0.5f * (CrossPy_1 + CrossPy_2));
			
			if (pxCenterMin < pxCenterMax) {
				CrossPosXMin = CrossPx_1 >= CrossPx_4 ? CrossPx_4 : CrossPx_1;
				px = CrossPosXMin;
				handleJsonObj.WriteToFileXml(fileName, "CrossPosXMin", px.ToString());
				
				CrossPosXMax = CrossPx_2 >= CrossPx_3 ? CrossPx_2 : CrossPx_3;
				px = CrossPosXMax;
				handleJsonObj.WriteToFileXml(fileName, "CrossPosXMax", px.ToString());
			}
			else {
				CrossPosXMin = CrossPx_1 <= CrossPx_4 ? CrossPx_4 : CrossPx_1;
				px = CrossPosXMin;
				handleJsonObj.WriteToFileXml(fileName, "CrossPosXMin", px.ToString());
				
				CrossPosXMax = CrossPx_2 <= CrossPx_3 ? CrossPx_2 : CrossPx_3;
				px = CrossPosXMax;
				handleJsonObj.WriteToFileXml(fileName, "CrossPosXMax", px.ToString());
			}
			
			if (pyCenterMin < pyCenterMax) {
				CrossPosYMin = CrossPy_3 >= CrossPy_4 ? CrossPy_4 : CrossPy_3;
				py = CrossPosYMin;
				handleJsonObj.WriteToFileXml(fileName, "CrossPosYMin", py.ToString());
				
				CrossPosYMax = CrossPy_1 >= CrossPy_2 ? CrossPy_1 : CrossPy_2;
				py = CrossPosYMax;
				handleJsonObj.WriteToFileXml(fileName, "CrossPosYMax", py.ToString());
			}
			else {
				CrossPosYMin = CrossPy_3 <= CrossPy_4 ? CrossPy_4 : CrossPy_3;
				py = CrossPosYMin;
				handleJsonObj.WriteToFileXml(fileName, "CrossPosYMin", py.ToString());
				
				CrossPosYMax = CrossPy_1 <= CrossPy_2 ? CrossPy_1 : CrossPy_2;
				py = CrossPosYMax;
				handleJsonObj.WriteToFileXml(fileName, "CrossPosYMax", py.ToString());
			}
			break;

		case AdjustGunDrossState.GunCrossCen:
			Debug.Log("save pcvr GunCross Info...");
			XKShuiQiangCrossCtrl.GXCen = (int)px;
			XKShuiQiangCrossCtrl.GYCen = (int)py;
			XKShuiQiangCrossCtrl.GXMax = (int)CrossPosXMax;
			XKShuiQiangCrossCtrl.GYMax = (int)CrossPosYMax;
			XKShuiQiangCrossCtrl.GXMin = (int)CrossPosXMin;
			XKShuiQiangCrossCtrl.GYMin = (int)CrossPosYMin;
			XKShuiQiangCrossCtrl.GetInstance().SavePlayerGunInfo();
			break;
		}
		
		CrossPosDisX = (uint)Mathf.Abs((float)CrossPosXMax - CrossPosXMin) + 1;
		CrossPosDisY = (uint)Mathf.Abs((float)CrossPosYMax - CrossPosYMin) + 1;
		
		/*ScreenLog.Log("CrossPosDisX " + CrossPosDisX
		          + ", CrossPosDisY " + CrossPosDisY
		          + ", CrossPosXMax " + CrossPosXMax
		          + ", CrossPosXMin " + CrossPosXMin
		          + ", CrossPosYMax " + CrossPosYMax
		          + ", CrossPosYMin " + CrossPosYMin);*/
		#endif
	}

	#if TEST_INPUT_CROSS_POS
	void OnGUI()
	{
		string strA = "posInput "+Input.mousePosition.ToString("f2")
			+", pcvrCrossPos "+MousePosition.ToString("f2");
		GUI.Label(new Rect(10f, 5f, Screen.width, Screen.height), strA);
	}
	#endif

	public static void CheckCrossPosition()
	{
		if (SetPanelUiRoot.IsJiaoZhunCross) {
			return;
		}

		if (CrossPosDisX <= 2 || CrossPosDisY <= 2) {
			return;
		}
		
		Vector3 pos = Vector3.zero;
		Vector3 mousePosCur = Input.mousePosition;
		if (bIsHardWare) {
			mousePosCur = MousePosition;
		}

		#if USE_ZHUNXING_JZ_36
		int indexVal = 0;
		int i = 0;
		for (i = 0; i < 6; i++) {
			if (CrossDtList[i].CrossPosXMin < CrossDtList[i].CrossPosXMax) {
				if (CrossDtList[i].CrossPosXMin <= mousePosCur.x
				    && mousePosCur.x <= CrossDtList[i].CrossPosXMax) {
					ShuiQiangX = i;
					break;
				}
			}
			else {
				if (CrossDtList[i].CrossPosXMin >= mousePosCur.x
				    && mousePosCur.x >= CrossDtList[i].CrossPosXMax) {
					ShuiQiangX = i;
					break;
				}
			}
		}

		int shuiQiangYTmp = 0;
		for (shuiQiangYTmp = 0; shuiQiangYTmp < 6; shuiQiangYTmp++) {
			i = shuiQiangYTmp * 6;
			if (CrossDtList[i].CrossPosYMin < CrossDtList[i].CrossPosYMax) {
				if (shuiQiangYTmp == 0 && CrossDtList[i].CrossPosYMax <= mousePosCur.y) {
					break;
				}

				if (shuiQiangYTmp == 5 && CrossDtList[i].CrossPosYMin >= mousePosCur.y) {
					break;
				}

				if (CrossDtList[i].CrossPosYMin <= mousePosCur.y
				    && mousePosCur.y <= CrossDtList[i].CrossPosYMax) {
					break;
				}
			}
			else {
				if (shuiQiangYTmp == 0 && CrossDtList[i].CrossPosYMax >= mousePosCur.y) {
					break;
				}
				
				if (shuiQiangYTmp == 5 && CrossDtList[i].CrossPosYMin <= mousePosCur.y) {
					break;
				}

				if (CrossDtList[i].CrossPosYMin >= mousePosCur.y
				    && mousePosCur.y >= CrossDtList[i].CrossPosYMax) {
					break;
				}
			}
		}

		indexVal = (int)((shuiQiangYTmp * 6) + ShuiQiangX);
//		if (Time.frameCount % 200 == 0) {
//			ScreenLog.Log("ShuiQiangX "+ ShuiQiangX +", ShuiQiangY "+shuiQiangYTmp
//			          +", indexVal "+indexVal);
//		}
		//ShuiQiangX是正的, ShuiQiangY是反的.
		//ShuiQiangX -> (0 - 0) (1 - 1) (2 - 2) (3 - 3) (4 - 4) (5 - 5)
		//ShuiQiangY -> (0 - 5) (1 - 4) (2 - 3) (3 - 2) (4 - 1) (5 - 0)
		ShuiQiangY = 5 - shuiQiangYTmp;

		CrossPosXMax = CrossDtList[indexVal].CrossPosXMax;
		CrossPosXMin = CrossDtList[indexVal].CrossPosXMin;
		CrossPosYMax = CrossDtList[indexVal].CrossPosYMax;
		CrossPosYMin = CrossDtList[indexVal].CrossPosYMin;
		CrossPosDisX = CrossDtList[indexVal].CrossPosDisX;
		CrossPosDisY = CrossDtList[indexVal].CrossPosDisY;

		if (CrossPosXMin < CrossPosXMax) {
			mousePosCur.x = mousePosCur.x < CrossPosXMin ? CrossPosXMin : mousePosCur.x;
			mousePosCur.x = mousePosCur.x < CrossPosXMax ? mousePosCur.x : CrossPosXMax;
		}
		else {
			mousePosCur.x = mousePosCur.x > CrossPosXMin ? CrossPosXMin : mousePosCur.x;
			mousePosCur.x = mousePosCur.x > CrossPosXMax ? mousePosCur.x : CrossPosXMax;
		}
		
		if (CrossPosYMin < CrossPosYMax) {
			mousePosCur.y = mousePosCur.y < CrossPosYMin ? CrossPosYMin : mousePosCur.y;
			mousePosCur.y = mousePosCur.y < CrossPosYMax ? mousePosCur.y : CrossPosYMax;
		}
		else {
			mousePosCur.y = mousePosCur.y > CrossPosYMin ? CrossPosYMin : mousePosCur.y;
			mousePosCur.y = mousePosCur.y > CrossPosYMax ? mousePosCur.y : CrossPosYMax;
		}

		pos.x = (int)(Mathf.Abs(mousePosCur.x - CrossPosXMin + 1) * ScreenW) / CrossPosDisX;
		pos.y = (int)(Mathf.Abs(mousePosCur.y - CrossPosYMin + 1) * ScreenH) / CrossPosDisY;
		pos.x += ShuiQiangX * ScreenW;
		pos.y += ShuiQiangY * ScreenH;
		#endif

		#if USE_LINE_HIT_CROSS
		if (!pcvr.bIsHardWare) {
			if (CrossPosXMin < CrossPosXMax) {
				mousePosCur.x = mousePosCur.x < CrossPosXMin ? CrossPosXMin : mousePosCur.x;
				mousePosCur.x = mousePosCur.x < CrossPosXMax ? mousePosCur.x : CrossPosXMax;
			}
			else {
				mousePosCur.x = mousePosCur.x > CrossPosXMin ? CrossPosXMin : mousePosCur.x;
				mousePosCur.x = mousePosCur.x > CrossPosXMax ? mousePosCur.x : CrossPosXMax;
			}
			
			if (CrossPosYMin < CrossPosYMax) {
				mousePosCur.y = mousePosCur.y < CrossPosYMin ? CrossPosYMin : mousePosCur.y;
				mousePosCur.y = mousePosCur.y < CrossPosYMax ? mousePosCur.y : CrossPosYMax;
			}
			else {
				mousePosCur.y = mousePosCur.y > CrossPosYMin ? CrossPosYMin : mousePosCur.y;
				mousePosCur.y = mousePosCur.y > CrossPosYMax ? mousePosCur.y : CrossPosYMax;
			}
			
			pos.x = (int)(Mathf.Abs(mousePosCur.x - CrossPosXMin + 1) * 1360f) / CrossPosDisX;
			pos.y = (int)(Mathf.Abs(mousePosCur.y - CrossPosYMin + 1) * 768f) / CrossPosDisY;
		}
		else {
			pos = XKShuiQiangCrossCtrl.CrossPos;
		}
		#endif

		#if !USE_LINE_HIT_CROSS && !USE_ZHUNXING_JZ_36
		if (CrossPosXMin < CrossPosXMax) {
			mousePosCur.x = mousePosCur.x < CrossPosXMin ? CrossPosXMin : mousePosCur.x;
			mousePosCur.x = mousePosCur.x < CrossPosXMax ? mousePosCur.x : CrossPosXMax;
		}
		else {
			mousePosCur.x = mousePosCur.x > CrossPosXMin ? CrossPosXMin : mousePosCur.x;
			mousePosCur.x = mousePosCur.x > CrossPosXMax ? mousePosCur.x : CrossPosXMax;
		}
		
		if (CrossPosYMin < CrossPosYMax) {
			mousePosCur.y = mousePosCur.y < CrossPosYMin ? CrossPosYMin : mousePosCur.y;
			mousePosCur.y = mousePosCur.y < CrossPosYMax ? mousePosCur.y : CrossPosYMax;
		}
		else {
			mousePosCur.y = mousePosCur.y > CrossPosYMin ? CrossPosYMin : mousePosCur.y;
			mousePosCur.y = mousePosCur.y > CrossPosYMax ? mousePosCur.y : CrossPosYMax;
		}

		pos.x = (int)(Mathf.Abs(mousePosCur.x - CrossPosXMin + 1) * 1360f) / CrossPosDisX;
		pos.y = (int)(Mathf.Abs(mousePosCur.y - CrossPosYMin + 1) * 768f) / CrossPosDisY;
		#endif

		CrossPosition = pos;
		Instance.OnUpdateCross();
	}

	#region Click Button Envent
	public delegate void EventHandel();
	public event EventHandel OnUpdateCrossEvent;
	public void OnUpdateCross()
	{
		if (OnUpdateCrossEvent != null) {
			OnUpdateCrossEvent();
		}
	}
	#endregion

	/**
	 * ShuiQiangX -> 水枪准星在水平方向的区域.
	 */
	static float ShuiQiangX = 0f;
	/**
	 * ShuiQiangYouY -> 水枪准星在竖直方向的区域.
	 */
	static float ShuiQiangY = 0f;
	public static float ScreenW = 1360f / 6f;
	public static float ScreenH = 768f / 6f;

	static void RandomJiaoYanDt()
	{	
		for (int i = 1; i < 4; i++) {
			JiaoYanDt[i] = (byte)UnityEngine.Random.Range(0x00, 0x7b);
		}
		JiaoYanDt[0] = 0x00;
		for (int i = 1; i < 4; i++) {
			JiaoYanDt[0] ^= JiaoYanDt[i];
		}
	}

	public void StartJiaoYanIO()
	{
		if (!bIsHardWare) {
			return;
		}

		if (IsJiaoYanHid) {
			return;
		}
		
		if (!HardwareCheckCtrl.IsTestHardWare) {
			if (JiaoYanSucceedCount >= JiaoYanFailedMax) {
				return;
			}
			
			if (JiaoYanState == JIAOYANENUM.FAILED && JiaoYanFailedCount >= JiaoYanFailedMax) {
				return;
			}
		}
		else {
			HardwareCheckCtrl.Instance.SetJiaMiJYMsg("校验中...", JiaMiJiaoYanEnum.Null);
		}
		RandomJiaoYanDt();
		JiaoYanCheckCount = 0;
		IsJiaoYanHid = true;
		//ScreenLog.Log("开始校验...");
	}

	float TimeJiaoYanIO;
	void CheckIsCloseJiaoYanIO()
	{
		if (!IsJiaoYanHid) {
			return;
		}
		TimeJiaoYanIO += Time.deltaTime;

		if (TimeJiaoYanIO >= 3f) {
			TimeJiaoYanIO = 0f;
			CloseJiaoYanIO();
		}
	}
	
	void CloseJiaoYanIO()
	{
		if (!IsJiaoYanHid) {
			return;
		}
		IsJiaoYanHid = false;
		OnEndJiaoYanIO(JIAOYANENUM.FAILED);
	}

	void OnEndJiaoYanIO(JIAOYANENUM val)
	{
		IsJiaoYanHid = false;
		switch (val) {
		case JIAOYANENUM.FAILED:
			JiaoYanFailedCount++;
			if (HardwareCheckCtrl.IsTestHardWare) {
				HardwareCheckCtrl.Instance.JiaMiJiaoYanFailed();
			}
			break;
			
		case JIAOYANENUM.SUCCEED:
			JiaoYanSucceedCount++;
			JiaoYanFailedCount = 0;
			if (HardwareCheckCtrl.IsTestHardWare) {
				HardwareCheckCtrl.Instance.JiaMiJiaoYanSucceed();
			}
			break;
		}
		JiaoYanState = val;
		//ScreenLog.Log("*****JiaoYanState "+JiaoYanState);
		
		if (JiaoYanFailedCount >= JiaoYanFailedMax || IsJiOuJiaoYanFailed) {
			//JiaoYanFailed
			if (IsJiOuJiaoYanFailed) {
				//JiOuJiaoYanFailed
				//ScreenLog.Log("JOJYSB...");
			}
			else {
				//JiaMiXinPianJiaoYanFailed
				//ScreenLog.Log("JMXPJYSB...");
				IsJiaMiJiaoYanFailed = true;
			}
		}
	}
	public static bool IsJiaMiJiaoYanFailed;
	
	enum JIAOYANENUM
	{
		NULL,
		SUCCEED,
		FAILED,
	}
	static int JiaoYanCheckCount;
	static JIAOYANENUM JiaoYanState = JIAOYANENUM.NULL;
	static byte JiaoYanFailedMax = 0x01;
	static byte JiaoYanSucceedCount;
	static byte JiaoYanFailedCount;
	static byte[] JiaoYanDt = new byte[4];
	static byte[] JiaoYanMiMa = new byte[4];
	static byte[] JiaoYanMiMaRand = new byte[4];
	byte JiOuJiaoYanCount;
	byte JiOuJiaoYanMax = 5;
	public static bool IsJiOuJiaoYanFailed;

	void InitJiaoYanMiMa()
	{
		JiaoYanMiMa[1] = 0x8e; //0x8e
		JiaoYanMiMa[2] = 0xc3; //0xc3
		JiaoYanMiMa[3] = 0xd7; //0xd7
		JiaoYanMiMa[0] = 0x00;
		for (int i = 1; i < 4; i++) {
			JiaoYanMiMa[0] ^= JiaoYanMiMa[i];
		}
	}
	
	void RandomJiaoYanMiMaVal()
	{
		for (int i = 0; i < 4; i++) {
			JiaoYanMiMaRand[i] = (byte)UnityEngine.Random.Range(0x00, (JiaoYanMiMa[i] - 1));
		}
		
		byte TmpVal = 0x00;
		for (int i = 1; i < 4; i++) {
			TmpVal ^= JiaoYanMiMaRand[i];
		}
		
		if (TmpVal == JiaoYanMiMaRand[0]) {
			JiaoYanMiMaRand[0] = JiaoYanMiMaRand[0] == 0x00 ?
				(byte)UnityEngine.Random.Range(0x01, 0xff) : (byte)(JiaoYanMiMaRand[0] + UnityEngine.Random.Range(0x01, 0xff));
		}
	}
}

//#if USE_ZHUNXING_JZ_36
public enum PcvrJZCrossPoint
{
	Num4 = 4,
	Num7 = 7,
	Num49 = 49,
}
//#endif

public enum PcvrValState
{
	ValMax,
	ValMin,
	ValCenter
}

public enum PcvrShuiBengState
{
	Level_1,
	Level_2
}

public enum LedState
{
	Liang,
	Shan,
	Mie
}