using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActiveDaJuCtrl : MonoBehaviour {
	public PlayerBtState PlayerType = PlayerBtState.PLAYER_1;

	public UISprite PlayerSpeedSprite;
	public UISprite DaoJuTextSprite;
	UISprite DaoJuSprite;

	int TypeDaoJuActive = -1;
	public static List <int> TypeDaoJuList = new List<int>{0, 0, 0, 0, 0};
	public static bool IsContainsDJ;
	public static int TypeDJIndex;

	static ActiveDaJuCtrl _InstanceP1;
	public static ActiveDaJuCtrl GetInstanceP1()
	{
		return _InstanceP1;
	}
	
	static ActiveDaJuCtrl _InstanceP2;
	public static ActiveDaJuCtrl GetInstanceP2()
	{
		return _InstanceP2;
	}

	// Use this for initialization
	void Awake () {
		DaoJuSprite = GetComponent<UISprite>();
		if (PlayerType == PlayerBtState.PLAYER_1) {
			_InstanceP1 = this;
			ActivePlayerBlood(true);
		}
		else {
			_InstanceP2 = this;
			ActivePlayerBlood(false);
		}

		//SetTypeDaoJuList(3, 1); //test
	}

	public void ActivePlayerBlood(bool isEnabled)
	{
		PlayerSpeedSprite.fillAmount = 0f;
		PlayerSpeedSprite.enabled = isEnabled;
		if (!isEnabled) {
			//PlayerType = -1;
			DaoJuTextSprite.enabled = false;
			DaoJuSprite.enabled = false;
		}

		if (isEnabled) {
			if (PlayerType != PlayerBtState.PLAYER_1) {
				int val = ActiveDaJuCtrl.GetInstanceP1().GetTypeDaoJuActive();
				if (val > 0 && val < 6) {
					ActiveDaoJuType(val);
				}
			}
			else {
				if (TypeDaoJuActive > 0 && TypeDaoJuActive < 6) {
					ActiveDaoJuType(TypeDaoJuActive);
				}
			}
		}
	}

	public void SetPlayerMvSpeedSpriteInfo(float val)
	{
		if (!PlayerSpeedSprite.enabled) {
			return;
		}

		if (PlayerType == PlayerBtState.PLAYER_2) {
			val -= 0.2f;
			if (val < 0f) {
				val = 0f;
			}
		}

		if (Mathf.Abs(PlayerSpeedSprite.fillAmount - val) < 0.01f) {
			return;
		}
		PlayerSpeedSprite.fillAmount = val;
	}

	public int GetTypeDaoJuActive()
	{
		return TypeDaoJuActive;
	}

	public void ActiveDaoJuType(int typeVal)
	{
		TypeDaoJuActive = typeVal;
		if (typeVal == -1) {
			if (!IsContainsDJ) {
				DaoJuTextSprite.enabled = false;
				DaoJuSprite.enabled = false;
			}
			else {
				ActiveDaoJuType(TypeDJIndex + 1);
			}
			return;
		}

		int playerIndex = 1;
		if (PlayerType != PlayerBtState.PLAYER_1) {
			playerIndex = 2;
		}
		DaoJuSprite.spriteName = "x" + typeVal;
		DaoJuTextSprite.spriteName = "zp" + playerIndex + "_" + typeVal;

		DaoJuTextSprite.enabled = true;
		DaoJuSprite.enabled = true;
	}

	public static void SetTypeDaoJuList(int typeVal, int val)
	{
		ResetDaoJuListInfo();
		int index = typeVal - 1;
		if (index > 0) {
			TypeDaoJuList[index] = val;
		}

		if (TypeDaoJuList.Contains(1)) {
			int i = 0;
			for (i = 0; i < TypeDaoJuList.Count; i++) {
				if (TypeDaoJuList[i] == 1) {
					break;
				}
			}
			TypeDJIndex = i;
			IsContainsDJ = true;
			//Debug.Log("tmp ****** " + i);
		}
		else {
			IsContainsDJ = false;
		}
	}

	static void ResetDaoJuListInfo()
	{
		for (int i = 0; i < TypeDaoJuList.Count; i++) {
			TypeDaoJuList[i] = 0;
		}
	}
}
