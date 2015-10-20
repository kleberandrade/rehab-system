using UnityEngine;
using System.Collections;

/// <summary>
/// Classe básica para comportalmente da cena
/// </summary>
public class SceneBehaviour : MonoBehaviour 
{
    /// <summary>
    /// Referência para o gerenciador de audios
    /// </summary>
    private SoundManager m_SoundManager;

    /// <summary>
    /// Referência para o gerênciador de telas
    /// </summary>
    private ScreenManager m_ScreenManager;

    /// <summary>
    /// Captura as referências da variável no início
    /// </summary>
	void Start () 
    {
        m_ScreenManager = ScreenManager.Instance;
        m_SoundManager = SoundManager.Instance;
	}

    /// <summary>
    /// Toca uma efeito sonoro
    /// </summary>
    /// <param name="clip">Efeito sonoro a ser tocado</param>
    public void PlaySoundFX(AudioClip clip)
    {
        m_SoundManager.PlaySoundFX(clip);
    }

    /// <summary>
    /// Toca uma música de fundo
    /// </summary>
    /// <param name="clip">Música a ser tocada</param>
    public void PlayMusic(AudioClip clip)
    {
        m_SoundManager.PlayMusic(clip);
    }

    /// <summary>
    /// Carrega uma nova cena
    /// </summary>
    /// <param name="levelName">Nome da cena</param>
    public void LoadLevel(string levelName)
    {
        m_ScreenManager.LoadLevel(levelName);
    }

    /// <summary>
    /// Carrega a cena anterior
    /// </summary>
    public void LoadPreviousLevel()
    {
        m_ScreenManager.LoadPreviousLevel();
    }

    /// <summary>
    /// Carrega a cena anterior
    /// </summary>
    /// <param name="clip">Som a ser tocado</param>
    public void LoadPreviousLevelWithPlaySound(AudioClip clip)
    {
        m_SoundManager.PlaySoundFX(clip);
        m_ScreenManager.LoadPreviousLevel();
    }

    /// <summary>
    /// Carrega uma nova cena
    /// </summary>
    /// <param name="levelName">Nome da cena</param>
    /// <param name="clip">Som a ser tocado</param>
    public void LoadLevelWithPlaySound(string levelName, AudioClip clip)
    {
        m_SoundManager.PlaySoundFX(clip);
        LoadLevel(levelName);
    }
}
