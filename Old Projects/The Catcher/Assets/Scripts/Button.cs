using UnityEngine;
using System.Collections;


public class Button : MonoBehaviour
{
    public Material buttonNormal;
    public Material buttonHover;
    public Material buttonDown;
    public AudioClip audioButtonClick;
    public bool exit = false;
    public bool loadingScreen = false;
    public string nextSceneToLoad = string.Empty;

    void Start()
    {
        if (buttonNormal)
            GetComponent<Renderer>().material = buttonNormal;
    }

    void OnMouseEnter()
    {
        if (buttonHover)
            GetComponent<Renderer>().material = buttonHover;
    }

    void OnMouseUp()
    {
        if (buttonNormal)
            GetComponent<Renderer>().material = buttonNormal;
    }

    void OnMouseExit()
    {
        if (buttonNormal)
            GetComponent<Renderer>().material = buttonNormal;
    }

    void OnMouseDown()
    {
        if (buttonDown)
            GetComponent<Renderer>().material = buttonDown;

        if (audioButtonClick)
            AudioSource.PlayClipAtPoint(audioButtonClick, transform.position);

        if (exit)
            ScreenManager.Instance.Quit();
        else 
            ScreenManager.Instance.Load(nextSceneToLoad, loadingScreen);
    }
}