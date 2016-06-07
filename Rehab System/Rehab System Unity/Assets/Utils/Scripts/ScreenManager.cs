using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : Singleton<ScreenManager>
{
    private FadeTransition m_FadeTransition;
    private Stack<string> m_StackScenes;

    void Start()
    {
        m_FadeTransition = FadeTransition.Instance;
        m_StackScenes = new Stack<string>();
    }

    public void LoadPreviousLevel()
    {
        string previousLevel = m_StackScenes.Pop();
        LoadLevel(previousLevel, true);
    }

    public void LoadLevel(string sceneNameToLoad)
    {
        LoadLevel(sceneNameToLoad, false);
    }

    private void LoadLevel(string sceneNameToLoad, bool previousScene)
    {
        if (!previousScene)
        {
            m_StackScenes.Push(Application.loadedLevelName);
            Debug.Log("Previous Level: " + Application.loadedLevelName);
        }

        StartCoroutine("Loading", sceneNameToLoad);
    }

    IEnumerator Loading(string name)
    {
        yield return new WaitForSeconds(m_FadeTransition.BeginFade(FadeDirection.Out));

        if (!name.Equals("Quit"))
        {
            Application.LoadLevel(name);
        }
        else
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }

    public void Quit()
    {
        StartCoroutine("Loading", "Quit");
    }

    public void Pause()
    {
        Time.timeScale = 1.0f - Time.timeScale;
    }
}
