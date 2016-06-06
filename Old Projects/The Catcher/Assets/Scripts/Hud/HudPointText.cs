using UnityEngine;
using System.Collections;

public class HudPointText : MonoBehaviour 
{
    public string prefix = string.Empty;
    public string pointMask = "000";
    public float amplitude = 0.5f;
    public float pulseTime = 0.1f;
    public AudioClip pulseAudioClip;
    [Range(0.0f, 1.0f)]
    public float volumePulse = 0.5f;
    
    private TextMesh text;
    private PulseState pulseState = PulseState.Idle;
    private float startTime;
    private Vector3 target;
    private Vector3 origin;
    

    public int Point { get; private set; }

    void Start()
    {
        text = GetComponentInChildren<TextMesh>();
        SetText();
    }

    void Update()
    {
        if (pulseState == PulseState.Up)
        {
            transform.position = Vector3.Lerp(origin, target, (Time.time - startTime) / pulseTime);
            if (Vector3.Distance(transform.position, target) <= 0.0f)
            {
                if (pulseAudioClip)
                    SoundManager.Instance.Play(pulseAudioClip, volumePulse, false);

                startTime = Time.time;
                pulseState = PulseState.Down;
            }
        }
        
        if (pulseState == PulseState.Down)
        {
            transform.position = Vector3.Lerp(target, origin, (Time.time - startTime) / pulseTime);
            if (Vector3.Distance(transform.position, origin) <= 0.0f)
                pulseState = PulseState.Idle;
        }
    }

    void SetText()
    {
        text.text = string.Format("{0}{1}", prefix, Point.ToString(pointMask));
    }

    public void AddPoint(int point)
    {
        this.Point += point;
        PulseAnimation();
        SetText();
    }

    public void PulseAnimation()
    {
        if (pulseState == PulseState.Idle)
        {
            origin = transform.position;
            target = origin - Vector3.forward * amplitude;
            startTime = Time.time;
            pulseState = PulseState.Up;
        }
    }
}

public enum PulseState
{
    Idle,
    Up,
    Down
}
