using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Xml;

public class GameDropDownList : MonoBehaviour
{
    [SerializeField]
    private GameObject m_GameItem;
    [SerializeField]
    private TextAsset m_XMLText;

    private GridLayoutGroup m_Grid;
    
    void Awake()
    {
        m_Grid = GetComponent<GridLayoutGroup>();
    }

    void Start()
    {
        RefreshList();
    }

    void AddItem(string text)
    {
        GameObject localItem = (GameObject)Instantiate(m_GameItem, Vector3.zero, Quaternion.identity);
        localItem.GetComponentInChildren<Text>().text = text;
        localItem.transform.SetParent(m_Grid.transform);
        localItem.transform.localScale = Vector3.one;
    }

    public void RefreshList()
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(m_XMLText.text);
        XmlNodeList games = xmlDoc.GetElementsByTagName("game");

        foreach (XmlNode game in games)
            AddItem(game.Attributes["name"].Value);
    }
}
