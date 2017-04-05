using UnityEngine;
using System.Collections;

public class OutPuBuCtrl : MonoBehaviour {

	Transform TriggerTran;
	float TimeVal;
	
	void Start()
	{
		TriggerTran = transform;
	}
	
	// Update is called once per frame
	void Update()
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
		if (vecA.y <= vecB.y) {
			return;
		}

		vecA.y = vecB.y = 0f;
		float dis = Vector3.Distance(vecA, vecB);
		if (dis <= 50f) {
			
			vecA = TriggerTran.forward;
			vecB = TriggerTran.position - GameCtrlXK.PlayerTran.position;
			vecA.y = vecB.y = 0f;
			float cosAB = Vector3.Dot(vecA, vecB);
			if (cosAB <= 0f) {
				gameObject.SetActive(false);
				IntoPuBuCtrl.IsIntoPuBu = false;
				PlayerAutoFire.ResetIsRestartMove();
				PlayerAutoFire.HandlePlayerOutPubuEvent();

				GameCtrlXK.GetInstance().InitDelayClosePlayerBoxCollider();
				CameraShake.GetInstance().SetRadialBlurActive(false, CameraShake.BlurStrengthPubu);
				CameraShake.GetInstance().SetActiveCamOtherPoint(false, CamDirPos.FIRST, null);
				
				if (PlayerAutoFire.PlayerMvSpeed > 100f) {
					HeatDistort.GetInstance().InitPlayScreenWater();
				}
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
}
