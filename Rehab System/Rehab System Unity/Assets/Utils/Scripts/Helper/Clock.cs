using System.Collections;
using UnityEngine;

/// <summary>
/// Classe que simula um relógio com tempo crescente e decrescente
/// </summary>
public class Clock : MonoBehaviour
{
    /// <summary>
    /// Tempo total do relógio
    /// </summary>
    [SerializeField]
    private float m_TotalSeconds = 75.0f;

    /// <summary>
    /// Som que o relógio faz, muito usado para indicar que o tempo esta chegando ao final
    /// </summary>
    [SerializeField]
    private AudioClip m_ClockSound = null;

    /// <summary>
    /// Tempo restante para começar a tocar o som (uma vez por segundo)
    /// </summary>
    [SerializeField]
    private int m_TimeToPlaySound = 5;

    /// <summary>
    /// Guarda o tempo inicial
    /// </summary>
    private float m_StartTime = 0.0f;

    /// <summary>
    /// Armazena passado até o momento
    /// </summary>
    private float m_ElapsedTime = 0.0f;

    /// <summary>
    /// Indica se o relógio não esta parado
    /// </summary>
    private bool m_Working = false;

    /// <summary>
    /// Referência para o gerênciador de audios
    /// </summary>
    private SoundManager m_Sound;

    /// <summary>
    /// Inicializa as variáveis
    /// </summary>
    void Start()
    {
        m_Sound = SoundManager.Instance;
    }

    /// <summary>
    /// Atualização por Frames/Segundo
    /// </summary>
    void Update()
    {
        if (m_Working)
        {
            m_ElapsedTime = Mathf.Clamp(Time.time - m_StartTime, 0.0f, m_TotalSeconds);
            if (ElapsedTime(ClockType.CountDown) <= m_TimeToPlaySound)
            {
                m_TimeToPlaySound--;
                m_Sound.PlaySoundFX(m_ClockSound);
            }
            m_Working = float.Equals(m_ElapsedTime, m_TotalSeconds);
        }
    }


    /// <summary>
    /// Inicia o contador de tempo
    /// </summary>
    public void StartTime()
    {
        m_Working = true;
        m_StartTime = Time.time;
    }

    /// <summary>
    /// Para a contagem de tempo
    /// </summary>
    public void StopTime()
    {
        m_Working = false;
    }

    /// <summary>
    /// Verifica quanto tempo foi passado
    /// </summary>
    /// <param name="clockType">Tipo do relógio (StopWatcher|Countdown)</param>
    /// <returns></returns>
    public int ElapsedTime(ClockType clockType)
    {
        if (clockType == ClockType.StopWatch)
            return Mathf.CeilToInt(m_ElapsedTime);

        return Mathf.CeilToInt(m_TotalSeconds - m_ElapsedTime);
    }

    /// <summary>
    /// Quanto tempo foi passado em porcentagem
    /// </summary>
    /// <returns>porcentagem do tempo</returns>
    public float PercentageOfElapsedTime()
    {
        return m_ElapsedTime / m_TotalSeconds;
    }

    /// <summary>
    /// Verifica se o relógio esta parado
    /// </summary>
    public bool IsStopped
    {
        get
        {
            return !m_Working;
        }
    }
}

public enum ClockType
{
    CountDown,
    StopWatch
}