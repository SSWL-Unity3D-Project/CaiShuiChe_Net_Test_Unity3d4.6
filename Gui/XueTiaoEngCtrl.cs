using UnityEngine;
using System.Collections;

public class XueTiaoEngCtrl : MonoBehaviour {
	
	UISprite XueTiaoEngSprite;
	public static XueTiaoEngCtrl _Instance;
	public static XueTiaoEngCtrl GetInstance()
	{
		return _Instance;
	}

	// Use this for initialization
	void Awake () {
		_Instance = this;
		XueTiaoEngSprite = GetComponent<UISprite>();
	}

	public UISprite GetXueTiaoEngSprite()
	{
		return XueTiaoEngSprite;
	}
}
