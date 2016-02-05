using UnityEngine;
using UnityEngine.UI;
using System.Globalization;
using System.Collections;

using System;
using System.IO;
using System.Text;

[ RequireComponent( typeof(InputAxisManager) ) ]
public class Robot : MonoBehaviour 
{
	private const int VERTICAL = 0;		// or RIGHT? 	DP - Dorsiflexion/Plantarflexion
	private const int HORIZONTAL = 1;	// or LEFT?		IE - Inversion/Eversion
	private const float QUADRANTS = 0.70710678118654752440084436210485f;

    private bool connected;
    public bool Connected { get { return connected; } set { connected = value; } }

    private bool helperEnabled;
    public bool HelperEnabled { set { helperEnabled = value; } }

	public Text playerScoreText, machineScoreText, lazyScoreText; 	// UI Scores
	private float playerScore, machineScore, lazyScore;				// Value Scores
	public float lazySpeed, lazyForce;

	// Communication with another scripts
	public PlayerController player;
	public BallController ball;

	//private Connection connection;
	private InputAxis horizontal, vertical;

	// Communication
	private Vector2 input, position;
    private Vector2 setpoint;

	[Space]

	public Vector2 centerSpring;
	public Vector2 freeSpace;
	public float stiffness, damping;				// Stiffness and Damping

	//private string textFile = "./LogFileAnkle - " + DateTime.Now.ToString("yy-MM-dd HH-mm") + ".txt";

	void Awake () 
	{
		playerScoreText.text = "Player\n0";
		machineScoreText.text = "Machine\n0";
		lazyScoreText.text = "Lazy Time\n0";
		playerScore = 0;
		machineScore = 0;
		lazyScore = 0;
	}

	void Start ()
	{
        connected = false;
//		connection = GetComponent<Connection>();
//		File.WriteAllText (textFile, "Horizontal\t" +
//		                   			 "Vertical" + 
//		                   			Environment.NewLine + 
//									 "Time\t" +
//									 "SqrPos\t\t" +
//									 "Pos\t\t" +
//									 "FVel\t\t" +
//									 "Vel\t\t" +
//									 "Torque\t" +
//		                   			 "CenterSpring\t\t" +
//		                   			 "FreeSpace\t\t" +
//		                   			 "Stiffness" +
//		                   			 "Damping" +
//		                   			Environment.NewLine);

        horizontal = GetComponent<InputAxisManager>().GetAxis( "1" );
        vertical = GetComponent<InputAxisManager>().GetAxis( "0" );

        if( horizontal == null ) horizontal = GetComponent<InputAxisManager>().GetAxis( "Horizontal", InputAxisType.Keyboard );
        if( vertical == null ) vertical = GetComponent<InputAxisManager>().GetAxis( "Vertical", InputAxisType.Keyboard );
	}

	void Update()
	{
		// Update scores
		lazyScore = Time.time - (playerScore + machineScore);
		playerScoreText.text = "Player\n" + playerScore.ToString ("F1");
		machineScoreText.text = "Machine\n" + machineScore.ToString ("F1");
		lazyScoreText.text = "Lazy Time\n" + lazyScore.ToString("F1");
	}

	void FixedUpdate() 
	{
        if( connected )
		{
			input = new Vector2( horizontal.NormalizedPosition, vertical.NormalizedPosition );

			if( new Vector2( horizontal.Force, horizontal.Force ).magnitude > lazyForce ) machineScore += Time.deltaTime;
			else if( new Vector2( horizontal.Velocity, horizontal.Velocity ).magnitude > lazySpeed ) playerScore += Time.deltaTime;

            centerSpring = Vector2.Lerp( centerSpring, setpoint, 1.0f );
            freeSpace = Vector2.Lerp( freeSpace, Vector2.zero, 1.0f );

			// Set variables to send to robot
			vertical.Position = centerSpring.y;
			horizontal.Position = centerSpring.x;
			vertical.Velocity = freeSpace.y;
			horizontal.Velocity = freeSpace.x;

			vertical.Stiffness = stiffness;
			horizontal.Stiffness = stiffness;
			vertical.Damping = damping;
			horizontal.Damping = damping;

			// Print the all variables
//			File.AppendAllText( textFile, + Time.time + "\t"
//			                              + input.x + "\t" 
//			                              + input.y  + "\t" );
//
//			File.AppendAllText( textFile, vertical.Position + "\t" + vertical.Velocity + "\t" + vertical.Force );
//			File.AppendAllText( textFile, horizontal.Position + "\t" + horizontal.Velocity + "\t" + horizontal.Force );
//
//			File.AppendAllText(textFile, centerSpring.x + "\t");
//			File.AppendAllText(textFile, centerSpring.y + "\t");
//			File.AppendAllText(textFile, freeSpace.x + "\t");
//			File.AppendAllText(textFile, freeSpace.y + "\t");
//			File.AppendAllText(textFile, Stiffness + "\t");
//			File.AppendAllText(textFile, D + "\t");
//
//			File.AppendAllText(textFile, Environment.NewLine);
			
		} 

        Calibration();
	}

    public Vector2 ReadInput()
    {
        if( connected ) return input; 

        return Vector2.zero;
    }

    public void WriteSetpoint( Vector2 setpoint )
    {
        if( helperEnabled ) centerSpring = setpoint;
    }

	void Calibration()
	{
        if( horizontal.Position > horizontal.MaxValue ) horizontal.MaxValue = horizontal.Position;
        else if( horizontal.Position < horizontal.MinValue ) horizontal.MinValue = horizontal.Position;

        if( vertical.Position > vertical.MaxValue ) vertical.MaxValue = vertical.Position;
        else if( vertical.Position < vertical.MinValue ) vertical.MinValue = vertical.Position;
	}
}
