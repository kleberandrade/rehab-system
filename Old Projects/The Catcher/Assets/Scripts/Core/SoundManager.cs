using UnityEngine;
using System.Collections.Generic;

public class SoundManager : Singleton<SoundManager>
{
    private List<AudioSource> sources;
    public int pooledAmount = 10;

    void Start()
    {
        sources = new List<AudioSource>();
        for (int i = 0; i < pooledAmount; i++)
            sources.Add(gameObject.AddComponent<AudioSource>());
    }

    public AudioSource Play(AudioClip clip)
    {
        return Play(clip, 1.0f, false);
    }

    public AudioSource Play(AudioClip clip, float volume)
    {
        return Play(clip, volume, false);
    }

    public AudioSource Play(AudioClip clip, float volume, bool loop)
    {
        for (int i = 0; i < sources.Count; i++)
        {
            if (!sources[i].isPlaying)
            {
                sources[i].clip = clip;
                sources[i].volume = volume;
                sources[i].loop = loop;
                sources[i].Play();
                return sources[i];
            }
        }

        sources.Add(gameObject.AddComponent<AudioSource>());
        sources[sources.Count - 1].clip = clip;
        sources[sources.Count - 1].Play();
        return sources[sources.Count - 1];
    }
}
