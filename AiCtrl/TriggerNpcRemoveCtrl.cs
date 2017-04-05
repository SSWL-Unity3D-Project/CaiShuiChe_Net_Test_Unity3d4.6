using UnityEngine;
using System.Collections;

public class TriggerNpcRemoveCtrl : MonoBehaviour {
	public GameObject[] RemoveSpawnPointNpc;
	
	Transform TriggerTran;
	float TimeVal;

	void Start()
	{
		TriggerTran = transform;
	}

	void Update()
	{
		if (GlobalData.GetInstance().gameMode == GameMode.OnlineMode && Network.peerType == NetworkPeerType.Disconnected) {
			gameObject.SetActive(false);
			return;
		}
		
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
				RemoveAllSpawnPointNpc();
			}
		}
	}
	
	void OnDrawGizmosSelected()
	{
		if (!enabled) {
			return;
		}
		CheckTransformScale();
		UpdateMeshFilter();
	}

	void RemoveAllSpawnPointNpc()
	{
		SpawnNpcPointCtrl SpawnNpcScript;
		int max = RemoveSpawnPointNpc.Length;
		for(int i = 0; i < max; i++)
		{
			if(RemoveSpawnPointNpc[i] != null)
			{
				SpawnNpcScript = RemoveSpawnPointNpc[i].GetComponent<SpawnNpcPointCtrl>();
				if(SpawnNpcScript != null)
				{
					SpawnNpcScript.RemoveSpawnPointNpc();
				}
			}
		}
	}

	void CheckTransformScale()
	{
		if (transform.localScale.y != 1f || transform.localScale.z != 0f) {
			Vector3 size = Vector3.zero;
			size = transform.localScale;
			size.y = 1f;
			size.z = 0f;
			transform.localScale = size;
		}
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
