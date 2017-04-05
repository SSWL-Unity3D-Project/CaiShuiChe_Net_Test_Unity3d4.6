using UnityEngine;
using System.Collections;
using Frederick.ProjectAircraft;

public class GameCoin : MonoBehaviour {
	
	public GameObject G_yi_shiwei;
	public GameObject G_yi_gewei;
	public GameObject G_Gang_Obj;
	public GameObject G_xu_shiwei;
	public GameObject G_xu_gewei;

	public GameObject FreeModeTextObj;
	
	private UISprite yi_shiwei;
	private UISprite yi_gewei;
	private UISprite xu_shiwei;
	private UISprite xu_gewei;
	
	int CoinCur = 0;
	int needCoin = 0;
	AudioSource BtAudioSource;

	public static GameCoin _Instance;
	public static GameCoin GetInstance()
	{
		return _Instance;
	}

	// Use this for initialization
	void Start()
	{
		_Instance = this;
		InitSprite();
		if(GlobalData.GetInstance().IsFreeMode)
		{
			HiddenCoinInfo();
			FreeModeTextObj.SetActive(true);
			return;
		}
		FreeModeTextObj.SetActive(false);
	}

	void PlayHitCoinBtAudio()
	{
		if(BtAudioSource != null && BtAudioSource.isPlaying
		   && BtAudioSource.clip == AudioListCtrl.GetInstance().AudioTouBi) {
			return;
		}
		BtAudioSource = AudioManager.Instance.PlaySFX(AudioListCtrl.GetInstance().AudioTouBi);
	}

	// Update is called once per frame
	void Update()
	{
		if(GlobalData.GetInstance().IsFreeMode)
		{
			return;
		}

		if(needCoin != GlobalData.GetInstance().XUTOUBI)
		{
			needCoin = GlobalData.GetInstance().XUTOUBI;
			ConvertNumToImg("xu", GlobalData.GetInstance().XUTOUBI);
		}

		if(pcvr.bIsHardWare)
		{
			if(GlobalData.GetInstance().Icoin != pcvr.GetInstance().CoinNumCurrent)
			{
				if(GlobalData.GetInstance().Icoin < pcvr.GetInstance().CoinNumCurrent)
				{
					PlayHitCoinBtAudio();
				}
				GlobalData.GetInstance().Icoin = pcvr.GetInstance().CoinNumCurrent;
				ConvertNumToImg("yi", GlobalData.GetInstance().Icoin);
			}
			
			if(GlobalData.GetInstance().Icoin != CoinCur)
			{
				CoinCur = GlobalData.GetInstance().Icoin;
				ConvertNumToImg("yi", GlobalData.GetInstance().Icoin);
			}
		}
		else
		{
			if( Input.GetKeyUp(KeyCode.T) )
			{
				PlayHitCoinBtAudio();
				GlobalData.GetInstance().Icoin++;
				ConvertNumToImg("yi", GlobalData.GetInstance().Icoin);
			}

			if(GlobalData.GetInstance().Icoin != CoinCur)
			{
				CoinCur = GlobalData.GetInstance().Icoin;
				ConvertNumToImg("yi", GlobalData.GetInstance().Icoin);
			}
		}

		if (GlobalData.GetInstance().Icoin >= GlobalData.GetInstance().XUTOUBI
		    && GlobalData.GetInstance().gameMode == GameMode.SoloMode) {

			if (GameOverCtrl.GetInstance().CheckIsActiveOver()) {
				if (InsertCoinCtrl.GetInstanceP1().CheckIsActiveObj()) {
					InsertCoinCtrl.GetInstanceP1().HiddenInsertCoin();
					StartBtCtrl.GetInstanceP1().InitStartBtCartoon();
				}
			}
		}
	}

	public void ConvertNumToImg(string mod,int num)
	{
		if(mod == "yi")
		{
			if(num > 99)
			{
				yi_shiwei.spriteName = "9";
				yi_gewei.spriteName = "9";
			}
			else
			{
				int coinShiWei = (int)((float)num/10.0f);
				yi_shiwei.spriteName = coinShiWei.ToString();
				yi_gewei.spriteName = (num%10).ToString();
			}

			if(num >= GlobalData.GetInstance().XUTOUBI)
			{
				if( (StartBtCtrl.GetInstanceP2() != null && !StartBtCtrl.GetInstanceP2().CheckIsActivePlayer())
				   && (GameOverCtrl.GetInstance() != null && !GameOverCtrl.GetInstance().CheckIsActiveOver())
				   && (FinishPanelCtrl.GetInstance() != null && !FinishPanelCtrl.GetInstance().CheckIsActiveFinish()))
				{
					InsertCoinCtrl.GetInstanceP2().HiddenInsertCoin();
					StartBtCtrl.GetInstanceP2().InitStartBtCartoon();
				}
			}
		}
		else if(mod == "xu")
		{
			xu_shiwei.spriteName = (num/10).ToString();
			xu_gewei.spriteName = (num%10).ToString();
		}
	}

	void InitSprite()
	{
		yi_shiwei = G_yi_shiwei.GetComponent<UISprite>();
		yi_gewei = G_yi_gewei.GetComponent<UISprite>();
		xu_gewei = G_xu_gewei.GetComponent<UISprite>();
		xu_shiwei = G_xu_shiwei.GetComponent<UISprite>();
	}

	public void HiddenCoinInfo()
	{
		yi_shiwei.enabled = false;
		yi_gewei.enabled = false;
		xu_shiwei.enabled = false;
		xu_gewei.enabled = false;
		G_Gang_Obj.SetActive(false);
	}
}
