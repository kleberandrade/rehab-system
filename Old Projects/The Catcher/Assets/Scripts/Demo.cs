using UnityEngine;
using System.Collections;

public class Demo : MonoBehaviour
{
    public DemoDirection direction = DemoDirection.None;
    public bool starting = false;
    [Range(0.01f, 10.0f)]
    public float time;

    private Vector3 origin;
    private Vector3 target;
    private float startTime;
    private DemoTransition transition = DemoTransition.None;

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 100, 200, 30), "Show"))
        {
            Show();
        }

        if (GUI.Button(new Rect(10, 150, 200, 30), "Hide"))
        {
            Hide();
        }
    }

    void Start()
    {
        target = transform.position;
        origin = target;

        MoveToStartPosition();

        transform.position = origin;

        if (starting)
            Show();
    }

    void Update()
    {
        if (transition == DemoTransition.Showing)
            transform.position = Vector3.Lerp(origin, target, (Time.time - startTime) / time);

        if (transition == DemoTransition.Hiding)
            transform.position = Vector3.Lerp(target, origin, (Time.time - startTime) / time);
    }

    public void Show()
    { 
        if (transition != DemoTransition.Showing)
        {
            transition = DemoTransition.Showing;
            startTime = Time.time;
        }
    }

    public void Hide()
    {
        if (transition != DemoTransition.Hiding)
        {
            transition = DemoTransition.Hiding;
            startTime = Time.time;
        }
    }

    void MoveToStartPosition()
    {
        float distance = (transform.position - Camera.main.transform.position).z;

        switch (direction)
        {
            case DemoDirection.Up:
                origin.y = Camera.main.ViewportToWorldPoint(new Vector3(0.0f, 1.7f, distance)).y;
                break;
            case DemoDirection.Down:
                origin.y = Camera.main.ViewportToWorldPoint(new Vector3(0.0f, -0.7f, distance)).y;
                break;
            case DemoDirection.Left:
                origin.x = Camera.main.ViewportToWorldPoint(new Vector3(-0.7f, 0.0f, distance)).x;
                break;
            case DemoDirection.Right:
                origin.x = Camera.main.ViewportToWorldPoint(new Vector3(1.7f, 0.0f, distance)).x;
                break;
        }

        startTime = Time.time;
    }
}

public enum DemoTransition
{
    None,
    Showing,
    Hiding
}

public enum DemoDirection
{
    None,
    Left,
    Up,
    Right,
    Down
}