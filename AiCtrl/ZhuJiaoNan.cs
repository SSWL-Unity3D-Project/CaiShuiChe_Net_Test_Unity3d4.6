using UnityEngine;
using System.Collections;

public class ZhuJiaoNan : MonoBehaviour {

	Animator AnimatorCom;
	PlayerAutoFire AutoFireScript;
	PlayerAiActionCtrl ActionCtrlSript;
	
	bool IsCannotTurn;

	static ZhuJiaoNan _Instance;
	public static ZhuJiaoNan GetInstance()
	{
		return _Instance;
	}

	// Use this for initialization
	void Start()
	{
		_Instance = this;
		AnimatorCom = GetComponent<Animator>();
		Transform rootObj = transform.root;
		AutoFireScript = rootObj.GetComponentInChildren<PlayerAutoFire>();
		ActionCtrlSript = rootObj.GetComponent<PlayerAiActionCtrl>();
	}

	public void UpdateZhuJiaoNanAction(bool isTurnLeft, bool isTurnRight)
	{
		if (!StartGameTimeCtrl.GetInstance().GetIsPlayStartTime()) {
			return;
		}

		if (Camera.main != null && !Camera.main.enabled) {
			return;
		}

		if (IsCannotTurn) {
			return;
		}

		if (AnimatorCom.GetBool("TurnLeft") != isTurnLeft) {
			AnimatorCom.SetBool("TurnLeft", isTurnLeft);
			if (Network.peerType != NetworkPeerType.Disconnected)
			{
				int turnLeftVal = isTurnLeft ? 1 : 0;
				int turnRightVal = isTurnRight ? 1 : 0;
				//WaterwheelPlayerNetCtrl.GetInstance().SetZhuJiaoNanAction(turnLeftVal, turnRightVal);
				ActionCtrlSript.SetZhuJiaoNanAction(turnLeftVal, turnRightVal);
			}
		}
		
		if (AnimatorCom.GetBool("TurnRight") != isTurnRight) {
			AnimatorCom.SetBool("TurnRight", isTurnRight);
			if (Network.peerType != NetworkPeerType.Disconnected)
			{
				int turnLeftVal = isTurnLeft ? 1 : 0;
				int turnRightVal = isTurnRight ? 1 : 0;
				//WaterwheelPlayerNetCtrl.GetInstance().SetZhuJiaoNanAction(turnLeftVal, turnRightVal);
				ActionCtrlSript.SetZhuJiaoNanAction(turnLeftVal, turnRightVal);
			}
		}
	}
	
	public void SetZhuJiaoNanAction(int turnLeftVal, int turnRightVal)
	{
		bool isTurnLeft = turnLeftVal == 1 ? true : false;
		bool isTurnRight = turnRightVal == 1 ? true : false;
		if (AnimatorCom.GetBool("TurnLeft") != isTurnLeft) {
			AnimatorCom.SetBool("TurnLeft", isTurnLeft);
		}
		
		if (AnimatorCom.GetBool("TurnRight") != isTurnRight) {
			AnimatorCom.SetBool("TurnRight", isTurnRight);
		}
	}

	public void PlayFailAction()
	{
		if (IsCannotTurn) {
			return;
		}

		IsCannotTurn = true;
		AnimatorCom.SetBool("Fail", true);
		//ZhuJiaoNv.GetInstance().PlayFailAction();
		AutoFireScript.ZhuJiaoNvPlayFailAction();

		if (Network.peerType != NetworkPeerType.Disconnected)
		{
			WaterwheelPlayerNetCtrl.GetInstance().PlayZhuJiaoNanAction("Fail");
		}
	}
	
	public void PlayWinAction()
	{
		if (IsCannotTurn) {
			return;
		}

		IsCannotTurn = true;
		int key = Random.Range(0, 1000) % 2;
		if (key == 1) {
			AnimatorCom.SetBool("Win_1", true);
		}
		else {
			AnimatorCom.SetBool("Win_2", true);
		}
		//ZhuJiaoNv.GetInstance().PlayWinAction(key);
		AutoFireScript.ZhuJiaoNvPlayWinAction(key);

		
		if (Network.peerType != NetworkPeerType.Disconnected)
		{
			string action = key == 1 ? "Win_1" : "Win_2";
			WaterwheelPlayerNetCtrl.GetInstance().PlayZhuJiaoNanAction(action);
		}
	}

	public void PlayZhuJiaoNanAction(string action)
	{
		AnimatorCom.SetBool(action, true);
	}
}
