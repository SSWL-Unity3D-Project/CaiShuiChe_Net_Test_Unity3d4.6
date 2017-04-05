using UnityEngine;
using System.Collections;
using Frederick.ProjectAircraft;

public class GameOverCtrl : MonoBehaviour {
	public GameObject GameOverImg;

	UISprite OverSprite;
	TweenScale OverTScl;
	
	bool IsInitPlayHiddenOver;
	UISprite GameOverImgSprite;
	TweenScale GameOverImgTScl;

	public static GameOverCtrl _Instance;
	public static GameOverCtrl GetInstance()
	{
		return _Instance;
	}

	// Use this for initialization
	void Awake()
	{
		_Instance = this;
		gameObject.SetActive(false);
		OverSprite = GetComponent<UISprite>();
		if(GlobalData.GetInstance().gameMode == GameMode.OnlineMode)
		{
			OverTScl = GetComponent<TweenScale>();
			OverTScl.enabled = false;
		}
		else
		{
			GameOverImgSprite = GameOverImg.GetComponent<UISprite>();
			GameOverImgTScl = GameOverImg.GetComponent<TweenScale>();
			GameOverImgSprite.enabled = false;
			GameOverImg.SetActive(false);
		}
	}

	public void ShowGameOverImg()
	{
		if (GameOverImgSprite.enabled) {
			return;
		}
		AudioListCtrl.PlayAudio(AudioListCtrl.GetInstance().AudioGameShiBai);
		AudioManager.Instance.PlaySFX(AudioListCtrl.GetInstance().AudioGameOver);
		
		if (GlobalData.GetInstance().gameMode == GameMode.SoloMode)
		{
			ZhuJiaoNan.GetInstance().PlayFailAction();
		}
		else {
			WaterwheelPlayerNetCtrl.GetInstance().PlayZhuJiaoNanFailAction();
		}

		GameOverImg.SetActive(true);
		GameOverImg.transform.localScale = new Vector3(10f, 10f, 1f);
		GameOverImgSprite.enabled = true;
		GameOverImgTScl.enabled = true;
		Invoke("HiddenQuWeiGameOverImg", 1f);
	}

	void HiddenQuWeiGameOverImg()
	{
		GameCtrlXK.GetInstance().CloseAllCameras();
		Invoke("GameOverBackToMovieScene", 4.0f);
	}

	public bool CheckIsActiveOver()
	{
		if (gameObject.activeSelf || (GameOverImg != null && GameOverImg.activeSelf)) {
			return true;
		}
		return false;
	}

	public void ShowContinueGame()
	{
		if(gameObject.activeSelf)
		{
			return;
		}
		gameObject.SetActive(true);
		OverSprite.enabled = true;
		
		HeadCtrlPlayer.GetInstanceP1().SetHeadColor();
		if(GlobalData.GetInstance().gameMode == GameMode.SoloMode)
		{
			if(!GlobalData.GetInstance().IsFreeMode)
			{
				if(GlobalData.GetInstance().Icoin >= GlobalData.GetInstance().XUTOUBI)
				{
					StartBtCtrl.GetInstanceP1().InitStartBtCartoon();
					StartBtCtrl.GetInstanceP2().CloseStartBtCartoon();
				}
				else
				{
					InsertCoinCtrl.GetInstanceP1().ShowInsertCoin();
					InsertCoinCtrl.GetInstanceP2().HiddenInsertCoin();
				}
			}
			else
			{
				StartBtCtrl.GetInstanceP1().InitStartBtCartoon();
				StartBtCtrl.GetInstanceP2().CloseStartBtCartoon();
			}
		}
		else
		{
			EventDelegate.Add(OverTScl.onFinished, delegate{
				Invoke("InitPlayHiddenOver", 2.0f);
			});
			OverTScl.ResetToBeginning();
			OverTScl.enabled = true;
			OverTScl.PlayForward();
			GameTimeCtrl.GetInstance().PlayShowGameOverAudio();
			StartBtCtrl.GetInstanceP2().CloseStartBtCartoon();
		}
		HeadCtrlPlayer.GetInstanceP2().SetHeadColor();
	}

	void InitPlayHiddenOver()
	{
		if(IsInitPlayHiddenOver)
		{
			return;
		}
		IsInitPlayHiddenOver = true;
		StartCoroutine(PlayHiddenOver());
		GameTimeCtrl.GetInstance().PlayHiddenGameOverAudio();
	}

	IEnumerator PlayHiddenOver()
	{
		OverSprite.fillAmount -= 0.05f;
		if(OverSprite.fillAmount <= 0f)
		{
			OverSprite.fillAmount = 0f;
			StopCoroutine("PlayHiddenOver");
			yield return new WaitForSeconds(0.5f);

			GameCtrlXK.GetInstance().CloseAllCameras();
			Invoke("GameOverBackToMovieScene", 4.0f);
			yield break;
		}

		yield return new WaitForSeconds(0.03f);

		yield return StartCoroutine(PlayHiddenOver());
	}

	void GameOverBackToMovieScene()
	{
		GlobalData.GetInstance().gameLeve = GameLeve.Movie;
		System.GC.Collect();
		//Application.LoadLevel((int)GameLeve.Movie);

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

	public void HiddenContinueGame()
	{
		if(!gameObject.activeSelf)
		{
			return;
		}
		gameObject.SetActive(false);
		OverSprite.enabled = false;
	}
}
