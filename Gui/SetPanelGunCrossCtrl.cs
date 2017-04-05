using UnityEngine;
using System.Collections;

public class SetPanelGunCrossCtrl : MonoBehaviour {

	public GameObject [] AimObjArray;

	Transform ObjTran;
	GameObject CrossParObj;

	static SetPanelGunCrossCtrl _Instance;
	public static SetPanelGunCrossCtrl GetInstance()
	{
		return _Instance;
	}

	// Use this for initialization
	void Awake () {
		_Instance = this;
		CrossParObj = transform.parent.gameObject;
		ObjTran = transform;
		SetGunCrossActive(false);
	}

	void Start()
	{
		pcvr.GetInstance().OnUpdateCrossEvent += OnUpdateCrossEvent;
	}
	
	// Update is called once per frame
	void Update () {
		ObjTran.localPosition = pcvr.CrossPosition;
	}

	void OnUpdateCrossEvent()
	{
		if (Application.loadedLevel != (int)GameLeve.SetPanel) {
			pcvr.GetInstance().OnUpdateCrossEvent -= OnUpdateCrossEvent;
			return;
		}
		ObjTran.localPosition = pcvr.CrossPosition;
	}

	public void SetGunCrossActive(bool isActive)
	{
		if (isActive == CrossParObj.activeSelf) {
			return;
		}
		CrossParObj.SetActive(isActive);
	}

	public void SetAimObjArrayActive(bool isActive)
	{
		int max = AimObjArray.Length;
		for (int i = 0; i < max; i++) {
			if (AimObjArray[i] != null && AimObjArray[i].activeSelf != isActive) {
				AimObjArray[i].SetActive(isActive);
			}
		}
	}
}
