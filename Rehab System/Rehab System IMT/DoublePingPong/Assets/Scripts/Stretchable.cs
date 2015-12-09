using UnityEngine;
using System.Collections;

public class Stretchable : MonoBehaviour {

//	public Vector3 origin = new Vector3(5f, 0.5f, 0f);
//	public GameObject stretcher;
	public float maxScaleXZ = 0.5f;
	public Vector3 parentScale = Vector3.one;

	public void SetForm(Vector3 startPos, Vector3 endPos)
	{
		Vector3 direction = (endPos - startPos)/2;
		transform.position = (startPos + endPos) / 2;
		transform.rotation = Quaternion.LookRotation(Vector3.up, direction);
		transform.localScale = new Vector3(Mathf.Clamp (1/direction.magnitude, 0f, maxScaleXZ)/parentScale.x, 
		                                   direction.magnitude/parentScale.z, 
		                                   Mathf.Clamp (1/direction.magnitude, 0f, maxScaleXZ)/parentScale.y);
	}
}
