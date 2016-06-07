using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    public GUISkin bigCustomSkin;
    public GUISkin smallCustomSkin;
    public Texture companyLogo;
    public Texture universityLogo;

    private LanguageManager lang;
    private bool quitMessage = false;

    void Start()
    {
        lang = LanguageManager.Instance;
    }

	void OnGUI ()
    {
        if (bigCustomSkin)
            GUI.skin = bigCustomSkin;

        LogIn();

        if (smallCustomSkin)
            GUI.skin = smallCustomSkin;

        LeftButtons();

        FootNote();
	}

    void LeftButtons()
    {
        GUILayout.BeginArea(new Rect(Screen.width * 0.05f, Screen.height - 200, 150, 200));
        GUILayout.BeginVertical();

        BeginHorizontalCenter();
        if (GUILayout.Button(lang.GetText("account_button").ToUpper(), GUILayout.Width(130)))
        {
            ScreenManager.Instance.Load("Account", false);
        }
        EndHorizontalCenter();
        
        BeginHorizontalCenter();
        if (GUILayout.Button(lang.GetText("options_button").ToUpper(), GUILayout.Width(130)))
        {
            ScreenManager.Instance.Load("Options", false);
        }
        EndHorizontalCenter();

        BeginHorizontalCenter();
        if (GUILayout.Button(lang.GetText("credits_button").ToUpper(), GUILayout.Width(130)))
        {
            ScreenManager.Instance.Load("Credits", false);
        }
        EndHorizontalCenter();

        GUILayout.Space(20.0f);

        BeginHorizontalCenter();
        if (GUILayout.Button(lang.GetText("exit_button").ToUpper(), GUILayout.Width(130)))
        {
            UIDialog.OnYesClick += ExitGame;
            UIDialog.Instance.Show(
                lang.GetText("exit_title"),
                lang.GetText("exit_message"),
                400.0f, 
                120.0f,
                UIDialogType.NoYes);
        }
        EndHorizontalCenter();

        GUILayout.Space(20.0f);

        BeginHorizontalCenter();
        GUILayout.Label("1.0.23");
        EndHorizontalCenter();

        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    void LogIn()
    {
        GUILayout.BeginArea(new Rect(0, Screen.height - 320, Screen.width, 320));
        GUILayout.BeginVertical();

        BeginHorizontalCenter();
        GUILayout.Label(lang.GetText("login").ToUpper());
        EndHorizontalCenter();

        BeginHorizontalCenter();
        GUILayout.TextField("", GUILayout.Width(180));
        EndHorizontalCenter();

        BeginHorizontalCenter();
        GUILayout.Label(lang.GetText("password").ToUpper());
        EndHorizontalCenter();

        BeginHorizontalCenter();
        GUILayout.TextField("", GUILayout.Width(180));
        EndHorizontalCenter();

        BeginHorizontalCenter();
        GUILayout.Toggle(false, lang.GetText("remember_login"));
        EndHorizontalCenter();
        GUILayout.Space(20.0f);

        BeginHorizontalCenter();
        if (GUILayout.Button(lang.GetText("login_button").ToUpper(), GUILayout.Width(180)))
        {
            //UIDialog.OnYesClick += ExitGame;
            UIDialog.Instance.Show(
                lang.GetText("login_title"),
                lang.GetText("login_message"),
                400.0f,
                120,
                UIDialogType.Yes,
                null,
                "Cancel");
        }
        EndHorizontalCenter();

        if (smallCustomSkin)
            GUI.skin = smallCustomSkin;
        
        BeginHorizontalCenter();
        GUILayout.Label(string.Format("{0}: America", lang.GetText("region")), GUILayout.Width(180));
        EndHorizontalCenter();

        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    void FootNote()
    {
        GUILayout.BeginArea(new Rect(Screen.width * 0.85f, Screen.height - 180, 200, 200));
        BeginHorizontalCenter();
        if (universityLogo)
            GUILayout.Label(universityLogo, GUILayout.Width(140), GUILayout.Height(140));
        EndHorizontalCenter();
        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(0, Screen.height - 160, Screen.width, 320));
        BeginHorizontalCenter();
        if (companyLogo)
            GUILayout.Label(companyLogo, GUILayout.Width(260));
        EndHorizontalCenter();
        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(0, Screen.height - 50, Screen.width, 320));
        GUILayout.Label(lang.GetText("register"));
        GUILayout.EndArea();
    }

    void Language()
    {

    }

    void BeginHorizontalCenter()
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
    }

    void EndHorizontalCenter()
    {
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

    void ExitGame()
    {
        ScreenManager.Instance.Quit();
    }
}


