using UnityEngine;
using System.Collections;

public class AiMark : MonoBehaviour
{
	private Transform _mNextMark = null;
	public Transform mNextMark
	{
		get
		{
			return _mNextMark;
		}
		set
		{
			_mNextMark = value;
		}
	}
	
	private int mMarkCount = 0;
	
	public void setMarkCount( int count )
	{
		mMarkCount = count;
	}
	
	public int getMarkCount()
	{
		return mMarkCount;
	}
	
	// Use this for initialization
	void Start()
	{
		this.enabled = false;
	}

	void OnDrawGizmosSelected()
	{
		if (!enabled) {
			return;
		}
		CheckPathMarkScale();
		CheckBoxCollider();
		UpdateMeshFilter();

		Transform parTran = transform.parent;
		if (parTran == null) {
			return;
		}

		AiPathCtrl pathScript = parTran.GetComponent<AiPathCtrl>();
		pathScript.DrawPath();
	}
	
	void CheckPathMarkScale()
	{
		if (transform.localScale.y == 0f) {
			return;
		}
		Vector3 size = Vector3.zero;
		size = transform.localScale;
		size.y = 0f;
		transform.localScale = size;
	}

	void CheckBoxCollider()
	{
		BoxCollider boxCol = GetComponent<BoxCollider>();
		if (boxCol != null) {
			DestroyImmediate(boxCol);
		}
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
}

