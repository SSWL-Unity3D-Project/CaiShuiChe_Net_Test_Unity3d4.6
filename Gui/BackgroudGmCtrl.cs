using UnityEngine;
using System.Collections;

public class BackgroudGmCtrl : MonoBehaviour {

	public GameObject LoadingObj;
	UISprite BgSprite;
	static BackgroudGmCtrl _Instance;
	public static BackgroudGmCtrl GetInstance()
	{
		return _Instance;
	}

	// Use this for initialization
	void Awake () {
		_Instance = this;
		BgSprite = GetComponent<UISprite>();
		BgSprite.enabled = true;
	}

	public void CloseBackgroudImg()
	{
		gameObject.SetActive(false);
		if (LoadingObj != null) {
			LoadingObj.SetActive(false);
		}
	}

	public void OpenBackgroudImg()
	{
		BgSprite.fillAmount = 0f;
		gameObject.SetActive(true);
		StartCoroutine(SetBgSpriteInfo());
	}

	IEnumerator SetBgSpriteInfo()
	{
		while (BgSprite.fillAmount < 1.0f) {
			BgSprite.fillAmount += 0.03f;
			yield return new WaitForSeconds(0.02f);
		}
	}
}
