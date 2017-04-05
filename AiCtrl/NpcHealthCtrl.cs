using UnityEngine;
using System.Collections;

public class NpcHealthCtrl : MonoBehaviour
{
	[Range(0f, 5f)] public float DamageTime = 4f;
	int CountDamage;
	bool IsDeadObj;
	static float TimeDaoJuJiHuoVal;
	WaterwheelPlayerNetCtrl PlayerNetScript;
	NetworkView netView;

	void Start()
	{
		if (GlobalData.GetInstance().gameMode == GameMode.OnlineMode) {
			netView = networkView;

			bool isChangeParent = false;
			switch (tag) {
			case "DianDaoFuObj":
			case "DingShenFuObj":
			case "HuanWeiFuObj":
			case "HuanYingFuObj":
			case "JuLiFuObj":
				isChangeParent = true;
				break;
			}

			if (isChangeParent) {
				transform.parent = GameCtrlXK.MissionCleanup;
			}
		}

		DaoJuNetCtrl daoJuScript = GetComponent<DaoJuNetCtrl>();
		PlayerNetScript = GetComponent<WaterwheelPlayerNetCtrl>();
		if (PlayerNetScript != null || daoJuScript != null) {
			DamageTime = 0.05f;
		}
		else {
			DamageTime = 0f;
		}
	}

	public void SetObjType(string type)
	{
		tag = type;
		transform.parent = GameCtrlXK.MissionCleanup;
		if (Network.peerType != NetworkPeerType.Disconnected && netView != null) {
			netView.RPC("SendSetDaoJuObjType", RPCMode.OthersBuffered);
		}
	}

	[RPC]
	void SendSetDaoJuObjType(string type)
	{
		tag = type;
		transform.parent = GameCtrlXK.MissionCleanup;
	}

	public void OnDamage(float unitTime)
	{
		if (PlayerNetScript != null) {
			if (PlayerNetScript == WaterwheelPlayerNetCtrl.GetInstance()) {
				return;
			}
			PlayerNetScript.SetActivePlayerGunWaterObj(1);
		}

		if (IsDeadObj) {
			return;
		}
		CountDamage++;

		if(unitTime * CountDamage >= DamageTime)
		{
			IsDeadObj = true;
			if(GlobalData.GetInstance().gameMode == GameMode.SoloMode)
			{
				NpcMoveCtrl npcScript = gameObject.GetComponent<NpcMoveCtrl>();
				ZaiTiNpcCtrl npcZaiTiScript = gameObject.GetComponent<ZaiTiNpcCtrl>();
				if(npcScript != null || npcZaiTiScript != null)
				{
					if(npcZaiTiScript != null)
					{
						npcZaiTiScript.ShootedByPlayer(0);
						PlayerAutoFire.AddPlayerShootNpcNum();
					}
					else
					{
						if(npcScript.NPC_Type == NPC_STATE.BOSS_NPC || npcScript.NPC_Type == NPC_STATE.ZAI_TI_NPC)
						{
							return;
						}
						else
						{
							npcScript.ShootedByPlayer();
							PlayerAutoFire.AddPlayerShootNpcNum();
						}
					}
				}
				else
				{
					WaterwheelPlayerCtrl.GetInstance().ShootingDeadObj(gameObject);
				}
			}
			else
			{
				if (PlayerNetScript != null) {
					ResetDamageInfo();
					PlayerNetScript.ActiveXuanYunState();
					//ScreenLog.Log("ActiveXuanYunState -> name "+gameObject.name);
				}
				else {
					switch (tag) {
					case "DianDaoFuObj":
					case "DingShenFuObj":
					case "HuanWeiFuObj":
					case "HuanYingFuObj":
					case "JuLiFuObj":
						if (Time.realtimeSinceStartup - TimeDaoJuJiHuoVal < 1f) {
							return;
						}
						TimeDaoJuJiHuoVal = Time.realtimeSinceStartup;
						break;
					}
					WaterwheelPlayerNetCtrl.GetInstance().ShootingDeadObj(gameObject);
				}
			}
		}
	}

	void ResetDamageInfo()
	{
		IsDeadObj = false;
		CountDamage = 0;
	}

	public void RemoveGameObj()
	{
		if (GlobalData.GetInstance().gameMode != GameMode.OnlineMode) {
			DeleteGameObj();
		}
		else {
			HiddenGameObj();
			if (Network.peerType != NetworkPeerType.Disconnected && netView != null) {
				netView.RPC("SendHiddenDaoJuObj", RPCMode.OthersBuffered);
			}
		}
	}

	void DeleteGameObj()
	{
		if (gameObject != null) {
			Destroy(gameObject);
		}
	}

	void HiddenGameObj()
	{
		if (!gameObject.activeSelf) {
			return;
		}

		IsDeadObj = true;
		gameObject.SetActive(false);
		if (!IsInvoking("ShowGameObj")) {
			Invoke("ShowGameObj", 2f);
		}
	}

	void ShowGameObj()
	{
		IsDeadObj = false;
		CountDamage = 0;
		gameObject.SetActive(true);
	}

	[RPC]
	void SendHiddenDaoJuObj()
	{
		HiddenGameObj();
	}
}

