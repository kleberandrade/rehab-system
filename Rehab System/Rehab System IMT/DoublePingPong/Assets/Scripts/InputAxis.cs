using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class InputAxis
{
	protected string name;
	public string Name { get { return name; } }

	protected float position = 0.0f, velocity = 0.0f, force = 0.0f;
	protected float feedbackPosition = 0.0f, feedbackVelocity = 0.0f;
	protected float stiffness = 0.0f, damping = 0.0f;
	private float maxValue, minValue, range = 1.0f;

	protected BitArray setpointsMask;

	public virtual void Init( string axisName, float initialPosition = 0.0f )
	{
		name = axisName;
		position = initialPosition;
	}

	public virtual void Update( float updateTime ) {}

	public float Position { get { return position; } set { feedbackPosition = value; setpointsMask[ 0 ] = true; } }
	public float Velocity { get { return velocity; } set { feedbackVelocity = value; setpointsMask[ 1 ] = true; } }
	public float Force { get { return position; } }

	public float Stiffness { set { stiffness = value; setpointsMask[ 2 ] = true; } }
	public float Damping { set { damping = value; setpointsMask[ 3 ] = true; } }

	public float NormalizedPosition { get { return ( 2 * ( position - minValue ) / range - 1.0f ); } 
		                              set { feedbackPosition = ( ( value + 1.0f ) * range / 2.0f ) + minValue; setpointsMask[ 0 ] = true; } }
	public float NormalizedVelocity { get { return ( 2 * velocity / range ); } 
		                              set { feedbackVelocity = ( value * range / 2.0f ); setpointsMask[ 1 ] = true; } }
	public float NormalizedForce { get { return ( 2 * ( force - minValue ) / range - 1.0f ); } }

	public float MaxValue { set { maxValue = value; range = ( maxValue - minValue != 0.0f ) ? maxValue - minValue : 1.0f; } }
	public float MinValue { set { minValue = value; range = ( maxValue - minValue != 0.0f ) ? maxValue - minValue : 1.0f; } }
}

public class LocalInputAxis : InputAxis
{
	public override void Update( float updateTime )
	{
		position += velocity * updateTime;
	}
}

public class RemoteInputAxis : InputAxis
{
	protected byte id;

	public override void Init( string axisName, float initialPosition = 0.0f )
	{
		base.Init( axisName, initialPosition );
		id = byte.Parse( axisName );
	}

	public virtual void Connect() {}
}

public class MouseInputAxis : LocalInputAxis
{
	public override void Update( float updateTime )
	{
		velocity = Input.GetAxis( name ) / updateTime;
		base.Update( updateTime );
	}
}

public class KeyboardInputAxis : LocalInputAxis
{
	public override void Update( float updateTime )
	{
		velocity = Input.GetAxis( name );
		base.Update( updateTime );
	}
}

