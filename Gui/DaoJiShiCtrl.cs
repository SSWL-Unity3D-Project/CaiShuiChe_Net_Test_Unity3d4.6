using UnityEngine;
using System.Collections;
using Frederick.ProjectAircraft;

public class DaoJiShiCtrl : MonoBehaviour {

	UISprite DaoJiShiSprite;
	
	bool IsInitPlay;
	public static int TimeVal = 9;
	string DaoJiShiName = "daoJiShi";
	bool IsStopCheckAddSpeed;

	public static DaoJiShiCtrl _Instance;
	public static DaoJiShiCtrl GetInstance()
	{
		return _Instance;
	}

	// Use this for initialization
	void Awake()
	{
		_Instance = this;
		TimeVal = 9;
		DaoJiShiSprite = GetComponent<UISprite>();
	}

	public void InitPlayDaoJiShi()
	{
		if(IsInitPlay)
		{
			return;
		}
		IsInitPlay = true;

		TimeVal = 9;
		DaoJiShiSprite.spriteName = DaoJiShiName + TimeVal.ToString();
		DaoJiShiSprite.enabled = true;
		StartCoroutine("PlayDaoJiShi");
	}

	public void StopDaoJiShi()
	{
		if(!IsInitPlay || !DaoJiShiSprite.enabled)
		{
			return;
		}

		if (GlobalData.GetInstance().gameMode != GameMode.OnlineMode) {
			IsInitPlay = false;
		}
		else {
			WaterwheelPlayerNetCtrl.GetInstance().StopMovePlayer();
		}
		DaoJiShiSprite.enabled = false;
		StopCoroutine("PlayDaoJiShi");
	}

	public bool CheckIsPlayDaoJiShi()
	{
		return IsInitPlay;
	}

	public bool GetIsStopCheckAddSpeed()
	{
		return IsStopCheckAddSpeed;
	}

	IEnumerator PlayDaoJiShi()
	{
		if(TimeVal < 1)
		{
			StopDaoJiShi();
			
			StartBtCtrl.GetInstanceP1().CloseStartBtCartoon(); //close player startBt cartoon
			if (GlobalData.GetInstance().gameMode == GameMode.SoloMode) {
				
				GameOverCtrl.GetInstance().HiddenContinueGame();
				GameOverCtrl.GetInstance().ShowGameOverImg();
				//FinishPanelCtrl.GetInstance().ShowFinishPanel(); //Show Finish Panel
			}
			else {

				IsStopCheckAddSpeed = true;
				GameCtrlXK.IsStopMoveAiPlayer = true;
				WaterwheelPlayerNetCtrl.GetInstance().ResetPlayerInfo();
				if (WaterwheelPlayerNetCtrl.GetInstance().GetPlayerRankNo() == 1) {
					FinishPanelCtrl.GetInstance().ShowFinishPanel();
				}
				else {
					//show finishPanelPlayer
					FinishPanelCtrl.GetInstancePlayer().ShowFinishPanel();
				}
			}
			yield break;
		}
		AudioManager.Instance.PlaySFX(AudioListCtrl.GetInstance().AudioGameDaoJiShi);
		DaoJiShiSprite.spriteName = DaoJiShiName + TimeVal.ToString();
		yield return new WaitForSeconds(1f);

		TimeVal--;
		yield return StartCoroutine("PlayDaoJiShi");
	}
}
