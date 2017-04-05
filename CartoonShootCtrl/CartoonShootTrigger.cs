using UnityEngine;
using System.Collections;

public class CartoonShootTrigger : MonoBehaviour {
	
	public CartoonShootCamState CamState = CartoonShootCamState.NULL;
	public Transform GenSuiTran;
	public Transform DingDianTranCamPoint;
	public Transform DingDianMiaoZhunTranAimPoint;
	public Transform ZiYouYiDongTranCamPath;

	public GameObject[] NpcObj;
	public string[] ActionState;
	Animator[] AniComponent = new Animator[10];

	[Range(0.001f, 100f)] public float WorldTime = 1f;
	[Range(0.5f, 100f)] public float ResetWorldTime = 1f;

	public static bool IsActiveShootCamera;
	static bool IsActiveResetWorldTime;
	float TimeRecord;

	Transform TriggerTran;
	float TimeVal;
	
	// Use this for initialization
	void Start()
	{
		TriggerTran = transform;
		switch (CamState) {
		case CartoonShootCamState.GenSuiCamera:
			if (GenSuiTran == null) {
				GenSuiTran.name = "null";
			}
			break;
			
		case CartoonShootCamState.DingDianMiaoZhunCamera:
			if (DingDianTranCamPoint == null) {
				DingDianTranCamPoint.name = "null";
			}

			if (DingDianMiaoZhunTranAimPoint == null) {
				DingDianMiaoZhunTranAimPoint.name = "null";
			}
			break;
			
		case CartoonShootCamState.DingDianBuMiaoZhunCamera:
			if (DingDianTranCamPoint == null) {
				DingDianTranCamPoint.name = "null";
			}
			break;
			
		case CartoonShootCamState.ZiYouYiDongCamera:
			if (ZiYouYiDongTranCamPath == null) {
				ZiYouYiDongTranCamPath.name = "null";
			}
			break;
		}

		for (int i = 0; i < NpcObj.Length; i++) {
			if (ActionState[i] != "" && NpcObj[i] != null) {
				AniComponent[i] = NpcObj[i].GetComponent<Animator>();
			}
		}
	}
	
	// Update is called once per frame
	void Update()
	{
		if (Time.realtimeSinceStartup - TimeVal < 0.1f) {
			return;
		}
		TimeVal = Time.realtimeSinceStartup;
		
		Vector3 vecA = TriggerTran.position;
		Vector3 vecB = CartoonShootCamCtrl.GZhuJiaoTran.position;
		vecA.y = vecB.y = 0f;
		float dis = Vector3.Distance(vecA, vecB);
		if (dis <= 15f) {
			
			vecA = TriggerTran.forward;
			vecB = TriggerTran.position - CartoonShootCamCtrl.GZhuJiaoTran.position;
			vecA.y = vecB.y = 0f;
			float cosAB = Vector3.Dot(vecA, vecB);
			if (cosAB <= 0f) {
				IsActiveShootCamera = true;
				gameObject.SetActive(false);

				if (IsActiveResetWorldTime) {
					IsActiveResetWorldTime = false;
					Invoke("DelayCheckResetWorldTime", 0.3f);
				}
				else {
					DelayCheckResetWorldTime();
				}
				TimeRecord = Time.realtimeSinceStartup;
				Time.timeScale = WorldTime;

				for (int i = 0; i < ActionState.Length; i++) {
					if (AniComponent[i] != null) {
						AniComponent[i].SetBool(ActionState[i], true);
					}
				}

				switch (CamState) {
				case CartoonShootCamState.GenSuiCamera:
					CartoonShootCamCtrl.GetInstance().ActiveGenSuiCamera(GenSuiTran);
					break;
					
				case CartoonShootCamState.DingDianMiaoZhunCamera:
					CartoonShootCamCtrl.GetInstance().ActiveDingDianMiaoZhunCamera(DingDianTranCamPoint,
					                                                               DingDianMiaoZhunTranAimPoint);
					break;
					
				case CartoonShootCamState.DingDianBuMiaoZhunCamera:
					CartoonShootCamCtrl.GetInstance().ActiveDingDianBuMiaoZhunCamera(DingDianTranCamPoint);
					break;
					
				case CartoonShootCamState.ZiYouYiDongCamera:
					CartoonShootCamCtrl.GetInstance().ActiveZiYouYiDongCamera(ZiYouYiDongTranCamPath);
					break;
				}
			}
		}
	}

	void DelayCheckResetWorldTime()
	{
		IsActiveResetWorldTime = true;
		InvokeRepeating("CheckResetWorldTime", 0f, 0.1f);
	}

	void CheckResetWorldTime()
	{
		if (!IsActiveResetWorldTime) {
			CancelInvoke("CheckResetWorldTime");
			return;
		}

		if (Time.realtimeSinceStartup - TimeRecord < ResetWorldTime) {
			return;
		}
		Time.timeScale = 1f;
	}
}