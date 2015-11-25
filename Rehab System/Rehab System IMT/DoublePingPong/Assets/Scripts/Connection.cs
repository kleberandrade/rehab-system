using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Linq;

public class Connection : MonoBehaviour {

	// Indice relativo da variavel
	public const int POSITION = 0; 		// Compartilhado para robo e game
	public const int VELOCITY = 1; 		// Compartilhado para robo e game
	public const int STIFF = 2;			// Game
	public const int DAMP = 3;			// Game
	public const int ACC = 2;			// Robo
	public const int FORCE = 3;			// Robo

	public const int N_VAR = 4; 		// Numero de variaveis envolvidas
	private const int BIT_SIZE = 4; 	// Numero de bit da mascara; Deve ser multiplo de 2
	private const int INFO_SIZE = 4;	// 4 Float; 8 Double
	
	private int n_Robots;
	private int mask_size; 
	private byte[] activeMask;
	private byte[][][] gameStade;
	private float[][] robotStade;

	private float delayCount;
	public float timeDelay;

//================================
	private NetworkClientTCP clientHere = new NetworkClientTCP();
//================================

	void Start()
	{
		timeDelay = 0.01f;
		clientHere.Connect ("192.168.0.66", 8000, 0); // Here 192.168.0.67
	//	clientHere.SendString ("Conectado!"); 
	//	clientHere.ReceiveString ();
		InitializeVariables (2); // Entre com o numero de robos
	//	ClearMask ();
	}

	void FixedUpdate()
	{
		SendMsg ();
		ClearMask ();
		ReadMsg ();
	}

	public void SetStatus(int robot, float mag, int variable)
	{
		activeMask[(BIT_SIZE * robot) / 8] |= (byte)(0x1 << (variable + robot*BIT_SIZE));
		gameStade [robot] [variable] = BitConverter.GetBytes (mag);
		return;
	}

	public float ReadStatus(int robot, int variable)
	{
		return robotStade [robot] [variable];
	}

	private void SendMsg ()
	{
		byte[] msg = new byte[activeMask.Length + (N_VAR * INFO_SIZE) * n_Robots];

		System.Buffer.BlockCopy (activeMask, 0, msg, 0, activeMask.Length);
		for (int i = 0; i < n_Robots; i++) 
		{
			for (int j = 0; j < N_VAR; j++)
			{
				System.Buffer.BlockCopy (gameStade [i][j], 0, msg, activeMask.Length + INFO_SIZE * (j + N_VAR * i), gameStade [i][j].Length);
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
			for (int i = 0; i < n_Robots; i++)
			{
				for (int j = 0; j < N_VAR; j++)
				{
					robotStade[i][j] = BitConverter.ToSingle (buffer, 1 + INFO_SIZE*(j + N_VAR*i));
				}
//				Debug.Log ("Robot " + (i+1) + "- Pos: " + robotStade[i][0].ToString() + ", Vel:" + robotStade[i][1].ToString() + ", Acc:" + robotStade[i][2].ToString() + ", For:" + robotStade[i][3].ToString());
			}
		}
		return;
	}

	private void InitializeVariables(int n_robots)
//	public Connection(int n_robots)
	{
		n_Robots = n_robots;
		mask_size = (BIT_SIZE * n_robots) / 8 < 0 ? 1 : (BIT_SIZE * n_robots) / 8;

		activeMask = new byte[mask_size];

		gameStade = new byte[n_robots][][];
		robotStade = new float[n_robots][];

		for (int i = 0; i < n_robots; i++) 
		{
			gameStade[i] = new byte[N_VAR][];
			robotStade[i] = new float[N_VAR];
			for (int j = 0; j < N_VAR; j++)
			{
				SetStatus (i, 0f, j);
			}
		}
		ClearMask ();
		return;
	}
	
	public void ClearMask()
	{
		for (int i = 0; i < activeMask.Length; i++)
			activeMask[i] = 0x0;
	}

	public void CloseConnection()
	{
		clientHere.Disconnect ();
	}
}