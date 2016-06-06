using UnityEngine;
using System.Collections;

public class UIDialog : Singleton<UIDialog>
{
    #region Delegate and Events
    public delegate void ActionHandler();
    public static event ActionHandler OnYesClick;
    public static event ActionHandler OnNoClick;
    #endregion

    #region Fields
    private GUISkin customSkin;
    private string message = "UIDialog message";
    private string title = "Title";
    private float height = 120.0f;
    private float widht = 400.0f;
    private string yesButton = "Yes";
    private string noButton = "No";
    private UIDialogType dialogType = UIDialogType.NoYes; 
    private bool showing = false;
    #endregion

    void Start()
    {
        customSkin = Resources.Load("Skin/UIDialog") as GUISkin;
    }

	void OnGUI () 
    {
        if (customSkin)
            GUI.skin = customSkin;

	    if (showing)
        {
            GUI.ModalWindow(0, 
                new Rect(Screen.width / 2.0f - widht / 2.0f,
                    Screen.height / 2.0f - height / 2.0f,
                    widht, height), 
                DialogBox, 
                title); 
        }
    }

    public void Show(string title, string message, float width, float height, UIDialogType dialogType)
    {
        this.yesButton = "Yes";
        this.noButton = "No";
        this.message = message;
        this.title = title;
        this.widht = width;
        this.height = height;
        this.dialogType = dialogType;
        this.showing = true;
    }

    public void Show(string title, string message, float width, float height, UIDialogType dialogType, string noButton, string yesButton)
    {
        this.yesButton = yesButton;
        this.noButton = noButton;
        this.message = message;
        this.title = title;
        this.widht = width;
        this.height = height;
        this.dialogType = dialogType;
        this.showing = true;
    }

    public void Hide()
    {
        this.showing = false;
        OnYesClick = null;
        OnNoClick = null;
    }

    void DialogBox(int windowID)
    {
        GUILayout.BeginVertical();
        GUILayout.FlexibleSpace();

        GUILayout.Label(message);

        GUILayout.Space(10.0f);

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (dialogType == UIDialogType.No || dialogType == UIDialogType.NoYes)
        {
            if (GUILayout.Button(noButton, GUILayout.Width(100)))
            {
                if (OnNoClick != null)
                    OnNoClick();

                Hide();
            }

            GUILayout.Space(30.0f);
        }

        if (dialogType == UIDialogType.Yes || dialogType == UIDialogType.NoYes)
        {
            if (GUILayout.Button(yesButton, GUILayout.Width(100)))
            {
                if (OnYesClick != null)
                    OnYesClick();

                Hide();
            }
        }
        GUILayout.FlexibleSpace(); 
        GUILayout.EndHorizontal();

        GUILayout.FlexibleSpace(); 
        GUILayout.EndVertical();
    }
}

public enum UIDialogType
{
    No,
    Yes,
    NoYes
}
