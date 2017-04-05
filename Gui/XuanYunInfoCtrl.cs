using UnityEngine;
using System.Collections;

public class XuanYunInfoCtrl : MonoBehaviour {

	UISprite XuanYunSprite;

	static XuanYunInfoCtrl _Instance;
	public static XuanYunInfoCtrl GetInstance()
	{
		return _Instance;
	}

	// Use this for initialization
	void Start () {
		_Instance = this;
		XuanYunSprite = GetComponent<UISprite>();
		XuanYunSprite.enabled = false;
		gameObject.SetActive(false);
	}

	public void ShowXuanYunState()
	{
		if (gameObject.activeSelf) {
			return;
		}
		gameObject.SetActive(true);
		XuanYunSprite.enabled = true;

		CancelInvoke("ClosePlayerXuanYunState");
		Invoke("ClosePlayerXuanYunState", 3f);
	}

	void ClosePlayerXuanYunState()
	{
		HiddenXuanYunState();
		WaterwheelPlayerNetCtrl.GetInstance().CloseXuanYunState();
	}

	public void HiddenXuanYunState()
	{
		if (!gameObject.activeSelf) {
			return;
		}
		gameObject.SetActive(false);
		XuanYunSprite.enabled = false;
	}
}
