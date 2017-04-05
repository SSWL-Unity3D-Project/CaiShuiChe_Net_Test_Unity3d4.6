using UnityEngine;
using System.Collections;
using Frederick.ProjectAircraft;

using System.Collections.Generic;

public class GameCtrlXK : MonoBehaviour {

	public LayerMask NpcVertHitLayer;
	public LayerMask PlayerVertHitLayer;
	public LayerMask WaterLayerMask;
	public LayerMask NpcAmmoHitLayer;
	public Transform AiPathCtrlTran;

	public GameObject ScreenWaterParticlePrefab;
	[Range(10f, 100f)] public float PlayerZhuanXiangVal = 90f;
	[Range(0f, 5f)] public float TimeNpcSpawnExplode = 0.5f;
	[Range(0f, 1f)] public float ZhuJiaoNvCenterPerPx = 0.3f;

	[Range(0.1f, 0.9f)] public float NpcBuWaWaVal = 0.3f;

	[Range(1, 100)] public int PlayerShootNpc_1 = 5;
	[Range(1, 100)] public int PlayerShootNpc_2 = 10;

	[Range(1, 100)] public int PlayerHitZhangAi_1 = 5;
	[Range(1, 100)] public int PlayerHitZhangAi_2 = 10;
	[Range(0.1f, 1f)] public float PlayerSteerKey = 0.1f;
	[Range(1f, 10f)] public float ActiveJuLiFuTime = 6f;
	public float JiaSuWuDiTime = 4f;

	public MeshFilter PathMarkFilter;
	public Mesh TriggerFilterMesh;

	public static AudioClip AudioHitBt;
	public Transform PlayerMarkTest;
	public static Transform PlayerTran;

	Vector3 ScreenWaterPos;
	public static string WaterLayer = "Water";
	public static bool IsStopMoveAiPlayer;
	public static int NpcShakeCamVal = 4;
	public static int NpcHitPlayerShakeSpeed = 15;
	public static float PlayerZhuanXiangPTVal = 90f;
	public static float PlayerZhuanXiangJSVal = 135f;

	static Transform _MissionCleanup;
	public static Transform MissionCleanup
	{
		get
		{
			if (_MissionCleanup == null) {
				GameObject objMis = new GameObject("_MissionCleanup");
				_MissionCleanup = objMis.transform;
			}
			return _MissionCleanup;
		}
	}

	//DanJi Mode
//	UISprite DanJiXueTiaoEng;

	//Link Mode

	public static GameCtrlXK _Instance;
	public static GameCtrlXK GetInstance()
	{
		if (_Instance == null) {
			GameObject obj = GameObject.Find("_GameCtrlXK");
			if (obj != null) {
				_Instance = obj.GetComponent<GameCtrlXK>();
			}
		}
		return _Instance;
	}

	// Use this for initialization
	void Start()
	{
		_Instance = this;

		IsStopMoveAiPlayer = false;
		if (PlayerShootNpc_2 < PlayerShootNpc_1) {
			Debug.LogError("PlayerShootNpc_1 and PlayerShootNpc_2 was wrong!");
			PlayerTran.name = "null";
		}

		if (PlayerHitZhangAi_2 < PlayerHitZhangAi_1) {
			Debug.LogError("PlayerHitZhangAi_1 and PlayerHitZhangAi_2 was wrong!");
			PlayerTran.name = "null";
		}

		AudioHitBt = AudioListCtrl.GetInstance().AudioStartBt;
		AudioManager.Instance.PlayBGM(AudioListCtrl.GetInstance().AudioGameBeiJing, true);
		if(GlobalData.GetInstance().gameMode == GameMode.SoloMode)
		{
			//DanJiXueTiaoEng = XueTiaoEngCtrl.GetInstance().GetXueTiaoEngSprite();

			//InitDanJiXueTiaoEng();
			//WaterwheelPlayerCtrl.PlayerZhuanXiangVal = PlayerZhuanXiangVal;
			HeadCtrlPlayer.GetInstanceP1().StopColor();

			StartBtCtrl.GetInstanceP2().CloseStartBtCartoon();
			HeadCtrlPlayer.GetInstanceP2().SetHeadColor();
			if(!GlobalData.GetInstance().IsFreeMode)
			{
				if(GlobalData.GetInstance().Icoin >= GlobalData.GetInstance().XUTOUBI)
				{
					StartBtCtrl.GetInstanceP2().InitStartBtCartoon();
				}
				else
				{
					InsertCoinCtrl.GetInstanceP2().ShowInsertCoin();
				}
			}
			else
			{
				StartBtCtrl.GetInstanceP2().InitStartBtCartoon();
			}
			ShowAllCameras();

			//InvokeRepeating("LoopSubXueTiao", 3f, 100f); //test
			//InitFillPlayerBlood(); //test
		}
		else
		{
			/*if (NetworkRpcMsgCtrl.MaxLinkServerCount > 0 && NetworkRpcMsgCtrl.MaxLinkServerCount != NetworkRpcMsgCtrl.NoLinkClientCount) {
				CloseAllCameras();
			}
			else {
				ShowAllCameras();
			}*/

			//WaterwheelPlayerNetCtrl.PlayerZhuanXiangVal = PlayerZhuanXiangVal;
			HeadCtrlPlayer.GetInstanceP1().StopColor();
			
			StartBtCtrl.GetInstanceP2().CloseStartBtCartoon();
			HeadCtrlPlayer.GetInstanceP2().SetHeadColor();
			if(!GlobalData.GetInstance().IsFreeMode)
			{
				if(GlobalData.GetInstance().Icoin >= GlobalData.GetInstance().XUTOUBI)
				{
					StartBtCtrl.GetInstanceP2().InitStartBtCartoon();
				}
				else
				{
					InsertCoinCtrl.GetInstanceP2().ShowInsertCoin();
				}
			}
			else
			{
				StartBtCtrl.GetInstanceP2().InitStartBtCartoon();
			}
		}

		SetPanelCtrl.GetInstance();
		PlayerAutoFire.ResetPlayerHitZhangAiNum();
		PlayerAutoFire.ResetPlayerShootNpcNum();

		//QueryLinkIp.GetInstance().CheckLinkIpArray();
		//TestIpLink();

		CancelInvoke("FreeMemory");
		InvokeRepeating("FreeMemory", 30000f, 10000f);
	}
	
	void SetScreenWaterPos(Vector3 pos)
	{
		ScreenWaterPos = pos;
	}

	public void CreateScreenWaterParticle()
	{
		if (PlayerTran == null) {
			return;
		}
		GameObject obj = (GameObject)Instantiate(ScreenWaterParticlePrefab);
		Transform tran = obj.transform;
		tran.parent = PlayerTran;
		tran.localPosition = ScreenWaterPos;
		Destroy(obj, 3f);
	}

	void TestIpLink()
	{
		int max = QueryLinkIp.IpList.Count;
		for (int i = 0; i < max; i++) {
			Debug.Log("ip_" + i + ": " + QueryLinkIp.IpList[i]);
		}
	}

//	void InitDanJiXueTiaoEng()
//	{
//		DanJiXueTiaoEng.fillAmount = 1f;
//		DanJiXueTiaoEng.enabled = true;
//	}

//	void LoopSubXueTiao()
//	{
//		SubDanJiXueTiaoEng(10f);
//		if(DanJiXueTiaoEng.fillAmount <= 0f)
//		{
//			CancelInvoke("LoopSubXueTiao");
//		}
//	}
	
	public void CloseAllCameras()
	{
		BackgroudGmCtrl.GetInstance().OpenBackgroudImg();
	}
	
	public void FindPlayerTran()
	{
		if (PlayerTran == null) {
			if (GlobalData.GetInstance().gameMode == GameMode.OnlineMode) {
				if (WaterwheelPlayerNetCtrl.GetInstance() != null) {
					PlayerTran = WaterwheelPlayerNetCtrl.GetInstance().transform;
					if (PlayerTran != null) {
						SetScreenWaterPos( WaterwheelPlayerNetCtrl.GetInstance().ScreenWaterParticle.localPosition );
					}
				}
			}
			else {
				if (WaterwheelPlayerCtrl.GetInstance() != null) {
					PlayerTran = WaterwheelPlayerCtrl.GetInstance().transform;
					if (PlayerTran != null) {
						SetScreenWaterPos( WaterwheelPlayerCtrl.GetInstance().ScreenWaterParticle.localPosition );
					}
				}
			}
			return;
		}
	}

	public void ShowAllCameras()
	{
		BackgroudGmCtrl.GetInstance().CloseBackgroudImg();

		if (GlobalData.GetInstance().gameMode == GameMode.OnlineMode) {
			StartGameTimeCtrl.GetInstance().DelayPlayTime();
		}
		else {
			CloseAllGUI();
			MoveCameraByPath.GetInstance().StartMoveCamera();
			/*CancelInvoke("DelayStartMoveCamera");
			Invoke("DelayStartMoveCamera", 0.5f);*/
		}
	}

	void DelayStartMoveCamera()
	{
		MoveCameraByPath.GetInstance().StartMoveCamera();
	}

	void CloseAllGUI()
	{
		GameTimeCtrl.GetInstance().SetGameTimeIsActive(false);
		XingXingCtrl.GetInstance().SetXingXingIsActive(false);
		HeadCtrlPlayer.GetInstanceP1().SetPlayerHeadIsActive(false);
		HeadCtrlPlayer.GetInstanceP2().SetPlayerHeadIsActive(false);
	}

	public void ShowAllGUI()
	{
		GameTimeCtrl.GetInstance().SetGameTimeIsActive(true);
		XingXingCtrl.GetInstance().SetXingXingIsActive(true);
		HeadCtrlPlayer.GetInstanceP1().SetPlayerHeadIsActive(true);
		HeadCtrlPlayer.GetInstanceP2().SetPlayerHeadIsActive(true);
	}

	/// <summary>
	/// Actives the type of the player dao ju. typeVal: 1 -> huanWeiFu, 2 -> huanYingFu, 3 -> juLiFu, 4 -> dianDaoFu, 5 -> dingShenFu
	/// </summary>
	/// <param name="typeVal">Type value.</param>
	public void ActivePlayerDaoJuType(int typeVal)
	{
		HitDaoJuCtrl.GetInstance().SpawnHitDaoJuSprite((DaoJuTypeIndex)typeVal);

		/*if (StartBtCtrl.GetInstanceP1().CheckIsActivePlayer()) {
			ActiveDaJuCtrl.GetInstanceP1().ActiveDaoJuType(typeVal);
			HeadCtrlPlayer.GetInstanceP1().InitChangeHeadUI();
		}

		if (StartBtCtrl.GetInstanceP2().CheckIsActivePlayer()) {
			ActiveDaJuCtrl.GetInstanceP2().ActiveDaoJuType(typeVal);
			HeadCtrlPlayer.GetInstanceP2().InitChangeHeadUI();
		}*/
	}

	public void SetPlayerMvSpeedSpriteInfo(float val)
	{
		if (StartBtCtrl.GetInstanceP1().CheckIsActivePlayer()) {
			ActiveDaJuCtrl.GetInstanceP1().SetPlayerMvSpeedSpriteInfo(val);
		}
		
		if (StartBtCtrl.GetInstanceP2().CheckIsActivePlayer()) {
			ActiveDaJuCtrl.GetInstanceP2().SetPlayerMvSpeedSpriteInfo(val);
		}
	}

	public void SetPlayerBoxColliderState(bool isActive)
	{
		if (GlobalData.GetInstance().gameMode == GameMode.SoloMode) {
			WaterwheelPlayerCtrl.GetInstance().SetPlayerBoxColliderState(isActive);
		}
		else {
			WaterwheelPlayerNetCtrl.GetInstance().SetPlayerBoxColliderState(isActive);
		}
	}

	public void InitDelayClosePlayerBoxCollider()
	{
		CancelInvoke("DelayClosePlayerBoxCollider");
		Invoke("DelayClosePlayerBoxCollider", 2f);
	}

	public void DelayClosePlayerBoxCollider()
	{
		if (GlobalData.GetInstance().gameMode == GameMode.SoloMode) {
			WaterwheelPlayerCtrl.GetInstance().SetPlayerBoxColliderState(false);
		}
		else {
			WaterwheelPlayerNetCtrl.GetInstance().SetPlayerBoxColliderState(false);
		}
	}

	void FreeMemory()
	{
		System.GC.Collect();
	}

	/*void OnGUI()
	{
		float px = Screen.width - 200f;
		GUI.Box(new Rect(px, 50f, 200f, 30f), "PlayerHitZhangAiNum " + PlayerAutoFire.PlayerHitZhangAiNum.ToString());
		GUI.Box(new Rect(px, 90f, 200f, 30f), "PlayerShootNpcNum " + PlayerAutoFire.PlayerShootNpcNum.ToString());
		GUI.Box(new Rect(px, 130f, 200f, 30f), "PlayerSpeed " + PlayerAutoFire.PlayerMvSpeed.ToString());
	}*/
}

public enum DaoJuTypeIndex : int
{
	Close = -1,
	NULL,
	huanWeiFu,
	huanYingFu,
	juLiFu,
	dianDaoFu,
	dingShenFu,
	shenXingState
}