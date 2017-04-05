using UnityEngine;
using System.Collections;

public class InputEventCtrl : MonoBehaviour {
	
	public static float VerticalVal;
	float HorizontalVal;
	
	public static bool IsClickFireBtDown;
	public static uint SteerValCur;
	public static uint TaBanValCur;

	static private InputEventCtrl Instance = null;
	//static GameObject InputEventCtrlObj = null;

	static public InputEventCtrl GetInstance()
	{
		if(Instance == null)
		{
			GameObject obj = new GameObject("_InputEventCtrl");
			//DontDestroyOnLoad(obj);
			Instance = obj.AddComponent<InputEventCtrl>();
			//InputEventCtrlObj = obj;

			pcvr.GetInstance();
		}
		return Instance;
	}

	#region Click Button Envent
	public delegate void EventHandel(ButtonState val);
	public event EventHandel ClickStartBtOneEvent;
	public void ClickStartBtOne(ButtonState val)
	{
		if(ClickStartBtOneEvent != null)
		{
			ClickStartBtOneEvent( val );
			pcvr.StartLightStateP1 = LedState.Mie;
		}
	}
	
	public event EventHandel ClickStartBtTwoEvent;
	public void ClickStartBtTwo(ButtonState val)
	{
		if(ClickStartBtTwoEvent != null)
		{
			ClickStartBtTwoEvent( val );
			pcvr.StartLightStateP2 = LedState.Mie;
		}
	}

	public event EventHandel ClickSetEnterBtEvent;
	public void ClickSetEnterBt(ButtonState val)
	{
		SetEnterBtSt = val;
		if(ClickSetEnterBtEvent != null)
		{
			ClickSetEnterBtEvent( val );
		}
		
		if (val == ButtonState.DOWN) {
			TimeSetEnterMoveBt = Time.time;
		}
	}

	public event EventHandel ClickSetMoveBtEvent;
	public void ClickSetMoveBt(ButtonState val)
	{
		if(ClickSetMoveBtEvent != null)
		{
			ClickSetMoveBtEvent( val );
		}
	}

	public event EventHandel ClickFireBtEvent;
	public void ClickFireBt(ButtonState val)
	{
		if(ClickFireBtEvent != null)
		{
			ClickFireBtEvent( val );
		}
	}
	
	public event EventHandel ClickStopDongGanBtEvent;
	public void ClickStopDongGanBt(ButtonState val)
	{
		if(ClickStopDongGanBtEvent != null)
		{
			ClickStopDongGanBtEvent( val );
		}

		if (val == ButtonState.DOWN) {
			if (DongGanUICtrl.Instance != null) {
				pcvr.DongGanState = (byte)(pcvr.DongGanState == 1 ? 0 : 1);
				DongGanUICtrl.Instance.ShowDongGanUI(pcvr.DongGanState);
			}
		}
	}
	#endregion

	public float GetHorVal()
	{
		return HorizontalVal;
	}
	
	float TimeSetEnterMoveBt;
	ButtonState SetEnterBtSt = ButtonState.UP;
	void Update()
	{
		if (SetEnterBtSt == ButtonState.DOWN && Time.time - TimeSetEnterMoveBt > 2f) {
			HardwareBtCtrl.OnRestartGame();
		}

		HorizontalVal = pcvr.mGetSteer;
		VerticalVal = pcvr.TanBanDownCount_P1;
		if (pcvr.bIsHardWare && !pcvr.IsGetValByKey) {
			return;
		}

		if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.001f) {
			SteerValCur = (uint)(Input.GetAxis("Horizontal") > 0f ? 2 : 0);
		}
		else {
			SteerValCur = 1;
		}
		TaBanValCur = (uint)(Input.GetAxis("Vertical") + 1f);
		
		//StartBt PlayerOne
		if(Input.GetKeyUp(KeyCode.G))
		{
			ClickStartBtOne( ButtonState.UP );
		}

		if(Input.GetKeyDown(KeyCode.G))
		{
			ClickStartBtOne( ButtonState.DOWN );
		}
		
		//StartBt PlayerTwo
		if(Input.GetKeyUp(KeyCode.K))
		{
			ClickStartBtTwo( ButtonState.UP );
		}
		
		if(Input.GetKeyDown(KeyCode.K))
		{
			ClickStartBtTwo( ButtonState.DOWN );
		}

		//setPanel enter button
		if(Input.GetKeyUp(KeyCode.F4))
		{
			ClickSetEnterBt( ButtonState.UP );
		}
		
		if(Input.GetKeyDown(KeyCode.F4))
		{
			ClickSetEnterBt( ButtonState.DOWN );
		}

		//setPanel move button
		if(Input.GetKeyUp(KeyCode.F5))
		{
			ClickSetMoveBt( ButtonState.UP );
			//FramesPerSecond.GetInstance().ClickSetMoveBtEvent( ButtonState.UP );
		}
		
		if(Input.GetKeyDown(KeyCode.F5))
		{
			ClickSetMoveBt( ButtonState.DOWN );
			//FramesPerSecond.GetInstance().ClickSetMoveBtEvent( ButtonState.DOWN );
		}

		//Fire button
		if(Input.GetKeyUp(KeyCode.Mouse0))
		{
			IsClickFireBtDown = false;
			ClickFireBt( ButtonState.UP );
		}

		if(Input.GetKeyDown(KeyCode.Mouse0))
		{
			IsClickFireBtDown = true;
			ClickFireBt( ButtonState.DOWN );
		}

		//DongGan button
		if(Input.GetKeyUp(KeyCode.P))
		{
			ClickStopDongGanBt( ButtonState.UP );
		}
		
		if(Input.GetKeyDown(KeyCode.P))
		{
			ClickStopDongGanBt( ButtonState.DOWN );
		}
	}
}

public enum ButtonState : int
{
	UP = 1,
	DOWN = -1
}