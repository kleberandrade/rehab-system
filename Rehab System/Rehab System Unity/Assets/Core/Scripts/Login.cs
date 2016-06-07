using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    private SceneBehaviour m_Scene;
    [SerializeField]
    private string m_NextSceneNave = "General";
    [SerializeField] 
    private InputField m_NameInputField;
    [SerializeField]
    private InputField m_PasswordInputField;
    [SerializeField]
    private Toggle m_RememberToggle;
    private string m_RememberKey = "RememberLogin";

	void Awake ()
    {
        m_Scene = GetComponent<SceneBehaviour>();
        if (PlayerPrefs.HasKey(m_RememberKey))
            m_NameInputField.text = PlayerPrefs.GetString(m_RememberKey);
	}
	
	public void SignIn ()
    {
        if (!UserValidate(m_NameInputField.text, m_PasswordInputField.text)){
            return;
        }

        Remember();

        m_Scene.LoadLevel(m_NextSceneNave);
	}

    void Remember()
    {
        if (m_RememberToggle.isOn)
            PlayerPrefs.SetString(m_RememberKey, m_NameInputField.text);
        else
            PlayerPrefs.DeleteKey(m_RememberKey);

        PlayerPrefs.Save();
    }

    bool UserValidate(string name, string password)
    {
        Debug.Log("User: " + name + " | Password: " + password);



        return name.Equals("kleber") && password.Equals("123");
    }
}
