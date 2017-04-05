using UnityEngine;
using System.Collections;
using Frederick.ProjectAircraft;

public class FinishPanelCtrl : MonoBehaviour {
	public FinishPanelNum PanelNum = FinishPanelNum.FinishPanel;
	
	TweenPosition FinishPanelTPos;
	bool IsInitHidden;
	
	public static bool IsCanLoadSetPanel;
	public TweenScale FinishTScl;

	public static FinishPanelCtrl _Instance;
	public static FinishPanelCtrl GetInstance()
	{
		return _Instance;
	}

	public static FinishPanelCtrl _InstancePlayer;
	public static FinishPanelCtrl GetInstancePlayer()
	{
		return _InstancePlayer;
	}

	// Use this for initialization
	void Awake()
	{
		MyCOMDevice.ComThreadClass.IsLoadingLevel = false;
		//Debug.Log("***********PanelNum " + PanelNum);
		if (PanelNum == FinishPanelNum.FinishPanel) {
			_Instance = this;
		}
		else {
			_InstancePlayer = this;
		}
		FinishPanelTPos = GetComponent<TweenPosition>();
		IsCanLoadSetPanel = true;
		gameObject.SetActive(false);
	}

	public void InitHiddenFinishPanel()
	{
		if(IsInitHidden)
		{
			return;
		}
		IsInitHidden = true;
		gameObject.SetActive(true);

		GameCoin.GetInstance().HiddenCoinInfo();

		EventDelegate.Add(FinishPanelTPos.onFinished, delegate {
			HiddenFinishPanel();
		});

		FinishPanelTPos.ResetToBeginning();
		FinishPanelTPos.PlayForward();
		AudioManager.Instance.PlaySFX(AudioListCtrl.GetInstance().AudioFinishPanel);
	}

	void HiddenFinishPanel()
	{
		if(!gameObject.activeSelf)
		{
			return;
		}
		gameObject.SetActive(false);

		GameCtrlXK.GetInstance().CloseAllCameras();
		Invoke("GotoMoviePanel", 4f);
	}

	void GotoMoviePanel()
	{
		GlobalData.GetInstance().gameLeve = GameLeve.Movie;
		System.GC.Collect();

		if (GlobalData.GetInstance().gameMode == GameMode.OnlineMode) {

			if (Network.isClient || Network.peerType == NetworkPeerType.Disconnected) {
				Application.LoadLevel((int)GameLeve.Movie);
				NetworkServerNet.GetInstance().ResetMasterServerHost();
			}
			else if (Network.isServer) {
				NetworkServerNet.GetInstance().ResetMasterServerHostLoop();
			}
		}
		else
		{
			Application.LoadLevel((int)GameLeve.Movie);
		}
		GlobalData.GetInstance().gameMode = GameMode.None;
	}

	public void ShowFinishPanel()
	{
		if(gameObject.activeSelf)
		{
			return;
		}
		gameObject.SetActive(true);
		IsCanLoadSetPanel = false;
		GameCtrlXK.IsStopMoveAiPlayer = true;
		//Debug.Log("ShowFinishPanel*************");

		if(GlobalData.GetInstance().gameMode == GameMode.SoloMode)
		{
			//Show JiaShiLevel
			EventDelegate.Add(FinishTScl.onFinished, delegate {
				Invoke("ActiveJiaShiLevel", 0.5f);
			});
			AudioListCtrl.PlayAudio(AudioListCtrl.GetInstance().AudioGameHuanHu);
			ZhuJiaoNan.GetInstance().PlayWinAction();
		}
		else
		{
			EventDelegate.Add(FinishTScl.onFinished, delegate {
				Invoke("InitShowFinishRankCtrl", 0f);
			});

			if (WaterwheelPlayerNetCtrl.GetInstance().GetIsRunToEndPoint()) {
				AudioListCtrl.PlayAudio(AudioListCtrl.GetInstance().AudioGameHuanHu);
				WaterwheelPlayerNetCtrl.GetInstance().PlayZhuJiaoNanWinAction();
			}
			else {
				AudioListCtrl.PlayAudio(AudioListCtrl.GetInstance().AudioGameShiBai);
				WaterwheelPlayerNetCtrl.GetInstance().PlayZhuJiaoNanFailAction();
				//Debug.Log("******************************fail");
			}
		}
		FinishTScl.enabled = true;
		AudioManager.Instance.PlaySFX(AudioListCtrl.GetInstance().AudioFinishPanel);
		
		//Invoke("InitHiddenFinishPanel", 3f); //test
	}

	void InitShowFinishRankCtrl()
	{
		GameTimeCtrl.GetInstance().ShowFinishRankCtrl();
	}

	void ActiveJiaShiLevel()
	{
		JiaShiLevelCtrl.GetInstance().ShowJiaShiLevel();
	}
	
	public bool CheckIsActiveFinish()
	{
		return gameObject.activeSelf;
	}
}

public enum FinishPanelNum
{
	FinishPanel,
	FinishPanelPlayer
}