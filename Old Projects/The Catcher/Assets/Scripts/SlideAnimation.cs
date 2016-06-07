using UnityEngine;
using System.Collections;

public class SlideAnimation : MonoBehaviour
{
    public SlideDirection direction;
    public bool starting = false;
    [Range(0.01f, 10.0f)]
    public float time = 1.0f;

    private Vector3 origin;
    private Vector3 target;
    private float startTime;
    private SlideTransition transition = SlideTransition.None;

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
        if (transition == SlideTransition.Showing)
        {
            transform.position = Vector3.Lerp(origin, target, (Time.time - startTime) / time);
            if (Vector3.Distance(transform.position, target) <= 0.0f)
                transition = SlideTransition.None;
        }
            

        if (transition == SlideTransition.Hiding)
        {
            transform.position = Vector3.Lerp(target, origin, (Time.time - startTime) / time);
            if (Vector3.Distance(transform.position, origin) <= 0.0f)
                transition = SlideTransition.None;
        }
            
    }

    public void Show()
    {
        if (transition != SlideTransition.Showing)
        {
            transition = SlideTransition.Showing;
            startTime = Time.time;
        }
    }

    public void Hide()
    {
        if (transition != SlideTransition.Hiding)
        {
            transition = SlideTransition.Hiding;
            startTime = Time.time;
        }
    }

    void MoveToStartPosition()
    {
        float distance = (transform.position - Camera.main.transform.position).z;

        switch (direction)
        {
            case SlideDirection.Up:
                origin.y = Camera.main.ViewportToWorldPoint(new Vector3(0.0f, 1.7f, distance)).y;
                break;
            case SlideDirection.Down:
                origin.y = Camera.main.ViewportToWorldPoint(new Vector3(0.0f, -0.7f, distance)).y;
                break;
            case SlideDirection.Left:
                origin.x = Camera.main.ViewportToWorldPoint(new Vector3(-0.7f, 0.0f, distance)).x;
                break;
            case SlideDirection.Right:
                origin.x = Camera.main.ViewportToWorldPoint(new Vector3(1.7f, 0.0f, distance)).x;
                break;
        }

        startTime = Time.time;
    }
}

public enum SlideTransition
{
    None,
    Showing,
    Hiding
}

public enum SlideDirection
{
    None,
    Left,
    Up,
    Right,
    Down
}