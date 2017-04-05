using UnityEngine;
using System.Collections;

public class ShenXingInfoCtrl : MonoBehaviour {

	UISprite ShenXingSprite;

	public static ShenXingInfoCtrl _Instance;
	public static ShenXingInfoCtrl GetInstance()
	{
		return _Instance;
	}

	// Use this for initialization
	void Start()
	{
		_Instance = this;
		ShenXingSprite = GetComponent<UISprite>();
		ShenXingSprite.enabled = false;
		gameObject.SetActive(false);
	}

	public void ShowShenXingInfo()
	{
		if(gameObject.activeSelf)
		{
			return;
		}
		ShenXingSprite.enabled = true;
		gameObject.SetActive(true);
	}

	public void HiddenShenXingInfo()
	{
		if(!gameObject.activeSelf)
		{
			return;
		}
		ShenXingSprite.enabled = false;
		gameObject.SetActive(false);
	}
}
