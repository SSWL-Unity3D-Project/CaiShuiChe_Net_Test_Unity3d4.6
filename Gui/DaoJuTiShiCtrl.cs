using UnityEngine;
using System.Collections;

public class DaoJuTiShiCtrl : MonoBehaviour {

	UISprite TiShiSprite;
	static DaoJuTiShiCtrl _Instance;
	public static DaoJuTiShiCtrl GetInstance()
	{
		return _Instance;
	}

	// Use this for initialization
	void Start()
	{
		TiShiSprite = GetComponent<UISprite>();
		_Instance = this;
		gameObject.SetActive(false);
	}

	public void ShowDaoJuTiShi(DaoJuState val)
	{
		switch (val) {
		case DaoJuState.DingShenFu:
			TiShiSprite.spriteName = "DingShenFu";
			break;

		case DaoJuState.HuanYingFu:
			TiShiSprite.spriteName = "HuanYingFu";
			break;
			
		case DaoJuState.JuLiFu:
			TiShiSprite.spriteName = "JuLiFu";
			break;
		
		default:
			return;
		}
		
		gameObject.SetActive(true);
		CancelInvoke("HiddenDaoJuTiShi");
		Invoke("HiddenDaoJuTiShi", 2f);
	}

	void HiddenDaoJuTiShi()
	{
		gameObject.SetActive(false);
	}
}
