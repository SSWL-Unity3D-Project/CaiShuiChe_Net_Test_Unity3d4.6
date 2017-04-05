using UnityEngine;
using System.Collections;

public class PlayerAiActionCtrl : MonoBehaviour {
	
	NetworkView netView;
	ChuanShenCtrl ChuanShenScript;
	ZhuJiaoNan ZhuJiaoNanScript;

	// Use this for initialization
	void Start()
	{
		netView = GetComponent<NetworkView>();
		ChuanShenScript = GetComponentInChildren<ChuanShenCtrl>();
		ZhuJiaoNanScript = GetComponentInChildren<ZhuJiaoNan>();
	}

	
	public void SetChuanShenAction(int turnLeftVal, int turnRightVal)
	{
		netView.RPC("SendAiChuanShenAction", RPCMode.OthersBuffered, turnLeftVal, turnRightVal);
	}
	
	[RPC]
	void SendAiChuanShenAction(int turnLeftVal, int turnRightVal)
	{
		if (ChuanShenScript == null) {
			return;
		}
		ChuanShenScript.SetChuanShenAction(turnLeftVal, turnRightVal);
	}

	public void SetZhuJiaoNanAction(int turnLeftVal, int turnRightVal)
	{
		netView.RPC("SendAiZhuJiaoNanAction", RPCMode.OthersBuffered, turnLeftVal, turnRightVal);
	}
	
	[RPC]
	void SendAiZhuJiaoNanAction(int turnLeftVal, int turnRightVal)
	{
		ZhuJiaoNanScript.SetZhuJiaoNanAction(turnLeftVal, turnRightVal);
	}
}
