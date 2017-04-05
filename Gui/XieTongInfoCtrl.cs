using UnityEngine;
using System.Collections;

public class XieTongInfoCtrl : MonoBehaviour {
	
	UISprite XieTongSprite;
	
	public static XieTongInfoCtrl _Instance;
	public static XieTongInfoCtrl GetInstance()
	{
		return _Instance;
	}
	
	// Use this for initialization
	void Start()
	{
		_Instance = this;
		XieTongSprite = GetComponent<UISprite>();
		XieTongSprite.enabled = false;
		gameObject.SetActive(false);
	}
	
	public void ShowXieTongInfo()
	{
		if(gameObject.activeSelf)
		{
			return;
		}
		XieTongSprite.enabled = true;
		gameObject.SetActive(true);
	}
	
	public void HiddenXieTongInfo()
	{
		if(!gameObject.activeSelf)
		{
			return;
		}
		XieTongSprite.enabled = false;
		gameObject.SetActive(false);
	}
}
