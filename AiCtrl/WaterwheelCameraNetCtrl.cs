using UnityEngine;
using System.Collections;

using Frederick.ProjectAircraft;

public class WaterwheelCameraNetCtrl : MonoBehaviour {
	
	public GameObject PlayerGunWaterObj;

	static private float smoothPer = 1f;
	
	static public bool bIsAimPlayer = false;
	
	private float mSmooth = 15f;
	private float smoothVal = 0f;
	private float followSpeed = 0.01f;
	
	//private Transform CamPoint_backTmp = null;
	private Transform AimPoint = null;
	public static Transform CamPoint_back = null; //back
	public static Transform CamPointBackNear = null;
	
	private Transform BackPointParent;
	Vector3 BackLocalPos;
	
	static private Transform mCamTran = null;
	private CamDirPos camDir = CamDirPos.BACK;
	
	public static WaterwheelCameraNetCtrl _Instance;
	public static WaterwheelCameraNetCtrl GetInstance()
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
		
		Screen.showCursor = false;
		smoothVal =  mSmooth * 0.015f;
		
		AudioManager.Instance.SetParentTran(transform);
		SetPlayerGunWaterObjActive(false);

		MakeCamFollowPlayer(); //test
	}
	
	void FixedUpdate()
	{
		if (!CameraShake.IsActiveCamOtherPoint) {
			setCameraPos();
		}
	}
	
	public void setAimPlayerInfo()
	{
		WaterwheelPlayerNetCtrl aimObjScripte = WaterwheelPlayerNetCtrl.GetInstance();
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
	
	void setCameraPos()
	{
		if(AimPoint == null)
		{
			return;
		}
		
		//		if(Time.timeScale != 0f)
		//		{
		//			camDir = CamDir.BACK;
		//		}
		
		float aimObjMoveSpeed = WaterwheelPlayerNetCtrl.GetInstance().GetMoveSpeed();
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
	
	public void SetPlayerGunWaterObjActive(bool isActive)
	{
		if (isActive) {
			PlayerGunWaterObj.SetActive(isActive);
		}
		if (!IsInvoking("DelayHiddenGunWaterObj")) {
			Invoke("DelayHiddenGunWaterObj", 1.5f);
		}
	}

	void DelayHiddenGunWaterObj()
	{
		PlayerGunWaterObj.SetActive(false);
	}
}