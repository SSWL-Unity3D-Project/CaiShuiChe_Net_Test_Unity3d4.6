using UnityEngine;
using System.Collections;

using System.Collections.Generic;

public class NpcMark : MonoBehaviour {

	public bool IsDoRoot;
	[Range(0f, 20f)] public float RootAniTime = 3f;

	List<Transform> nodes = new List<Transform>(){};

	void Start()
	{
		UpdateFlyNodes();
		this.enabled = false;
	}

	void OnDrawGizmosSelected()
	{
		if(!enabled)
		{
			return;
		}
		CheckPathMarkScale();
		UpdateMeshFilter();

		Transform parTran = transform.parent;
		NpcMark markScript = parTran.GetComponent<NpcMark>();
		if(markScript != null)
		{
			markScript.DrawPath();
			return;
		}
		else
		{
			if(parTran.childCount > 1)
			{
				List<Transform> nodesTran = new List<Transform>(parTran.GetComponentsInChildren<Transform>()){};
				nodesTran.Remove(parTran);
				iTween.DrawPath(nodesTran.ToArray(), Color.blue);
				return;
			}
		}
	}
	
	public void DrawPath ()
	{
		OnDrawGizmosSelected();
	}

	void CheckPathMarkScale()
	{
		if (transform.localScale.y == 1f) {
			return;
		}
		Vector3 size = Vector3.zero;
		size = transform.localScale;
		size.y = 1f;
		transform.localScale = size;
	}
	
	void UpdateMeshFilter()
	{
		MeshFilter filter = GetComponent<MeshFilter>();
		if (filter != null
		    && GameCtrlXK.GetInstance().PathMarkFilter != null
		    && filter.sharedMesh != GameCtrlXK.GetInstance().PathMarkFilter.sharedMesh) {
			DestroyImmediate(filter);
			MeshFilter newFilter = gameObject.AddComponent<MeshFilter>();
			newFilter.sharedMesh = GameCtrlXK.GetInstance().PathMarkFilter.sharedMesh;
		}
	}

	public List<Transform> GetFlyNodes()
	{
		return nodes;
	}

	void UpdateFlyNodes()
	{
		if (transform.childCount <= 0) {
			nodes.Clear();
			return;
		}
		
		if (nodes.Count > 0 && nodes.Count == transform.childCount + 1) {
			for (int i = 0; i < transform.childCount + 1; i++) {
				if (i == 0) {
					if (nodes[i] != transform) {
						nodes.Clear();
						break;
					}
				}
				else {
					if (nodes[i] != transform.GetChild(i - 1)) {
						nodes.Clear();
						break;
					}
				}
			}
		}
		
		if (nodes.Count == transform.childCount + 1) {
			return;
		}
		
		nodes.Clear();
		nodes.Add(transform);
		for (int i = 0; i < transform.childCount; i++) {
			nodes.Add(transform.GetChild(i));
		}
	}
}

public enum NpcRunState
{
	NULL,
	RUN_1,
	RUN_2
}
