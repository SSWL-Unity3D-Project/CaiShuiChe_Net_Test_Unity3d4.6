using UnityEngine;
using System.Collections;
//using System.Collections.Generic;

public class LinkPlayerCtrl : MonoBehaviour {
	
	public UISprite WaitSprite;
	public UISprite [] LinkPlayerSprite;

	static LinkPlayerCtrl _Instance;
	public static LinkPlayerCtrl GetInstance()
	{
		return _Instance;
	}

	// Use this for initialization
	void Start () {
		_Instance = this;
		InitLinkPlayer();
	}

	void InitLinkPlayer()
	{
		int j = 0;
		for (int i = 0; i < RankingCtrl.MaxPlayerRankNum; i++) {
			j = i + 1;
			LinkPlayerSprite[i].spriteName = "WPlayer_" + j;
		}

		j = 1;
		for (int i = RankingCtrl.MaxPlayerRankNum - 1; i > RankingCtrl.ServerPlayerRankNum - 1; i--) {
			LinkPlayerSprite[i].spriteName = "WAiPlayer_" + j;
			j++;
		}
	}

	public void DisplayLinkPlayerName(int val)
	{
		int tmpVal = 0;
		if (val > RankingCtrl.ServerPlayerRankNum) {
			for (int i = RankingCtrl.ServerPlayerRankNum; i < val; i++) {
				tmpVal = i + 1;
				LinkPlayerSprite[i].spriteName = "WPlayer_" + tmpVal;
			}
		}

		tmpVal = val + 1;
		LinkPlayerSprite[val].spriteName = "WPlayer_" + tmpVal;
	}

	public UISprite GetWaitPlayerSprite()
	{
		return WaitSprite;
	}
}
