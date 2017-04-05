using UnityEngine;
using System.Collections;

public class PlayerRankNumCtrl : MonoBehaviour {

	GameObject RankNumObj;
	UISprite RankNumSprite;
	static PlayerRankNumCtrl _Instance;
	public static PlayerRankNumCtrl GetInstance()
	{
		return _Instance;
	}

	// Use this for initialization
	void Start()
	{
		_Instance = this;
		RankNumSprite = GetComponent<UISprite>();

		RankNumObj = gameObject;
		gameObject.SetActive(false);
	}

	public void ShowPlayerRankNum(int num)
	{
		if (num < 1 || num > 8) {
			return;
		}
		RankNumSprite.spriteName = "No" + num;
		RankNumObj.SetActive(true);
	}
}
