using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioListener))]
public class SoundManager : Singleton<SoundManager>
{
    public float m_TransitionTime = 1.0f;
    private AudioSource[] m_SoundFXSources = new AudioSource[4];
    private AudioSource[] m_MusicSources = new AudioSource[2];
    private int m_SoundFXIndex = 0;
    private int m_MusicIndex = 0;
    private float m_MusicVolume = 0.3f;
    private float m_SoundFXVolume = 1.0f;
    private bool m_MusicIsOn;
    private bool m_SoundFXIsOn;

    void Awake()
    {
        for (int i = 0; i < m_SoundFXSources.Length; i++)
        {
            m_SoundFXSources[i] = gameObject.AddComponent<AudioSource>();
            m_SoundFXSources[i].loop = false;
            m_SoundFXSources[i].volume = m_SoundFXVolume;
        }

        for (int i = 0; i < m_MusicSources.Length; i++)
        {
            m_MusicSources[i] = gameObject.AddComponent<AudioSource>();
            m_MusicSources[i].loop = true;
            m_MusicSources[i].volume = m_MusicVolume;
        }
    }

    void Start()
    {
        m_SoundFXIsOn = true;
        m_MusicIsOn = true;
    }

    void Update()
    {
        // Faz a transição da música de fundo
        m_MusicSources[1 - m_MusicIndex].volume = Mathf.Lerp(0.0f, 1.0f, m_TransitionTime * Time.deltaTime);
        m_MusicSources[m_MusicIndex].volume = Mathf.Lerp(1.0f, 0.0f, m_TransitionTime * Time.deltaTime);
    }

    public void PlaySoundFX(AudioClip clip)
    {
        if (!clip)
            return;

        m_SoundFXSources[m_SoundFXIndex].clip = clip;
        m_SoundFXSources[m_SoundFXIndex].Play();
        m_SoundFXIndex = ++m_SoundFXIndex % m_SoundFXSources.Length;
    }

    public void PlayMusic(AudioClip clip)
    {
        if (!clip)
            return;

        m_MusicIndex = ++m_MusicIndex % m_MusicSources.Length;
        m_MusicSources[m_MusicIndex].clip = clip;
        m_MusicSources[m_MusicIndex].Play();
    }

    public bool MusicIsOn
    {
        get
        {
            return m_MusicIsOn;
        }

        set
        {
            m_MusicIsOn = value;
            for (int i = 0; i < m_MusicSources.Length; i++)
                m_MusicSources[i].mute = !m_MusicIsOn;
        }
    }

    public bool SoundFXIsOn
    {
        get
        {
            return m_SoundFXIsOn;
        }

        set
        {
            m_SoundFXIsOn = value;
            for (int i = 0; i < m_SoundFXSources.Length; i++)
                m_SoundFXSources[i].mute = !m_SoundFXIsOn;
        }
    }
}
