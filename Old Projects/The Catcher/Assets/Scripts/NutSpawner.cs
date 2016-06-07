using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NutSpawner : MonoBehaviour
{
    #region Delegates and Events
    public delegate void ActionHandler();
    public static event ActionHandler OnSpawn;
    public static event ActionHandler OnEndSpawn;
    #endregion

    public AudioClip windClip;
    [Range(0.0f, 1.0f)]
    public float volumeWindClip = 0.3f;
    public int nutTotal = 20;
    public float timeToOscilattion = 1.0f;
    public float periodOscillator = 35.0f;
    public float amplitudeOscillator = 3.0f;
    public bool randomPosition = false;

    private List<Oscillator> oscilattors;
    private float normalPeriodOscillator = 2.0f;
    private float normalAmplitudeOscillator = 1.0f;
    private TextMesh nutRemainingText;

    void Start()
    {
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Tree");
        oscilattors = new List<Oscillator>();
        for (int i = 0; i < gos.Length; i++ )
        {
            oscilattors.Add(gos[i].GetComponent<Oscillator>());
            normalAmplitudeOscillator = oscilattors[i].amplitude;
            normalPeriodOscillator = oscilattors[i].period;
        }

        if (GetComponent<ParticleSystem>())
            GetComponent<ParticleSystem>().enableEmission = false;

        nutRemainingText = GameObject.FindGameObjectWithTag("Nut Remaining").GetComponent<TextMesh>();
        nutRemainingText.text = nutTotal.ToString("000");
    }


    void NutBeginBorn()
    {
        if (windClip)
            SoundManager.Instance.Play(windClip, volumeWindClip, false);

        if (GetComponent<ParticleSystem>())
            GetComponent<ParticleSystem>().enableEmission = true;

        foreach (Oscillator o in oscilattors)
        {
            o.period = periodOscillator;
            o.amplitude = amplitudeOscillator;
        }

        StartCoroutine("NutEndBorn");
    }

    IEnumerator NutEndBorn()
    {
        yield return new WaitForSeconds(timeToOscilattion);

        if (GetComponent<ParticleSystem>())
            GetComponent<ParticleSystem>().enableEmission = false;

        foreach (Oscillator o in oscilattors)
        {
            o.period = normalPeriodOscillator;
            o.amplitude = normalAmplitudeOscillator;
        }
    }

    public void SetRemainingText()
    {
        nutTotal--;
        nutRemainingText.text = nutTotal.ToString("000");
    }

	public void Spawn () 
    {
        if (nutTotal == 0)
        {
            if (OnEndSpawn != null)
                OnEndSpawn();

            return;
        }

        GameObject go = NutPooler.Instance.NextObject();
        
        if (go == null)
            return;

        if (randomPosition)
            transform.position = NextRandomPosition();

        go.transform.position = transform.position;
        go.transform.rotation = transform.rotation;
        go.SetActive(true);

        NutBeginBorn();
        SetRemainingText();

	}

    Vector3 NextRandomPosition()
    {
        Vector3 position = transform.position;
        position.x = Random.Range(Camera.main.ViewportToWorldPoint(new Vector3(0.1f, 0.0f, 29.0f)).x, Camera.main.ViewportToWorldPoint(new Vector3(0.9f, 0.0f, 29.0f)).x);

        return position;
    }

}
