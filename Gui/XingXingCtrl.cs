using UnityEngine;
using System.Collections;

public class XingXingCtrl : MonoBehaviour
{
	int StarNum;
	public UISprite XingNLSprite;

	Transform TranA;
	Transform TranB;
	public TweenTransform ChuanTweenTranA;
	GameObject ChuanUiGX;
	
	bool IsActiveChuan;
	public static bool IsPlayerCanHitNPC = true;

	public static XingXingCtrl _Instance;
	public static XingXingCtrl GetInstance()
	{
		return _Instance;
	}

	// Use this for initialization
	void Awake()
	{
		_Instance = this;
		IsPlayerCanHitNPC = true;
		TranA = ChuanTweenTranA.from;
		TranB = ChuanTweenTranA.to;
		ChuanUiGX = ChuanTweenTranA.gameObject;
		ChuanUiGX.SetActive(false);

		EventDelegate.Add(ChuanTweenTranA.onFinished, delegate{
			BackChuanChangeTran();
		});
		EventDelegate.Add(ChuanTweenTranA.onFinished, delegate{
			ResetIsActiveChuanChangeTran();
		});
	}

	public void AddStarNum()
	{
		if (NengLiangTiaoCtrl.IsStartPlay || XingNLSprite.fillAmount >= 1f) {
			return;
		}

		//StarNum += 8; //test
		StarNum++;
		XingNLSprite.fillAmount = (float)StarNum / 20f;
		if (StarNum >= 20) {
			StarNum = 0;
			//XingNLSprite.fillAmount = 0f;
			if (GlobalData.GetInstance().gameMode == GameMode.SoloMode) {
				NengLiangLiZiTXCtrl.GetInstance().MoveLiZiToPathEnd();
			}
		}
		ActiveChuanChangeTran();
	}

	public void ResetNengLiangInfo()
	{
		XingNLSprite.fillAmount = 0f;
	}
	
	public void SetXingXingIsActive(bool isActive)
	{
		if (gameObject.activeSelf != isActive) {
			gameObject.SetActive(isActive);
		}
	}

	void ActiveChuanChangeTran()
	{
		if (IsActiveChuan) {
			return;
		}
		//Debug.Log("ActiveChuanChangeTran***");
		IsActiveChuan = true;
		ChuanUiGX.SetActive(true);
		ChuanTweenTranA.from = TranA;
		ChuanTweenTranA.to = TranB;
		ChuanTweenTranA.ResetToBeginning();
		ChuanTweenTranA.enabled = true;
		ChuanTweenTranA.PlayForward();
	}

	void BackChuanChangeTran()
	{
		//Debug.Log("BackChuanChangeTran***");
		ChuanTweenTranA.from = TranB;
		ChuanTweenTranA.to = TranA;
		ChuanTweenTranA.PlayForward();
	}

	void ResetIsActiveChuanChangeTran()
	{
		//Debug.Log("ResetIsActiveChuanChangeTran***");
		IsActiveChuan = false;
		ChuanUiGX.SetActive(false);
	}
}

