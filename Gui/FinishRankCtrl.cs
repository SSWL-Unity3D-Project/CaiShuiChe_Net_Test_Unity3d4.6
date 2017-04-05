using UnityEngine;
using System.Collections;
using Frederick.ProjectAircraft;

public class FinishRankCtrl : MonoBehaviour {
	
	public FinishPanelNum PanelNum = FinishPanelNum.FinishPanel;

	public GameObject FinishRankObj_1;
	public GameObject FinishRankObj_2;
	public GameObject FinishRankObj_3;
	public GameObject FinishRankObj_4;
	public GameObject FinishRankObj_5;
	public GameObject FinishRankObj_6;
	public GameObject FinishRankObj_7;
	public GameObject FinishRankObj_8;
	
	public GameObject WFinishRankObj_1;
	public GameObject WFinishRankObj_2;
	public GameObject WFinishRankObj_3;
	public GameObject WFinishRankObj_4;
	public GameObject WFinishRankObj_5;
	public GameObject WFinishRankObj_6;
	public GameObject WFinishRankObj_7;
	public GameObject WFinishRankObj_8;

	UISprite []RankSpriteArray;
	UISprite []WRankSpriteArray;

	/*public AudioClip AudioRankTPos;
	public AudioClip AudioRankTRot;*/

	TweenPosition FinishRankTPos;
	TweenRotation FinishRankTRot;
	bool IsInitShowFinishRank;
	
	public static FinishPanelNum HiddenFinishNum = FinishPanelNum.FinishPanel;

	float TimeHiddenFinish = 12f;

	public static FinishRankCtrl _Instance;
	public static FinishRankCtrl GetInstance()
	{
		return _Instance;
	}
	
	public static FinishRankCtrl _InstancePlayer;
	public static FinishRankCtrl GetInstancePlayer()
	{
		return _InstancePlayer;
	}

	// Use this for initialization
	void Awake () {
		if (PanelNum == FinishPanelNum.FinishPanel) {
			_Instance = this;
		}
		else {
			_InstancePlayer = this;
		}

		FinishRankTPos = GetComponent<TweenPosition>();
		FinishRankTPos.enabled = false;

		FinishRankTRot = GetComponent<TweenRotation>();
		FinishRankTRot.enabled = false;

		RankSpriteArray = new UISprite[8];
		RankSpriteArray[0] = FinishRankObj_1.GetComponent<UISprite>();
		RankSpriteArray[1] = FinishRankObj_2.GetComponent<UISprite>();
		RankSpriteArray[2] = FinishRankObj_3.GetComponent<UISprite>();
		RankSpriteArray[3] = FinishRankObj_4.GetComponent<UISprite>();
		RankSpriteArray[4] = FinishRankObj_5.GetComponent<UISprite>();
		RankSpriteArray[5] = FinishRankObj_6.GetComponent<UISprite>();
		RankSpriteArray[6] = FinishRankObj_7.GetComponent<UISprite>();
		RankSpriteArray[7] = FinishRankObj_8.GetComponent<UISprite>();

		if (WFinishRankObj_1 != null) {
			WRankSpriteArray = new UISprite[RankingCtrl.MaxPlayerRankNum];
			WRankSpriteArray[0] = WFinishRankObj_1.GetComponent<UISprite>();
			WRankSpriteArray[1] = WFinishRankObj_2.GetComponent<UISprite>();
			WRankSpriteArray[2] = WFinishRankObj_3.GetComponent<UISprite>();
			WRankSpriteArray[3] = WFinishRankObj_4.GetComponent<UISprite>();
			WRankSpriteArray[4] = WFinishRankObj_5.GetComponent<UISprite>();
			WRankSpriteArray[5] = WFinishRankObj_6.GetComponent<UISprite>();
			WRankSpriteArray[6] = WFinishRankObj_7.GetComponent<UISprite>();
			WRankSpriteArray[7] = WFinishRankObj_8.GetComponent<UISprite>();
		}

		for (int i = 0; i < RankingCtrl.MaxPlayerRankNum; i++) {
			RankSpriteArray[i].enabled = false;
			if (WRankSpriteArray != null) {
				WRankSpriteArray[i].enabled = false;
			}
		}

		gameObject.SetActive(false);
	}

	string GetRankSpriteName(int rankVal)
	{
		int val = rankVal - 1;
		//Debug.Log("i " + val + ", name " + RankingCtrl.mRankPlayer[val].RankListName);
		return RankingCtrl.mRankPlayer[val].RankListName;
	}

	bool CheckIsRunToEndPoint(int rankVal)
	{
		int val = rankVal - 1;
		return RankingCtrl.mRankPlayer[val].IsRunToEndPoint;
	}

	int GetWRankNum()
	{
		int i = 0;
		if (!WaterwheelPlayerNetCtrl.GetInstance().GetIsRunToEndPoint()) {
			for (i = 0; i < RankingCtrl.MaxPlayerRankNum; i ++) {
				if (!RankingCtrl.mRankPlayer[i].IsRunToEndPoint) {
					break;
				}
			}
		}
		else {
			int playerRankNo = WaterwheelPlayerNetCtrl.GetInstance().GetPlayerRankNo();
			if (playerRankNo < RankingCtrl.MaxPlayerRankNum) {
				for (i = playerRankNo; i < RankingCtrl.MaxPlayerRankNum; i ++) {
					if (!RankingCtrl.mRankPlayer[i].IsRunToEndPoint) {
						break;
					}
				}
			}
			else
			{
				i = RankingCtrl.MaxPlayerRankNum;
			}
		}
		return i;
	}

	bool CheckIsHandlePlayer(int indexVal)
	{
		bool isHandlePlayer = false;
		if (indexVal >= 0 && indexVal < RankingCtrl.MaxPlayerRankNum) {
			/*Debug.Log("CheckIsHandlePlayer********indexVal " + indexVal
			          + ", ** name " + RankingCtrl.mRankPlayer[indexVal].Name);*/
			if (RankingCtrl.mRankPlayer[indexVal].AimMarkDataScript != null
			    && RankingCtrl.mRankPlayer[indexVal].AimMarkDataScript.CheckIsHandlePlayer()) {
				isHandlePlayer = true;
			}
		}
		return isHandlePlayer;
	}

	void InitRankSpriteInfo(int rankVal)
	{
		/*Debug.Log("test*******************InitRankSpriteInfo*********");
		for(int i = 0; i < RankingCtrl.MaxPlayerRankNum; i++)
		{
			if(RankingCtrl.mRankPlayer[i] != null)
			{
				Debug.Log("*******************name " + RankingCtrl.mRankPlayer[i].Name);
			}
		}*/

		GameObject PlayerSpriteObj = null;
		if (rankVal != 1) {

			int wrankNum = GetWRankNum();
			//Debug.Log("wrankNum*******************   " + wrankNum);
			for (int i = 0; i < wrankNum; i++) {
				RankSpriteArray[i].spriteName = GetRankSpriteName(i + 1);
				RankSpriteArray[i].enabled = true;
				if (CheckIsHandlePlayer(i) && PlayerSpriteObj == null) {
					PlayerSpriteObj = RankSpriteArray[i].gameObject;
				}
			}

			for (int i = 0; i < RankingCtrl.MaxPlayerRankNum - wrankNum; i++) {
				WRankSpriteArray[i].spriteName = GetRankSpriteName(i + 1 + wrankNum);
				WRankSpriteArray[i].enabled = true;
				if (CheckIsHandlePlayer(i + wrankNum) && PlayerSpriteObj == null) {
					PlayerSpriteObj = WRankSpriteArray[i].gameObject;
				}
			}
		}
		else {
			for (int i = 0; i < RankingCtrl.MaxPlayerRankNum; i++) {
				RankSpriteArray[i].spriteName = GetRankSpriteName(i + 1);
				RankSpriteArray[i].enabled = true;
				if (CheckIsHandlePlayer(i) && PlayerSpriteObj == null) {
					PlayerSpriteObj = RankSpriteArray[i].gameObject;
				}
			}
		}

		if (PlayerSpriteObj != null) {
			TweenColor twCol = PlayerSpriteObj.AddComponent<TweenColor>();
			twCol.enabled = false;
			twCol.to = Color.green;
			twCol.style = UITweener.Style.PingPong;
			twCol.ResetToBeginning();
			twCol.enabled = true;

			twCol.PlayForward();

			TweenScale twScl = PlayerSpriteObj.AddComponent<TweenScale>();
			twScl.enabled = false;
			twScl.to = new Vector3(1.4f, 1.4f, 1f);
			twScl.style = UITweener.Style.PingPong;
			twScl.ResetToBeginning();
			twScl.enabled = true;
			
			twScl.PlayForward();
		}
	}

	public void InitShowFinishRank(int rankVal)
	{
		if(IsInitShowFinishRank)
		{
			return;
		}
		IsInitShowFinishRank = true;
		HiddenFinishNum = PanelNum;
		InitRankSpriteInfo(rankVal);
		
		gameObject.SetActive(true);
		Invoke("InitFinishRankBackToMovieScene", TimeHiddenFinish);
	}

	void ActiveFinishRankTPos()
	{
		EventDelegate.Add(FinishRankTPos.onFinished, delegate {
			Invoke("ActiveFinishRankTRot", 0.5f);
		});
		FinishRankTPos.ResetToBeginning();
		FinishRankTPos.enabled = true;
		FinishRankTPos.PlayForward();
		//AudioManager.Instance.PlaySFX(AudioRankTPos);
	}

	void ActiveFinishRankTRot()
	{
		EventDelegate.Add(FinishRankTRot.onFinished, delegate {
			Invoke("FlashPlayerRank", 0.1f);
		});
		FinishRankTRot.ResetToBeginning();
		FinishRankTRot.enabled = true;
		FinishRankTRot.PlayForward();
		//AudioManager.Instance.PlaySFX(AudioRankTRot);
	}

	void FlashPlayerRank()
	{
		Invoke("InitFinishRankBackToMovieScene", TimeHiddenFinish);
	}

	void InitFinishRankBackToMovieScene()
	{
		if (HiddenFinishNum == FinishPanelNum.FinishPanel) {
			FinishPanelCtrl.GetInstance().InitHiddenFinishPanel();
		}
		else {
			FinishPanelCtrl.GetInstancePlayer().InitHiddenFinishPanel();
		}
	}
}
