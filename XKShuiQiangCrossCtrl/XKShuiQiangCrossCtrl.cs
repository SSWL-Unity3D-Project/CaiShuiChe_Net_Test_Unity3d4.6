#define DRAW_HIT_LINE
#define SHOW_GUI_INFO
//#define TEST_GUN_ROT
using UnityEngine;
using System.Collections;
using System;

public class XKShuiQiangCrossCtrl : MonoBehaviour
{
	static XKShuiQiangCrossCtrl _Instance;
	public static XKShuiQiangCrossCtrl GetInstance()
	{
		return _Instance;
	}

	void Awake()
	{
		_Instance = this;
		
		GlobalData.GetInstance();
		fileName = GlobalData.fileName;
		handleJsonObj = GlobalData.handleJsonObj;
	}

	// Use this for initialization
	void Start()
	{
		InitXianShiQi();
		InitPlayerGunInfo();
		#if DRAW_HIT_LINE
		GunQGTr.gameObject.SetActive(false);
		GunDZ.SetActive(false);
		#endif
	}

	//枪底座.
	public GameObject GunDZ;
	//枪管.
	public Transform GunQGTr;
	//枪.
	public Transform GunTr;
	//显示器.
	public Transform XianShiQiTr;
	//枪管发出的射线在显示器上碰撞点.
	Vector3 PosHit;
	public LayerMask CheckLayer;
	public Transform PlayerCrossTr;
	// Update is called once per frame
	void FixedUpdate()
	{
		if (!pcvr.bIsHardWare) {
			return;
		}
		UpdateGunQGTr();

		RaycastHit hitInfo;
		Vector3 startPos = GunQGTr.position;
		Physics.Raycast(startPos, GunQGTr.forward, out hitInfo, 30f, CheckLayer);
		if (hitInfo.collider != null) {
			PosHit = hitInfo.point;
			UpdatePlayerCrossTr();
			#if DRAW_HIT_LINE && UNITY_EDITOR
			Debug.DrawLine(startPos, PosHit, Color.green);
			#endif
		}
		else {
			#if DRAW_HIT_LINE && UNITY_EDITOR
			Debug.DrawLine(startPos, startPos + (GunQGTr.forward * 15f), Color.red);
			#endif
		}
	}

	public static Vector3 CrossPos;
	float OffsetMaxPX = 75f;
	float OffsetMaxPY = 75f;
	void UpdatePlayerCrossTr()
	{
		//CrossPos.x = (PosHit.x / XianShiQiTr.localScale.x) * 1360f;
		//CrossPos.y = (PosHit.y / XianShiQiTr.localScale.y) * 768f;

		//fix posX.
		float posX = (PosHit.x / XianShiQiTr.localScale.x) * 1360f;
//		float offsetPX = (OffsetMaxPX * PosHit.x) / XianShiQiTr.localScale.x;
//		CrossPos.x = posX - offsetPX < 0f ? 0f : (posX - offsetPX);
		if (PosHit.x < GPX && PosHit.y > GPY) {
			float offsetPX = (OffsetMaxPX * (GPX - PosHit.x)) / GPX;
			CrossPos.x = posX - offsetPX < 0f ? 0f : (posX - offsetPX);
		}
		else {
			CrossPos.x = posX;
		}

//		if (PosHit.y > GPY) {
//			CrossPos.x = posX;
//		}
//		else {
//			float offsetPX = (OffsetMaxPX * Mathf.Abs(PosHit.x - GPX)) / (XianShiQiTr.localScale.x - GPX);
//			if (PosHit.x >= GPX) {
//				CrossPos.x = posX - offsetPX;
//			}
//			else {
//				CrossPos.x = posX + offsetPX;
//			}
//		}
//		CrossPos.x = posX;

		//fix posY.
		float posY = (PosHit.y / XianShiQiTr.localScale.y) * 768f;
		float offsetPY = (OffsetMaxPY * PosHit.y) / XianShiQiTr.localScale.y;
		CrossPos.y = posY + offsetPY > 768f ? 768f : (posY + offsetPY);
		if (PlayerCrossTr == null) {
			return;
		}
		PlayerCrossTr.localPosition = CrossPos;
	}

	public void SetPlayerCrossTr(Transform crossTr)
	{
		if (PlayerCrossTr != null) {
			PlayerCrossTr.gameObject.SetActive(false);
		}
		PlayerCrossTr = crossTr;
	}

	//显示器尺寸(英寸*2.54cm => 英寸*0.0254m).
	static float XianShiQiChiCun = 50f * 0.0254f;
	static float XianShiQiXianBianKey = Mathf.Sqrt(Mathf.Pow(16f, 2) + Mathf.Pow(9f, 2));
	static bool IsInitXianShiQi;
	void InitXianShiQi()
	{
		if (IsInitXianShiQi) {
			return;
		}
		IsInitXianShiQi = true;
		float sx = (XianShiQiChiCun*16f)/XianShiQiXianBianKey;
		sx += 0.115f;
		float sy = (XianShiQiChiCun*9f)/XianShiQiXianBianKey;
		XianShiQiTr.localScale = new Vector3(sx, sy, 1f);
		XianShiQiTr.position = new Vector3(sx*0.5f, sy*0.5f, 0f);
	}

	//枪的电位器极值信息(0-4095).
	public static int GXMin = 0;
	public static int GYMin = 0;
	public static int GXMax = 900;
	public static int GYMax = 900;
	//枪管垂直显示器时电位器信息.
	public static int GXCen = 15 * 42;
	public static int GYCen = 15 * 13;
	//枪管电位器在X/Y轴上的信息.
	static int GXCur;
	static int GYCur;
	//电位器齿轮比(枪上齿轮半径/电位器上齿轮半径).
	static float GChiLunXPer = 4f;
	static float GChiLunYPer = 4f;
	//电位器每过15单位,则枪的转角会变化1度.
	const byte GAUnit = 15;
	//枪的轴心到显示器的距离.
	static float GSDis = 1.08f;
	//枪管到显示器左/下边沿时与显示器法线的夹角.
	static float GAXMin;
	static float GAYMin;
	//枪的轴心相对显示器的坐标.
	static float GPX;
	static float GPY;
	//枪管相对显示器法线在X/Y轴上的夹角.
	static float GAXCur;
	static float GAYCur;

	#if TEST_GUN_ROT
	public int GXCurTest;
	public int GYCurTest;
	#endif
	
	string fileName = "";
	HandleJson handleJsonObj;
	public void SavePlayerGunInfo()
	{
		IsInitPlayerGunInfo = false;
		handleJsonObj.WriteToFileXml(fileName, "GXMin", GXMin.ToString());
		handleJsonObj.WriteToFileXml(fileName, "GYMin", GYMin.ToString());
		handleJsonObj.WriteToFileXml(fileName, "GXMax", GXMax.ToString());
		handleJsonObj.WriteToFileXml(fileName, "GYMax", GYMax.ToString());
		handleJsonObj.WriteToFileXml(fileName, "GXCen", GXCen.ToString());
		handleJsonObj.WriteToFileXml(fileName, "GYCen", GYCen.ToString());
		InitPlayerGunInfo(1);
	}

	static bool IsInitPlayerGunInfo;
	void InitPlayerGunInfo(int key = 0)
	{
		if (IsInitPlayerGunInfo) {
			return;
		}
		IsInitPlayerGunInfo = true;

		if (key == 0) {
			string val = handleJsonObj.ReadFromFileXml(fileName, "GXMin");
			if (val == null || val == "") {
				val = "4090";
				handleJsonObj.WriteToFileXml(fileName, "GXMin", val);
			}
			GXMin =  Convert.ToInt32(val);
			
			val = handleJsonObj.ReadFromFileXml(fileName, "GYMin");
			if (val == null || val == "") {
				val = "2105";
				handleJsonObj.WriteToFileXml(fileName, "GYMin", val);
			}
			GYMin =  Convert.ToInt32(val);
			
			val = handleJsonObj.ReadFromFileXml(fileName, "GXMax");
			if (val == null || val == "") {
				val = "925";
				handleJsonObj.WriteToFileXml(fileName, "GXMax", val);
			}
			GXMax =  Convert.ToInt32(val);
			
			val = handleJsonObj.ReadFromFileXml(fileName, "GYMax");
			if (val == null || val == "") {
				val = "360";
				handleJsonObj.WriteToFileXml(fileName, "GYMax", val);
			}
			GYMax =  Convert.ToInt32(val);
			
			val = handleJsonObj.ReadFromFileXml(fileName, "GXCen");
			if (val == null || val == "") {
				val = "3250";
				handleJsonObj.WriteToFileXml(fileName, "GXCen", val);
			}
			GXCen =  Convert.ToInt32(val);
			
			val = handleJsonObj.ReadFromFileXml(fileName, "GYCen");
			if (val == null || val == "") {
				val = "1700";
				handleJsonObj.WriteToFileXml(fileName, "GYCen", val);
			}
			GYCen =  Convert.ToInt32(val);
			
			val = handleJsonObj.ReadFromFileXml(fileName, "GSDis");
			if (val == null || val == "") {
				val = "108"; //毫米.
				handleJsonObj.WriteToFileXml(fileName, "GSDis", val);
			}
			GSDis =  (float)Convert.ToInt32(val) / 100f; //米.

			val = handleJsonObj.ReadFromFileXml(fileName, "OffsetMaxPX");
			if (val == null || val == "") {
				val = "75";
				handleJsonObj.WriteToFileXml(fileName, "OffsetMaxPX", val);
			}
			OffsetMaxPX =  (float)Convert.ToInt32(val);

			val = handleJsonObj.ReadFromFileXml(fileName, "OffsetMaxPY");
			if (val == null || val == "") {
				val = "75";
				handleJsonObj.WriteToFileXml(fileName, "OffsetMaxPY", val);
			}
			OffsetMaxPY =  (float)Convert.ToInt32(val);
		}

//		GXMin = 4090;
//		GYMin = 2105;
//		GXMax = 925;
//		GYMax = 360;

//		GXCen = 3202;
//		GYCen = 1736;
//		GSDis = 1.08f;

		GAXMin = Mathf.Abs((float)GYMin - GYCen) / (GChiLunYPer * GAUnit);
		GAYMin = Mathf.Abs((float)GXMin - GXCen) / (GChiLunXPer * GAUnit);
		//Debug.Log("GAXMin "+GAXMin+", GAYMin "+GAYMin);
		GPX = GSDis * Mathf.Tan((GAYMin / 180f) * Mathf.PI);
		GPY = GSDis * Mathf.Tan((GAXMin / 180f) * Mathf.PI);
		GunTr.position = new Vector3(GPX, GPY, -GSDis);
	}

	void UpdateGunQGTr()
	{
		#if TEST_GUN_ROT
		GXCur = GXCurTest;
		GYCur = GYCurTest;
		#else
		GXCur = (int)pcvr.MousePosition.x;
		GYCur = (int)pcvr.MousePosition.y;
		#endif

		if (GYMax >= GYMin) {
			if (GYCur < GYMin) {
				GYCur = GYMin;
			}

			if (GYCur > GYMax) {
				GYCur = GYMax;
			}
			GAXCur = ((float)GYCen - GYCur) / (GChiLunYPer * GAUnit);
		}
		else {
			if (GYCur < GYMax) {
				GYCur = GYMax;
			}
			
			if (GYCur > GYMin) {
				GYCur = GYMin;
			}
			GAXCur = ((float)GYCur - GYCen) / (GChiLunYPer * GAUnit);
		}

		if (GXMax >= GXMin) {
			if (GXCur < GXMin) {
				GXCur = GXMin;
			}
			
			if (GXCur > GXMax) {
				GXCur = GXMax;
			}
			GAYCur = ((float)GXCur - GXCen) / (GChiLunXPer * GAUnit);
		}
		else {
			if (GXCur < GXMax) {
				GXCur = GXMax;
			}
			
			if (GXCur > GXMin) {
				GXCur = GXMin;
			}
			GAYCur = ((float)GXCen - GXCur) / (GChiLunXPer * GAUnit);
		}
		GunQGTr.localEulerAngles = new Vector3(GAXCur, GAYCur, 0f);
	}

	#if SHOW_GUI_INFO
	void OnGUI()
	{
		GUI.color = Color.green;
		Vector3 gunRot = new Vector3(Mathf.Abs(GAXCur), Mathf.Abs(GAYCur), 0f);
		string strA = "PosHit "+PosHit.ToString("f2")
			+", GAngleVal "+gunRot.ToString("f2")
			+", CrossPos "+CrossPos.ToString("f2");
		GUI.Label(new Rect(10f, 60f, Screen.width, 30f), strA);

		strA = "RealScreen "+XianShiQiTr.localScale.ToString("f3")
			+", GunTrPos "+GunTr.position.ToString("f3")
			+", ScreenW "+Screen.width+" ScreenH "+Screen.height;
		GUI.Label(new Rect(10f, 90f, Screen.width, 30f), strA);
	}
	#endif
}