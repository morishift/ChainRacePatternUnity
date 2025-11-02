
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sample
{
    /// <summary>
    /// Component for playing sound effects
    /// </summary>
    public class SoundPlayer : MonoBehaviour
    {
        static SoundPlayer instance;
        const int MaxAudioSourceCount = 5;

        [SerializeField]
        AudioClip[] audioClips;
        List<AudioSource> audioSources = new List<AudioSource>();       

        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            for (int i = 0; i < MaxAudioSourceCount; ++i)
            {
                audioSources.Add(gameObject.AddComponent<AudioSource>());
            }
        }

        /// <summary>
        /// Plays a sound by type
        /// </summary>
        public void PlaySound(SoundType sound)
        {
            int index = (int)sound;
            if (index >= audioClips.Length)
            {
                return;
            }
            PlayAudioClip(audioClips[(int)sound]);
        }

        /// <summary>
        /// Plays the specified audio clip
        /// </summary>
        public void PlayAudioClip(AudioClip clip)
        {
            int index = audioSources.FindIndex(source => !source.isPlaying);
            if (index < 0)
            {
                index = 0;
            }
            var source = audioSources[index];
            audioSources.RemoveAt(index);
            audioSources.Add(source);
            source.Stop();
            source.clip = clip;
            source.Play();
        }

        /// <summary>
        /// Gets the SoundPlayer instance
        /// </summary>
        public static SoundPlayer Get()
        {
            return instance;
        }
    }
}

