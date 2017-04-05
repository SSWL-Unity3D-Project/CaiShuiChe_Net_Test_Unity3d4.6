using UnityEngine;
using System.Collections;

public class XueDiTiShiCtrl : MonoBehaviour {
	GameObject TiShiObj;
	static XueDiTiShiCtrl _Instance;
	public static XueDiTiShiCtrl GetInstance()
	{
		return _Instance;
	}

	// Use this for initialization
	void Start()
	{
		_Instance = this;
		TiShiObj = gameObject;
		HiddenXueDiTiShi();

		UISprite infoSprite = GetComponent<UISprite>();
		infoSprite.enabled = true;
	}

	public void ShowXueDiTiShi()
	{
		if (TiShiObj.activeSelf) {
			return;
		}
		TiShiObj.SetActive(true);
		Invoke("HiddenXueDiTiShi", 4f);
	}

	void HiddenXueDiTiShi()
	{
		TiShiObj.SetActive(false);
	}
}
