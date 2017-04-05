using UnityEngine;
using System.Collections;
using Frederick.ProjectAircraft;

public class MoveCameraByPath : MonoBehaviour {

	public CameraPathCtrl [] CamPath = new CameraPathCtrl[9];
	public CameraPathCtrl CamPathTest;
	public static bool IsMovePlayer;
	bool IsInitCamPathInfo;
	int [] PathArray = new int[3];
	
	int PathMoveNum;
	GameObject GmObj;
	Transform GmTran;
	NoiseEffect NoiseScript;

	//float SubScreenOverVal = 0.1f;
	bool IsPlayAudioBt;
	bool IsInitChangeScreenOverlay;

	//float IntensityValMin = 0f;
	//float IntensityValMax = 1.5f;
	ScreenOverlay ScreenOverScript;

	static MoveCameraByPath _Instance;
	public static MoveCameraByPath GetInstance()
	{
		return _Instance;
	}

	// Use this for initialization
	void Awake () {
		_Instance = this;
		IsMovePlayer = false;
		/*if (GlobalData.GetInstance().gameMode == GameMode.SoloMode) {
			IsMovePlayer = false;
		}
		else {
			IsMovePlayer = true;
		}*/
		NoiseScript = GetComponent<NoiseEffect>();
		ScreenOverScript = GetComponent<ScreenOverlay>();
		ScreenOverScript.enabled = false;

		GmObj = gameObject;
		GmTran = transform;
		InitCamPathInfo();

		if (CamPathTest != null) {
			Invoke("TestDelayMoveCamera", 1f);
		}
		InputEventCtrl.GetInstance().ClickStartBtOneEvent += clickStartBtOneEvent;
	}

	void TestDelayMoveCamera()
	{
		MoveCamera(CamPathTest);
	}
	
	public bool GetIsInitCamPathInfo()
	{
		return IsInitCamPathInfo;
	}

	void InitCamPathInfo()
	{
		if (IsInitCamPathInfo) {
			return;
		}
		IsInitCamPathInfo = true;

		int [ , ] arrayPathNum = {
			{0, 1, 2},
			{0, 2, 1},
			{1, 0, 2},
			{1, 2, 0},
			{2, 0, 1},
			{2, 1, 0}
		};

		int randValA = 0;
		int randValB = Random.Range(0, 10000) % 6;
		randValA = Random.Range(0, 3000);
		if (randValA > 2000) {
			randValA = 6;
		}
		else if (randValA > 1000) {
			randValA = 3;
		}
		else {
			randValA = 0;
		}

		for (int i = 0; i < 3; i++) {
			PathArray[i] = randValA + arrayPathNum[randValB, i];
			/*int pathNumTest = PathArray[i] + 1;
			Debug.Log("PathArray ********** " + pathNumTest);*/
		}
	}

	public void StartMoveCamera()
	{
		if (CamPathTest != null) {
			Debug.LogError("StartMoveCamera -> CamPathTest is not null");
			return;
		}

		if (IsInitChangeScreenOverlay) {
			return;
		}

		if (!IsInitCamPathInfo) {
			InitCamPathInfo();
		}

		CameraPathCtrl path = CamPath[ PathArray[PathMoveNum] ];
		if (path == null) {
			Debug.LogError("path should not null, PathMoveNum = " + PathMoveNum);
			return;
		}
		//MoveCamera(path);

		PathMoveNum++;
		clickStartBtOne();
	}

	void MoveCamera(CameraPathCtrl path)
	{
		if (path == null) {
			Debug.LogError("MoveCamera -> path is null!");
			return;
		}
		NoiseScript.enabled = false;

		if (path.IsOnlyChangeRot) {
			TweenTransform TweenTranscript = GmObj.GetComponent<TweenTransform>();
			if (TweenTranscript == null) {
				TweenTranscript = gameObject.AddComponent<TweenTransform>();
			}
			TweenTranscript.enabled = false;

			GmTran.position = path.CamRotation_1.position;
			GmTran.rotation = path.CamRotation_1.rotation;

			TweenTranscript.from = path.CamRotation_1;
			TweenTranscript.to = path.CamRotation_2;
			TweenTranscript.duration = path.TimeVal;
			TweenTranscript.style = UITweener.Style.Once;
			TweenTranscript.ResetToBeginning();
			
			EventDelegate.Add(TweenTranscript.onFinished, delegate{
				OnFinishMoveCamera();
			});

			TweenTranscript.enabled = true;
			TweenTranscript.PlayForward();
		}
		else if (path.IsMvPosByCamPath) {
			GmTran.position = path.GetCamPathTranFirst().position;
			GmTran.rotation = path.GetCamPathTranFirst().rotation;

			iTween.MoveTo(GmObj, iTween.Hash("path", path.GetCamPathTranArray(),
			                                 "time", path.TimeVal,
			                                 "easeType", iTween.EaseType.linear,
			                                 "oncomplete", "OnFinishMoveCamera"));
		}
		else if (path.IsMvRotByCamPath) {
			GmTran.position = path.GetCamPathTranFirst().position;
			GmTran.rotation = path.GetCamPathTranFirst().rotation;

			iTween.MoveTo(GmObj, iTween.Hash("path", path.GetCamPathTranArray(),
			                                 "time", path.TimeVal,
			                                 "orienttopath", true,
			                                 "easeType", iTween.EaseType.linear,
			                                 "oncomplete", "OnFinishMoveCamera"));
		}
		else if (path.IsMvAimByCamPath) {
			GmTran.position = path.GetCamPathTranFirst().position;
			GmTran.rotation = path.GetCamPathTranFirst().rotation;
			
			iTween.MoveTo(GmObj, iTween.Hash("path", path.GetCamPathTranArray(),
			                                 "time", path.TimeVal,
			                                 "looktarget", path.CamAimPoint,
			                                 "easeType", iTween.EaseType.linear,
			                                 "oncomplete", "OnFinishMoveCamera"));
		}
	}

	void clickStartBtOneEvent(ButtonState val)
	{
		if(val == ButtonState.DOWN)
		{
			return;
		}
		clickStartBtOne();
	}
	
	void clickStartBtOne()
	{
		StartGameBtCtrl.GetInstance().CloseStartBtCartoon();
		StopMoveCamera();
	}

	void StopMoveCamera()
	{
		if (IsMovePlayer || IsInitChangeScreenOverlay) {
			return;
		}

		if (GetComponent<iTween>() != null) {
			iTween.Stop(gameObject);
		}

		TweenTransform TweenTranscript = GmObj.GetComponent<TweenTransform>();
		if (TweenTranscript != null && TweenTranscript.enabled) {
			TweenTranscript.enabled = false;
		}

		/*if (GameCtrlXK.AudioHitBt != null && !IsPlayAudioBt) {
			IsPlayAudioBt = true;
			Invoke("ResetIsPlayAudioBt", 0.8f);
			AudioManager.Instance.PlaySFX(GameCtrlXK.AudioHitBt);
		}*/
		NoiseScript.enabled = false;
		InitChangeScreenOverlay();
	}

	void ResetMoveCamera()
	{
		/*if (IsMovePlayer) {
			return;
		}
		//move camera over
		IsMovePlayer = true;*/

		if (!IsInitCamPathInfo) {
			return;
		}
		IsInitCamPathInfo = false;
		
		CancelInvoke("ActivePlayerFollowCamera");
		Invoke("ActivePlayerFollowCamera", 0.2f);
	}

	void ResetIsPlayAudioBt()
	{
		IsPlayAudioBt = false;
	}

	void OnFinishMoveCamera()
	{
		//Debug.Log("OnFinishMoveCamera...");
		if (!NoiseScript.enabled && CamPathTest == null) {
			
			if (IsInitChangeScreenOverlay) {
				return;
			}

			if (!IsPlayAudioBt) {
				IsPlayAudioBt = true;
				Invoke("ResetIsPlayAudioBt", 0.8f);
				AudioManager.Instance.PlaySFX(AudioListCtrl.GetInstance().AudioCameraSwitch);
			}

			NoiseScript.enabled = true;
			if (PathMoveNum >= 3) {
				StartGameBtCtrl.GetInstance().CloseStartBtCartoon();
				ResetMoveCamera();
				return;
			}

			CancelInvoke("StartMoveCamera");
			Invoke("StartMoveCamera", 0.2f);
		}
		/*else {
			IsMovePlayer = true;
		}*/
	}

	void InitChangeScreenOverlay()
	{
		if (IsInitChangeScreenOverlay) {
			return;
		}
		IsInitChangeScreenOverlay = true;

		/*ScreenOverScript.blendMode = ScreenOverlay.OverlayBlendMode.Multiply;
		ScreenOverScript.intensity = IntensityValMax;
		ScreenOverScript.enabled = true;*/
		StartCoroutine(MakeScreenOverlayInfoToMin());
	}

	IEnumerator MakeScreenOverlayInfoToMin()
	{
		/*while (ScreenOverScript.intensity > IntensityValMin) {
			ScreenOverScript.intensity -= SubScreenOverVal;
			yield return new WaitForSeconds(0.03f);
		}
		ScreenOverScript.intensity = IntensityValMin;
		
		yield return new WaitForSeconds(0.5f);*/

		StartCoroutine(MakeScreenOverlayInfoToMax());
		WaterwheelCameraCtrl.GetInstance().ResetPlayerFollowCameraPos();
		yield break;
	}
	
	IEnumerator MakeScreenOverlayInfoToMax()
	{
		/*while (ScreenOverScript.intensity < IntensityValMax) {
			ScreenOverScript.intensity += SubScreenOverVal;
			yield return new WaitForSeconds(0.03f);
		}
		ScreenOverScript.intensity = IntensityValMax;
		ScreenOverScript.enabled = false;*/
		ResetMoveCamera();
		yield break;
	}

	void ActivePlayerFollowCamera()
	{
		this.enabled = false;
		NoiseScript.enabled = false;
		GameCtrlXK.GetInstance().ShowAllGUI();
		StartGameTimeCtrl.GetInstance().DelayPlayTime();
		WaterwheelCameraCtrl.GetInstance().ActivePlayerFollowCamera ();
		InputEventCtrl.GetInstance().ClickStartBtOneEvent -= clickStartBtOneEvent;

		if (GameCtrlXK.GetInstance().PlayerMarkTest == null) {
			PlayerAutoFire.ResetIsRestartMove();
		}
		Debug.Log("ActivePlayerFollowCamera....");

		if (CartoonShootCamCtrl.GetInstance() != null) {
			CartoonShootCamCtrl.GetInstance().CheckGenSuiCamTranStartGame();
		}
	}
}
