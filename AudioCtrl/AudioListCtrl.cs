using UnityEngine;
using System.Collections;
using Frederick.ProjectAircraft;

public class AudioListCtrl : MonoBehaviour {
	
	public AudioClip AudioGameBeiJing;
	public AudioClip AudioMovieBeiJing;
	public AudioClip AudioXuanZe;
	public AudioClip AudioTouBi;
	public AudioClip AudioMovieJingGao;
	public AudioClip AudioModeChange;
	public AudioClip AudioStartBt;

	public AudioClip AudioChengJiu;
	/// <summary>
	/// Play the audio when player time over.
	/// </summary>
	public AudioClip AudioGameDaoJiShi;
	/// <summary>
	/// Play the audio when game time below 10s.
	/// </summary>
	public AudioClip AudioTimeDaoJiShi;
	public AudioClip AudioFinishPanel;
	/*public AudioClip AudioRankTPos;
	public AudioClip AudioRankTRot;*/
	public AudioClip AudioGameOver;
	public AudioClip AudioCameraSwitch;
	public AudioClip AudioShowGameOver;
	public AudioClip AudioHiddenGameOver;
	public AudioClip AudioJiaShiLevel;
	public AudioClip AudioSheJiLevel;
	public AudioClip AudioTimeGo;

	public AudioClip AudioShipHit_1;
	public AudioClip AudioShipHit_2;
	public AudioClip AudioShipMove;
	public AudioClip AudioFengXue;

	public AudioClip AudioXiong;
	public AudioClip AudioShiZi;
	public AudioClip AudioLaoHu;
	public AudioClip AudioChangJingLu;
	public AudioClip AudioBossShip;

	public AudioClip AudioGameHuanHu;
	public AudioClip AudioGameShiBai;
	/// <summary>
	/// Play the audio when player yueJie or fangXiangCuoWu.
	/// </summary>
	public AudioClip AudioGameJingGao;
	public AudioClip AudioShuiLei;
	public AudioClip AudioShuiLeiBaoZha;
	public AudioClip AudioPingZiPoSui;
	public AudioClip AudioShuiQiangIntoWater;
	public AudioClip AudioRoomPoSui;
	public AudioClip AudioShooting;
	public AudioClip AudioActiveDaoJu;
	public AudioClip AudioFuMianBuff;
	public AudioClip AudioJuLiFu;
	public AudioClip AudioHuanYingFu;
	public AudioClip AudioTimeJiaShiDian;
	public AudioClip AudioPuBuJiaSu;
	
	static AudioSource JingGaoAudio;
	static AudioListCtrl _Instance;
	public static AudioListCtrl GetInstance()
	{
		return _Instance;
	}

	void Awake()
	{
		_Instance = this;
		gameObject.SetActive(false);

		if (AudioPuBuJiaSu == null) {
			AudioPuBuJiaSu.name = "null";
		}
	}

	public static AudioSource PlayAudio(AudioClip audio)
	{
		return AudioManager.Instance.PlaySFX(audio);
	}

	public static void PlayAudioLoopJingGao()
	{
		if (JingGaoAudio != null) {
			return;
		}
		JingGaoAudio = AudioManager.Instance.PlaySFXLoop(AudioListCtrl.GetInstance().AudioGameJingGao);
	}

	public static void StopAudioLoopJingGao()
	{
		if (JingGaoAudio == null) {
			return;
		}
		JingGaoAudio.Stop();
		JingGaoAudio = null;
	}
}
