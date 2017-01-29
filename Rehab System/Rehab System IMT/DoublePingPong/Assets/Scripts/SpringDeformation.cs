using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringDeformation : MonoBehaviour 
{
	public Transform[] endPoints = new Transform[ 2 ];

	private float baseDistance = 0.0f;
	private Vector3 baseScale = Vector3.one;

	void Start() 
	{
		baseDistance = Vector3.Distance( endPoints[ 0 ].position, endPoints[ 1 ].position );
		baseScale = transform.localScale;
	}
	
	void Update() 
	{
		transform.position = ( endPoints[ 0 ].position + endPoints[ 1 ].position ) / 2.0f;

		float currentDistance = Vector3.Distance( endPoints[ 0 ].position, endPoints[ 1 ].position );
		transform.localScale = Vector3.Scale( baseScale, new Vector3( 1.0f, currentDistance / baseDistance, 1.0f ) );
	}
}
