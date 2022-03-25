using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MovingBall : Singleton<MovingBall>
{
    [SerializeField] private float speed = 0.0f;
    public UnityAction OnEndEvent = null;

    private SpriteRenderer spriteRenderer = null;
    private List<Vector2> waypoints = null;
    private int waypointIndex = 0;

    private bool started = false;

    protected override void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!started)
            return;

        if (waypointIndex < waypoints.Count)
        {
            var targetPos = waypoints[waypointIndex];
            transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            if ((Vector2)transform.position == targetPos)
                ++waypointIndex;
        }
        else
        {
            if (OnEndEvent != null)
                OnEndEvent.Invoke();
            Init();
        }
    }

    public void Init()
    {
        waypointIndex = 0;
        started = false;
        gameObject.SetActive(false);
    }

    public void Move(Sprite sprite, List<Vector2> lst)
    {
        spriteRenderer.sprite = sprite;
        waypoints = lst;
        transform.position = waypoints[0];
        ++waypointIndex;
        started = true;
        gameObject.SetActive(true);
    }
}
