using UnityEngine;
using System.Collections;

public class TriggerBingShan : MonoBehaviour {

	[Range(1f, 10f)] public float TimeZhuanXiang = 2f;
	//static float PlayerZhuanXiangVal = 200f;
	public static bool IsSetPlayerZhuanXiang;

	void Start()
	{
		IsSetPlayerZhuanXiang = false;
	}

	void OnCollisionEnter(Collision collision)
	{
		string tagVal = collision.transform.root.tag;
		if (tagVal == "Player" && !IsInvoking("DelayResetPlayerZhuanXiangVal")
		    && !IsSetPlayerZhuanXiang) {
			IsSetPlayerZhuanXiang = true;
			/*WaterwheelPlayerCtrl.PlayerZhuanXiangVal = PlayerZhuanXiangVal;
			WaterwheelPlayerNetCtrl.PlayerZhuanXiangVal = PlayerZhuanXiangVal;
			Invoke("DelayResetPlayerZhuanXiangVal", TimeZhuanXiang);*/
		}
	}

	public static void DelayResetPlayerZhuanXiangVal()
	{
		IsSetPlayerZhuanXiang = false;
		/*float val = GameCtrlXK.GetInstance().PlayerZhuanXiangVal;
		WaterwheelPlayerCtrl.PlayerZhuanXiangVal = val;
		WaterwheelPlayerNetCtrl.PlayerZhuanXiangVal = val;*/
	}
}
