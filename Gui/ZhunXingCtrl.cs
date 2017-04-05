using UnityEngine;
using System.Collections;

public class ZhunXingCtrl : MonoBehaviour {

	bool IsFixZhunXing;
	public UISprite ZhunXingSprite;
	public UISprite ZhunXingJuLiFuSprite;

	public static ZhunXingCtrl _Instance;
	public static ZhunXingCtrl GetInstance()
	{
		return _Instance;
	}

	// Use this for initialization
	void Start()
	{
		_Instance = this;
		//ZhunXingSprite = GetComponent<UISprite>();
		ZhunXingSprite.enabled = false;
		ZhunXingJuLiFuSprite.enabled = false;
		gameObject.SetActive(false);

		IsFixZhunXing = !Screen.fullScreen;
		pcvr.GetInstance().OnUpdateCrossEvent += OnUpdateCrossEvent;
	}

	// Update is called once per frame
	void Update()
	{
		if (ZhunXingSprite.enabled || ZhunXingJuLiFuSprite.enabled)
		{
			CheckZhunXingImg();

			if(IsFixZhunXing != Screen.fullScreen)
			{
				IsFixZhunXing = Screen.fullScreen;
				int iScreenW = FreeModeCtrl.GetSystemMetrics(FreeModeCtrl.SM_CXSCREEN);
				int iScreenH = FreeModeCtrl.GetSystemMetrics(FreeModeCtrl.SM_CYSCREEN);
				if(!Screen.fullScreen)
				{
					iScreenW = Screen.width;
					iScreenH = Screen.height;
				}

				float sx = (1360f *(float)iScreenH) / (768f * (float)iScreenW);
				//ScreenLog.Log("sx **** " + sx + ", Screen: " + iScreenW + ", " + iScreenH );
				transform.localScale = new Vector3(sx, transform.localScale.y, transform.localScale.z);
			}
			transform.localPosition = pcvr.CrossPosition;
		}
	}
	
	void OnUpdateCrossEvent()
	{
		if (Application.loadedLevel == (int)GameLeve.Movie
		    || Application.loadedLevel == (int)GameLeve.SetPanel) {
			pcvr.GetInstance().OnUpdateCrossEvent -= OnUpdateCrossEvent;
			return;
		}

		if (ZhunXingSprite.enabled || ZhunXingJuLiFuSprite.enabled) {
			transform.localPosition = pcvr.CrossPosition;
		}
	}

	void CheckZhunXingImg()
	{
		if (!GlobalData.GetInstance().IsActiveJuLiFu) {
			ZhunXingSprite.enabled = true;
			ZhunXingJuLiFuSprite.enabled = false;
		}
		else {
			ZhunXingSprite.enabled = false;
			ZhunXingJuLiFuSprite.enabled = true;
		}
	}

	public void ShowPlayerZhunXing()
	{
		if (pcvr.bIsHardWare && !pcvr.IsGetValByKey) {
			return;
		}

		if(gameObject.activeSelf) {
			return;
		}

		CheckZhunXingImg();
		ZhunXingSprite.enabled = true;
		gameObject.SetActive(true);
	}

	public void ClosePlayerZhunXing()
	{
		if(!gameObject.activeSelf)
		{
			return;
		}
		
		ZhunXingSprite.enabled = false;
		gameObject.SetActive(false);
	}
}
