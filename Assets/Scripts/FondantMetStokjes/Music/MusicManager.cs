using System;
using UnityEngine;

namespace FondantMetStokjes.Music
{
    [Serializable]
    public struct Stem
    {
        public AudioClip clip;
        public bool waitForLoop;
        public float volume;
        public AudioSource Source { get; internal set; }
    }
    
    public class MusicManager : MonoBehaviour
    {
        public float volumeFadeSpeed = 1.0f;
        [SerializeField] Stem[] stems;
        private int loops = 0;
        private int previousSource0Time = 0;
        
        void Start()
        {
            for (int i = 0; i < stems.Length; i++)
            {
                GameObject stemObject = new GameObject("Source " + i, typeof(AudioSource));
                stemObject.transform.parent = transform;
                stems[i].Source = stemObject.GetComponent<AudioSource>();
                stems[i].Source.clip = stems[i].clip;
                stems[i].Source.loop = true;
                stems[i].Source.volume = stems[i].volume;
                stems[i].Source.Play();
            }
            RefreshVolumes(true);
        }

        void Update()
        {
            int source0Time = stems[0].Source.timeSamples;
            if (previousSource0Time > source0Time)
            {
                // We're earlier in the song than previous frame. We looped!
                loops++;
                RefreshVolumes(true);
            }
            RefreshVolumes(false);
        
            previousSource0Time = source0Time;
        }

        public void RefreshVolumes(bool loop)
        {
            for (int i = 0; i < stems.Length; i++)
            {
                if (stems[i].waitForLoop)
                {
                    if (loop)
                    {
                        stems[i].Source.volume = stems[i].volume;
                    }
                }
                else
                {
                    float targetVolume = stems[i].volume;
                    float fadeSpeed = volumeFadeSpeed * Time.deltaTime;
                    stems[i].Source.volume = Mathf.MoveTowards(stems[i].Source.volume, targetVolume,fadeSpeed);
                }
            }
        }

        public void SetVolume(int stem, float volume)
        {
            stems[stem].volume = volume;
        }
    }
}
