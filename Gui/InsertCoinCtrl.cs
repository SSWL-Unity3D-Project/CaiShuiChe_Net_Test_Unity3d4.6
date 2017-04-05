using UnityEngine;
using System.Collections;

public class InsertCoinCtrl : MonoBehaviour {
	public PlayerBtState PlayerState;

	UISprite InsertCoinSprite;

	public static InsertCoinCtrl _InstanceP1;
	public static InsertCoinCtrl _InstanceP2;
	public static InsertCoinCtrl GetInstanceP1()
	{
		return _InstanceP1;
	}
	
	public static InsertCoinCtrl GetInstanceP2()
	{
		return _InstanceP2;
	}

	// Use this for initialization
	void Awake()
	{
		InsertCoinSprite = GetComponent<UISprite>();
		switch(PlayerState)
		{
		case PlayerBtState.PLAYER_1:
			_InstanceP1 = this;
			break;

		case PlayerBtState.PLAYER_2:
			_InstanceP2 = this;
			break;
		}
		HiddenInsertCoin();
	}

	public bool CheckIsActiveObj()
	{
		return gameObject.activeSelf;
	}

	public void ShowInsertCoin()
	{
		if(gameObject.activeSelf)
		{
			return;
		}
		gameObject.SetActive(true);
		InsertCoinSprite.enabled = true;
	}

	public void HiddenInsertCoin()
	{
		if(!gameObject.activeSelf)
		{
			return;
		}
		gameObject.SetActive(false);
		InsertCoinSprite.enabled = false;
	}
}
