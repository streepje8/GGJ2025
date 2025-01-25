using UnityEngine;

namespace FondantMetStokjes.Music
{
    public class MusicManager : MonoBehaviour
    {
        [SerializeField] AudioClip[] stems;
        [SerializeField] float[] volumes;
        private AudioSource[] sources;

        void OnValidate()
        {
            if (volumes.Length != stems.Length)
            {
                float[] old = volumes;
                volumes = new float[stems.Length];
                for (int i = 0; i < Mathf.Min(old.Length, volumes.Length); i++)
                {
                    volumes[i] = old[i];
                }

                if (old.Length < volumes.Length)
                {
                    for (int i = old.Length; i < volumes.Length; i++)
                    {
                        volumes[i] = volumes[i - 1];
                    }
                }
            }

            for (int i = 0; i < volumes.Length; i++)
            {
                volumes[i] = Mathf.Clamp01(volumes[i]);
            }
        }
        
        void Start()
        {
            sources = new AudioSource[stems.Length];
            for (int i = 0; i < stems.Length; i++)
            {
                GameObject stemObject = new GameObject("Source " + i, typeof(AudioSource));
                stemObject.transform.parent = transform;
                sources[i] = stemObject.GetComponent<AudioSource>();
                sources[i].clip = stems[i];
                sources[i].loop = true;
                sources[i].Play();
            }
        }

        void Update()
        {
            RefreshVolumes();
        }

        public void SetVolume(int stem, float volume)
        {
            volumes[stem] = volume;
            sources[stem].volume = volume;
        }

        public void RefreshVolumes()
        {
            for (int i = 0; i < sources.Length; i++)
            {
                sources[i].volume = volumes[i];
            }
        }
    }
}
