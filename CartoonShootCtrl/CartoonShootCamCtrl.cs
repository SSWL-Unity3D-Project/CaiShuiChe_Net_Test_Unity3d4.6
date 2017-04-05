using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CartoonShootCamCtrl : MonoBehaviour {
	
	public string OnFinishMoveCamera = "OnFinishMoveCameraByZiYouYiDongPath";
	public Transform ZhuJiaoTran;
	public static Transform GZhuJiaoTran;

	public Transform GenSuiCamTran;

	public bool IsAutoMoveZhuJiao;
	public static bool GIsAutoMoveZhuJiao;

	public bool IsTiaoGuoDongHua;
	public static bool GIsTiaoGuoDongHua;

	CartoonShootCamState CamState = CartoonShootCamState.NULL;

	Transform MainCamTran;
	Transform CamAimPoint;
	
	CartoonShootPathCtrl CartoonShootPathScript;
	Transform ZiYouYiDongCamPtahTran;

	GameObject CameraObj;
	int MarkCurrentIndex;
	
	public bool IsUpdateGenSuiCamTran;
	public Transform TestGenSuiCamTan;
	public iTweenEvent[] ITweenEventArray;
	bool IsLookAtPoint;

	static CartoonShootCamCtrl _Instance;
	public static CartoonShootCamCtrl GetInstance()
	{
		return _Instance;
	}

	// Use this for initialization
	void Awake()
	{
		_Instance = this;
		MainCamTran = Camera.main.transform;
		CameraObj = MainCamTran.gameObject;
		if (ZhuJiaoTran == null) {
			ZhuJiaoTran.name = "null";
		}
		GZhuJiaoTran = ZhuJiaoTran;
		GIsAutoMoveZhuJiao = IsAutoMoveZhuJiao;
		GIsTiaoGuoDongHua = IsTiaoGuoDongHua;
		//Application.targetFrameRate = 30;
	}

	// Update is called once per frame
	//void Update()
	//{
		//if (Input.GetKeyUp(KeyCode.P)) {
			//ActiveGenSuiCamera(TestGenSuiTran);
			//ActiveDingDianMiaoZhunCamera(TestDingDianMiaoZhunTranCamPoint, TestDingDianMiaoZhunTranAimPoint);
			//ActiveDingDianBuMiaoZhunCamera(TestDingDianMiaoZhunTranCamPoint);
			//ActiveZiYouYiDongCamera(TestZiYouYiDongTranCamPath);
		//}
		//CheckCameraState();
	//}

	void FixedUpdate()
	{
		CheckCameraState();
	}

	public void CheckCameraState()
	{
		if (CamState != CartoonShootCamState.DingDianMiaoZhunCamera) {
			StopLookAtPoint();
		}

		switch(CamState) {
		case CartoonShootCamState.GenSuiCamera:
			break;
			
		case CartoonShootCamState.DingDianMiaoZhunCamera:
			StartLookAtPoint();
			//MainCamTran.LookAt(CamAimPoint);

			/*if (Time.realtimeSinceStartup - timeMiaoZhunVal > 0.1f) {
				timeMiaoZhunVal = Time.realtimeSinceStartup;
				Vector3 posA = CamAimPoint.position;
				Vector3 posB = MainCamTran.position;
				Vector3 vecF = posA - posB;
				MainCamTran.forward = Vector3.Lerp(MainCamTran.forward, vecF.normalized, 0.5f);
			}*/
			break;
			
		case CartoonShootCamState.DingDianBuMiaoZhunCamera:
			break;
			
		case CartoonShootCamState.ZiYouYiDongCamera:
			break;
		}

		UpdateGenSuiCamTran();
	}

	void StartLookAtPoint()
	{
		if (IsLookAtPoint) {
			return;
		}
		IsLookAtPoint = true;
		StartCoroutine(LoopLookAtPoint());
	}

	void StopLookAtPoint()
	{
		IsLookAtPoint = false;
		StopCoroutine(LoopLookAtPoint());
	}

	IEnumerator LoopLookAtPoint()
	{
		while (IsLookAtPoint) {
			yield return new WaitForSeconds(0.01f);
			MainCamTran.LookAt(CamAimPoint);
		}
		yield break;
	}
	
	void OnDrawGizmosSelected()
	{
		if (!enabled) {
			return;
		}

		if (CartoonShootPathScript != null) {
			CartoonShootPathScript.DrawPath();
		}
	}

	public void ActiveGenSuiCamera(Transform tran)
	{
		CamState = CartoonShootCamState.GenSuiCamera;
		
		GenSuiCamTran = tran;
		MainCamTran.parent = GenSuiCamTran;
		MainCamTran.localPosition = Vector3.zero;
		MainCamTran.localEulerAngles = Vector3.zero;
	}

	public void ActiveDingDianMiaoZhunCamera(Transform tranCamPoint, Transform tranAimPoint)
	{
		CamState = CartoonShootCamState.DingDianMiaoZhunCamera;

		MainCamTran.parent = null;
		MainCamTran.position = tranCamPoint.position;
		MainCamTran.rotation = tranCamPoint.rotation;

		CamAimPoint = tranAimPoint;
	}

	public void ActiveDingDianBuMiaoZhunCamera(Transform tranCamPoint)
	{
		CamState = CartoonShootCamState.DingDianBuMiaoZhunCamera;
		
		MainCamTran.parent = null;
		MainCamTran.position = tranCamPoint.position;
		MainCamTran.rotation = tranCamPoint.rotation;
	}

	public void ActiveZiYouYiDongCamera(Transform tranCamPath)
	{
		CamState = CartoonShootCamState.ZiYouYiDongCamera;
		if (tranCamPath.childCount < 1) {
			Debug.LogError("ActiveZiYouYiDongCamera -> childCount was wrong!");
			return;
		}
		MainCamTran.parent = null;
		MarkCurrentIndex = 0;
		MainCamTran.position = tranCamPath.GetChild(MarkCurrentIndex).position;
		MainCamTran.rotation = tranCamPath.GetChild(MarkCurrentIndex).rotation;
		ZiYouYiDongCamPtahTran = tranCamPath;
		CartoonShootPathScript = tranCamPath.GetComponent<CartoonShootPathCtrl>();

		MoveCameraByZiYouYiDongPath();
	}

	void MoveCameraByZiYouYiDongPath()
	{
		if (CartoonShootPathScript.iTweenEventIndex != -1) {
			ITweenEventArray[CartoonShootPathScript.iTweenEventIndex].enabled = true;
		}
		else {
			
			if (CartoonShootPathScript.IsMoveByPathSpeed) {
				
				iTweenEvent itweenScript = CartoonShootPathScript.ITweenEventCom;
				if (itweenScript != null) {
					itweenScript.enabled = true;
				}
				else {
					
					if (ZiYouYiDongCamPtahTran.childCount <= 1) {
						return;
					}
					
					List<Transform> nodes = new List<Transform>(ZiYouYiDongCamPtahTran.GetComponentsInChildren<Transform>()){};
					nodes.Remove(ZiYouYiDongCamPtahTran);
					if (CartoonShootPathScript.IsAimObj) {
						iTween.MoveTo(CameraObj, iTween.Hash("path", nodes.ToArray(),
						                                     "speed", CartoonShootPathScript.MoveSpeed,
						                                     "looktarget", CartoonShootPathScript.AimTran,
						                                     "looktime", CartoonShootPathScript.LookTime,
						                                     "easeType", iTween.EaseType.linear,
						                                     "oncomplete", "OnFinishMoveCameraByZiYouYiDongPath"));
					}
					else {
						iTween.MoveTo(CameraObj, iTween.Hash("path", nodes.ToArray(),
						                                     "speed", CartoonShootPathScript.MoveSpeed,
						                                     "orienttopath", true,
						                                     "easeType", iTween.EaseType.linear,
						                                     "oncomplete", "OnFinishMoveCameraByZiYouYiDongPath"));
					}
				}
			}
			else {
				
				Transform tran_0 = ZiYouYiDongCamPtahTran.GetChild(MarkCurrentIndex);
				Transform tran_1 = ZiYouYiDongCamPtahTran.GetChild(MarkCurrentIndex + 1);
				CartoonShootMark mark = tran_0.GetComponent<CartoonShootMark>();
				Transform[] path = new Transform[2];
				
				tran_0.position = MainCamTran.position;
				tran_0.rotation = MainCamTran.rotation;
				path[0] = tran_0;
				path[1] = tran_1;
				if (CartoonShootPathScript.IsAimObj) {
					iTween.MoveTo(CameraObj, iTween.Hash("path", path,
					                                     "speed", mark.MoveSpeed,
					                                     "looktarget", CartoonShootPathScript.AimTran,
					                                     "looktime", CartoonShootPathScript.LookTime,
					                                     "easeType", iTween.EaseType.linear,
					                                     "oncomplete", "OnFinishMoveCameraByZiYouYiDongPath"));
				}
				else {
					
					iTween.LookTo(CameraObj, iTween.Hash("path", path,
					                                     "looktarget", tran_1,
					                                     "time", mark.LookTime,
					                                     "easeType", iTween.EaseType.linear,
					                                     "oncomplete", "OnFinishMoveCameraByRotation"));
				}
			}
		}
	}

	void OnFinishMoveCameraByRotation()
	{
		Transform tran_0 = ZiYouYiDongCamPtahTran.GetChild(MarkCurrentIndex);
		Transform tran_1 = ZiYouYiDongCamPtahTran.GetChild(MarkCurrentIndex + 1);
		CartoonShootMark mark = tran_0.GetComponent<CartoonShootMark>();
		Transform[] path = new Transform[2];
		
		tran_0.position = MainCamTran.position;
		tran_0.rotation = MainCamTran.rotation;
		path[0] = tran_0;
		path[1] = tran_1;
		iTween.MoveTo(CameraObj, iTween.Hash("path", path,
		                                     "speed", mark.MoveSpeed,
		                                     "orienttopath", true,
		                                     "easeType", iTween.EaseType.linear,
		                                     "oncomplete", "OnFinishMoveCameraByZiYouYiDongPath"));
	}

	void OnFinishMoveCameraByZiYouYiDongPath()
	{
		if (CartoonShootPathScript.IsMoveByPathSpeed) {
			Debug.Log("OnFinishMoveCameraByZiYouYiDongPath...");
			CamState = CartoonShootCamState.NULL;
			return;
		}
		else {
			if (MarkCurrentIndex >= ZiYouYiDongCamPtahTran.childCount - 2) {
				Debug.Log("OnFinishMoveCameraByZiYouYiDongPath...");
				CamState = CartoonShootCamState.NULL;
				return;
			}
			MarkCurrentIndex++;
			MoveCameraByZiYouYiDongPath();
		}
	}
	
	public void CheckGenSuiCamTranStartGame()
	{
		if (GenSuiCamTran != null) {
			CamState = CartoonShootCamState.GenSuiCamera;

			MainCamTran.parent = GenSuiCamTran;
			MainCamTran.localPosition = Vector3.zero;
			MainCamTran.localEulerAngles = Vector3.zero;
		}
	}

	public void UpdateGenSuiCamTran()
	{
		if (!IsUpdateGenSuiCamTran) {
			return;
		}
		
		if (TestGenSuiCamTan != null) {
			MainCamTran.position = TestGenSuiCamTan.position;
			MainCamTran.rotation = TestGenSuiCamTan.rotation;
		}
	}
}

public enum CartoonShootCamState
{
	NULL,
	GenSuiCamera,
	DingDianMiaoZhunCamera,
	DingDianBuMiaoZhunCamera,
	ZiYouYiDongCamera
}
