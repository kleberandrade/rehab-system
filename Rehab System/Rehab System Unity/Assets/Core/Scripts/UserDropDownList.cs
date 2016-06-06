using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UserDropDownList : MonoBehaviour
{
    [SerializeField]
    private GameObject m_UserItem;

    private Text m_UserSelectedText;

    private List<User> m_Users;

    private GridLayoutGroup m_Grid;

    void Awake()
    {
        m_Grid = GetComponent<GridLayoutGroup>();
    }

    void Start()
    {
        m_Users = new List<User>();
        m_Users.Add(new User() { Name = "Kleber" });
        m_Users.Add(new User() { Name = "Guilherme" });
        m_Users.Add(new User() { Name = "Glauco" });
        m_Users.Add(new User() { Name = "Caio" });
        m_Users.Add(new User() { Name = "Viviane" });
        m_Users.Add(new User() { Name = "Marcelo" });
        m_Users.Add(new User() { Name = "Henrique" });
        m_Users.Add(new User() { Name = "Rafael" });
        m_Users.Add(new User() { Name = "Thales" });

        RefreshList();
    }

    void AddUser(string userName)
    {
        GameObject localItem = (GameObject)Instantiate(m_UserItem, Vector3.zero, Quaternion.identity);
        localItem.GetComponentInChildren<Text>().text = userName;
        localItem.transform.SetParent(m_Grid.transform);
        localItem.transform.localScale = Vector3.one;
    }

    public void RefreshList()
    {
        foreach (User user in m_Users)
            AddUser(user.Name);
    }

    void OnClick()
    {

    }
}
