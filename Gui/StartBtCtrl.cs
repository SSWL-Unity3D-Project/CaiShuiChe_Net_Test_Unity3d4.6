using UnityEngine;
using System.Collections;
using Frederick.ProjectAircraft;

public class StartBtCtrl : MonoBehaviour {
	public PlayerBtState BtState;
	AudioClip AudioHitBt;
	bool IsActivePlayer;

	AudioSource BtAudioSource;
	UISprite BtSprite;

	public static StartBtCtrl _InstanceP1;
	public static StartBtCtrl _InstanceP2;
	public static StartBtCtrl GetInstanceP1()
	{
		return _InstanceP1;
	}

	public static StartBtCtrl GetInstanceP2()
	{
		return _InstanceP2;
	}

	// Use this for initialization
	void Awake()
	{
		switch(BtState)
		{
		case PlayerBtState.PLAYER_1:
			_InstanceP1 = this;
			IsActivePlayer = true;
			//ActiveDaJuCtrl.GetInstanceP1().ActivePlayerBlood(true);
			InputEventCtrl.GetInstance().ClickStartBtOneEvent += clickStartBtOneEvent;
			break;

		case PlayerBtState.PLAYER_2:
			_InstanceP2 = this;
			IsActivePlayer = false;
			//ActiveDaJuCtrl.GetInstanceP2().ActivePlayerBlood(false);
			InputEventCtrl.GetInstance().ClickStartBtTwoEvent += clickStartBtTwoEvent;
			break;
		}

		BtSprite = GetComponent<UISprite>();
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
		if(!IsInvoking("PlayCartoon"))
		{
			return;
		}

		if(DaoJiShiCtrl.TimeVal <= 1)
		{
			return;
		}

		if (FinishPanelCtrl.GetInstance().CheckIsActiveFinish()) {
			return;
		}
		
		bool isEnablePlayer = false;
		PlayHitStartBtAudio();
		if(!GlobalData.GetInstance().IsFreeMode)
		{
			if(GlobalData.GetInstance().Icoin >= GlobalData.GetInstance().XUTOUBI)
			{
				GlobalData.GetInstance().Icoin -= GlobalData.GetInstance().XUTOUBI;
				pcvr.GetInstance().SubPlayerCoin( GlobalData.GetInstance().XUTOUBI );
				GameCoin.GetInstance().ConvertNumToImg("yi", GlobalData.GetInstance().Icoin);
				
				if(GlobalData.GetInstance().Icoin >= GlobalData.GetInstance().XUTOUBI)
				{
					StartBtCtrl.GetInstanceP2().InitStartBtCartoon();
				}
				else
				{
					InsertCoinCtrl.GetInstanceP2().ShowInsertCoin();
				}
				HeadCtrlPlayer.GetInstanceP1().StopColor();
				isEnablePlayer = true;
			}
		}
		else
		{
			HeadCtrlPlayer.GetInstanceP1().StopColor();
			HeadCtrlPlayer.GetInstanceP2().PlayColor();
			StartBtCtrl.GetInstanceP2().InitStartBtCartoon();
			isEnablePlayer = true;
		}

		if(isEnablePlayer)
		{
			IsActivePlayer = true;
			CloseStartBtCartoon();
			if(GlobalData.GetInstance().gameMode == GameMode.SoloMode)
			{
				ActiveDaJuCtrl.GetInstanceP1().ActivePlayerBlood(true);
				DaoJiShiCtrl.GetInstance().StopDaoJiShi();
				GameOverCtrl.GetInstance().HiddenContinueGame();
				GameTimeCtrl.GetInstance().InitPlayGameTime(45);
				//GameCtrlXK.GetInstance().InitFillPlayerBlood();
			}
		}
	}

	public void ActivePlayerOne()
	{
		if(IsActivePlayer)
		{
			return;
		}
		
		if(DaoJiShiCtrl.TimeVal <= 1)
		{
			return;
		}
		
		if (FinishPanelCtrl.GetInstance().CheckIsActiveFinish()) {
			return;
		}
		
		bool isEnablePlayer = false;
		PlayHitStartBtAudio();
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
			HeadCtrlPlayer.GetInstanceP1().StopColor();
			isEnablePlayer = true;
		}
		else
		{
			HeadCtrlPlayer.GetInstanceP1().StopColor();
			HeadCtrlPlayer.GetInstanceP2().PlayColor();
			StartBtCtrl.GetInstanceP2().InitStartBtCartoon();
			isEnablePlayer = true;
		}
		
		if(isEnablePlayer)
		{
			IsActivePlayer = true;
			CloseStartBtCartoon();
			if(GlobalData.GetInstance().gameMode == GameMode.SoloMode)
			{
				ActiveDaJuCtrl.GetInstanceP1().ActivePlayerBlood(true);
				DaoJiShiCtrl.GetInstance().StopDaoJiShi();
				GameOverCtrl.GetInstance().HiddenContinueGame();
				GameTimeCtrl.GetInstance().InitPlayGameTime(1);
				//GameCtrlXK.GetInstance().InitFillPlayerBlood();
			}
		}
	}

	void clickStartBtTwoEvent(ButtonState val)
	{
		if(val == ButtonState.DOWN)
		{
			return;
		}
		clickStartBtTwo();
	}

	void PlayHitStartBtAudio()
	{
		if(AudioHitBt == null)
		{
			AudioHitBt = GameCtrlXK.AudioHitBt;
		}

		if(BtAudioSource != null && BtAudioSource.isPlaying && BtAudioSource.clip == AudioHitBt)
		{
			return;
		}

		BtAudioSource = AudioManager.Instance.PlaySFX(AudioHitBt);
	}

	void clickStartBtTwo()
	{
		if(!IsInvoking("PlayCartoon"))
		{
			return;
		}

		bool isEnablePlayer = false;
		PlayHitStartBtAudio();
		if(!GlobalData.GetInstance().IsFreeMode)
		{
			if(GlobalData.GetInstance().Icoin >= GlobalData.GetInstance().XUTOUBI)
			{
				GlobalData.GetInstance().Icoin -= GlobalData.GetInstance().XUTOUBI;
				pcvr.GetInstance().SubPlayerCoin( GlobalData.GetInstance().XUTOUBI );
				GameCoin.GetInstance().ConvertNumToImg("yi", GlobalData.GetInstance().Icoin);
				CloseStartBtCartoon();
				isEnablePlayer = true;
			}
		}
		else
		{
			CloseStartBtCartoon();
			isEnablePlayer = true;
		}

		if(isEnablePlayer)
		{
			IsActivePlayer = true;
			ActiveDaJuCtrl.GetInstanceP2().ActivePlayerBlood(true);
			ZhunXingCtrl.GetInstance().ShowPlayerZhunXing();
		}
	}

	public void InitStartBtCartoon()
	{
		if(IsInvoking("PlayCartoon"))
		{
			return;
		}
		BtSprite.enabled = true;

		switch(BtState)
		{
		case PlayerBtState.PLAYER_1:
			pcvr.StartLightStateP1 = LedState.Shan;
			//_InstanceP1 = this;
			break;
			
		case PlayerBtState.PLAYER_2:
			pcvr.StartLightStateP2 = LedState.Shan;
			HeadCtrlPlayer.GetInstanceP2().PlayColor();
			break;
		}
		InvokeRepeating("PlayCartoon", 0f, 0.5f);
	}

	void PlayCartoon()
	{
		switch(BtSprite.spriteName)
		{
		case "1":
			BtSprite.spriteName = "2";
			break;
			
		case "2":
			BtSprite.spriteName = "1";
			break;
		}
	}

	public void CloseStartBtCartoon()
	{
		CancelInvoke("PlayCartoon");
		BtSprite.enabled = false;
		
		switch(BtState)
		{
		case PlayerBtState.PLAYER_1:
			pcvr.StartLightStateP1 = LedState.Mie;
			break;
			
		case PlayerBtState.PLAYER_2:
			pcvr.StartLightStateP2 = LedState.Mie;
			HeadCtrlPlayer.GetInstanceP2().StopColor();
			break;
		}
	}

	public void ResetIsActivePlayer()
	{
		IsActivePlayer = false;
		ZhunXingCtrl.GetInstance().ClosePlayerZhunXing();
		DirectionInfoCtrl.GetInstance().HiddenDirWrong();
		PlayerYueJieCtrl.GetInstance().ClosePlayerYueJie();
		
		ActiveDaJuCtrl.GetInstanceP1().ActivePlayerBlood(false);
		ActiveDaJuCtrl.GetInstanceP2().ActivePlayerBlood(false);
	}
	
	public bool CheckIsActivePlayer()
	{
		return IsActivePlayer;
	}
}

public enum PlayerBtState
{
	PLAYER_1,
	PLAYER_2
}