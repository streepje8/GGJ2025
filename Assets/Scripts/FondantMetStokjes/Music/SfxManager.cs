using System;
using UnityEngine;

[System.Serializable]
public struct SfxClip
{
    public AudioClip clip;
    public float volume;
}

public class SfxManager : MonoBehaviour
{
    public static SfxManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    public void SpawnSound(SfxClip clip, Vector3 position)
    {
        if (!clip.clip)
            return; // Dit is makkelijker dan deze check overal doen. Wel jammer van het gebrek aan errors.
        var obj = new GameObject(clip.clip.name);
        obj.transform.parent = gameObject.transform;
        obj.transform.position = position;
        AudioSource source = obj.AddComponent<AudioSource>();
        source.clip = clip.clip;
        source.volume = clip.volume;
        source.Play();
        Destroy(obj,clip.clip.length + 0.1f);
    }
}
