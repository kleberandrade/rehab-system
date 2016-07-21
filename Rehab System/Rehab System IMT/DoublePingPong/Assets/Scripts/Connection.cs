using UnityEngine;
//using UnityEngine.UI;

using System;
using System.IO;
using System.Text;

using System.Threading;
using System.Collections;

using System.Net;
using System.Net.Sockets;
using System.Linq;


public class Connection : MonoBehaviour {

	enum ConnectStatus {waiting, connecting, connected, disconnected};

	// Indice relativo da variavel
	public const int CENTERSPRING = 0; 	// Game
	public const int FREESPACE = 1; 	// Game
	public const int STIFF = 2;			// Game
	public const int DAMP = 3;			// Game

	public const int POSITION = 0; 		// Robo
	public const int VELOCITY = 1; 		// Robo
	public const int ACC = 2;			// Robo
	public const int FORCE = 3;			// Robo

	public const int N_VAR = 4; 		// Numero de variaveis envolvidas
	private const int BIT_SIZE = 4; 	// Numero de bit da mascara; Deve ser multiplo de 2
	private const int INFO_SIZE = 4;	// 4 Float; 8 Double
	
	private int n_Robots;
//	private int mask_size; 
//	private byte[] activeMask;
	private byte[][][] gameStade;
	private float[][] robotStade;
	private byte[] gameStatus;
	private short robotStatus;

	private float delayCount;

	private Thread connectingThread;
	//private volatile bool stopThread;

	//public bool connected;
	ConnectStatus connectStatus;

//================================
	private NetworkClientTCP clientHere = new NetworkClientTCP();
//================================

//	private string textFile = @"D:\Users\Thales\Documents\Faculdade\2015 - 201x - Mestrado\RehabLab\RehabSystem\Rehab System\Rehab System IMT\DoublePingPong\Logs\LogFilePos.txt";

	void Start()
	{
//		stopThread = false;
		connectingThread = new Thread (Connect);
		connectingThread.Start ();
		if (clientHere.IsConnected ())
			connectStatus = ConnectStatus.connected;
		else
			connectStatus = ConnectStatus.waiting;
	}


	void FixedUpdate()
	{
		switch (connectStatus)
		{
			case ConnectStatus.connected:
				SendMsg ();
				//			ClearMask ();
				ReadMsg ();
				break;
		}
	}

	void Connect()
	{
		bool inLoop = true;
		while (inLoop)
		{
			switch (connectStatus)
			{
				case ConnectStatus.waiting:
					Debug.Log ("Starting connection");
					clientHere.Connect ("192.168.0.66", 8000, 0); // Here 192.168.0.67
					//	clientHere.SendString ("Connected!"); 

					InitializeVariables (2); // Entre com o numero de robos
					connectStatus = ConnectStatus.connecting;
					break;
				case ConnectStatus.connecting:
					Debug.Log ("Trying to connect");
					if (clientHere.IsConnected ())
						connectStatus = ConnectStatus.connected;
					break;
				default:
					inLoop = false;
					break;
			}
		}
	}

	public void SetStatus(int robot, float mag, int variable)
	{
//		activeMask[(BIT_SIZE * robot) / 8] |= (byte)(0x1 << (variable + robot*BIT_SIZE));
		gameStade [robot] [variable] = BitConverter.GetBytes (mag);
		return;
	}

	public bool IsConnected()
	{
		return clientHere.IsConnected ();
	}

	public void SetStatus(short status)
	{
		gameStatus = BitConverter.GetBytes((short)(status + 1));
	}

	public float ReadStatus(int robot, int variable)
	{
		return robotStade [robot] [variable];
	}

	public short ReadStatus()
	{
		return robotStatus;
	}

	private void SendMsg ()
	{
		byte[] msg = new byte[sizeof(short) + (N_VAR * INFO_SIZE) * n_Robots];
		System.Buffer.BlockCopy (gameStatus, 0, msg, 0, sizeof(short));
//		System.Buffer.BlockCopy (activeMask, 0, msg, 0, activeMask.Length);
		for (int i = 0; i < n_Robots; i++) 
		{
			for (int j = 0; j < N_VAR; j++)
			{
				System.Buffer.BlockCopy (gameStade [i][j], 0, msg, sizeof(short) + INFO_SIZE * (j + N_VAR * i), gameStade [i][j].Length);
			}
		}
		clientHere.SendByte (msg);
		return;
	}

	private void ReadMsg()
	{
		byte[] buffer = clientHere.ReceiveByte ();

		// Check if message is different than zero
		bool check = false;
		foreach(byte element in buffer)
		{
			if (element != 0x0)
			{
				check = true;
				break;
			}
		}

		if (check)
		{
			robotStatus = BitConverter.ToInt16 (buffer, 0);
			for (int i = 0; i < n_Robots; i++)
			{
				for (int j = 0; j < N_VAR; j++)
				{
					robotStade[i][j] = BitConverter.ToSingle (buffer, sizeof(short) + INFO_SIZE*(j + N_VAR*i));
				}
				//				Debug.Log ("Robot " + (i+1) + "- Pos: " + robotStade[i][0].ToString() + ", Vel:" + robotStade[i][1].ToString() + ", Acc:" + robotStade[i][2].ToString() + ", For:" + robotStade[i][3].ToString());
			}
/*			for (int j = 0; j < N_VAR; j++)
			{
				for (int i = 0; i < n_Robots; i++)
				{
					File.AppendAllText(textFile, robotStade[i][j] + "\t");
				}
			}
			File.AppendAllText(textFile, Environment.NewLine);*/

			if (robotStatus == 0)
				Debug.Log ("Disconnected?");
	//			connectStatus = ConnectStatus.disconnected;
		}
		return;
	}

	private void InitializeVariables(int n_robots)
//	public Connection(int n_robots)
	{
		n_Robots = n_robots;
//		mask_size = (BIT_SIZE * n_robots) / 8 < 0 ? 1 : (BIT_SIZE * n_robots) / 8;

//		activeMask = new byte[mask_size];

		gameStatus = new byte[sizeof(short)];
		gameStade = new byte[n_robots][][];
		robotStade = new float[n_robots][];

		SetStatus ((short)n_robots);
		for (int i = 0; i < n_robots; i++) 
		{
			gameStade[i] = new byte[N_VAR][];
			robotStade[i] = new float[N_VAR];
			for (int j = 0; j < N_VAR; j++)
			{
				SetStatus (i, 0f, j);
			}
		}
//		ClearMask ();
		return;
	}
	
/*	public void ClearMask()
	{
		for (int i = 0; i < activeMask.Length; i++)
			activeMask[i] = 0x0;
	}*/

	public void CloseConnection()
	{
		Destroy (this);
	}

	void OnDestroy()
	{
		connectStatus = ConnectStatus.disconnected;
//		stopThread = true;
		connectingThread.Abort ();
		clientHere.Disconnect ();
	}
}