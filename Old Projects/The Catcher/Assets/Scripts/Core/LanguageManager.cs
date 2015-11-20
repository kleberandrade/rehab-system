using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Globalization;

public class LanguageManager : Singleton<LanguageManager> 
{
    private Dictionary<string, string> textTable = new Dictionary<string, string>();
    private TextAsset xmlData;
    private XmlDocument xmlDocument = new XmlDocument();
    private string locale;
    private string directory = "Lang/";
    private string register = "Locale";
    private string defaultLocale = "en";

    public CultureInfo Culture { get; private set; }

    public string Locale 
    {
        get
        {
            return locale;
        }

        set
        {
            if (Load(value))
            {
                locale = value;
                Culture = CultureInfo.CreateSpecificCulture(locale);
                PlayerPrefs.SetString(register, locale);
                PlayerPrefs.Save();
            }
        }
    }

    void Start()
    {
        if (PlayerPrefs.HasKey(register))
            Locale = PlayerPrefs.GetString(register);
        else
            Locale = defaultLocale;
    }

    bool Load(string newLocale)
    {
        // Carregando o documento
        xmlData = Resources.Load<TextAsset>(Path.Combine(directory, string.Format("lang-{0}", newLocale)));
        if (xmlData == null)
            return false;

        xmlDocument.LoadXml(xmlData.text);

        // Lendo o documento .xml
        XmlNode root = xmlDocument.FirstChild;
        textTable.Clear();
        foreach (XmlNode node in root.ChildNodes)
            textTable.Add(node.Attributes["name"].Value, node.InnerText);

        return true;
    }

    public string GetText(string index)
    {
        if (!textTable.ContainsKey(index))
            return string.Empty;

        return textTable[index];
    }
}


