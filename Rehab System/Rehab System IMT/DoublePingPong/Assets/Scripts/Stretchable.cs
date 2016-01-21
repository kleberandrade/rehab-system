using UnityEngine;
using System.Collections;

public class Stretchable : MonoBehaviour {

//	public Vector3 origin = new Vector3(5f, 0.5f, 0f);
//	public GameObject stretcher;
	public float maxScaleXZ = 0.5f, minScaleXZ = 0.2f;
	public float scaleFactorXZ = 5f, scaleFactorY = 1f;

	public void SetForm(Vector3 startPos, Vector3 endPos)
	{
		Vector3 direction = (endPos - startPos)/2;
		transform.position = (startPos + endPos) / 2;
		transform.rotation = Quaternion.LookRotation(Vector3.up, direction);
		transform.localScale = new Vector3(Mathf.Lerp (minScaleXZ, maxScaleXZ, direction.magnitude / 10f), 
		                                   Mathf.Clamp(scaleFactorY * direction.magnitude, 0.1f, 20f), 
		                                   Mathf.Lerp (minScaleXZ, maxScaleXZ, direction.magnitude / 10f));

	}
}
