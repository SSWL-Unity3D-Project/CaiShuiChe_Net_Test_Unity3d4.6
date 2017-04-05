using UnityEngine;
using System.Collections;

public class TriggerAddGameTime : MonoBehaviour
{
	[Range(1, 600)] public int AddTimeVal = 60;
	Transform TriggerTran;
	float TimeVal;

	// Use this for initialization
	void Start()
	{
		TriggerTran = transform;
	}

	// Update is called once per frame
	void Update()
	{
		/*if (Input.GetKeyUp(KeyCode.P)) {
			AddGameTimeCtrl.GetInstance().PlayAddGameTime(AddTimeVal);
		}*/

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
		if (dis <= 15f) {
			
			vecA = TriggerTran.forward;
			vecB = TriggerTran.position - GameCtrlXK.PlayerTran.position;
			vecA.y = vecB.y = 0f;
			float cosAB = Vector3.Dot(vecA, vecB);
			if (cosAB <= 0f) {
				gameObject.SetActive(false);
				StartBtCtrl.GetInstanceP1().ActivePlayerOne();
				GameDaoJiShiCtrl.GetInstance().StopDaoJiShi();
				AddGameTimeCtrl.GetInstance().PlayAddGameTime(AddTimeVal);
				AudioListCtrl.PlayAudio( AudioListCtrl.GetInstance().AudioTimeJiaShiDian );
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

