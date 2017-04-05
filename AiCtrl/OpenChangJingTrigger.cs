using UnityEngine;
using System.Collections;

public class OpenChangJingTrigger : MonoBehaviour {

	public GameObject [] OpenObjArray;
	Transform OpenTran;
	Transform PlayerTran;

	// Use this for initialization
	void Start () {
		int max = OpenObjArray.Length;
		for (int i = 0; i < max; i++) {
			if (OpenObjArray[i] != null) {
				OpenObjArray[i].SetActive(false);
			}
		}
		OpenTran = transform;

		/*if (GlobalData.GetInstance().gameMode == GameMode.SoloMode) {
			PlayerTran = WaterwheelPlayerCtrl.GetInstance().transform;
		}
		else {
			PlayerTran = WaterwheelPlayerNetCtrl.GetInstance().transform;
		}*/

		InitCheckPlayerDis();
	}

	void InitCheckPlayerDis()
	{
		gameObject.SetActive(false);
		CancelInvoke("CheckPlayerDis");
		InvokeRepeating("CheckPlayerDis", 6f, 0.3f);
	}

	void StopCheckPlayerPos()
	{
		CancelInvoke("CheckPlayerDis");
		int max = OpenObjArray.Length;
		for (int i = 0; i < max; i++) {
			if (OpenObjArray[i] != null) {
				OpenObjArray[i].SetActive(true);
			}
		}
	}

	void CheckPlayerDis()
	{
		if (PlayerTran == null) {
			if (GlobalData.GetInstance().gameMode == GameMode.SoloMode) {
				PlayerTran = WaterwheelPlayerCtrl.GetInstance().transform;
			}
			else {
				PlayerTran = WaterwheelPlayerNetCtrl.GetInstance().transform;
			}
			return;
		}

		Vector3 vecA = PlayerTran.position;
		Vector3 vecB = OpenTran.position;
		vecA.y = vecB.y = 0f;
		float dis = Vector3.Distance(vecA, vecB);
		if (dis <= 10f) {
			StopCheckPlayerPos();
			return;
		}
	}
}
