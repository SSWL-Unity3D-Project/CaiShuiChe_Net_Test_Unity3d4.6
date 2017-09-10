using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Frederick.ProjectAircraft;
public class Toubi : MonoBehaviour {

	public GameObject LinkPlayerObj;
	public GameObject BackgroudObj;
	public GameObject ActiveJiaShi;
	public GameObject ModeObj;
	public GameObject StartBtObj;
	public GameObject InsertCoinObj;
	public GameObject CenterObj;
	public GameObject MovieObj;

	public GameObject G_yi_shiwei;
	public GameObject G_yi_gewei;
	public GameObject G_Gang_Obj;
	public GameObject G_xu_shiwei;
	public GameObject G_xu_gewei;
	
	public GameObject LinkObj;
	public GameObject DanJiObj;

	public static AudioClip audioTouBiStatic;
	private UISprite yi_shiwei;
	private UISprite yi_gewei;
	private UISprite xu_shiwei;
	private UISprite xu_gewei;
	//private UIAtlas atlas;

	TweenScale movieTScale;
	UISprite backgroudSprite;
	TweenScale backgroudScl;
	TweenColor backgroudColor;

	public static Toubi _Instance;
	public static Toubi GetInstance()
	{
		return _Instance;
	}

	void Start()
	{
		_Instance = this;

		LinkPlayerObj.SetActive(false);
		backgroudSprite = BackgroudObj.GetComponent<UISprite>();
		backgroudScl = BackgroudObj.GetComponent<TweenScale>();
		backgroudColor = BackgroudObj.GetComponent<TweenColor>();
		backgroudScl.enabled = false;
		backgroudColor.enabled = false;

		ModeObj.transform.localPosition = new Vector3(-1258f, 0f, 0f);
		ModeObj.SetActive(false);

		StartBtObj.SetActive(false);
		ActiveJiaShi.SetActive(false);
		CenterObj.SetActive(false);

		LinkTScl_0 = LinkObj.GetComponents<TweenScale>()[0];
		LinkTScl_1 = LinkObj.GetComponents<TweenScale>()[1];
		DanJiTScl_0 = DanJiObj.GetComponents<TweenScale>()[0];
		DanJiTScl_1 = DanJiObj.GetComponents<TweenScale>()[1];
		
		LinkSprite = LinkObj.GetComponent<UISprite>();
		DanJiSprite = DanJiObj.GetComponent<UISprite>();

		MovieObj.SetActive(true);
		InitSprite();

		//GlobalData.GetInstance().IcoinCountChange += IcoinCountChange;
		ConvertNumToImg("xu", GlobalData.GetInstance().XUTOUBI);
		ConvertNumToImg("yi", GlobalData.GetInstance().Icoin);
		audioTouBiStatic = AudioListCtrl.GetInstance().AudioTouBi;

		ShowInsertCoinImg();

		//Toubi.PlayerPushCoin( 10 ); //test
		InputEventCtrl.GetInstance().ClickStartBtOneEvent += clickStartBtOneEvent;
		InputEventCtrl.GetInstance().ClickStartBtTwoEvent += clickStartBtTwoEvent;

		SetPanelCtrl.GetInstance();
		FinishPanelCtrl.IsCanLoadSetPanel = true; //reset IsCanLoadSetPanel
	}

	TweenPosition ModeTPos;
	void InitHiddenMode()
	{
		if(ModeTPos != null)
		{
			return;
		}
		
		isSelectMode = true;
		if(StartBtObj.activeSelf)
		{
			StartBtObj.SetActive(false);
			AudioManager.Instance.PlaySFX(AudioListCtrl.GetInstance().AudioStartBt);
		}
		
		ModeTPos = ModeObj.GetComponent<TweenPosition>();
		ModeTPos.from = ModeObj.transform.localPosition;
		ModeTPos.to = new Vector3(1440f, 0f, 0f);
		ModeTPos.ResetToBeginning();
		
		backgroudSprite.spriteName = GlobalData.GetInstance().gameMode != GameMode.SoloMode ? "Link" : "QuWei";
		if(GlobalData.GetInstance().gameMode == GameMode.SoloMode)
		{
			HiddenCoinInfo();
			backgroudScl.enabled = true;
			backgroudColor.enabled = true;
			
			LoadImgObj.SetActive(true);
			EventDelegate.Add(backgroudColor.onFinished, delegate{
				Invoke("StartIntoGame", 0.2f);
			});
		}
		else
		{
			LinkPlayerObj.SetActive(true);
			Invoke("ShowPlayerWait", 0.5f);
		}
		ModeTPos.PlayForward();
	}

	public bool GetIsSelectMode()
	{
		return isSelectMode;
	}

	UISprite waitSprite;
	TweenScale LinkScl;
	void ShowPlayerWait()
	{
		if(waitSprite != null)
		{
			return;
		}
		//waitSprite = LinkPlayerObj.GetComponent<UISprite>();
		waitSprite = LinkPlayerCtrl.GetInstance().GetWaitPlayerSprite();
		waitSprite.enabled = true;

		CancelInvoke("DelayShowStartBt");
		Invoke("DelayShowStartBt", 5f);

		InvokeRepeating("LoopWait", 0f, 0.5f);
	}

	void DelayShowStartBt()
	{
		if (RequestMasterServer.GetInstance().GetMovieMasterServerNum() != 1) {
			//Debug.Log("DelayShowStartBt -> Hidden startBt...");
			StartBtObj.SetActive(false);
			Invoke("DelayShowStartBt", 1f);
			return;
		}

		if (!Network.isServer) {
			return;
		}

		if (!StartBtObj.activeSelf){
			StartBtObj.SetActive(true);
		}
		Invoke("DelayShowStartBt", 0.03f);
	}


	public void MakeGameIntoWaterwheelNet()
	{
		StopShowWait(1);
	}

	void StopShowWait(int key)
	{
		if(!IsInvoking("LoopWait")) {
			return;
		}

		if (RequestMasterServer.GetInstance().GetMovieMasterServerNum() != 1 && key == 0) {
			Debug.Log("Cannot stop show wait, masterServerNum is wrong!");
			return;
		}
		CancelInvoke("DelayShowStartBt");
		CancelInvoke("LoopWait");

		AudioManager.Instance.PlaySFX(AudioListCtrl.GetInstance().AudioStartBt);
		waitSprite.enabled = false;
		StartBtObj.SetActive(false);
		//BackgroudObj.SetActive(false);

		if (key == 0) {
			if (Network.peerType == NetworkPeerType.Server || Network.peerType == NetworkPeerType.Client) {
				if (NetworkRpcMsgCtrl.GetInstance() != null) {
					NetworkRpcMsgCtrl.GetInstance().SendLoadLevel( (int)GameLeve.WaterwheelNet );
				}
			}
		}
		HiddenLinkPlayer();
	}

	void HiddenLinkPlayer()
	{
		if(!LinkPlayerObj.activeSelf)
		{
			return;
		}
		LinkPlayerObj.SetActive(false);

		HiddenCoinInfo();
		backgroudScl.enabled = true;
		backgroudColor.enabled = true;

		LoadImgObj.SetActive(true);
		EventDelegate.Add(backgroudColor.onFinished, delegate{
			Invoke("StartIntoGame", 0.2f);
		});
	}

	public bool CheckIsLoopWait()
	{
		if (IsInvoking("LoopWait")) {
			return true;
		}
		return false;
	}

	void LoopWait()
	{
		if (!waitSprite.enabled) {
			CancelInvoke("LoopWait");
			return;
		}
		/*switch(waitSprite.spriteName)
		{
		case "wait_0":
			waitSprite.spriteName = "wait_1";
			break;

		case "wait_1":
			waitSprite.spriteName = "wait_0";
			break;
		}*/
	}


	void HiddenCoinInfo()
	{
		yi_shiwei.enabled = false;
		yi_gewei.enabled = false;
		xu_shiwei.enabled = false;
		xu_gewei.enabled = false;
		G_Gang_Obj.SetActive(false);
	}

	public GameObject LoadImgObj;
	public GameObject LoadingBJObj;
	AsyncOperation AsyncStatus;

	public void StartIntoGame()
	{
		MyCOMDevice.ComThreadClass.IsLoadingLevel = true;
		LoadingCtrl.GetInstance().InitActiveAllObj();
		LoadingBJObj.SetActive(true);
		if(GlobalData.GetInstance().gameMode == GameMode.SoloMode)
		{
			IsIntoPlayGame = true;
			GlobalData.GetInstance().gameLeve = GameLeve.Waterwheel;
			System.GC.Collect();
			AsyncStatus = Application.LoadLevelAsync((int)GameLeve.Waterwheel);
		}
		else
		{
			GlobalData.GetInstance().gameLeve = GameLeve.WaterwheelNet;
			if (Network.peerType == NetworkPeerType.Server) {
				NetworkServerNet.GetInstance().RemoveMasterServerHost();
				if (NetworkRpcMsgCtrl.MaxLinkServerCount == 0) {
					NetworkRpcMsgCtrl.MaxLinkServerCount = NetworkRpcMsgCtrl.NoLinkClientCount;
				}
			}
			System.GC.Collect();
			AsyncStatus = Application.LoadLevelAsync((int)GameLeve.WaterwheelNet);
		}
		AsyncStatus.allowSceneActivation = false;

		Invoke("CheckAsyncIsDone", 8f);
	}

	public bool IsIntoPlayGame;
	void CheckAsyncIsDone()
	{
		if (!IsIntoPlayGame) {
			Invoke("CheckAsyncIsDone", 1f);
			return;
		}

		AsyncStatus.allowSceneActivation = true;
	}

	UISprite InsertCoinSprite;
	public void ShowInsertCoinImg()
	{
		if(InsertCoinSprite == null)
		{
			InsertCoinSprite = InsertCoinObj.GetComponent<UISprite>();
		}

		if(!GlobalData.GetInstance().IsFreeMode)
		{
			InsertCoinObj.transform.localPosition = new Vector3(0f, 58f, 0f);
			InsertCoinSprite.spriteName = "qingTouBi";
		}
		else
		{
			StartBtObj.SetActive(true);
			HiddenCoinInfo();

			InsertCoinObj.transform.localPosition = new Vector3(0f, 25f, 0f);
			InsertCoinSprite.spriteName = "mianFei";
		}
	}
	
	void clickStartBtOneEvent(ButtonState val)
	{
		//ScreenLog.Log("ChangeLeve::clickStartBtEvent -> val " + val);
		if(val == ButtonState.DOWN)
		{
			return;
		}
		
//		if(!bIsClickStartBt)
//		{
//			return;
//		}
		clickStartBtOne();
		
		//InputEventCtrl.GetInstance().ClickStartBtEvent -= clickStartBtEvent;
	}

	bool isActiveMode = false;
	bool isSelectMode = false;
	void clickStartBtOne()
	{
		if(!StartBtObj.activeSelf)
		{
			return;
		}

		if(!isActiveMode)
		{
			isActiveMode = true;
			HiddenActiveJiaShi();
			backgroudSprite.color = new Color(47f / 255f, 47f / 255f, 47f / 255f, 1f);

			AudioManager.Instance.PlaySFX(AudioListCtrl.GetInstance().AudioXuanZe);
			StartBtObj.SetActive(false);
            if (GameMovieCtrl.GetInstance().GameLinkSt == GameMovieCtrl.GameLinkEnum.LINK)
            {
                ModeObj.SetActive(true);
                SelecteCartoon.GetInstance().Invoke("InitSelecteCartoon", 0.5f);
            }

			if(!GlobalData.GetInstance().IsFreeMode)
			{
				//ScreenLog.Log("****************1111JianBi");
				if (!pcvr.bIsHardWare) {
					if (GlobalData.GetInstance().Icoin >= GlobalData.GetInstance().XUTOUBI) {
						GlobalData.GetInstance().Icoin -= GlobalData.GetInstance().XUTOUBI;
						ConvertNumToImg("yi", GlobalData.GetInstance().Icoin);
					}
				}
				else {
					subPlayerCoin();
				}
			}
			else
			{
				if(MovieObj.activeSelf)
				{
					movieTScale = MovieObj.GetComponent<TweenScale>();
					movieTScale.enabled = true;
					EventDelegate.Add(movieTScale.onFinished, delegate{
						Invoke("CloseMovie", 0f);
					});
				}
            }

            if (GameMovieCtrl.GetInstance().GameLinkSt == GameMovieCtrl.GameLinkEnum.NO_LINK)
            {
                if (!isSelectMode)
                {
                    GlobalData.GetInstance().gameMode = GameMode.SoloMode;
                    InitHiddenMode();
                }
            }
        }
		else if(!isSelectMode)
		{
			InitHiddenMode();
		}

		if(IsInvoking("LoopWait"))
		{
			StopShowWait(0);
		}
	}

	void clickStartBtTwoEvent(ButtonState val)
	{
		//ScreenLog.Log("ChangeLeve::clickStartBtEvent -> val " + val);
		if(val == ButtonState.DOWN)
		{
			return;
		}
		
		//		if(!bIsClickStartBt)
		//		{
		//			return;
		//		}
		clickStartBtTwo();
		
		//InputEventCtrl.GetInstance().ClickStartBtEvent -= clickStartBtEvent;
	}

	AudioSource AudioSourceJingGao;

	void PlayAudioJingGao()
	{
		if(AudioSourceJingGao != null
		   && AudioSourceJingGao.isPlaying
		   && AudioSourceJingGao.clip == AudioListCtrl.GetInstance().AudioMovieJingGao) {
			return;
		}
		AudioSourceJingGao = AudioManager.Instance.PlaySFX(AudioListCtrl.GetInstance().AudioMovieJingGao);
	}

	void clickStartBtTwo()
	{
		if(!StartBtObj.activeSelf)
		{
			return;
		}

		if(!isActiveMode)
		{
			PlayAudioJingGao();
			InitActiveJiaShi();
			return;
		}
	}

	void InitActiveJiaShi()
	{
		ActiveJiaShi.SetActive(true);
		if(IsInvoking("HiddenActiveJiaShi"))
		{
			CancelInvoke("HiddenActiveJiaShi");
		}
		Invoke("HiddenActiveJiaShi", 6.0f);
	}
	
	void HiddenActiveJiaShi()
	{
		if(!ActiveJiaShi.activeSelf)
		{
			return;
		}
		CancelInvoke("HiddenActiveJiaShi");
		ActiveJiaShi.SetActive(false);
	}

//	public void IcoinCountChange()
//	{
//		ConvertNumToImg("yi",GlobalData.GetInstance().Icoin);
//	}

	public void ConvertNumToImg(string mod,int num)
	{
		if(mod=="yi")
		{
			if(num>99)
			{
				yi_shiwei.spriteName="9";
				yi_gewei.spriteName="9";
			}
			else
			{
				int coinShiWei = (int)((float)num/10.0f);
				//ScreenLog.Log("********* coinShiWei " + coinShiWei);
				yi_shiwei.spriteName = coinShiWei.ToString();
				yi_gewei.spriteName = (num%10).ToString();
			}
		}
		else if(mod=="xu")
		{
			xu_shiwei.spriteName=(num/10).ToString();
			xu_gewei.spriteName=(num%10).ToString();
		}
	}

	public void InitSprite()
	{
		yi_shiwei = G_yi_shiwei.GetComponent<UISprite>() as UISprite;
		yi_gewei = G_yi_gewei.GetComponent<UISprite>() as UISprite;
		xu_gewei = G_xu_gewei.GetComponent<UISprite>() as UISprite;
		xu_shiwei = G_xu_shiwei.GetComponent<UISprite>() as UISprite;
	}

	public void subPlayerCoin()
	{
		if (GlobalData.GetInstance().Icoin >= GlobalData.GetInstance().XUTOUBI) {
			//ScreenLog.Log("***********subPlayerCoin");
			GlobalData.GetInstance().Icoin -= GlobalData.GetInstance().XUTOUBI;
			pcvr.GetInstance().SubPlayerCoin( GlobalData.GetInstance().XUTOUBI );
			ConvertNumToImg("yi", GlobalData.GetInstance().Icoin);
		}
	}

	float HorizontalVal;
	int needCoin = 0;
	// Update is called once per frame
	void Update ()
	{
		if (needCoin != GlobalData.GetInstance().XUTOUBI) {
			needCoin = GlobalData.GetInstance().XUTOUBI;
			ConvertNumToImg("xu", GlobalData.GetInstance().XUTOUBI);
		}

		if(pcvr.bIsHardWare)
		{
			if(GlobalData.GetInstance().Icoin != pcvr.GetInstance().CoinNumCurrent)
			{
				if(GlobalData.GetInstance().Icoin < pcvr.GetInstance().CoinNumCurrent)
				{
					AudioManager.Instance.PlaySFX(AudioListCtrl.GetInstance().AudioTouBi);
					GlobalData.GetInstance().Icoin = pcvr.GetInstance().CoinNumCurrent;
					ConvertNumToImg("yi", GlobalData.GetInstance().Icoin);
				}
			}
		}
		else
		{
			if( Input.GetKeyUp(KeyCode.T) )
			{
				AudioManager.Instance.PlaySFX(AudioListCtrl.GetInstance().AudioTouBi);
				GlobalData.GetInstance().Icoin++;
				ConvertNumToImg("yi", GlobalData.GetInstance().Icoin);
			}
		}
		HorizontalVal = InputEventCtrl.GetInstance().GetHorVal();
		
		if(!GlobalData.GetInstance().IsFreeMode)
		{
			if(GlobalData.GetInstance().Icoin >= GlobalData.GetInstance().XUTOUBI && MovieObj.activeSelf)
			{
				movieTScale = MovieObj.GetComponent<TweenScale>();
				movieTScale.enabled = true;
				EventDelegate.Add(movieTScale.onFinished, delegate{
					Invoke("CloseMovie", 0f);
				});
			}
		}

		if(ModeObj.activeSelf && !isSelectMode && Mathf.Abs(HorizontalVal) > 0.1f
		   && SelecteCartoon.GetInstance().CheckIsPlaySelect()) {
			InitPlayModeRot();
		}
	}

	TweenScale LinkTScl_0;
	TweenScale LinkTScl_1;
	TweenScale DanJiTScl_0;
	TweenScale DanJiTScl_1;

	UISprite LinkSprite;
	UISprite DanJiSprite;

	bool isInitModeRot = false;
	void InitPlayModeRot()
	{
		if(isInitModeRot)
		{
			return;
		}
		isInitModeRot = true;
		bool isPlayScale = false;
		SelecteCartoon.GetInstance().StopCartoon();
		if(!StartBtObj.activeSelf)
		{
			StartBtObj.SetActive(true);
		}

		AudioManager.Instance.PlaySFX(AudioListCtrl.GetInstance().AudioModeChange);
		if(HorizontalVal > 0.1f)
		{
			if(GlobalData.GetInstance().gameMode != GameMode.SoloMode)
			{
				isPlayScale = true;
			}
			GlobalData.GetInstance().gameMode = GameMode.SoloMode;
		}
		else if(HorizontalVal < -0.1f)
		{
			if(GlobalData.GetInstance().gameMode != GameMode.OnlineMode)
			{
				isPlayScale = true;
			}
			GlobalData.GetInstance().gameMode = GameMode.OnlineMode;
		}
		ChangeModeColor();

		switch(GlobalData.GetInstance().gameMode)
		{
		case GameMode.OnlineMode:
			LinkTScl_0.ResetToBeginning();
			LinkTScl_0.PlayForward();
			if(isPlayScale)
			{
				DanJiTScl_0.ResetToBeginning();
				DanJiTScl_0.PlayForward();
			}
			break;

		case GameMode.SoloMode:
			if(isPlayScale)
			{
				LinkTScl_1.ResetToBeginning();
				LinkTScl_1.PlayForward();
			}
			DanJiTScl_1.ResetToBeginning();
			DanJiTScl_1.PlayForward();
			break;
		}

		Invoke("ResetPlayModeRot", 1.0f);
	}

	void ResetPlayModeRot()
	{
		isInitModeRot = false;
	}

	void ChangeModeColor()
	{
		switch(GlobalData.GetInstance().gameMode)
		{
		case GameMode.OnlineMode:
			LinkSprite.spriteName = "LinkMode";
			DanJiSprite.spriteName = "QuWeiModeHui";
			break;
			
		case GameMode.SoloMode:
			LinkSprite.spriteName = "LinkModeHui";
			DanJiSprite.spriteName = "QuWeiMode";
			break;
		}
	}

	void CloseMovie()
	{
		GameMovieCtrl.GetInstance().stopPlayMovie();
		MovieObj.SetActive(false);
		InsertCoinObj.SetActive(false);
		
		if(!GlobalData.GetInstance().IsFreeMode)
		{
			StartBtObj.SetActive(true);
		}
		CenterObj.SetActive(true);
		AudioManager.Instance.PlayBGM(AudioListCtrl.GetInstance().AudioMovieBeiJing, true);
	}

	public static void PlayerPushCoin( int coin )
	{
		AudioManager.Instance.PlaySFX(audioTouBiStatic);
		GlobalData.GetInstance().Icoin = coin;
	}

	/*void OnGUI()
	{
		string str = "IsIntoPlayGame " + IsIntoPlayGame.ToString();
		GUI.Label(new Rect(0f, 400f, 1000f, 30f), str);
	}*/
}
