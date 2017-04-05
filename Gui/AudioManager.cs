namespace Frederick.ProjectAircraft
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    /// <summary>
    /// 声音管理器。
    /// </summary>
    public class AudioManager : MonoBehaviour
	{
		private readonly List<AudioSource> mSFXSources = new List<AudioSource>();
		private static AudioManager mInstance;
		private AudioListener mAudioListener;
		private AudioSource mBGMSource;

		static private GameObject DaojuFlyObj = null;
        /// <summary>
        /// 获取声音管理器的唯一实例。
        /// </summary>
        public static AudioManager Instance
        {
            get
            {
                if (mInstance == null)
				{
					if(GameMovieCtrl.AudioListenObjStatic != null)
					{
						GameMovieCtrl.AudioListenObjStatic.enabled = false;
					}

					GameObject root = new GameObject("_AudioManager");
                    mInstance = root.AddComponent<AudioManager>();
                    mInstance.mAudioListener = root.AddComponent<AudioListener>();
					if(mInstance.mAudioListener == null)
					{
						ScreenLog.LogWarning("mAudioListener is null");
					}

                    mInstance.mBGMSource = root.AddComponent<AudioSource>();

					DaojuFlyObj = GameObject.Find("DaojuFly");
					if(DaojuFlyObj != null)
					{
						root.transform.parent = DaojuFlyObj.transform;
						root.transform.localPosition = Vector3.zero;
					}
                }
                return mInstance;
            }
        }

		public void SetParentTran(Transform tran)
		{
			transform.parent = tran;
			transform.localPosition = Vector3.zero;
		}

        /// <summary>
        /// 播放背景音乐。
        /// </summary>
        /// <param name="clip">背景音乐</param>
        /// <param name="isLoop">是否循环播放</param>
        public void PlayBGM(AudioClip clip, bool isLoop)
		{
			if(clip == null)
			{
				return;
			}

            mBGMSource.loop = isLoop;
            mBGMSource.clip = clip;
            mBGMSource.Play();
        }

        /// <summary>
        /// 播放声音音效。
        /// </summary>
        /// <param name="clip">声音剪辑</param>
		public AudioSource PlaySFX(AudioClip clip)
		{
			if(clip == null)
			{
				return null;
			}

			//var freeSource = mSFXSources.FirstOrDefault(source => !source.isPlaying);
			AudioSource freeSource = mSFXSources.Find(source => !source.isPlaying);
            if (freeSource == null)
            {
                freeSource = gameObject.AddComponent<AudioSource>();
                mSFXSources.Add(freeSource);
			}
			freeSource.loop = false;
            freeSource.clip = clip;
            freeSource.Play();
			return freeSource;
		}

		public AudioSource PlaySFXLoop(AudioClip clip)
		{
			if(clip == null)
			{
				return null;
			}
			
			//var freeSource = mSFXSources.FirstOrDefault(source => !source.isPlaying);
			AudioSource freeSource = mSFXSources.Find(source => !source.isPlaying);
			if (freeSource == null)
			{
				freeSource = gameObject.AddComponent<AudioSource>();
				mSFXSources.Add(freeSource);
			}
			freeSource.clip = clip;
			freeSource.loop = true;
			freeSource.Play();
			return freeSource;
		}

        /// <summary>
        /// 停止背景音乐。
        /// </summary>
        public void StopBGM()
        {
            if(mBGMSource != null)
			{
                mBGMSource.Stop();
			}
        }
    }
}