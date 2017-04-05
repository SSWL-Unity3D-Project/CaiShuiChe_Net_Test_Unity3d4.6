using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraPathCtrl : MonoBehaviour {

	[Range(1f, 10f)] public float TimeVal;

	public bool IsOnlyChangeRot;
	public Transform CamRotation_1;
	public Transform CamRotation_2;

	/// <summary>
	/// Move camera by one path, donot change rotation.
	/// </summary>
	public bool IsMvPosByCamPath;

	/// <summary>
	/// Move camera by one path, and change rotation.
	/// </summary>
	public bool IsMvRotByCamPath;

	/// <summary>
	/// Move camera by one path, and aim one point.
	/// </summary>
	public bool IsMvAimByCamPath;
	Transform [] CamPathTran;
	public Transform CamAimPoint;

	// Use this for initialization
	void Awake () {
		int num = 0;
		if (IsOnlyChangeRot) {
			num++;
			if (CamRotation_1 == null || CamRotation_2 == null) {
				Debug.LogError("Should is fill CamRotation!");
				CamAimPoint = null;
				CamAimPoint.name = "null";
				return;
			}
		}
		else {
			InitCameraPath();
		}

		if (IsMvPosByCamPath) {
			num++;
			if (CamPathTran == null) {
				Debug.LogError("Should is fill CamPathTran!");
				CamAimPoint = null;
				CamAimPoint.name = "null";
				return;
			}
		}

		if (IsMvRotByCamPath) {
			num++;
			if (CamPathTran == null) {
				Debug.LogError("Should is fill CamPathTran!");
				CamAimPoint = null;
				CamAimPoint.name = "null";
				return;
			}
		}

		if (IsMvAimByCamPath) {
			num++;
			if (CamPathTran == null || CamAimPoint == null) {
				Debug.LogError("Should is fill CamPathTran and CamAimPoint!");
				CamAimPoint = null;
				CamAimPoint.name = "null";
				return;
			}
		}

		if (num != 1) {
			Debug.LogError("Should is select only one mode!");
			CamAimPoint = null;
			CamAimPoint.name = "null";
			return;
		}
	}

	public Transform GetCamPathTranFirst()
	{
		return CamPathTran[0];
	}

	public Transform [] GetCamPathTranArray()
	{
		return CamPathTran;
	}

	public void InitCameraPath()
	{
		List<Transform> nodes = new List<Transform>(transform.GetComponentsInChildren<Transform>()){};
		nodes.Remove(transform);
		CamPathTran = nodes.ToArray();
	}
	
	void OnDrawGizmosSelected()
	{
		if (!enabled) {
			return;
		}
		DrawPath();
	}

	public void DrawPath()
	{
		if(transform.childCount <= 1)
		{
			return;
		}
		
		List<Transform> nodes = new List<Transform>(transform.GetComponentsInChildren<Transform>()){};
		nodes.Remove(transform);
		iTween.DrawPath(nodes.ToArray(), Color.green);
	}
}
