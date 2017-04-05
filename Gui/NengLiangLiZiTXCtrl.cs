using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NengLiangLiZiTXCtrl : MonoBehaviour {

	public GameObject LiZiObj;
	public GameObject LiZiPath;
	Transform[] MarkArray;
	Transform[] MarkBackArray;
	public float MoveTime = 1.5f;

	static NengLiangLiZiTXCtrl _Instance;
	public static NengLiangLiZiTXCtrl GetInstance()
	{
		return _Instance;
	}

	// Use this for initialization
	void Start()
	{
		_Instance = this;
		Transform parTran = LiZiPath.transform;
		List<Transform> nodesTran = new List<Transform>(parTran.GetComponentsInChildren<Transform>()){};
		nodesTran.Remove(parTran);
		MarkArray = new Transform[nodesTran.Count];
		MarkBackArray = new Transform[nodesTran.Count];
		MarkArray = nodesTran.ToArray();
		nodesTran.Reverse();
		MarkBackArray = nodesTran.ToArray();
		LiZiPath.SetActive(false);
		LiZiObj.SetActive(false);

		/*Invoke("MoveLiZiToPathEnd", 5f);
		Invoke("MoveLiZiToPathStart", 10f);*/
	}

	public void MoveLiZiToPathEnd()
	{
		LiZiObj.SetActive(true);
		iTween.MoveTo(LiZiObj, iTween.Hash("path", MarkArray,
		                                   "time", MoveTime,
		                                      "easeType", iTween.EaseType.linear,
		                                	  "oncomplete", "OnCompelteMovePathEnd"));
	}

	void OnCompelteMovePathEnd()
	{
		Invoke("HiddenLiZiObj", 0.3f);
		if (GlobalData.GetInstance().gameMode == GameMode.SoloMode) {
			NengLiangQuanCtrl.GetInstance().MoveNengLiangQuanToEnd(DaoJuTypeIndex.shenXingState);
			/*WaterwheelPlayerCtrl.GetInstance().ActiveShenXingState(); //Player ShenXingState
			NengLiangTiaoCtrl.GetInStance().StartPlayNengLiangTiao();*/
		}
	}

	public void MoveLiZiToPathStart()
	{
		LiZiObj.SetActive(true);
		iTween.MoveTo(LiZiObj, iTween.Hash("path", MarkBackArray,
		                                   "time", MoveTime,
		                                   "easeType", iTween.EaseType.linear,
		                                   "oncomplete", "OnCompelteMovePathStart"));
	}
	
	void OnCompelteMovePathStart()
	{
		Invoke("HiddenLiZiObj", 0.3f);
	}

	void HiddenLiZiObj()
	{
		LiZiObj.SetActive(false);
	}
}
