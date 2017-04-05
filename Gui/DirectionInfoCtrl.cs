using UnityEngine;
using System.Collections;

public class DirectionInfoCtrl : MonoBehaviour
{
	public static DirectionInfoCtrl _Instance;
	public static DirectionInfoCtrl GetInstance()
	{
		return _Instance;
	}

	// Use this for initialization
	void Start ()
	{
		_Instance = this;
		gameObject.SetActive(false);

		UISprite DirSprite = GetComponent<UISprite>();
		DirSprite.enabled = true;
	}

	public void ShowDirWrongInfo()
	{
		if (StartGameTimeCtrl.GetInstance().CheckIsActiveStartTime()) {
			return;
		}

		if (PlayerYueJieCtrl.GetInstance().CheckIsShowPlayerYueJie()) {
			return;
		}

		if (gameObject.activeSelf) {
			return;
		}

		gameObject.SetActive(true);
		//AudioListCtrl.PlayAudio(AudioListCtrl.GetInstance().AudioGameJingGao);
		AudioListCtrl.PlayAudioLoopJingGao();
		PlayerAutoFire.HandlePlayerCloseShenXingState();
		PlayerAutoFire.AddPlayerHitZhangAiNum();
		if (!IsInvoking("MakePlayerBackToPath")) {
			Invoke("MakePlayerBackToPath", 1.5f);
		}
	}

	public void HiddenDirWrong()
	{
		if(!gameObject.activeSelf)
		{
			return;
		}
		CancelInvoke("MakePlayerBackToPath");
		gameObject.SetActive(false);
		
		AudioListCtrl.StopAudioLoopJingGao();
	}

	public bool GetIsActiveDirection()
	{
		return gameObject.activeSelf;
	}

	void MakePlayerBackToPath()
	{
		HiddenDirWrong();
		if (!StartBtCtrl.GetInstanceP1().CheckIsActivePlayer()) {
			return;
		}

		if (GlobalData.GetInstance().gameMode == GameMode.SoloMode) {
			
			WaterwheelPlayerCtrl.GetInstance().ResetPlayerPos(); //Reset Player pos
		}
		else {
			
			WaterwheelPlayerNetCtrl.GetInstance().ResetPlayerPos(); //Reset Player pos
		}
	}
}

