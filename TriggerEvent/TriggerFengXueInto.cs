using UnityEngine;
using System.Collections;

public class TriggerFengXueInto : MonoBehaviour {

	Transform TriggerTran;
	float TimeVal;
	//static float PlayerZhuanXiangVal = 200f;
	public static bool IsSetPlayerZhuanXiang;
	
	// Use this for initialization
	void Start()
	{
		TriggerTran = transform;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (GameCtrlXK.PlayerTran == null) {
			GameCtrlXK.GetInstance().FindPlayerTran();
			return;
		}
		
		if (Time.realtimeSinceStartup - TimeVal < 0.1f) {
			return;
		}
		TimeVal = Time.realtimeSinceStartup;
		
		Vector3 vecA = TriggerTran.position;
		Vector3 vecB = GameCtrlXK.PlayerTran.position;
		vecA.y = vecB.y = 0f;
		float dis = Vector3.Distance(vecA, vecB);
		if (dis <= 50f) {
			
			vecA = TriggerTran.forward;
			vecB = TriggerTran.position - GameCtrlXK.PlayerTran.position;
			vecA.y = vecB.y = 0f;
			float cosAB = Vector3.Dot(vecA, vecB);
			if (cosAB <= 0f) {
				gameObject.SetActive(false);
				CameraShake.GetInstance().PlayAudioSoureFengXue();
				//XueDiTiShiCtrl.GetInstance().ShowXueDiTiShi();
				SetPlayerZhuanXiangVal();
			}
		}
	}

	void OnDrawGizmosSelected()
	{
		if (!enabled) {
			return;
		}
		UpdateMeshFilter();
	}
	
	void UpdateMeshFilter()
	{
		MeshFilter filter = GetComponent<MeshFilter>();
		if (filter != null
		    && GameCtrlXK.GetInstance().TriggerFilterMesh != null
		    && filter.sharedMesh != GameCtrlXK.GetInstance().TriggerFilterMesh) {
			DestroyImmediate(filter);
			MeshFilter newFilter = gameObject.AddComponent<MeshFilter>();
			newFilter.sharedMesh = GameCtrlXK.GetInstance().TriggerFilterMesh;
		}
	}

	public static void SetPlayerZhuanXiangVal()
	{
		if (IsSetPlayerZhuanXiang) {
			return;
		}
		IsSetPlayerZhuanXiang = true;
		/*WaterwheelPlayerCtrl.PlayerZhuanXiangVal = PlayerZhuanXiangVal;
		WaterwheelPlayerNetCtrl.PlayerZhuanXiangVal = PlayerZhuanXiangVal;*/
	}
	
	public static void ResetPlayerZhuanXiangVal()
	{
		IsSetPlayerZhuanXiang = false;
		/*float val = GameCtrlXK.GetInstance().PlayerZhuanXiangVal;
		WaterwheelPlayerCtrl.PlayerZhuanXiangVal = val;
		WaterwheelPlayerNetCtrl.PlayerZhuanXiangVal = val;*/
	}
}
