using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Basket : MonoBehaviour
{
    #region Delegates and Events
    public delegate void ActionHandler();
    public static event ActionHandler OnCapture;
    #endregion

    public AudioClip collectedClip;
    public GameObject hitPointPrefab;
    public int pooledAmount = 3;
    public bool willGrow = true;

    private List<GameObject> pooledObjects;
    private int hitCombo = 0;

    void OnEnable()
    {
        Nut.OnGrounded += OnComboHitExit;
    }

    void OnDisable()
    {
        Nut.OnGrounded -= OnComboHitExit;
    }

    void OnComboHitExit()
    {
        hitCombo = 0;
    }

    void Start()
    {
        pooledObjects = new List<GameObject>();
        for (int i = 0; i < pooledAmount; i++)
        {
            GameObject go = (GameObject)Instantiate(hitPointPrefab);
            go.SetActive(false);
            pooledObjects.Add(go);
        }

        if (GetComponent<ParticleSystem>())
            GetComponent<ParticleSystem>().enableEmission = false;
    }

    GameObject NextObject()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
                return pooledObjects[i];
        }

        if (willGrow)
        {
            GameObject go = (GameObject)Instantiate(hitPointPrefab);
            pooledObjects.Add(go);
            return go;
        }

        return null;
    }

    void ThrowPoints()
    {
        GameObject go = this.NextObject();

        if (go == null)
            return;

        go.transform.position = transform.position;
        go.transform.rotation = Quaternion.identity;
        go.SetActive(true);
        go.transform.SendMessage("SetPoint", ++hitCombo);

        TotalPoints.Instance.Add(hitCombo);
    }

    void OnTriggerEnter(Collider hit)
    {
        hit.SendMessage("Collected");

        if (collectedClip)
            SoundManager.Instance.Play(collectedClip, 0.7f, false);

        if (GetComponent<ParticleSystem>())
        {
            GetComponent<ParticleSystem>().enableEmission = true;
            StartCoroutine("CatcherEffectDie");
        }
            
        ThrowPoints();

        Camera.main.SendMessage("Shake");

        if (OnCapture != null)
            OnCapture();
    }

    IEnumerator CatcherEffectDie()
    {
        yield return new WaitForSeconds(0.5f);
        GetComponent<ParticleSystem>().enableEmission = false;
    }
}
