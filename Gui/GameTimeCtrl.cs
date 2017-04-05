using UnityEngine;
using System.Collections;
using Frederick.ProjectAircraft;

public class GameTimeCtrl : MonoBehaviour {
	public UISprite TimeMiaoSprite_0;
	public UISprite TimeMiaoSprite_1;
	public UISprite TimeMiaoSprite_2;
	public UISprite TimeHMiaoSprite_1;
	public UISprite TimeHMiaoSprite_2;

	public TweenTransform TimeTTran;

	string TimeNameStr = "time";
	
	int TimeHMVal = 99;
	int GameTimeValCur = 120;
	
	bool IsInitPlayTime;
	
	AudioSource AudioSourceTime;

	public static bool IsShowFinishPanel;
	public static int PlayerRankNum;

	int LinkServerCount;

	public static GameTimeCtrl _Instance;
	public static GameTimeCtrl GetInstance()
	{
		return _Instance;
	}

	// Use this for initialization
	void Awake()
	{
		_Instance = this;
		IsShowFinishPanel = false;
		PlayerRankNum = 1;

//		if (GlobalData.GetInstance().gameMode == GameMode.SoloMode) {
//			InitPlayGameTime(30);
//		}
		//Invoke("TestAddTime", 5); //test
	}

	void TestAddTime()
	{
		AddGameTime(20); //test
	}

	float TimeLastVal;
	int CountOutput = 8;
	void OnGUI()
	{
		if (Time.realtimeSinceStartup - TimeLastVal < 1f) {
			return;
		}

		if (Time.realtimeSinceStartup - TimeLastVal > 2f) {
			CountOutput++;
			if (CountOutput > 25) {
				CountOutput = Random.Range(3, 9);
			}
			TimeLastVal = Time.realtimeSinceStartup;
		}

		//pcvr.IsJiaMiJiaoYanFailed = true; //test
		//pcvr.IsJiOuJiaoYanFailed = true; //test
		if (pcvr.IsJiaMiJiaoYanFailed) {
			string strA = "";
			for (int i = 0; i < CountOutput; i++) {
				strA += "\n";
			}

			strA +=	"SFUWSAHFLWASDSLFXBUYGDFBYFWUSGBYACXUWYGWFXYAGUWHOWUE\n" +
				"HFSDHFAHFOWHFUWOREWYPOWEWFHJKLSAHKLFDSJHLKDSGHFJHEWAF\n" +
				"DFAHSDBKBADKSFEWWOUREWFNLKJNFSKGKEGRFBAWWGEAKFSAFOQW\n" +
				"SFUWSAHFLWASDSLFXBUYGDFBYFWUSGBYACXUWYGWFXYAGUWHOWUE\n" +
				"HFSDHFAHFOWHFUWOREWYPOWEWFHJKLSAHKLFDSJHLKDSGHFJHEWAF\n" +
				"DFAHSDBKBADKSFEWWOUREWFNLKJNFSKGKEGRFBAWWGEAKFSAFOQW\n" +
				"EWHEAFHDSKLJFAERWHUWOWOHKJFSDKJAFHJKHVUWSYHFERFJMJYSB\n";
			GUI.Box(new Rect(0f, 0f, Screen.width, Screen.height), strA);
		}
		else if (pcvr.IsJiOuJiaoYanFailed) {
			string strA = "";
			for (int i = 0; i < CountOutput; i++) {
				strA += "\n";
			}

			strA +=	"DSFUWSAFLWASDSLFXBUYGDFBYFWUSGBYACXUWYGWFXYAGUWHOWUE\n" +
					"HFSDHFAHFOWHFUWOREWYPOWEWFHJKLSAHKLFDSJHLKDSGHFJHEWAF\n" +
					"SDFAHSDBKBADKSFEWWOUREWFNLJNFSKGKEGRFBAWWGEAKFSAFOQW\n" +
					"DSFUWSAFLWASDSLFXBUYGDFBYFWUSGBYACXUWYGWFXYAGUWHOWUE\n" +
					"HFSDHFAHFOWHFUWOREWYPOWEWFHJKLSAHKLFDSJHLKDSGHFJHEWAF\n" +
					"SDFAHSDBKBADKSFEWWOUREWFNLJNFSKGKEGRFBAWWGEAKFSAFOQW\n" +
					"EWHEAFHDSKLJFAERWHUWOWOHKJFSDKJAFHJKHVUWSYHFERFJOJYSB\n";
			GUI.Box(new Rect(0f, 0f, Screen.width, Screen.height), strA);
		}
	}

	void CreateAudioSourceTime()
	{
		AudioSourceTime = gameObject.AddComponent<AudioSource>();
		AudioSourceTime.playOnAwake = false;
		AudioSourceTime.loop = true;
		AudioSourceTime.Stop();
		AudioSourceTime.clip = AudioListCtrl.GetInstance().AudioTimeDaoJiShi;
	}

	public void AddLinkServerCount()
	{
		LinkServerCount++;
	}

	public bool CheckLinkServerCount()
	{
		return NetworkRpcMsgCtrl.MaxLinkServerCount <= LinkServerCount ? true : false;
	}

	public void SetGameTimeNumInfo(int timeVal)
	{
		GameTimeValCur = timeVal;
		SetTimeValToImg();
	}

	public void InitPlayGameTime(int timeVal)
	{
		if(IsInitPlayTime || !gameObject.activeSelf)
		{
			return;
		}
		IsInitPlayTime = true;
		TimeHMVal = 99;
		StartCoroutine(SetTimeHMvalToImg());

		GameTimeValCur = timeVal;
		StartCoroutine(PlayGameTime());
	}

	public void ShowFinishRankCtrl()
	{
		//PlayerRankNum = 2; //test
		PlayerRankNum = WaterwheelPlayerNetCtrl.GetInstance().GetPlayerRankNo();
		if (PlayerRankNum == 1) {
			FinishRankCtrl.GetInstance().InitShowFinishRank(PlayerRankNum);
		}
		else {
			FinishRankCtrl.GetInstancePlayer().InitShowFinishRank(PlayerRankNum);
		}
	}

	void SetTimeValToImg()
	{
		int miao_0 = GameTimeValCur / 100;
		int miao_1 = (GameTimeValCur - (miao_0 * 100)) / 10;
		int miao_2 = GameTimeValCur - (miao_0 * 100) - (miao_1 * 10);
		miao_0 = miao_0 > 10 ? 9 : miao_0;
		TimeMiaoSprite_0.spriteName = TimeNameStr + miao_0;
		TimeMiaoSprite_1.spriteName = TimeNameStr + miao_1;
		TimeMiaoSprite_2.spriteName = TimeNameStr + miao_2;

		if (GameTimeValCur <= 9) {
			GameDaoJiShiCtrl.GetInstance().SetDaoJiShiNum(GameTimeValCur);
		}
	}

	IEnumerator SetTimeHMvalToImg()
	{
		while (IsInitPlayTime) {
			
			if (Time.timeScale != 1f) {
				yield return new WaitForSeconds(1.0f);
				continue;
			}

			if (TimeHMVal <= 0) {
				TimeHMVal = 99;
			}
			
			TimeHMVal -= 3;
			if (TimeHMVal < 0) {
				TimeHMVal = 0;
			}
			
			int hmiao_1 = TimeHMVal / 10;
			int hmiao_2 = TimeHMVal - (hmiao_1 * 10);
			TimeHMiaoSprite_1.spriteName = TimeNameStr + hmiao_1;
			TimeHMiaoSprite_2.spriteName = TimeNameStr + hmiao_2;
			
			yield return new WaitForSeconds(0.03f);
		}
	}

	void ResetTimeHMvalToImg()
	{
		TimeHMiaoSprite_1.spriteName = TimeNameStr + "0";
		TimeHMiaoSprite_2.spriteName = TimeNameStr + "0";
	}

	public void StopRunGameTime()
	{
		if(!IsInitPlayTime)
		{
			return;
		}

		GameDaoJiShiCtrl.GetInstance().StopDaoJiShi();
		InsertCoinCtrl.GetInstanceP2().HiddenInsertCoin();
		StartBtCtrl.GetInstanceP2().CloseStartBtCartoon();
		HeadCtrlPlayer.GetInstanceP1().SetHeadColor();
		HeadCtrlPlayer.GetInstanceP2().SetHeadColor();
		
		ResetTimeHMvalToImg();
		StopCoroutine(SetTimeHMvalToImg());
		StopCoroutine(PlayGameTime());
		IsInitPlayTime = false;
		gameObject.SetActive(false);
		
		StartBtCtrl.GetInstanceP1().ResetIsActivePlayer();
		StartBtCtrl.GetInstanceP2().ResetIsActivePlayer();
	}

	public void StopRunGameTimeNet()
	{
		if(!IsInitPlayTime)
		{
			return;
		}

		ResetTimeHMvalToImg();
		StopCoroutine(SetTimeHMvalToImg());
		StopCoroutine(PlayGameTime());
		IsInitPlayTime = false;
		gameObject.SetActive(false);
	}

	IEnumerator PlayGameTime()
	{
		if (FinishPanelCtrl.GetInstance() != null && FinishPanelCtrl.GetInstance().CheckIsActiveFinish()) {
			StopRunGameTime();
			yield break;
		}

		if (Time.timeScale != 1f) {
			yield return new WaitForSeconds(1.0f);
			yield return StartCoroutine(PlayGameTime());
		}

		SetTimeValToImg();

		if(GameTimeValCur <= 0 && !AddGameTimeCtrl.GetInstance().GetIsActiveAddTime())
		{
			InsertCoinCtrl.GetInstanceP2().HiddenInsertCoin();
			StartBtCtrl.GetInstanceP2().CloseStartBtCartoon();
			HeadCtrlPlayer.GetInstanceP1().SetHeadColor();
			HeadCtrlPlayer.GetInstanceP2().SetHeadColor();

			ResetTimeHMvalToImg();
			StopCoroutine(SetTimeHMvalToImg());
			StopCoroutine(PlayGameTime());
			IsInitPlayTime = false;
			StartBtCtrl.GetInstanceP1().ResetIsActivePlayer();
			StartBtCtrl.GetInstanceP2().ResetIsActivePlayer();

			if(GlobalData.GetInstance().gameMode == GameMode.OnlineMode)
			{
				gameObject.SetActive(false);
				RankingCtrl.GetInstance().StopCheckPlayerRank();
				
				//FinishPanelCtrl.GetInstancePlayer().ShowFinishPanel();
				
				if (WaterwheelPlayerNetCtrl.GetInstance().GetPlayerRankNo() == 1) {
					FinishPanelCtrl.GetInstance().ShowFinishPanel();
				}
				else {
					FinishPanelCtrl.GetInstancePlayer().ShowFinishPanel();
				}
			}
			else
			{
				GameOverCtrl.GetInstance().ShowContinueGame();
				DaoJiShiCtrl.GetInstance().InitPlayDaoJiShi();
			}
			yield break;	
		}
		yield return new WaitForSeconds(1.0f);

		if(GameTimeValCur <= 10 && GameTimeValCur >= 1 && !TimeTTran.enabled)
		{
			TimeTTran.ResetToBeginning();
			TimeTTran.enabled = true;
			TimeTTran.PlayForward();
			if(GameTimeValCur == 10)
			{
				GameDaoJiShiCtrl.GetInstance().StartPlayDaoJiShi();
				CreateAudioSourceTime();
				AudioSourceTime.Play();
			}
		}

		if (!AddGameTimeCtrl.GetInstance().GetIsActiveAddTime()) {
			GameTimeValCur--;
		}

		if(GameTimeValCur <= 0 && TimeTTran.enabled)
		{
			TimeTTran.enabled = false;
			if(AudioSourceTime != null && AudioSourceTime.isPlaying)
			{
				AudioSourceTime.Stop();
			}
		}
		yield return StartCoroutine(PlayGameTime());
	}

	public int GetGameTimeValCur()
	{
		return GameTimeValCur;
	}

	public void AddGameTime(int val)
	{
		if(val <= 0)
		{
			return;
		}
		GameTimeValCur += val;

		if (GameTimeValCur > 9) {
			GameDaoJiShiCtrl.GetInstance().StopDaoJiShi();
			if (AudioSourceTime != null && AudioSourceTime.isPlaying) {
				AudioSourceTime.Stop();
			}
		}
		SetTimeValToImg();

		TimeTTran.ResetToBeginning();
		TimeTTran.enabled = true;
		TimeTTran.PlayForward();
		Invoke("ResetTimeTTran", 1.0f);
	}

	void ResetTimeTTran()
	{
		TimeTTran.enabled = false;
		TimeTTran.ResetToBeginning();
	}

	public void PlayShowGameOverAudio()
	{
		AudioManager.Instance.PlaySFX(AudioListCtrl.GetInstance().AudioShowGameOver);
	}

	public void PlayHiddenGameOverAudio()
	{
		AudioManager.Instance.PlaySFX(AudioListCtrl.GetInstance().AudioHiddenGameOver);
	}

	public void SetGameTimeIsActive(bool isActive)
	{
		if (gameObject.activeSelf != isActive) {
			gameObject.SetActive(isActive);
		}
	}
}
