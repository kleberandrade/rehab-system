using UnityEngine;
using System.Collections;
using System;
using System.Text;

public class Chronometer : Singleton<Chronometer>
{
    #region Delegates and Events
    public delegate void ChronometerEvent();
    public static event ChronometerEvent OnStartTime;
    public static event ChronometerEvent OnResumeTime;
    public static event ChronometerEvent OnStopTime;
    public static event ChronometerEvent OnPauseTime;
    #endregion

    #region Fields
    public ChronometerType chronometerType = ChronometerType.CountDown;
    public float maxTimeInSeconds = 0;
    private float startTime = 0.0f;
    private float endTime = 0.0f;

    private bool paused = false;
    private bool stopped = true;
    #endregion

    private TextMesh text;

    void Start()
    {
        text = GetComponent<TextMesh>();
    }

    void OnGUI()
    {
        text.text = TotalSeconds.ToString("000");
    }

    void Update()
    {
        if (!stopped)
        {
            endTime = Time.time;
            if (TotalSeconds == 0)
                StopTime();
        }
    }

    #region Methods
    public void StartTime()
    {
        stopped = false;
        startTime = Time.time;
        if (OnStartTime != null)
            OnStartTime();
    }

    public void ResumeTime()
    {
        paused = false;
        Time.timeScale = 1.0f;
        if (OnResumeTime != null)
            OnResumeTime();
    }

    public void PauseTime()
    {
        paused = true;
        Time.timeScale = 0.0f;
        if (OnPauseTime != null)
            OnPauseTime();
    }

    public void StopTime()
    {
        stopped = true;
        if (OnStopTime != null)
            OnStopTime();
    }
    #endregion

    public int TotalSeconds
    {
        get
        {
            if (chronometerType == ChronometerType.StopWatch)
                return Mathf.CeilToInt(endTime - startTime);
            else
                return Mathf.CeilToInt(Mathf.Clamp(maxTimeInSeconds - (endTime - startTime), 0.0f, maxTimeInSeconds));
        }
    }

    public bool IsPaused
    {
        get
        {
            return this.paused;
        }
    }

    public bool IsStopped
    {
        get
        {
            return this.stopped;
        }
    }
}

public enum ChronometerType
{
    CountDown,
    StopWatch
}