using UnityEngine;
using System.Collections;

public class StartGameBtCtrl : MonoBehaviour {

	UISprite BtSprite;
	static StartGameBtCtrl _Instance;
	public static StartGameBtCtrl GetInstance()
	{
		return _Instance;
	}

	// Use this for initialization
	void Awake()
	{
		_Instance = this;
		BtSprite = GetComponent<UISprite>();
		InitStartBtCartoon();
	}
	
	void InitStartBtCartoon()
	{
		if(IsInvoking("PlayCartoon"))
		{
			return;
		}
		BtSprite.enabled = true;
		pcvr.StartLightStateP1 = LedState.Shan;
		InvokeRepeating("PlayCartoon", 0f, 0.5f);
	}
	
	void PlayCartoon()
	{
		switch(BtSprite.spriteName)
		{
		case "StartGameBt_1":
			BtSprite.spriteName = "StartGameBt_2";
			break;
			
		case "StartGameBt_2":
			BtSprite.spriteName = "StartGameBt_1";
			break;
		}
	}

	public void CloseStartBtCartoon()
	{
		CancelInvoke("PlayCartoon");
		BtSprite.enabled = false;
		gameObject.SetActive(false);
		pcvr.StartLightStateP1 = LedState.Mie;
	}
}
