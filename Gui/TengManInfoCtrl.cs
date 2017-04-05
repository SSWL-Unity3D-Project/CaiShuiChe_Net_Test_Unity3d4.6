using UnityEngine;
using System.Collections;

public class TengManInfoCtrl : MonoBehaviour
{
	bool IsActiveTengManInfo;

	public static TengManInfoCtrl _Instance;
	public static TengManInfoCtrl GetInstance()
	{
		return _Instance;
	}

	// Use this for initialization
	void Start ()
	{
		_Instance = this;
		gameObject.SetActive(false);
	}

	public void ShowTengManInfo()
	{
		IsActiveTengManInfo = true;
		if(!gameObject.activeSelf)
		{
			gameObject.SetActive(true);
			CancelInvoke("HiddenTengManInfo");
			Invoke("HiddenTengManInfo", 3f);
		}
		else
		{
			CancelInvoke("HiddenTengManInfo");
			Invoke("HiddenTengManInfo", 3f);
		}
	}

	void HiddenTengManInfo()
	{
		if(gameObject.activeSelf)
		{
			IsActiveTengManInfo = false;
			gameObject.SetActive(false);
		}
	}

	public bool GetIsActiveTengManInfo()
	{
		return IsActiveTengManInfo;
	}
}

