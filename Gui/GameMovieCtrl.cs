using UnityEngine;
using System.Collections;

public class GameMovieCtrl : MonoBehaviour {
	
	public MovieTexture move;
	public MovieTexture moveServer;
	
	static bool isStopMovie = false;
	
	AudioListener AudioListenObj;
	public static AudioListener AudioListenObjStatic;
	AudioSource AudioSourceObj;
	
	static GameMovieCtrl _instance;

	GameObject AudioManagerObj;
	AudioListener AudioLis;
	
	// Use this for initialization
	void Start()
	{
		AudioListener.volume = (float)GlobalData.GameAudioVolume / 10f;
		if(_instance == null) {
			_instance = this;
			//DontDestroyOnLoad( gameObject );
			PlayMovie();
		}
		pcvr.DongGanState = 0;
		//Invoke("DelayCheckPcvr", Random.Range(5f, 15f));
	}

	void DelayCheckPcvr()
	{
		pcvr.GetInstance().StartJiaoYanIO();
	}
	
	public static GameMovieCtrl GetInstance()
	{
		return _instance;
	}
	
	public void PlayMovie () {
		if(AudioListenObj == null)
		{
			AudioListenObj = gameObject.GetComponent<AudioListener>();
		}
		AudioListenObjStatic = AudioListenObj;
		AudioListenObjStatic.enabled = true;
		
		renderer.enabled = true;
		//FreeModeCtrl.IsServer = true;
		if(FreeModeCtrl.IsServer)
		{
			renderer.material.mainTexture = moveServer;
			moveServer.loop = true;
			moveServer.Play();
		}
		else
		{
			renderer.material.mainTexture = move;
			move.loop = true;
			move.Play();
		}
		
		if(AudioSourceObj == null)
		{
			AudioSourceObj = transform.GetComponent<AudioSource>();
		}
		
		if(FreeModeCtrl.IsServer)
		{
			AudioSourceObj.clip = moveServer.audioClip;
			AudioSourceObj.enabled = false;
			AudioSourceObj.Stop();
		}
		else
		{
			AudioSourceObj.clip = move.audioClip;
			AudioSourceObj.enabled = true;
			AudioSourceObj.Play();
		}
		
		if(AudioLis == null)
		{
			AudioLis = gameObject.GetComponent<AudioListener>();
		}
		AudioLis.enabled = true;
	}
	
	public void stopPlayMovie()
	{
		if(isStopMovie)
		{
			return;
		}
		isStopMovie = true;

		if(AudioListenObjStatic != null)
		{
			AudioListenObjStatic.enabled = false;
		}

		//transform.parent = null;
		if(FreeModeCtrl.IsServer)
		{
			moveServer.Stop();
		}
		else
		{
			move.Stop();
		}
		
		renderer.enabled = false;
		isStopMovie = false;
		
		if(AudioSourceObj == null)
		{
			AudioSourceObj = transform.GetComponent<AudioSource>();
		}
		AudioSourceObj.enabled = false;
	}
}
