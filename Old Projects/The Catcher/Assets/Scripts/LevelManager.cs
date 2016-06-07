using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour 
{
    #region Properties
    public AudioClip audioTheme;
    [Range(0.0f, 1.0f)]
    public float audioThemeVolume = 1.0f;
    public AudioClip backAudioTheme;
    [Range(0.0f, 1.0f)]
    public float backAudioThemVolume = 0.4f;

    public Transform finalText;
    public Transform finalScoreBackground;
    public TextMesh clearScoreText;

    public float showScoreBackgroundTime = 3.0f;
    public float showClearScoreTime = 0.5f;
    public float time = 60.0f;


    private Chronometer chronometer;
    private Score score;
    private TotalPoints totalPoints;

    private AudioSource theme;
    private AudioSource themeBack;

    #endregion

    void Start()
    {
        if (audioTheme)
            SoundManager.Instance.Play(audioTheme, audioThemeVolume, true);

        if (backAudioTheme)
            SoundManager.Instance.Play(backAudioTheme, backAudioThemVolume, true);

        // Singletons
        chronometer = Chronometer.Instance;
        chronometer.maxTimeInSeconds = time;
        score = Score.Instance;
        totalPoints = TotalPoints.Instance;
    }

	void OnEnable () 
    {
        Counter.EndCounter += OnEndCounting;
        NutSpawner.OnEndSpawn += OnEndNutSpawn;
        Chronometer.OnStopTime += OnStopTime;
	}

	void OnDisable ()
    {
        Counter.EndCounter -= OnEndCounting;
        NutSpawner.OnEndSpawn -= OnEndNutSpawn;
        Chronometer.OnStopTime -= OnStopTime;
	}

    void OnEndNutSpawn()
    {
        Invoke("StopTime", 0.5f);
        chronometer.transform.SendMessageUpwards("Hide");
        score.transform.SendMessage("Hide");
        totalPoints.transform.SendMessage("Hide");
    }

    void OnEndCounting()
    {
        chronometer.transform.SendMessageUpwards("Show");
        score.transform.SendMessage("Show");
        totalPoints.transform.SendMessage("Show");
        Invoke("StartTime", 0.5f);
    }

    void OnStopTime()
    {
        if (finalText)
            finalText.gameObject.SetActive(true);

        Invoke("ShowScoreBackground", showScoreBackgroundTime);

        theme.Stop();
        themeBack.Stop();
    }

    void ShowScoreBackground()
    {
        if (finalScoreBackground)
            finalScoreBackground.gameObject.SetActive(true);

        Invoke("ShowClearScore", showClearScoreTime);
    }

    void ShowClearScore()
    {
        clearScoreText.text = string.Format("Clear Score: {0}",
            (chronometer.TotalSeconds + score.Point + totalPoints.Point).ToString("0000000"));

        if (clearScoreText)
            clearScoreText.gameObject.SetActive(true);

        Invoke("ShowButtonOK", 2.0f);
    }

    void StartTime()
    {
        chronometer.StartTime();
    }

    void StopTime()
    {
        chronometer.StopTime();
    }

    void ShowButtonOK()
    {
        clearScoreText.transform.GetChild(0).gameObject.SetActive(true);
    }
}