using System;
using UnityEngine;
using System.Collections;
using Frederick.ProjectAircraft;

public class StartGameTimeCtrl : MonoBehaviour {

	public TweenScale TimeScaleToBig;
	public TweenScale TimeScaleToSmall;

	int TimeCount = 3;
	UISprite TimeSprite;
	int GameTimeVal = 30;
	bool IsPlayStartTime;

	static StartGameTimeCtrl _Instance;
	public static StartGameTimeCtrl GetInstance()
	{
		return _Instance;
	}

	// Use this for initialization
	void Awake () {
		_Instance = this;

		TimeSprite = GetComponent<UISprite>();
		TimeSprite.enabled = false;
		TimeSprite.spriteName = "go_" + TimeCount.ToString();
		MoveCameraByPath.IsMovePlayer = false;

		int diffVal = Convert.ToInt32(GlobalData.GetInstance().GameDiff);
		GameDiffState diffState = (GameDiffState)diffVal;
		switch (diffState) {
		case GameDiffState.GameDiffLow:
			if (GlobalData.GetInstance().gameMode == GameMode.OnlineMode) {
				GameTimeVal = 420;
			}
			else {
				GameTimeVal = 240;
			}
			WaterwheelPlayerCtrl.mMaxVelocityFoot = 70;
			WaterwheelPlayerNetCtrl.mMaxVelocityFoot = 95;
			break;
			
		case GameDiffState.GameDiffMiddle:
			if (GlobalData.GetInstance().gameMode == GameMode.OnlineMode) {
				GameTimeVal = 360;
			}
			else {
				GameTimeVal = 120;
			}
			WaterwheelPlayerCtrl.mMaxVelocityFoot = 60;
			WaterwheelPlayerNetCtrl.mMaxVelocityFoot = 85;
			break;
			
		case GameDiffState.GameDiffHigh:
			if (GlobalData.GetInstance().gameMode == GameMode.OnlineMode) {
				GameTimeVal = 300;
			}
			else {
				GameTimeVal = 100;
			}
			WaterwheelPlayerCtrl.mMaxVelocityFoot = 50;
			WaterwheelPlayerNetCtrl.mMaxVelocityFoot = 75;
			break;
		}
		WaterwheelPlayerCtrl.mMaxVelocityFootMS = (float)WaterwheelPlayerCtrl.mMaxVelocityFoot / 3.6f;

		//GameTimeVal = 300000; //test
		//GameTimeVal = 12;

		/*if (GlobalData.GetInstance().gameMode == GameMode.SoloMode) {
			Invoke("DelayPlayTime", 1f);
		}*/
	}

	public bool GetIsPlayStartTime()
	{
		return IsPlayStartTime;
	}

	public void DelayPlayTime()
	{
		if (IsPlayStartTime) {
			return;
		}
		IsPlayStartTime = true;

		transform.localScale = new Vector3(0f, 0f, 1f);
		TimeSprite = GetComponent<UISprite>();
		TimeSprite.enabled = true;

		Invoke("MakeStartTimeToBig", 0.5f);
		GameTimeCtrl.GetInstance().SetGameTimeNumInfo(GameTimeVal);
	}

	void MakeStartTimeToBig()
	{
		if (TimeCount < 0) {
			//stop DaoJiShi
			gameObject.SetActive(false);
			MoveCameraByPath.IsMovePlayer = true;
			GameTimeCtrl.GetInstance().InitPlayGameTime(GameTimeVal);
			return;
		}

		TimeScaleToBig.ResetToBeginning();
		SubStartTime();
		EventDelegate.Add(TimeScaleToBig.onFinished, delegate{
			MakeStartTimeToSmall();
		});
		TimeScaleToBig.PlayForward();
		TimeScaleToBig.enabled = true;
	}

	void MakeStartTimeToSmall()
	{
		TimeScaleToSmall.ResetToBeginning();
		EventDelegate.Add(TimeScaleToSmall.onFinished, delegate{
			MakeStartTimeToBig();
		});
		TimeScaleToSmall.PlayForward();
		TimeScaleToSmall.enabled = true;
	}

	void SubStartTime()
	{
		TimeSprite.spriteName = "go_" + TimeCount.ToString();
		AudioManager.Instance.PlaySFX(AudioListCtrl.GetInstance().AudioTimeGo);
		//Debug.Log("SubStartTime ***** TimeCount " + TimeCount + ", name " + TimeSprite.spriteName);
		
		TimeCount--;
	}

	public bool CheckIsActiveStartTime()
	{
		return gameObject.activeSelf;
	}
}
