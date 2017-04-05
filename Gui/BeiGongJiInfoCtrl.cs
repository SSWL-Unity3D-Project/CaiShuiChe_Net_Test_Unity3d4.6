using UnityEngine;
using System.Collections;

public class BeiGongJiInfoCtrl : MonoBehaviour {

	AudioSource AudioBeiGongJi;
	UISprite BeiGongJiSprite;
	
	public static BeiGongJiInfoCtrl _Instance;
	public static BeiGongJiInfoCtrl GetInstance()
	{
		return _Instance;
	}
	
	// Use this for initialization
	void Start()
	{
		_Instance = this;
		BeiGongJiSprite = GetComponent<UISprite>();
		BeiGongJiSprite.enabled = false;
		AudioBeiGongJi = audio;
		gameObject.SetActive(false);
	}
	
	public void ShowGongJiInfo()
	{
		if (gameObject.activeSelf) {
			return;
		}

		BeiGongJiSprite.enabled = true;
		gameObject.SetActive(true);
		if (AudioBeiGongJi != null) {
			AudioBeiGongJi.Play();
		}
	}
	
	public void HiddenGongJiInfo()
	{
		if (!gameObject.activeSelf) {
			return;
		}

		BeiGongJiSprite.enabled = false;
		gameObject.SetActive(false);
		if (AudioBeiGongJi != null) {
			AudioBeiGongJi.Stop();
		}
	}
}
