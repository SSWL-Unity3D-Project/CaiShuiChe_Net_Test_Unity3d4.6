using UnityEngine;
using System.Collections;

public class NengLiangLiZiCtrl : MonoBehaviour {

	public float MvSpeed = 0.7f;
	Transform AimTran;
	Transform LiZiTran;
	float TimeSpawn;
	// Use this for initialization
	void Start()
	{
		TimeSpawn = Time.realtimeSinceStartup;
		AimTran = WaterwheelCameraCtrl.GetInstance().NengLiangAimTran;
		LiZiTran = transform;
	}
	
	// Update is called once per frame
	void Update()
	{
		float dis = Vector3.Distance(LiZiTran.position, AimTran.position);
		if (dis <= 0.5f || Time.realtimeSinceStartup - TimeSpawn > 8f) {
			XingXingCtrl.GetInstance().AddStarNum();
			Destroy(gameObject);
			return;
		}

		Vector3 forwardVal = AimTran.position - LiZiTran.position;
		float valSpeed = WaterwheelPlayerCtrl.GetInstance().GetMoveSpeed();
		valSpeed = (valSpeed / (3.6f * 30)) + MvSpeed;
		Vector3 mvPos = forwardVal.normalized * valSpeed;
		LiZiTran.position += mvPos;
	}
}
