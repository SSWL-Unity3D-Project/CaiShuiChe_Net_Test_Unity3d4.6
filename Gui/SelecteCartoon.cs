using UnityEngine;
using System.Collections;

public class SelecteCartoon : MonoBehaviour {

	//public static bool IsStopCartoon;
	
	UISprite selecteSprite;
	public UISprite SelecteSprite;

	int count = 0;
	bool IsCanSelect;

	public static SelecteCartoon _Instance;
	public static SelecteCartoon GetInstance()
	{
		return _Instance;
	}

	void Awake()
	{
		_Instance = this;
		gameObject.SetActive(false);

		selecteSprite = GetComponent<UISprite>();
		selecteSprite.enabled = false;
	}
	
	public void InitSelecteCartoon()
	{
		if(IsInvoking("PlayCartoon"))
		{
			return;
		}
		//IsStopCartoon = false;
		gameObject.SetActive(true);
		selecteSprite.enabled = true;
		InvokeRepeating("PlayCartoon", 0.0f, 0.5f);
	}

	public bool CheckIsPlaySelect()
	{
		if(IsInvoking("PlayCartoon"))
		{
			return true;
		}
		return IsCanSelect;
	}

	public void StopCartoon()
	{
		if(!IsInvoking("PlayCartoon"))
		{
			return;
		}
		CancelInvoke("PlayCartoon");
		gameObject.SetActive(false);
		IsCanSelect = true;
	}
	
	void PlayCartoon()
	{
//		if(IsStopCartoon)
//		{
//			CancelInvoke("PlayCartoon");
//			gameObject.SetActive(false);
//			return;
//		}

		count++;
		if(count >= 5)
		{
			count = 1;
		}
		SelecteSprite.spriteName = count.ToString();
	}
}
