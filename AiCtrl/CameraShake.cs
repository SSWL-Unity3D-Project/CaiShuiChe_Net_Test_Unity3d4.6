using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour {

	RadialBlur RadialBlurScript;
	MotionBlur BlurScript;
	private Transform mCamTran;	//Main Camera transform
	private float fCamShakeImpulse = 0.0f;	//Camera Shake Impulse
	float minShakeVal = 0.05f;
	
	public static bool IsActiveCamOtherPoint;
	private Transform mCamPoint_forward = null; //forward
	private Transform mCamPoint_right = null; //right
	private Transform mCamPoint_free = null;
	Transform mCamPoint_first;
	Transform CamPointUp;
	
	CamDirPos CamDir;
	Transform CamPoint_back = null; //back
	Transform CamPointBackNear = null;
	
	public static float BlurStrengthPubu = 2.2f;
	public static float BlurStrengthHuanYingFu = 2f;
	
	bool IsActiveRadialBlur;
	float MinSp = 55f;
	float MaxSp = 90f;
	float MinBlur = 0.2f;
	float MaxBlur = 1.5f;
	float KeyBlur = 0f;

	GameObject PlayerObj;
	public static AudioSource AudioSoureFengXue;
	bool IsActiveHuanYingFu;

	static private CameraShake _Instance;
	public static CameraShake GetInstance()
	{
		return _Instance;
	}

	// Use this for initialization
	void Awake () {
		_Instance = this;
		IsActiveCamOtherPoint = false;
		mCamTran = camera.transform;
		RadialBlurScript = GetComponent<RadialBlur>();
		RadialBlurScript.enabled = false;

		//CreateAudioSoureFengXue();
		BlurScript = GetComponent<MotionBlur>();
		BlurScript.enabled = false;

		InitSetPlayerSpeedRadialBlur();
		//InvokeRepeating("TestCamShake", 3f, 3f);
	}

	void TestCamShake()
	{
		SetCameraShakeImpulseValue();
	}

	void FixedUpdate()
	{
		if (!IsActiveCamOtherPoint) {
			//camera transitions
			CameraMain();
		}
	}

	void Update()
	{
		if (IsActiveCamOtherPoint) {
			ActiveCameraOtherPoint();
		}
		
		/*if (Input.GetKeyUp(KeyCode.P)) {
			SetActiveCamOtherPoint(true, CamDirPos.PLAYER_UP, null);
		}*/

		/*if (Input.GetKeyUp(KeyCode.P)) {
			HeatDistort.GetInstance().InitPlayScreenWater();
		}*/

		/*if (Input.GetKeyUp(KeyCode.P)) {
			GameCtrlXK.GetInstance().CreateScreenWaterParticle();
		}*/
	}

	void CreateAudioSoureFengXue()
	{
		AudioSoureFengXue = gameObject.AddComponent<AudioSource>();
		AudioSoureFengXue.clip = AudioListCtrl.GetInstance().AudioFengXue;
		AudioSoureFengXue.loop = true;
		AudioSoureFengXue.Stop();
	}
	
	public void PlayAudioSoureFengXue()
	{
		if (AudioSoureFengXue == null) {
			CreateAudioSoureFengXue();
		}

		if (AudioSoureFengXue.isPlaying) {
			return;
		}
		
		AudioSoureFengXue.volume = 0f;
		AudioSoureFengXue.Play();
		StartCoroutine(AddAudioSoureFengXueVolume());
	}
	
	IEnumerator AddAudioSoureFengXueVolume()
	{
		while (AudioSoureFengXue.volume < 1f) {
			AudioSoureFengXue.volume += 0.01f;
			yield return new WaitForSeconds(0.03f);
		}
	}
	
	public void StopAudioSoureFengXue()
	{
		StartCoroutine(SubAudioSoureFengXueVolume());
	}
	
	IEnumerator SubAudioSoureFengXueVolume()
	{
		if (AudioSoureFengXue == null) {
			yield break;
		}

		while (AudioSoureFengXue.volume < 1f) {
			AudioSoureFengXue.volume -= 0.01f;
			yield return new WaitForSeconds(0.03f);
		}
		AudioSoureFengXue.Stop();
	}

	public bool CheckCamForwardHit()
	{
		if (PlayerObj == null) {
			if (GameCtrlXK.PlayerTran != null) {
				PlayerObj = GameCtrlXK.PlayerTran.gameObject;
			}
			return false;
		}

		if (GameCtrlXK.PlayerTran.forward.y < 0f) {
			return false;
		}

		if (CamPoint_back == null) {
			if (GlobalData.GetInstance().gameMode == GameMode.SoloMode) {
				CamPointBackNear = WaterwheelCameraCtrl.CamPointBackNear;
				CamPoint_back = WaterwheelCameraCtrl.CamPoint_back;
			}
			else {
				CamPointBackNear = WaterwheelCameraNetCtrl.CamPointBackNear;
				CamPoint_back = WaterwheelCameraNetCtrl.CamPoint_back;
			}
			return false;
		}

		Vector3 startPos = CamPointBackNear.position;
		Vector3 endPos = CamPoint_back.position;
		float dis = Vector3.Distance(startPos, endPos);
		if (startPos.y > endPos.y) {
			startPos.y = endPos.y;
		}
		else {
			endPos.y = startPos.y;
		}
		
		RaycastHit hitInfo;
		Vector3 forwardVal = endPos - startPos;
		Physics.Raycast(startPos, forwardVal, out hitInfo, dis);
		if (hitInfo.collider != null) {
			GameObject objHit = hitInfo.collider.gameObject;
			Transform tranRootHit = objHit.transform.root;

			if (PlayerObj != tranRootHit.gameObject
			    && tranRootHit.name != GameCtrlXK.MissionCleanup.name
			    && tranRootHit.GetComponent<CamNotCheckHitObj>() == null
			    && objHit.GetComponent<CamNotCheckHitObj>() == null) {
				forwardVal = forwardVal.normalized;
				mCamTran.position = hitInfo.point - forwardVal * 0.6f;
				return true;
			}
		}
		return false;
	}

	public void SetActiveCamOtherPoint(bool isActive, CamDirPos camDirPosVal, Transform camFreePoint)
	{
		IsActiveCamOtherPoint = isActive;
		if (isActive) {
			switch (camDirPosVal) {
			case CamDirPos.BACK:
				Debug.LogError("SetActiveCamOtherPoint -> camDirPosVal should not is BackCamPoint");
				CamPointUp = null;
				CamPointUp.name = "null";
				return;

			case CamDirPos.FREE:
				if (camFreePoint == null) {
					Debug.LogError("SetActiveCamOtherPoint -> camFreePoint should not is null");
					CamPointUp = null;
					CamPointUp.name = "null";
					return;
				}
				mCamPoint_free = camFreePoint;
				break;

			case CamDirPos.FIRST:
				GameCtrlXK.GetInstance().SetPlayerBoxColliderState(true);
				break;
			}
		}
		else {
			Time.timeScale = 1f;
			if (IntoPuBuCtrl.IsIntoPuBu) {
				IntoPuBuCtrl.PlayerMvSpeed += 20f;
			}
			GameCtrlXK.GetInstance().InitDelayClosePlayerBoxCollider();
		}
		CamDir = camDirPosVal;
	}

	public void SetCameraPointInfo()
	{
		if (GlobalData.GetInstance().gameMode == GameMode.SoloMode) {
			WaterwheelPlayerCtrl aimObjScripte = WaterwheelPlayerCtrl.GetInstance();
			if(aimObjScripte == null)
			{
				return;
			}
			
			mCamPoint_first = aimObjScripte.GetCamPointFirst();
			mCamPoint_right = aimObjScripte.GetCamPointRight();
			mCamPoint_forward = aimObjScripte.GetCamPointForward();
			CamPointUp = aimObjScripte.GetCamPointUp();
		}
		else {
			WaterwheelPlayerNetCtrl aimObjNetScripte = WaterwheelPlayerNetCtrl.GetInstance();
			if(aimObjNetScripte == null)
			{
				return;
			}
			
			mCamPoint_first = aimObjNetScripte.GetCamPointFirst();
			mCamPoint_right = aimObjNetScripte.GetCamPointRight();
			mCamPoint_forward = aimObjNetScripte.GetCamPointForward();
			CamPointUp = aimObjNetScripte.GetCamPointUp();
		}
	}

	void ActiveCameraOtherPoint()
	{
		switch(CamDir)
		{	
		case CamDirPos.FORWARD:
			mCamTran.position = mCamPoint_forward.position;
			mCamTran.eulerAngles = mCamPoint_forward.eulerAngles;
			break;
			
		case CamDirPos.RIGHT:
			mCamTran.position = mCamPoint_right.position;
			mCamTran.eulerAngles = mCamPoint_right.eulerAngles;
			break;
			
		case CamDirPos.FREE:
			mCamTran.position = mCamPoint_free.position;
			mCamTran.eulerAngles = mCamPoint_free.eulerAngles;
			break;
			
		case CamDirPos.FIRST:
			mCamTran.position = mCamPoint_first.position;
			mCamTran.eulerAngles = mCamPoint_first.eulerAngles;
			break;
			
		case CamDirPos.PLAYER_UP:
			mCamTran.position = CamPointUp.position;
			mCamTran.eulerAngles = CamPointUp.eulerAngles;
			break;
		}
	}

	/*
	*	FUNCTION: Controls camera movements
	*	CALLED BY: FixedUpdate()
	*/
	private void CameraMain()
	{
		//make the camera shake if the fCamShakeImpulse is not zero
		if(fCamShakeImpulse > 0.0f)
		{
			shakeCamera();
		}
	}
	
	/*
	*	FUNCTION: Make the camera vibrate. Used for visual effects
	*/
	void shakeCamera()
	{
		Vector3 pos = mCamTran.position;
		pos.x += Random.Range(0, 100) % 2 == 0 ? Random.Range(-fCamShakeImpulse, -minShakeVal) : Random.Range(minShakeVal, fCamShakeImpulse);
		pos.y += Random.Range(0, 100) % 2 == 0 ? Random.Range(-fCamShakeImpulse, -minShakeVal) : Random.Range(minShakeVal, fCamShakeImpulse);
		pos.z += Random.Range(0, 100) % 2 == 0 ? Random.Range(-fCamShakeImpulse, -minShakeVal) : Random.Range(minShakeVal, fCamShakeImpulse);
		mCamTran.position = pos;

		fCamShakeImpulse -= Time.deltaTime * fCamShakeImpulse * 4.0f;
		if(fCamShakeImpulse < minShakeVal)
		{
			fCamShakeImpulse = 0.0f;
			BlurScript.enabled = false;
		}
	}

	/*
	*	FUNCTION: Set the intensity of camera vibration
	*	PARAMETER 1: Intensity value of the vibration
	*/
	public void SetCameraShakeImpulseValue()
	{
		if(fCamShakeImpulse > 0.0f || IsActiveCamOtherPoint)
		{
			return;
		}
		fCamShakeImpulse = 0.5f;
		//BlurScript.enabled = true;
		AudioListCtrl.PlayAudio(AudioListCtrl.GetInstance().AudioShipHit_1);

		if (IntoPuBuCtrl.IsIntoPuBu) {
			HeatDistort.GetInstance().InitPlayScreenWater();
			//GameCtrlXK.GetInstance().CreateScreenWaterParticle();
		}
	}

	public void SetIsActiveHuanYingFu(bool isActive)
	{
		IsActiveHuanYingFu = isActive;
	}

	public bool GetIsActiveHuanYingFu()
	{
		return IsActiveHuanYingFu;
	}

	public void SetRadialBlurActive(bool isActive, float strengthVal)
	{
		if (IsActiveRadialBlur == isActive) {
			return;
		}
		RadialBlurScript.SampleStrength = strengthVal;
		RadialBlurScript.enabled = isActive;
		IsActiveRadialBlur = isActive;
	}

	void InitSetPlayerSpeedRadialBlur()
	{
		float DisSp = MaxSp - MinSp;
		if (DisSp == 0) {
			DisSp = 1f;
		}
		KeyBlur = (MaxBlur - MinBlur) / DisSp;
	}

	public void SetPlayerSpeedRadialBlur(float playerSpeed)
	{
		if (IsActiveRadialBlur) {
			return;
		}

		if (playerSpeed < MinSp) {
			if (RadialBlurScript.enabled) {
				RadialBlurScript.enabled = false;
			}
			return;
		}

		if (playerSpeed > MaxSp) {
			return;
		}

		float strengthVal = KeyBlur * (playerSpeed - MinSp) + MinBlur;
		if (strengthVal > MaxBlur) {
			strengthVal = MaxBlur;
		}

		RadialBlurScript.SampleStrength = strengthVal;
		if (!RadialBlurScript.enabled) {
			RadialBlurScript.enabled = true;
		}
	}
}
