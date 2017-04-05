using UnityEngine;
using System.Collections;
using Frederick.ProjectAircraft;

public class ChengJiuCtrl : MonoBehaviour {
	public GameObject ParticleObj;

	UISprite ChengJiuSprite;
	TweenScale ChengJiuTScl;
	public static ChengJiuCtrl _Instance;
	public static ChengJiuCtrl GetInstance()
	{
		return _Instance;
	}

	// Use this for initialization
	void Awake()
	{
		_Instance = this;
		ChengJiuSprite = GetComponent<UISprite>();
		ChengJiuSprite.enabled = false;

		ChengJiuTScl = GetComponent<TweenScale>();
		ChengJiuTScl.enabled = false;

		ParticleObj.SetActive(false);
		gameObject.SetActive(false);
	}

	public void InitShowChengJiu()
	{
		if(gameObject.activeSelf)
		{
			return;
		}
		gameObject.SetActive(true);
		ParticleObj.SetActive(true);

		Invoke("ShowChengJiu", 1.0f);
	}

	string GetChengJiuSpriteName()
	{
		int sheJiStarCount = SheJiLevelCtrl.SheJiStarCount;
		int jiaShiStarCount = JiaShiLevelCtrl.JiaShiStarCount;
		string nameStr = "ChengJiu_1";
		if (sheJiStarCount == 1 && jiaShiStarCount == 1) {
			nameStr = "ChengJiu_1";
		}
		else if (sheJiStarCount == 2 && jiaShiStarCount == 1) {
			nameStr = "ChengJiu_2";
		}
		else if (sheJiStarCount == 1 && jiaShiStarCount == 2) {
			nameStr = "ChengJiu_3";
		}
		else if (sheJiStarCount == 3 && jiaShiStarCount == 1) {
			nameStr = "ChengJiu_4";
		}
		else if (sheJiStarCount == 1 && jiaShiStarCount == 3) {
			nameStr = "ChengJiu_5";
		}
		else if (sheJiStarCount == 3 && jiaShiStarCount == 3) {
			nameStr = "ChengJiu_7";
		}
		else if (sheJiStarCount >= 2 && jiaShiStarCount >= 2) {
			nameStr = "ChengJiu_6";
		}
		return nameStr;
	}

	void ShowChengJiu()
	{
		EventDelegate.Add(ChengJiuTScl.onFinished, delegate {
			BackMovieScene();
		});

		ChengJiuSprite.spriteName = GetChengJiuSpriteName();
		ChengJiuSprite.enabled = true;

		ChengJiuTScl.ResetToBeginning();
		ChengJiuTScl.enabled = true;
		ChengJiuTScl.PlayForward();
		AudioManager.Instance.PlaySFX(AudioListCtrl.GetInstance().AudioChengJiu);
	}

	void BackMovieScene()
	{
		FinishPanelCtrl.GetInstance().Invoke("InitHiddenFinishPanel", 2.0f);
	}
}
