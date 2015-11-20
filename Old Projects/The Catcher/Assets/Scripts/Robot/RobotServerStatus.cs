using UnityEngine;
using System.Collections;

[RequireComponent (typeof(GUIText))]
public class RobotServerStatus : MonoBehaviour
{
    private RobotServer robot;

    void Start()
    {
        robot = RobotServer.Instance;
    }

    void OnEnable()
    {
        RobotServer.OnStarted += OnStarted;
        RobotServer.OnStopped += OnStopped;
        RobotServer.OnConnect += OnConnect;
        RobotServer.OnDisconnect += OnDisconnect;
        RobotServer.OnReceive += OnReceive;
    }

    void OnDisable()
    {
        RobotServer.OnStarted -= OnStarted;
        RobotServer.OnStopped -= OnStopped;
        RobotServer.OnConnect -= OnConnect;
        RobotServer.OnDisconnect -= OnDisconnect;
        RobotServer.OnReceive -= OnReceive;
    }

    void DebugLog()
    {
        Debug.Log(GetComponent<GUIText>().text.ToString());
    }

    void OnDisconnect()
    {
        GetComponent<GUIText>().text = "Client disconnect";
    }

    void OnStarted()
    {
        GetComponent<GUIText>().text = "Robot server started in port " + RobotServer.Instance.Port;
    }

    void OnStopped()
    {
        GetComponent<GUIText>().text = "Robot server stopped";
    }

    void OnReceive()
    {
        GetComponent<GUIText>().text = "Robot position (degrees): " + robot.RobotRequestData.Position.ToString();
    }

    void OnConnect()
    {
        GetComponent<GUIText>().text = "Client connected";
    }
}
