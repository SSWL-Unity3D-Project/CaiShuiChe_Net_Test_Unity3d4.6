using UnityEngine;
using System.Collections;

public class TriggerPlayerSpeedInto : MonoBehaviour {
	
	Transform TriggerTran;
	float TimeVal;

	public static bool IsSetPlayerSpeed;
	static int PlayerMaxVelocityFoot;
	
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
				SetPlayerSpeedVal();
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

	public static void SetPlayerSpeedVal()
	{
		if (IsSetPlayerSpeed) {
			return;
		}
		IsSetPlayerSpeed = true;

		if (GlobalData.GetInstance().gameMode == GameMode.SoloMode) {
			PlayerMaxVelocityFoot = WaterwheelPlayerCtrl.mMaxVelocityFoot;
			WaterwheelPlayerCtrl.mMaxVelocityFoot = 90;
			WaterwheelPlayerCtrl.mMaxVelocityFootMS = (float)WaterwheelPlayerCtrl.mMaxVelocityFoot / 3.6f;
		}
	}
	
	public static void ResetPlayerSpeedVal()
	{
		if (!IsSetPlayerSpeed) {
			return;
		}
		IsSetPlayerSpeed = false;
		
		if (GlobalData.GetInstance().gameMode == GameMode.SoloMode) {
			WaterwheelPlayerCtrl.mMaxVelocityFoot = PlayerMaxVelocityFoot;
			WaterwheelPlayerCtrl.mMaxVelocityFootMS = (float)WaterwheelPlayerCtrl.mMaxVelocityFoot / 3.6f;
		}
	}
}
