using UnityEngine;
using System.Collections;

[RequireComponent (typeof(TextMesh))]
public class HitPoint : MonoBehaviour
{
	public float distance = 10.0f;
	public float time = 10.0f;
    public float upStartDistance = 1.0f;

    private Color startColor;
    private Color endColor;
	private Vector3 startPosition;
	private Vector3 endPosition;
    private Color currentColor;
	private Vector3 direction = Vector3.up;
	private float startTime;
    private TextMesh textMesh;

    void Awake()
    {
        textMesh = GetComponent<TextMesh>();
        startColor = textMesh.color;

    }

    void OnEnable()
    {
        textMesh.color = startColor;
        startPosition = transform.position + Vector3.up * upStartDistance;
        endPosition = startPosition + direction * distance;
        endColor = startColor;
        endColor.a = 0.0f;
        startTime = Time.time;
    }

	void SetPoint(int point)
	{
		textMesh.text = string.Format("+{0}", point.ToString());
	}
	
	void FixedUpdate () 
    {
        float deltaTime = (Time.time - startTime);
        transform.position = Vector3.Lerp(startPosition, endPosition, deltaTime / time);
        textMesh.color = Color.Lerp(startColor, endColor, deltaTime / time);
        if (textMesh.color.a <= 0.0f)
            Destroy();
	}

    void Destroy()
    {
        gameObject.SetActive(false);
    }
}