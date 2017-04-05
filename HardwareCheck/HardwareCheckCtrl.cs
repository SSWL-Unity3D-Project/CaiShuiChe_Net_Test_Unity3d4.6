using UnityEngine;
using System.Collections;

public class HardwareCheckCtrl : MonoBehaviour {

	public UILabel BiZhiLable;
	public UILabel FangXiangLable;
	public UILabel JiaoTaBanLable;
	public UILabel ShuiQiangXLable;
	public UILabel ShuiQiangYLable;
	public UILabel AnJianLable;
	public UILabel StartLedP1;
	public UILabel StartLedP2;
	public UILabel ShuiQiangLabel;
	public GameObject JiaMiCeShiObj;
	public bool IsJiaMiCeShi;
	public static bool IsTestHardWare;
	public static HardwareCheckCtrl Instance;

	// Use this for initialization
	void Start()
	{
		Screen.SetResolution(1360, 768, false);
		Instance = this;
		IsTestHardWare = true;
		JiaMiCeShiObj.SetActive(IsJiaMiCeShi);
		BiZhiLable.text = "0";
		FangXiangLable.text = "0";
		JiaoTaBanLable.text = "0";
		ShuiQiangXLable.text = "0";
		ShuiQiangYLable.text = "0";
		AnJianLable.text = "";

		HardwareBtCtrl.StartLedP1 = StartLedP1;
		HardwareBtCtrl.StartLedP2 = StartLedP2;
		HardwareBtCtrl.ShuiQiangLabel = ShuiQiangLabel;
		
		GlobalData.GetInstance();
		InputEventCtrl.GetInstance().ClickSetEnterBtEvent += ClickSetEnterBtEvent;
		InputEventCtrl.GetInstance().ClickSetMoveBtEvent += ClickSetMoveBtEvent;
		InputEventCtrl.GetInstance().ClickFireBtEvent += ClickFireBtEvent;
		InputEventCtrl.GetInstance().ClickStartBtOneEvent += ClickStartP1BtEvent;
		InputEventCtrl.GetInstance().ClickStartBtTwoEvent += ClickStartP2BtEvent;
		InputEventCtrl.GetInstance().ClickStopDongGanBtEvent += ClickStopDongGanBtEvent;
	}
	
	// Update is called once per frame
	void Update()
	{
		BiZhiLable.text = pcvr.GetInstance().CoinNumCurrent.ToString();
		FangXiangLable.text = pcvr.SteerValCur.ToString();
		JiaoTaBanLable.text = pcvr.TaBanValCur.ToString();
		ShuiQiangXLable.text = pcvr.MousePosition.x.ToString();
		ShuiQiangYLable.text = pcvr.MousePosition.y.ToString();
	}

	void ClickSetEnterBtEvent(ButtonState val)
	{
		if (val == ButtonState.DOWN) {
			AnJianLable.text = "SetEnter Down";
		}
		else {
			AnJianLable.text = "SetEnter Up";
		}
	}

	void ClickSetMoveBtEvent(ButtonState val)
	{
		if (val == ButtonState.DOWN) {
			AnJianLable.text = "SetMove Down";
		}
		else {
			AnJianLable.text = "SetMove Up";
		}
	}

	void ClickStartP1BtEvent(ButtonState val)
	{
		if (val == ButtonState.DOWN) {
			AnJianLable.text = "StartP1 Down";
		}
		else {
			AnJianLable.text = "StartP1 Up";
		}
	}

	void ClickStartP2BtEvent(ButtonState val)
	{
		if (val == ButtonState.DOWN) {
			AnJianLable.text = "StartP2 Down";
		}
		else {
			AnJianLable.text = "StartP2 Up";
		}
	}
	
	void ClickStopDongGanBtEvent(ButtonState val)
	{
		if (val == ButtonState.DOWN) {
			AnJianLable.text = "DongGanBt Down";
		}
		else {
			AnJianLable.text = "DongGanBt Up";
		}
	}

	void ClickFireBtEvent(ButtonState val)
	{
		if (val == ButtonState.DOWN) {
			AnJianLable.text = "Fire Down";
		}
		else {
			AnJianLable.text = "Fire Up";
		}
	}

	public UILabel JiaMiJYLabel;
	public UILabel JiaMiJYMsg;
	public static bool IsOpenJiaMiJiaoYan;

	public void JiaMiJiaoYanFailed()
	{
		SetJiaMiJYMsg("", JiaMiJiaoYanEnum.Failed);
	}
	
	public void JiaMiJiaoYanSucceed()
	{
		SetJiaMiJYMsg("", JiaMiJiaoYanEnum.Succeed);
	}

	public void SetJiaMiJYMsg(string msg, JiaMiJiaoYanEnum key)
	{
		switch (key) {
		case JiaMiJiaoYanEnum.Succeed:
			JiaMiJYMsg.text = "校验成功";
			ResetJiaMiJYLabelInfo();
			ScreenLog.Log("校验成功");
			break;
			
		case JiaMiJiaoYanEnum.Failed:
			JiaMiJYMsg.text = "校验失败";
			ResetJiaMiJYLabelInfo();
			ScreenLog.Log("校验失败");
			break;
			
		default:
			JiaMiJYMsg.text = msg;
			ScreenLog.Log(msg);
			break;
		}
	}
	
	public static void CloseJiaMiJiaoYan()
	{
		if (!IsOpenJiaMiJiaoYan) {
			return;
		}
		IsOpenJiaMiJiaoYan = false;
	}

	void ResetJiaMiJYLabelInfo()
	{
		CloseJiaMiJiaoYan();
		JiaMiJYLabel.text = "加密校验";
	}
}

public enum JiaMiJiaoYanEnum
{
	Null,
	Succeed,
	Failed,
}