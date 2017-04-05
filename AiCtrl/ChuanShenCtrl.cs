using UnityEngine;
using System.Collections;

public class ChuanShenCtrl : MonoBehaviour {

	PlayerAiActionCtrl ActionCtrlSript;
	Animator AnimatorChuanShen;
	// Use this for initialization
	void Start()
	{
		AnimatorChuanShen = GetComponent<Animator>();

		GameObject rootObj = transform.root.gameObject;
		ActionCtrlSript = rootObj.AddComponent<PlayerAiActionCtrl>();
	}

	public void UpdateChuanShenAction(bool isTurnLeft, bool isTurnRight)
	{
		if (!StartGameTimeCtrl.GetInstance().GetIsPlayStartTime()) {
			return;
		}

		if (Camera.main != null && !Camera.main.enabled) {
			return;
		}

		if (AnimatorChuanShen.GetBool("TurnLeft") != isTurnLeft) {
			AnimatorChuanShen.SetBool("TurnLeft", isTurnLeft);
			if (Network.peerType != NetworkPeerType.Disconnected)
			{
				int turnLeftVal = isTurnLeft ? 1 : 0;
				int turnRightVal = isTurnRight ? 1 : 0;
				//WaterwheelPlayerNetCtrl.GetInstance().SetChuanShenAction(turnLeftVal, turnRightVal);
				ActionCtrlSript.SetChuanShenAction(turnLeftVal, turnRightVal);
			}
		}
		
		if (AnimatorChuanShen.GetBool("TurnRight") != isTurnRight) {
			AnimatorChuanShen.SetBool("TurnRight", isTurnRight);
			if (Network.peerType != NetworkPeerType.Disconnected)
			{
				int turnLeftVal = isTurnLeft ? 1 : 0;
				int turnRightVal = isTurnRight ? 1 : 0;
				//WaterwheelPlayerNetCtrl.GetInstance().SetChuanShenAction(turnLeftVal, turnRightVal);
				ActionCtrlSript.SetChuanShenAction(turnLeftVal, turnRightVal);
			}
		}
	}

	public void SetChuanShenAction(int turnLeftVal, int turnRightVal)
	{
		bool isTurnLeft = turnLeftVal == 1 ? true : false;
		bool isTurnRight = turnRightVal == 1 ? true : false;
		if (AnimatorChuanShen.GetBool("TurnLeft") != isTurnLeft) {
			AnimatorChuanShen.SetBool("TurnLeft", isTurnLeft);
		}
		
		if (AnimatorChuanShen.GetBool("TurnRight") != isTurnRight) {
			AnimatorChuanShen.SetBool("TurnRight", isTurnRight);
		}
	}
}
