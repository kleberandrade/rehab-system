using UnityEngine;
using System.Collections;

public class LoadingScreen : Singleton<LoadingScreen>
{
    // Loading screen
    private AsyncOperation async;
    public float Progress { get; set; }

    public Texture[] backgrounds;
    private int index = 0;
    public bool Enable { get; set; }
    public float MinTimeToLoad { get; set; }

    private bool showLoadingScreen = false;

    private bool quit = false;

    // Fade transition
    private Fade fade;

    void Start()
    {
        MinTimeToLoad = 2.0f;
        fade = Fade.Instance;
    }

    public void Load(string name)
    {
        RandomBackground();
        fade.AutoFadeIn = !Enable;
        StartCoroutine("Loading", name);
    }

    public void Quit()
    {
        fade.AutoFadeIn = !Enable;
        quit = true;
        StartCoroutine("Loading", name);
    }

    void Update()
    {
        if (async != null)
            Progress = async.progress;
    }

    void OnGUI()
    {
        GUI.depth = -500;
        if (showLoadingScreen)
        {
            if (backgrounds.Length > 0)
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), backgrounds[index]);
            GUILayout.Label("Loading: " + Progress);
        }
    }

    void RandomBackground()
    {
        if (backgrounds.Length > 0)
            index = Random.Range(0, backgrounds.Length);
    }

    IEnumerator Loading(string name)
    {
        yield return new WaitForSeconds(fade.BeginFade(FadeDir.Out));

        if (!quit)
        {
            if (Enable)
            {
                showLoadingScreen = true;
                yield return new WaitForSeconds(fade.BeginFade(FadeDir.In));
            }

            async = Application.LoadLevelAsync(name);

            while (!async.isDone)
                yield return new WaitForSeconds(MinTimeToLoad);

            if (Enable)
            {
                yield return new WaitForSeconds(fade.BeginFade(FadeDir.Out));
                showLoadingScreen = false;
                yield return new WaitForSeconds(fade.BeginFade(FadeDir.In));
            }
        }
        else
        {
            async = null;
            Application.Quit();
        }
    }
}
