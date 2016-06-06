using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System;
using System.Net;
using System.Threading;
using System.Xml;

public class RobotServer : Singleton<RobotServer>
{
    #region Delegates and Events
    public delegate void ServerEventArgs();
    public static event ServerEventArgs OnStarted;
    public static event ServerEventArgs OnStopped;
    public static event ServerEventArgs OnConnect;
    public static event ServerEventArgs OnReceive;
    public static event ServerEventArgs OnSended;
    public static event ServerEventArgs OnDisconnect;
    #endregion

    private TcpListener server = null;
    private TcpClient tcpClient = null;
    private NetworkStream networkStream = null;
    private TextAsset xmlData;
    private float lastReceiveTime;

    public int TimeOut { get; private set; }
    public int Port { get; private set; }
    public int SleepTime { get; private set; }
    public int Backlog { get; private set; }
    public bool IsRunning { get; private set; }

    public bool ClientConnected
    {
        get
        {
            if (tcpClient == null)
                return false;

            return tcpClient.Connected;
        }
    }

    public RequestData RobotRequestData { get; set; }

    public DispatcherData GameDispatcherData { get; set; }

    public IPAddress Address { get; private set; }

    void Start()
    {
        GameDispatcherData = new DispatcherData(64);
        RobotRequestData = new RequestData(64);
        Port = 13000;
        TimeOut = 1;
        Open();
    }

    public void Open()
    {
        server = new TcpListener(IPAddress.Any, Port);
        //server.Server.ReceiveTimeout = 2;
        //server.Server.SendTimeout = 2;
        server.Start();
        IsRunning = true;

        if (OnStarted != null)
            OnStarted();
    }

    void Update()
    {
        if (IsRunning)
        {
            if (tcpClient == null)
            {
                if (server.Pending())
                {
                    tcpClient = server.AcceptTcpClient();
                    networkStream = tcpClient.GetStream();

                    if (OnConnect != null)
                        OnConnect();

                    lastReceiveTime = Time.time;
                }
            }
            else
            {
                if (networkStream.DataAvailable)
                {
                    lastReceiveTime = Time.time;

                    networkStream.Read(RobotRequestData.Buffer, 0, RobotRequestData.Buffer.Length);
                    RobotRequestData.Deserialize();

                    if (OnReceive != null)
                        OnReceive();

                    GameDispatcherData.Serialize();
                    networkStream.Write(GameDispatcherData.Buffer, 0, GameDispatcherData.Buffer.Length);
                    networkStream.Flush();

                    if (OnSended != null)
                        OnSended();
                }

                if (Time.time - lastReceiveTime > TimeOut)
                    CloseClient();
            }
        }
    }

    public void CloseClient()
    {
        if (networkStream != null)
        {
            networkStream.Close();
            networkStream = null;
        }

        if (tcpClient != null)
        {
            tcpClient.Close();
            tcpClient = null;
        }

        if (OnDisconnect != null)
            OnDisconnect();
    }

    public void Stop()
    {
        CloseClient();

        IsRunning = false;

        if (server != null)
        {
            server.Stop();
            server = null;
        }

        if (OnStopped != null)
            OnStopped();
    }

    void OnApplicationQuit()
    {
        Stop();
    }
}
