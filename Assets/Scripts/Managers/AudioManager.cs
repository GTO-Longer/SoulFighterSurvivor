using System.Collections.Generic;
using DataManagement;
using UnityEngine;

namespace Managers
{
    public class AudioManager : MonoBehaviour
    {
        public Property<float> volume;
        
        public static AudioManager Instance { get; private set; }
        private const int initPoolSize = 30;

        private List<AudioSource> audioPool = new ();
        private Dictionary<string, AudioClip> clipCache = new ();
        private Dictionary<string, AudioSource> activeLoopingSounds = new ();
        
        public AudioClip backgroundClip;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitPool();
                volume = new Property<float>(0f);
                volume.PropertyChanged += (_, _) =>
                {
                    foreach (var source in audioPool)
                    {
                        source.volume = volume.Value;
                    }
                };
                
                if (backgroundClip != null)
                {
                    Play(backgroundClip, "bgm", true);
                }
                
                volume.Value = 0.5f;
            }
            else
            {
                Instance.backgroundClip = backgroundClip;
                Instance.Stop("bgm");
                Instance.Play(backgroundClip, "bgm", true);
                
                Destroy(gameObject);
            }
        }

        private void InitPool()
        {
            for (var i = 0; i < initPoolSize; i++)
            {
                CreateNewAudioSource();
            }
        }

        private AudioSource CreateNewAudioSource()
        {
            var source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            audioPool.Add(source);
            return source;
        }

        // 获取一个当前没在播放的AudioSource
        private AudioSource GetAvailableSource()
        {
            foreach (var source in audioPool)
            {
                if (!source.isPlaying) return source;
            }

            // 若池子满了则动态扩容
            return CreateNewAudioSource();
        }

        public AudioSource GetAudioSource()
        {
            var audioS = GetAvailableSource();
            audioPool.Remove(audioS);
            return audioS;
        }

        public void DestroyAudioSource(AudioSource audioSource)
        {
            if (audioPool.Contains(audioSource))
            {
                audioPool.Remove(audioSource);
            }
            
            Destroy(audioSource);
        }

        /// <summary>
        /// 播放音效
        /// </summary>
        public void Play(string fileName, string audioName, bool loop = false)
        {
            var clip = LoadClip(fileName, audioName);
            if (clip == null) return;
            if(loop && activeLoopingSounds.ContainsKey(audioName)) return;

            var source = GetAvailableSource();
            source.clip = clip;
            source.loop = loop;
            source.Play();

            if (loop)
            {
                activeLoopingSounds[audioName] = source;
            }
        }
        
        /// <summary>
        /// 播放音效
        /// </summary>
        public void Play(AudioClip clip, string audioName, bool loop = false)
        {
            if (clip == null) return;
            if(loop && activeLoopingSounds.ContainsKey(audioName)) return;

            var source = GetAvailableSource();
            source.clip = clip;
            source.loop = loop;
            source.Play();

            if (loop)
            {
                activeLoopingSounds[audioName] = source;
            }
        }

        /// <summary>
        /// 停止特定名称的音效
        /// </summary>
        public void Stop(string audioName)
        {
            if (activeLoopingSounds.ContainsKey(audioName))
            {
                activeLoopingSounds[audioName].Stop();
                activeLoopingSounds.Remove(audioName);
            }
        }

        /// <summary>
        /// 暂停特定名称的音效
        /// </summary>
        public void Pause(string audioName)
        {
            if (activeLoopingSounds.ContainsKey(audioName))
            {
                activeLoopingSounds[audioName].Pause();
            }
        }

        /// <summary>
        /// 继续播放特定名称的音效
        /// </summary>
        public void UnPause(string audioName)
        {
            if (activeLoopingSounds.ContainsKey(audioName))
            {
                activeLoopingSounds[audioName].UnPause();
            }
        }

        public bool IsPlaying(string audioName)
        {
            return activeLoopingSounds.ContainsKey(audioName) && activeLoopingSounds[audioName].isPlaying;
        }

        /// <summary>
        /// 停止所有正在播放的音效
        /// </summary>
        public void StopAll()
        {
            foreach (var source in audioPool)
            {
                source.Stop();
            }

            activeLoopingSounds.Clear();
        }

        /// <summary>
        /// 暂停所有正在播放的音效
        /// </summary>
        public void PauseAll()
        {
            foreach (var source in audioPool)
            {
                if (source.clip != backgroundClip)
                {
                    source.Pause();
                }
            }
        }

        /// <summary>
        /// 继续播放所有正在暂停的音效
        /// </summary>
        public void UnPauseAll()
        {
            foreach (var source in audioPool)
            {
                source.UnPause();
            }
        }

        private AudioClip LoadClip(string fileName, string audioName)
        {
            if (clipCache.ContainsKey(audioName)) return clipCache[audioName];

            var path = "Audios/" + fileName;
            var clip = Resources.Load<AudioClip>(path);
            if (clip != null) clipCache.Add(audioName, clip);

            if (clip == null)
            {
                Debug.LogWarning("[AudioManager] 未能找到音频文件：" + path);
            }
            
            return clip;
        }
    }
}