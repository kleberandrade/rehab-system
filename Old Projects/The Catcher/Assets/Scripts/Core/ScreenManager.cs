using UnityEngine;

public class ScreenManager : Singleton<ScreenManager>
{
    public bool HasLoadingScreen
    {
        get
        {
            return loadingScreen.Enable;
        }

        set
        {
            loadingScreen.Enable = value;
        }
    }

    // Pause screen
    private bool isPause = false;

    //Referência para Loading Screen
    private LoadingScreen loadingScreen;

    void Start()
    {
        loadingScreen = LoadingScreen.Instance;
        HasLoadingScreen = false;
    }

    public void Load(string name)
    {
        HasLoadingScreen = false;
        loadingScreen.Load(name);
    }

    public void Load(string name, bool hasLoadingScreen)
    {
        HasLoadingScreen = hasLoadingScreen;
        loadingScreen.Load(name);
    }

    public void Quit()
    {
        HasLoadingScreen = false;
        loadingScreen.Quit();
    }

    public void Pause()
    {
        isPause = !isPause;
        Time.timeScale = 1.0f - Time.timeScale;
    }
}