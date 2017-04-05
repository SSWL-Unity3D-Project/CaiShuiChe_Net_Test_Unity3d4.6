using UnityEngine;
using System.Collections;

public class IntoPuBuCtrl : MonoBehaviour {

	public Transform OutPuBuTran;
	public static bool IsIntoPuBu = false;
	public static int MvCount;
	public static float PlayerMvSpeed = 45f;
	public static Vector3 PlayerForward;
	Transform TriggerTran;
	float TimeVal;
	
	void Start()
	{
		if (OutPuBuTran == null) {
			Debug.LogError("OutPuBuTran should not is null");
			OutPuBuTran.name = "null";
			return;
		}
		TriggerTran = transform;
		IsIntoPuBu = false;
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
		vecA.y = vecB.y = 0f;
		float dis = Vector3.Distance(vecA, vecB);
		if (dis <= 50f) {
			
			vecA = TriggerTran.forward;
			vecB = TriggerTran.position - GameCtrlXK.PlayerTran.position;
			vecA.y = vecB.y = 0f;
			float cosAB = Vector3.Dot(vecA, vecB);
			if (cosAB <= 0f) {
				gameObject.SetActive(false);

				vecA = TriggerTran.position;
				vecB = OutPuBuTran.position;
				float disH = Mathf.Abs(vecA.y - vecB.y);
				vecA.y = vecB.y = 0f;
				float disV = Vector3.Distance(vecA, vecB);
				float gravity = -PlayerAutoFire.GravityValMax;
				float timeVal = Mathf.Sqrt( (2f * disH) / gravity );
				float speedVal = timeVal != 0f ? (disV / timeVal) : 50f;
				//Time.timeScale = 1f;
				Time.timeScale = 0.5f;
				CancelInvoke("ResetTimeScale");
				Invoke("ResetTimeScale", 0.5f);
				if (Time.timeScale >= 1f) {
					speedVal = (int)speedVal + 100f;
				}
				else {
					speedVal = (int)speedVal;
				}
				//Debug.Log("player speed = " + speedVal);

				PlayerMvSpeed = speedVal;
				PlayerForward = TriggerTran.forward;
				CameraShake.GetInstance().SetRadialBlurActive(true, CameraShake.BlurStrengthPubu);
				CameraShake.GetInstance().SetActiveCamOtherPoint(true, CamDirPos.FIRST, null);
				GameCtrlXK.GetInstance().SetPlayerBoxColliderState(true);
				AudioListCtrl.PlayAudio(AudioListCtrl.GetInstance().AudioPuBuJiaSu);

				IsIntoPuBu = true;
				MvCount = 0;
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

	void ResetTimeScale()
	{
		Time.timeScale = 1f;
		PlayerMvSpeed += 100f;
		CameraShake.GetInstance().SetActiveCamOtherPoint(false, CamDirPos.FIRST, null);
		PlayerAutoFire.HandlePlayerOutPubuEvent();
	}
}
