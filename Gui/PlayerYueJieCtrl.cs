using UnityEngine;
using System.Collections;

public class PlayerYueJieCtrl : MonoBehaviour {

	public static float YueJieSpeed = 30f;
	static PlayerYueJieCtrl _Instance;
	public static PlayerYueJieCtrl GetInstance()
	{
		return _Instance;
	}

	// Use this for initialization
	void Start () {
		_Instance = this;
		gameObject.SetActive(false);
		UISprite yueJieSprite = GetComponent<UISprite>();
		yueJieSprite.enabled = true;
	}

	public bool CheckIsShowPlayerYueJie()
	{
		return gameObject.activeSelf;
	}

	public void ShowPlayerYueJie()
	{
		if (StartGameTimeCtrl.GetInstance().CheckIsActiveStartTime()) {
			return;
		}

		if (gameObject.activeSelf) {
			return;
		}
		gameObject.SetActive(true);
		//AudioListCtrl.PlayAudio(AudioListCtrl.GetInstance().AudioGameJingGao);
		AudioListCtrl.PlayAudioLoopJingGao();
		DirectionInfoCtrl.GetInstance().HiddenDirWrong();
		PlayerAutoFire.ResetIsIntoPuBu();
		PlayerAutoFire.HandlePlayerCloseHuanYingFu();
		PlayerAutoFire.HandlePlayerCloseShenXingState();
		PlayerAutoFire.AddPlayerHitZhangAiNum();
	}

	public void ClosePlayerYueJie()
	{
		if (!gameObject.activeSelf) {
			return;
		}
		gameObject.SetActive(false);
		
		AudioListCtrl.StopAudioLoopJingGao();
	}
}
