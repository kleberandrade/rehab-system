using UnityEngine;
using System.Collections;

using System;
using System.IO;
using System.Text;

public class Logger : MonoBehaviour {

	public Connection connection;
	public ToAnkleRobot robot;

	//private string textFile = @"D:\Users\Thales\Documents\Faculdade\2015 - 201x - Mestrado\AnkleBot\LogFileAnkle - " + DateTime.Now.ToString("yy-MM-dd HH-mm") + ".txt";
	private string textFile = Application.dataPath + "\\Logs\\Session" + DateTime.Now.ToString("yy-MM-dd HH-mm") + ".txt";

	// Use this for initialization
	void Start () 
	{
		Directory.CreateDirectory(Application.dataPath + "\\Logs");
		File.WriteAllText (textFile, "Time\t" +
			"SPosX\tSPosY\t" +
			"EPosX\tEPosY\t" +
			"FVelX\tFVelY\t" +
/*			"VelX\tVelY\t" +
			"Torque\t\t" +
//			 "EventNumber\t" +
//			 "TorqueVec\t\t" +
//			 "dTorqueVec\t\t" +
		    "CtrSprX\tCtrSprY\t" +
			"FrSpcX\tFrSpcY\t" +
			"K" +
			"D" +*/
			Environment.NewLine);
	}


	// Update is called once per frame
	public void Register() 
	{
		File.AppendAllText(textFile, 
			+ Time.time + "\t"
			+ robot.wallPos.x + "\t" 
			+ robot.wallPos.y + "\t");

//		for (int j = 0; j < Connection.N_VAR; j++)
		for (int j = 0; j < 2; j++)
			for (int i = 1; i >= 0; i--)
				File.AppendAllText(textFile, connection.ReadStatus(i, j) + "\t");

		/*File.AppendAllText(textFile, enemy.eventCounter + "\t");

					fm = Mathf.Sqrt (connection.ReadStatus (0, Connection.FORCE) * connection.ReadStatus (0, Connection.FORCE) +
						 	 connection.ReadStatus (1, Connection.FORCE) * connection.ReadStatus (1, Connection.FORCE));
			fa = Mathf.Atan2 (connection.ReadStatus (1, Connection.FORCE),
							  connection.ReadStatus (0, Connection.FORCE));

			File.AppendAllText(textFile, fm + "\t");
			File.AppendAllText(textFile, fa + "\t");

			File.AppendAllText(textFile, (fm - dfm) / (Time.time - dt) + "\t");
			File.AppendAllText(textFile, (fa - dfa) / (Time.time - dt) + "\t");

			dfm = fm;
			dfa = fa;
			dt = Time.time;
*/
/*		File.AppendAllText(textFile, robot.centerSpring.x + "\t");
		File.AppendAllText(textFile, robot.centerSpring.y + "\t");
		File.AppendAllText(textFile, robot.freeSpace.x + "\t");
		File.AppendAllText(textFile, robot.freeSpace.y + "\t");
		File.AppendAllText(textFile, robot.K + "\t");
		File.AppendAllText(textFile, robot.D + "\t");
*/
		File.AppendAllText(textFile, Environment.NewLine);

	}
}
