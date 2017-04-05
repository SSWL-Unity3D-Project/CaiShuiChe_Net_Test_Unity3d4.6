using UnityEngine;
using System.Collections;

public class DongGanUICtrl : MonoBehaviour {
	public Texture[] DongGanUI;
	UITexture DongGanTexture;
	public static DongGanUICtrl Instance;
	// Use this for initialization
	void Start()
	{
		Instance = this;
		DongGanTexture = GetComponent<UITexture>();
		DongGanTexture.mainTexture = DongGanUI[0];
		gameObject.SetActive(false);
	}

	public void ShowDongGanUI(int index)
	{
		DongGanTexture.mainTexture = DongGanUI[index];
		gameObject.SetActive(true);

		if (index == 1) {
			Invoke("HiddenDongGanUI", 3f);
		}
	}

	void HiddenDongGanUI()
	{
		gameObject.SetActive(false);
	}
}