using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class AiPathGroupCtrl : MonoBehaviour {
	
	public static List<Transform> PathArray = null;

	// Use this for initialization
	void Awake()
	{
		SetPathArray();
		this.enabled = false;
	}
	
	void OnDrawGizmosSelected()
	{
		if (!enabled) {
			return;
		}
		SetAiPathIndexInfo();
	}

	void SetAiPathIndexInfo()
	{
		List<AiPathCtrl> PathArrayScript = new List<AiPathCtrl>(transform.GetComponentsInChildren<AiPathCtrl>()){};
		for (int i = 0; i < PathArrayScript.Count; i++) {
			PathArrayScript[i].SetPathIndexInfo(i);
		}
	}

	void SetPathArray()
	{
		List<AiPathCtrl> PathArrayScript = new List<AiPathCtrl>(transform.GetComponentsInChildren<AiPathCtrl>()){};
		PathArray = new List<Transform>(){};
		for (int i = 0; i < PathArrayScript.Count; i++) {
			PathArray.Add(PathArrayScript[i].transform);
		}
	}

	public static Transform FindAiPathTran(int index)
	{
		if (index < 0 || index >= PathArray.Count) {
			return null;
		}
		return PathArray[index];
	}
}
