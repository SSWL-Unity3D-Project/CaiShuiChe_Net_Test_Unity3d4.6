using UnityEngine;
using System.Collections;

public class NengLiangTiaoCtrl : MonoBehaviour {

	public float WuDiTime = 5f;
	public float NengLiangEndVal = 0.9f;
	public GameObject NengLiangBeiJing;
	public GameObject ChuanNengLiangGTX;
	public Renderer NengLiangTiao;
	public static bool IsStartPlay;
	public static bool IsStopNengLiangTeXiao;
	float DTime = 0.03f;
	float StartTime;
	float Speed;
	float Offset;

	static NengLiangTiaoCtrl _Instance;
	public static NengLiangTiaoCtrl GetInStance()
	{
		return _Instance;
	}

	// Use this for initialization
	void Start()
	{
		_Instance = this;
		IsStartPlay = false;

		Speed = DTime / WuDiTime;
		ChuanNengLiangGTX.SetActive(false);
		gameObject.SetActive(false);
	}

	IEnumerator UpdateNengLiangTiao()
	{
		while (true) {
			Offset += Speed;
			if (Offset >= NengLiangEndVal || IsStopNengLiangTeXiao) {
				IsStartPlay = false;
				gameObject.SetActive(false);
				ChuanNengLiangGTX.SetActive(false);
				NengLiangBeiJing.SetActive(true);
				NengLiangLiZiTXCtrl.GetInstance().MoveLiZiToPathStart();
				NengLiangQuanCtrl.GetInstance().MoveNengLiangQuanToStart(DaoJuTypeIndex.shenXingState);
				
				if (GlobalData.GetInstance().gameMode == GameMode.SoloMode) {
					WaterwheelPlayerCtrl.GetInstance().CloseShenXingState();
				}
				break;
			}
			NengLiangTiao.materials[0].SetTextureOffset("_MainTex", new Vector2(Offset, 0f));
			yield return new WaitForSeconds(DTime);
		}
	}

	public void StartPlayNengLiangTiao()
	{
		if (IsStartPlay) {
			return;
		}
		IsStartPlay = true;
		IsStopNengLiangTeXiao = false;
		Offset = 0f;
		NengLiangBeiJing.SetActive(false);
		gameObject.SetActive(true);
		ChuanNengLiangGTX.SetActive(true);
		NengLiangTiao.materials[0].SetTextureOffset("_MainTex", new Vector2(0f, 0f));
		XingXingCtrl.GetInstance().ResetNengLiangInfo();
		StartCoroutine(UpdateNengLiangTiao());
	}
}
