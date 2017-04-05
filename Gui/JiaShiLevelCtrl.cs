using UnityEngine;
using System.Collections;
using Frederick.ProjectAircraft;

public class JiaShiLevelCtrl : MonoBehaviour {
	public TweenPosition StarTPos_1;
	public TweenPosition StarTPos_2;
	public TweenPosition StarTPos_3;
	TweenScale StarTScl_1;
	TweenScale StarTScl_2;
	TweenScale StarTScl_3;

	public static int JiaShiStarCount;
	TweenPosition JiShiLevelTPos;

	public static JiaShiLevelCtrl _Instance;
	public static JiaShiLevelCtrl GetInstance()
	{
		return _Instance;
	}

	// Use this for initialization
	void Awake()
	{
		_Instance = this;
		JiaShiStarCount = 0;
		JiShiLevelTPos = GetComponent<TweenPosition>();

		gameObject.SetActive(false);
		JiShiLevelTPos.enabled = false;
		ResetJiaShiStar();
	}

	public void ShowJiaShiLevel()
	{
		if(gameObject.activeSelf)
		{
			return;
		}
		gameObject.SetActive(true);
		AudioManager.Instance.PlaySFX(AudioListCtrl.GetInstance().AudioJiaShiLevel);
		
		JiaShiStarCount = PlayerAutoFire.GetPlayerJiaShiLevelStar();
		//JiaShiStarCount = 2; //test
		if(JiaShiStarCount > 0)
		{
			//Shoe JiaShi Star
			EventDelegate.Add(JiShiLevelTPos.onFinished, delegate {
				ShowJiaShiStar_1();
			});
		}
		else
		{
			//Show SheJiLevel
			EventDelegate.Add(JiShiLevelTPos.onFinished, delegate {
				ActiveSheJiLevel();
			});
		}

		JiShiLevelTPos.ResetToBeginning();
		JiShiLevelTPos.enabled = true;
	}

	void ResetJiaShiStar()
	{
		StarTPos_1.gameObject.SetActive(false);
		StarTPos_2.gameObject.SetActive(false);
		StarTPos_3.gameObject.SetActive(false);

		StarTPos_1.ResetToBeginning();
		StarTPos_2.ResetToBeginning();
		StarTPos_3.ResetToBeginning();

		StarTScl_1 = StarTPos_1.GetComponent<TweenScale>();
		StarTScl_2 = StarTPos_2.GetComponent<TweenScale>();
		StarTScl_3 = StarTPos_3.GetComponent<TweenScale>();
		StarTScl_1.ResetToBeginning();
		StarTScl_2.ResetToBeginning();
		StarTScl_3.ResetToBeginning();
	}

	void ShowJiaShiStar_1()
	{
		if(JiaShiStarCount > 1)
		{
			//Shoe JiaShi Star
			EventDelegate.Add(StarTPos_1.onFinished, delegate {
				ShowJiaShiStar_2();
			});
		}
		else
		{
			//Show SheJiLevel
			EventDelegate.Add(StarTPos_1.onFinished, delegate {
				ActiveSheJiLevel();
			});
		}
		StarTPos_1.gameObject.SetActive(true);
		StarTPos_1.PlayForward();
		StarTScl_1.PlayForward();
		AudioManager.Instance.PlaySFX(AudioListCtrl.GetInstance().AudioJiaShiLevel);
	}

	void ShowJiaShiStar_2()
	{
		if(JiaShiStarCount > 2)
		{
			//Shoe JiaShi Star
			EventDelegate.Add(StarTPos_2.onFinished, delegate {
				ShowJiaShiStar_3();
			});
		}
		else
		{
			//Show SheJiLevel
			EventDelegate.Add(StarTPos_2.onFinished, delegate {
				ActiveSheJiLevel();
			});
		}
		StarTPos_2.gameObject.SetActive(true);
		StarTPos_2.PlayForward();
		StarTScl_2.PlayForward();
		AudioManager.Instance.PlaySFX(AudioListCtrl.GetInstance().AudioJiaShiLevel);
	}

	void ShowJiaShiStar_3()
	{
		//Show SheJiLevel
		EventDelegate.Add(StarTPos_3.onFinished, delegate {
			ActiveSheJiLevel();
		});

		StarTPos_3.gameObject.SetActive(true);
		StarTPos_3.PlayForward();
		StarTScl_3.PlayForward();
		AudioManager.Instance.PlaySFX(AudioListCtrl.GetInstance().AudioJiaShiLevel);
	}

	void ActiveSheJiLevel()
	{
		SheJiLevelCtrl.GetInstance().Invoke("ShowSheJiLevel", 1.0f);
	}
}
