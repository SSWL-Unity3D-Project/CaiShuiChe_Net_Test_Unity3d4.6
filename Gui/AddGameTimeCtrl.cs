using UnityEngine;
using System.Collections;

public class AddGameTimeCtrl : MonoBehaviour {

	public UISprite TimeSprite_0;
	public UISprite TimeSprite_1;
	public UISprite TimeSprite_2;
	public UISprite TimeSprite_3;

	int AddTimeVal;
	GameObject AddTimeObj;
	TweenScale TweenSclScript;
	TweenTransform TweenTranScript;

	static AddGameTimeCtrl _Instance;
	public static AddGameTimeCtrl GetInstance()
	{
		return _Instance;
	}

	// Use this for initialization
	void Start()
	{
		_Instance = this;
		TweenTranScript = GetComponent<TweenTransform>();
		TweenTranScript.enabled = false;
		EventDelegate.Add(TweenTranScript.onFinished, delegate{
			StartAddGameTime();
		});

		TweenSclScript = GetComponent<TweenScale>();
		TweenSclScript.enabled = false;
		EventDelegate.Add(TweenSclScript.onFinished, delegate{
			Invoke("DelayPlayTimeTweenTran", 0.2f);
		});

		AddTimeObj = gameObject;
		AddTimeObj.SetActive(false);
	}
	
	void SetTimeImg(int timeVal)
	{
		if (timeVal < 1) {
			Debug.LogError("SetTimeImg -> timeVal was wrong! timeVal = " + timeVal);
			timeVal = 10;
		}
		
		if (timeVal > 999) {
			Debug.LogError("SetTimeImg -> timeVal was wrong! timeVal = " + timeVal);
			timeVal = 999;
		}
		
		int miao_0 = timeVal / 100;
		int miao_1 = (timeVal - (miao_0 * 100)) / 10;
		int miao_2 = timeVal - (miao_0 * 100) - (miao_1 * 10);
		miao_0 = miao_0 > 10 ? 9 : miao_0;
		miao_0 = miao_0 == 0 ? 10 : miao_0;
		if (miao_0 == 10) {
			miao_1 = miao_1 == 0 ? 10 : miao_1;
		}
		
		TimeSprite_0.spriteName = "timeAdd_10";
		TimeSprite_1.spriteName = "timeAdd_" + miao_0;
		TimeSprite_2.spriteName = "timeAdd_" + miao_1;
		TimeSprite_3.spriteName = "timeAdd_" + miao_2;
		if (timeVal > 99) {
			TimeSprite_0.enabled = true;
			TimeSprite_1.enabled = true;
			TimeSprite_2.enabled = true;
			TimeSprite_3.enabled = true;
		}
		else if (timeVal > 9) {
			TimeSprite_0.enabled = false;
			TimeSprite_1.enabled = true;
			TimeSprite_2.enabled = true;
			TimeSprite_3.enabled = true;
		}
		else {
			TimeSprite_0.enabled = false;
			TimeSprite_1.enabled = false;
			TimeSprite_2.enabled = true;
			TimeSprite_3.enabled = true;
		}
	}

	public bool GetIsActiveAddTime()
	{
		return AddTimeObj.activeSelf;
	}

	public void PlayAddGameTime(int timeVal)
	{
		if (timeVal <= 0) {
			Debug.LogError("PlayAddGameTime -> timeVal was wrong! timeVal = " + timeVal);
			return;
		}

		if (AddTimeObj.activeSelf) {
			return;
		}

		TweenTranScript.ResetToBeginning();
		AddTimeObj.SetActive(true);

		AddTimeVal = timeVal;
		SetTimeImg(timeVal);
		//Debug.LogWarning("*****************StartAddGameTime " + timeVal);

		TweenSclScript.ResetToBeginning();
		TweenSclScript.enabled = true;
		TweenSclScript.PlayForward();
	}

	void DelayPlayTimeTweenTran()
	{
		TweenTranScript.enabled = true;
		TweenTranScript.PlayForward();
	}

	void StartAddGameTime()
	{
		//Debug.Log("*****************StartAddGameTime " + AddTimeVal);
		if (!AddTimeObj.activeSelf) {
			return;
		}
		AddTimeObj.SetActive(false);
		GameTimeCtrl.GetInstance().AddGameTime(AddTimeVal);
	}
}
