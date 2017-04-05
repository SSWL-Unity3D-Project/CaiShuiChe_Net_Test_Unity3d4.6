using UnityEngine;
using System.Collections;

public class LoadingCtrl : MonoBehaviour {

	public GameObject[] ActiveObjArray;
	public GameObject[] LoadingObjArray;
	static LoadingCtrl _Instance;

	int count = 0;
	float TimeLast;
	bool IsInitActive;
	GameObject LoadCtrlObj;

	public static LoadingCtrl GetInstance()
	{
		return _Instance;
	}

	void Start()
	{
		_Instance = this;
		LoadCtrlObj = gameObject;
		LoadCtrlObj.SetActive(false);
	}

	public void InitActiveAllObj()
	{
		if (IsInitActive) {
			return;
		}
		IsInitActive = true;
		for (int i = 0; i < LoadingObjArray.Length; i++) {
			LoadingObjArray[i].SetActive(true);
		}
		LoadCtrlObj.SetActive(true);

		TimeLast = Time.realtimeSinceStartup;
		InvokeRepeating("ActiveLoadingObj", 0f, 1f);
	}

	void ActiveLoadingObj()
	{
		if (Time.realtimeSinceStartup - TimeLast < 1.5f) {
			return;
		}
		TimeLast = Time.realtimeSinceStartup;
		TweenAlpha twAlpha = ActiveObjArray[count].GetComponent<TweenAlpha>();
		twAlpha.enabled = true;

		count++;
		if (count >= ActiveObjArray.Length) {
			CancelInvoke("ActiveLoadingObj");
		}
	}
}
