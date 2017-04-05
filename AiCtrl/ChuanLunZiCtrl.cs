using UnityEngine;
using System.Collections;

public class ChuanLunZiCtrl : MonoBehaviour {

	[Range(0f, 1f)] float ActionMoveSpeed = 0f;
	Animator AnimatorLunZi;
	float MaxPlayerSpeed = 60f;

	// Use this for initialization
	void Start()
	{
		AnimatorLunZi = GetComponent<Animator>();
		AnimatorLunZi.speed = 0f;
	}

	public void UpdateChuanLunZiAction(float speedVal)
	{
		SetChuanLunZiAction(speedVal);
		/*if (Network.peerType != NetworkPeerType.Disconnected
		    && !GameCtrlXK.IsStopMoveAiPlayer
		    && MoveCameraByPath.IsMovePlayer) {
			WaterwheelPlayerNetCtrl.GetInstance().SetChuanLunZiAction(speedVal);
		}*/
	}

	public void SetChuanLunZiAction(float speedVal)
	{
		if (speedVal > 100f) {
			AnimatorLunZi.speed = 1.8f;
			return;
		}
		float playerSpeed = speedVal;
		playerSpeed = playerSpeed > MaxPlayerSpeed ? MaxPlayerSpeed : playerSpeed;
		ActionMoveSpeed = playerSpeed / 60f;
		ActionMoveSpeed = ActionMoveSpeed > 0.6f ? 0.6f : ActionMoveSpeed;
		AnimatorLunZi.speed = ActionMoveSpeed;
	}

	public void CloseLunZiAction()
	{
		if (AnimatorLunZi == null) {
			AnimatorLunZi = GetComponent<Animator>();
		}
		AnimatorLunZi.speed = 0f;
		AnimatorLunZi.enabled = false;
		this.enabled = false;
	}
}
