using UnityEngine;
using System.Collections;

public class GameDaoJiShiCtrl : MonoBehaviour {
	public UISprite TimeSprite;
	public TweenAlpha TimeTWAlpha;

	GameObject TimeObj;
	static GameDaoJiShiCtrl _Instance;
	public static GameDaoJiShiCtrl GetInstance()
	{
		return _Instance;
	}

	// Use this for initialization
	void Start()
	{
		_Instance = this;
		TimeObj = gameObject;
		TimeObj.SetActive(false);
		TimeTWAlpha.enabled = false;
		SetDaoJiShiNum(9);
	}

	public void SetDaoJiShiNum(int num)
	{
		TimeSprite.spriteName = "timeSub_" + num;
		if (num <= 0) {
			StopDaoJiShi();
		}
	}

	public void StartPlayDaoJiShi()
	{
		if (TimeObj.activeSelf) {
			return;
		}
		TimeObj.SetActive(true);

		TimeTWAlpha.ResetToBeginning();
		TimeTWAlpha.enabled = true;
		TimeTWAlpha.PlayForward();
	}

	public void StopDaoJiShi()
	{
		if (!TimeObj.activeSelf) {
			return;
		}
		TimeObj.SetActive(false);
		TimeTWAlpha.enabled = false;
		SetDaoJiShiNum(9);
	}
}
