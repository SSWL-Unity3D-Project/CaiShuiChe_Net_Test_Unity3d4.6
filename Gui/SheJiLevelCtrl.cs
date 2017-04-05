using UnityEngine;
using System.Collections;
using Frederick.ProjectAircraft;

public class SheJiLevelCtrl : MonoBehaviour {
	public TweenPosition StarTPos_1;
	public TweenPosition StarTPos_2;
	public TweenPosition StarTPos_3;
	TweenScale StarTScl_1;
	TweenScale StarTScl_2;
	TweenScale StarTScl_3;

	TweenPosition SheJiLevelTPos;

	public static int SheJiStarCount;
	public static SheJiLevelCtrl _Instance;
	public static SheJiLevelCtrl GetInstance()
	{
		return _Instance;
	}

	// Use this for initialization
	void Awake()
	{
		_Instance = this;
		SheJiStarCount = 0;
		SheJiLevelTPos = GetComponent<TweenPosition>();
		
		gameObject.SetActive(false);
		SheJiLevelTPos.enabled = false;
		ResetSheJiStar();
	}
	
	public void ShowSheJiLevel()
	{
		if(gameObject.activeSelf)
		{
			return;
		}
		gameObject.SetActive(true);
		AudioManager.Instance.PlaySFX(AudioListCtrl.GetInstance().AudioSheJiLevel);
		
		SheJiStarCount = PlayerAutoFire.GetPlayerShootLevelStar();
		//SheJiStarCount = 2; //test
		if(SheJiStarCount > 0)
		{
			//Shoe SheJi Star
			EventDelegate.Add(SheJiLevelTPos.onFinished, delegate {
				ShowSheJiStar_1();
			});
		}
		else
		{
			//Show SheJiLevel
			EventDelegate.Add(SheJiLevelTPos.onFinished, delegate {
				ActiveChengJiu();
			});
		}
		
		SheJiLevelTPos.ResetToBeginning();
		SheJiLevelTPos.enabled = true;
	}
	
	void ResetSheJiStar()
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

	void ShowSheJiStar_1()
	{
		if(SheJiStarCount > 1)
		{
			//Shoe SheJi Star
			EventDelegate.Add(StarTPos_1.onFinished, delegate {
				ShowSheJiStar_2();
			});
		}
		else
		{
			//Show SheJiLevel
			EventDelegate.Add(StarTPos_1.onFinished, delegate {
				ActiveChengJiu();
			});
		}
		StarTPos_1.gameObject.SetActive(true);
		StarTPos_1.PlayForward();
		StarTScl_1.PlayForward();
		AudioManager.Instance.PlaySFX(AudioListCtrl.GetInstance().AudioSheJiLevel);
	}
	
	void ShowSheJiStar_2()
	{
		if(SheJiStarCount > 2)
		{
			//Shoe SheJi Star
			EventDelegate.Add(StarTPos_2.onFinished, delegate {
				ShowSheJiStar_3();
			});
		}
		else
		{
			//Show SheJiLevel
			EventDelegate.Add(StarTPos_2.onFinished, delegate {
				ActiveChengJiu();
			});
		}
		StarTPos_2.gameObject.SetActive(true);
		StarTPos_2.PlayForward();
		StarTScl_2.PlayForward();
		AudioManager.Instance.PlaySFX(AudioListCtrl.GetInstance().AudioSheJiLevel);
	}
	
	void ShowSheJiStar_3()
	{
		//Show SheJiLevel
		EventDelegate.Add(StarTPos_3.onFinished, delegate {
			ActiveChengJiu();
		});
		
		StarTPos_3.gameObject.SetActive(true);
		StarTPos_3.PlayForward();
		StarTScl_3.PlayForward();
		AudioManager.Instance.PlaySFX(AudioListCtrl.GetInstance().AudioSheJiLevel);
	}

	void ActiveChengJiu()
	{
		ChengJiuCtrl.GetInstance().InitShowChengJiu();
	}
}
