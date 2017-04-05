using UnityEngine;
using System.Collections;

using Frederick.ProjectAircraft;

public enum CamDirPos
{
	FORWARD,
	BACK,
	RIGHT,
	FREE,
	FIRST,
	PLAYER_UP
}

public class WaterwheelCameraCtrl : MonoBehaviour {
	
	public float TimeValNengLiang = 0.2f;
	public GameObject NengLiangLiZi;
	public Transform NengLiangAimTran;
	static private float smoothPer = 1f;

	static public bool bIsAimPlayer = false;
	
	private float mSmooth = 15f;
	private float smoothVal = 0f;
	private float followSpeed = 0.01f;

	private Transform AimPoint = null;
	public static Transform CamPoint_back = null; //back
	public static Transform CamPointBackNear = null;

	private Transform BackPointParent;
	Vector3 BackLocalPos;

	static private Transform mCamTran = null;
	private CamDirPos camDir = CamDirPos.BACK;
	
	bool IsFollowPlayer;
	Vector3 StartCamPos;
	Vector3 StartCamForward;

	public static WaterwheelCameraCtrl _Instance;
	public static WaterwheelCameraCtrl GetInstance()
	{
		return _Instance;
	}

	void Awake()
	{
		_Instance = this;

		float[] distances = new float[32];
		for(int i = 0; i < 32; i++)
		{
			distances[i] = 400f;
		}
		camera.layerCullDistances = distances;
		bIsAimPlayer = false;
		
		Random.seed = (int)(Time.realtimeSinceStartup * 100000f);
		mCamTran = transform;
		StartCamPos = mCamTran.position;
		StartCamForward = mCamTran.forward;
		
		Screen.showCursor = false;
		smoothVal =  mSmooth * 0.015f;

		AudioManager.Instance.SetParentTran(transform);

		MakeCamFollowPlayer(); //test
	}

	void FixedUpdate()
	{
		if (CartoonShootPlayerCtrl.IsActiveRunPlayer || CartoonShootTrigger.IsActiveShootCamera) {
			this.enabled = false;
			return;
		}

		if (!CameraShake.IsActiveCamOtherPoint) {
			setCameraPos();
		}
	}

	public void setAimPlayerInfo()
	{
		WaterwheelPlayerCtrl aimObjScripte = WaterwheelPlayerCtrl.GetInstance();
		if(aimObjScripte == null)
		{
			return;
		}
		
		smoothPer = 1f;
		AimPoint = aimObjScripte.GetCamAimPoint();
		CamPoint_back = aimObjScripte.GetCamPointBackFar();
		CamPointBackNear = aimObjScripte.GetCamPointBackNear();
		BackPointParent = CamPoint_back.parent;
		BackLocalPos = CamPoint_back.localPosition;
	}

	public void MakeCamFollowPlayer()
	{
		bIsAimPlayer = true;
	}

	public void EnableCamPointFirst()
	{
		camDir = CamDirPos.FIRST;
	}
	
	public void EnableCamPointBack()
	{
		camDir = CamDirPos.BACK;
	}

	public void ActivePlayerFollowCamera()
	{
		if (IsFollowPlayer) {
			return;
		}
		ResetPlayerFollowCameraPos();
		IsFollowPlayer = true;
	}
	
	public void ResetPlayerFollowCameraPos()
	{
		mCamTran.position = StartCamPos;
		mCamTran.forward = StartCamForward;
	}

	void setCameraPos()
	{
		if (!IsFollowPlayer)
		{
			return;
		}

		if(AimPoint == null)
		{
			return;
		}
		
//		if(Time.timeScale != 0f)
//		{
//			camDir = CamDir.BACK;
//		}

		float aimObjMoveSpeed = WaterwheelPlayerCtrl.GetInstance().GetMoveSpeed();
		if(!bIsAimPlayer)
		{
			mCamTran.position = Vector3.Lerp(mCamTran.position, CamPoint_back.position, followSpeed);
			mCamTran.LookAt(AimPoint);
		}
		else
		{
			Vector3 camPos = transform.position;
			switch(camDir)
			{	
			case CamDirPos.BACK:
				bool isAimBike = true;
				if(followSpeed < smoothVal)
				{
					followSpeed += 0.001f;
					if(followSpeed > smoothVal)
					{
						followSpeed = smoothVal;
					}
					
					camPos = Vector3.Lerp(mCamTran.position,
					                      CamPoint_back.position, followSpeed);
				}
				else
				{
					float dis = Vector3.Distance(mCamTran.position,
					                             CamPoint_back.position);
					if(aimObjMoveSpeed > 0f)
					{
						camPos = Vector3.Lerp(mCamTran.position,
						                      CamPoint_back.position, smoothPer * smoothVal);
						
						if(aimObjMoveSpeed < 5 && Time.timeScale == 1f)
						{
							isAimBike = false;
							camPos = transform.position;
							CamPoint_back.parent = null;
						}
						else if(CamPoint_back.parent == null)
						{
							CamPoint_back.parent = BackPointParent;
							CamPoint_back.localPosition = BackLocalPos;
						}
					}
					else
					{
						if(dis > 0.1f)
						{
							camPos = Vector3.Lerp(mCamTran.position,
							                      CamPoint_back.position, smoothVal);
						}
						else
						{
							isAimBike = false;
							camPos = transform.position;
						}
					}
				}

				if (!CameraShake.GetInstance().CheckCamForwardHit()) {
					mCamTran.position = camPos;
				}

				if(isAimBike)
				{
					mCamTran.LookAt(AimPoint);
				}
				break;
			}
		}
	}

	public void ResetPlayerCameraPos()
	{
		Vector3 pos = CamPoint_back.position;
		if (GameCtrlXK.PlayerTran) {
			pos = BackLocalPos + GameCtrlXK.PlayerTran.position;

			CamPoint_back.parent = BackPointParent;
			CamPoint_back.localPosition = BackLocalPos;
		}
		mCamTran.position = pos;
		mCamTran.LookAt(AimPoint);
	}

	public void SpawnPlayerNengLiangLiZi(Vector3 pos)
	{
		if (NengLiangTiaoCtrl.IsStartPlay) {
			return;
		}
		StartCoroutine(DelaySpawnPlayerNengLiangLiZi(pos));
	}

	IEnumerator DelaySpawnPlayerNengLiangLiZi(Vector3 pos)
	{
		float timeVal = GameCtrlXK.GetInstance().TimeNpcSpawnExplode + TimeValNengLiang;
		yield return new WaitForSeconds(timeVal);
		Instantiate(NengLiangLiZi, pos, transform.rotation);
	}
}