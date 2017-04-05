using UnityEngine;
using System.Collections;

public class TriggerIntoChangeCam : MonoBehaviour {

	public CamDirPos CameraDirPos = CamDirPos.RIGHT;
	public Transform CamFreePoint;
	public static Vector3 PlayerForward;

	[Range(0.01f, 1f)] public float WorldTimeScale = 1f;
	Transform TriggerTran;
	float TimeVal;

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
				//Time.timeScale = WorldTimeScale;
				if (IntoPuBuCtrl.IsIntoPuBu) {
					return;
				}

				PlayerForward = TriggerTran.forward;
				Time.timeScale = WorldTimeScale;
				CameraShake.GetInstance().SetActiveCamOtherPoint(true, CameraDirPos, CamFreePoint);
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
