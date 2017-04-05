using UnityEngine;
using System.Collections;

public class CloseChangJingTrigger : MonoBehaviour {
	
	public GameObject [] CloseObjArray;
	Transform CloseTran;
	Transform PlayerTran;
	
	// Use this for initialization
	void Start () {
		CloseTran = transform;
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
		int max = CloseObjArray.Length;
		for (int i = 0; i < max; i++) {
			if (CloseObjArray[i] != null) {
				CloseObjArray[i].SetActive(false);
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
		Vector3 vecB = CloseTran.position;
		vecA.y = vecB.y = 0f;
		float dis = Vector3.Distance(vecA, vecB);
		if (dis <= 10f) {
			StopCheckPlayerPos();
			return;
		}
	}
}
