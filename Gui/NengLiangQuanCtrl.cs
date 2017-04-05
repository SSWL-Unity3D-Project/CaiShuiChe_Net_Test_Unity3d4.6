using UnityEngine;
using System.Collections;

public class NengLiangQuanCtrl : MonoBehaviour {
	
	public float MvSpeed = 0.5f;
	Transform StartTran;
	Transform EndTran;
	Transform NengLiangQuanTran;
	bool IsMoveToEnd;
	bool IsMoveToStart;
	DaoJuTypeIndex RecordDJType = DaoJuTypeIndex.NULL;
	
	float TimeNengLiangQuan;
	static NengLiangQuanCtrl _Instance;
	public static NengLiangQuanCtrl GetInstance()
	{
		return _Instance;
	}

	// Use this for initialization
	void Start()
	{
		_Instance = this;
		NengLiangQuanTran = transform;
		gameObject.SetActive(false);
	}

	void Update()
	{
		if (gameObject.activeSelf) {
			if (Time.realtimeSinceStartup - TimeNengLiangQuan > 2.5f) {
				IsMoveToEnd = false;
				IsMoveToStart = false;
				gameObject.SetActive(false);
				//Debug.Log("Reset NengLiangQuan info...");
			}
		}
	}

	public void MoveNengLiangQuanToEnd(DaoJuTypeIndex val)
	{
		if (IsMoveToEnd) {
			RecordDJType = val;
			return;
		}
		IsMoveToEnd = true;

		if (EndTran == null) {
			if (GlobalData.GetInstance().gameMode == GameMode.SoloMode) {
				EndTran = WaterwheelPlayerCtrl.GetInstance().HuanYingFuObj.transform;
			}
			else {
				EndTran = WaterwheelPlayerNetCtrl.GetInstance().GetHuanYingFuTeXiaoTran();
			}
			StartTran = Camera.main.transform;
		}
		TimeNengLiangQuan = Time.realtimeSinceStartup;
		NengLiangQuanTran.position = StartTran.position;
		gameObject.SetActive(true);
		StartCoroutine(UpdateNengLiangQuanPosToEnd(val));
	}

	IEnumerator UpdateNengLiangQuanPosToEnd(DaoJuTypeIndex val)
	{
		while (true) {
			Vector3 posEnd = EndTran.position;
			Vector3 posStart = NengLiangQuanTran.position;
			if (Vector3.Distance(posEnd, posStart) < 0.5f) {
				IsMoveToEnd = false;
				gameObject.SetActive(false);

				if (GlobalData.GetInstance().gameMode == GameMode.SoloMode) {
					switch (val) {
					case DaoJuTypeIndex.shenXingState:
						WaterwheelPlayerCtrl.GetInstance().ActiveShenXingState(); //Player ShenXingState
						NengLiangTiaoCtrl.GetInStance().StartPlayNengLiangTiao();
						if (RecordDJType == DaoJuTypeIndex.huanYingFu) {
							WaterwheelPlayerCtrl.GetInstance().ActiveHuanYingFuState();
						}
						break;

					case DaoJuTypeIndex.huanYingFu:
						WaterwheelPlayerCtrl.GetInstance().ActiveHuanYingFuState();
						if (RecordDJType == DaoJuTypeIndex.shenXingState) {
							WaterwheelPlayerCtrl.GetInstance().ActiveShenXingState(); //Player ShenXingState
							NengLiangTiaoCtrl.GetInStance().StartPlayNengLiangTiao();
						}
						break;
					}
				}
				else {
					switch (val) {
					case DaoJuTypeIndex.huanYingFu:
						WaterwheelPlayerNetCtrl.GetInstance().ActiveHuanYingFuState();
						break;
					}
				}
				RecordDJType = DaoJuTypeIndex.NULL;
				break;
			}
			
			Vector3 forwardVal = posEnd - posStart;
			forwardVal = forwardVal.normalized;
			Vector3 pos = forwardVal * MvSpeed + posStart;
			NengLiangQuanTran.position = pos;
			yield return new WaitForSeconds(0.03f);
		}
	}

	public void MoveNengLiangQuanToStart(DaoJuTypeIndex val)
	{
		if (IsMoveToStart) {
			return;
		}
		IsMoveToStart = true;

		TimeNengLiangQuan = Time.realtimeSinceStartup;
		NengLiangQuanTran.position = EndTran.position;
		gameObject.SetActive(true);
		StartCoroutine(UpdateNengLiangQuanPosToStart(val));
	}

	IEnumerator UpdateNengLiangQuanPosToStart(DaoJuTypeIndex val)
	{
		while (true) {
			if (IsMoveToEnd) {
				IsMoveToStart = false;
				break;
			}

			Vector3 posEnd = StartTran.position;
			Vector3 posStart = NengLiangQuanTran.position;
			if (Vector3.Distance(posEnd, posStart) < 0.5f) {
				IsMoveToStart = false;
				gameObject.SetActive(false);
				break;
			}
			
			Vector3 forwardVal = posEnd - posStart;
			forwardVal = forwardVal.normalized;
			Vector3 pos = forwardVal * MvSpeed + posStart;
			NengLiangQuanTran.position = pos;
			yield return new WaitForSeconds(0.03f);
		}
	}
}
