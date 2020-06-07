using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class Ball : MonoBehaviour
{
    private Loggger log;
    private CircleCollider2D _collider;
    private Rigidbody2D _rigidbody2D;
    public Position BallPosition;
    public float speed = 10;
    public float moveSpeed = 2;
    public int BallId { get; private set; }
    public bool isHit = false;
    //TODO !!
    private bool click = false;

    public bool isTrigger
    {
        get { return _collider.isTrigger; }
        set
        {
            if (_collider == null)
                _collider = GetComponent<CircleCollider2D>();
            _collider.isTrigger = value;
        }
    }

    public bool Collider
    {
        get
        {
            return _collider.enabled;
        }
        set
        {
            if (_collider == null)
                _collider = GetComponent<CircleCollider2D>();
            _collider.enabled = value; 
        }
    }

    public TriggeredEvent onTriggeredEvent;

    void Start()
    {
        log = UnityLogProvider.get(this.GetType().Name);
        if (_collider == null)
            _collider = GetComponent<CircleCollider2D>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        onTriggeredEvent.AddListener(GameManager.instance.OnTriggered);

    }

    public void InitBall(BallData data, int id)
    {
        GetComponent<SpriteRenderer>().sprite = data.image;
        BallId = id;
    }

    public void Kill()
    {
        //TODO animation
        onTriggeredEvent.RemoveListener(GameManager.instance.OnTriggered);
        Destroy(gameObject);
    }

    public void Stop()
    {
        _rigidbody2D.velocity = Vector2.zero;
    }

    public void Launching()
    {
        _rigidbody2D.velocity = transform.up * speed;
    }

    public void MoveTo(Vector3 position, float speed = -1, Action finish = null)
    {
        float tmpSpeed = -1;
        if (speed >= 0)
        {
            tmpSpeed = moveSpeed;
            moveSpeed = speed;
        }
            
        StartCoroutine(MoveProcess(position, () =>
        {
            if (tmpSpeed >= 0)
                moveSpeed = tmpSpeed;
            finish?.Invoke();
        }));
    }
    IEnumerator MoveProcess(Vector3 position, Action finished)
    {
        while (Vector3.Distance(transform.position, position) > 0.001f)
        {
            //todo union speed params
            float step =  moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, position, step);
            yield return null;
        }
        transform.position = position;
        finished?.Invoke();
//        Debug.LogError("position " + transform.position);
        yield return null;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider != null)
        {
            Ball ball = collider.gameObject.CastToBall();
            if (!isHit && ball != null && ball.isTrigger)
            {
                Debug.LogError("OnTriggered ");
                isHit = true;
                Collider = false;
                isTrigger = true;
                Stop();
                onTriggeredEvent?.Invoke(this);
                Collider = true;
            }
        }
    }

    public static Ball Clone(Vector3 position, int index = -1, Transform parent = null)
    {
        Ball _ball = GameManager.instance.resourceManager.Get(index);
        Ball ball = _ball.gameObject.CloneBall(position, parent);
        return ball;
    }
}

public static class MyExtensions
{
    public static Ball CastToBall(this GameObject gameObject)
    {
        Assert.IsTrue(gameObject != null || gameObject.GetComponent<Ball>() != null, "Not found ball script");
        return gameObject.GetComponent<Ball>();
    }

    public static Ball CloneBall(this GameObject ball, Vector3 position, Transform parent = null)
    {
        GameObject _object = GameObject.Instantiate(ball, position, Quaternion.identity);
        if (_object == null)
            return null;
        if (parent == null)
            _object.transform.SetParent(GameManager.instance.spawnFolder);
        else
            _object.transform.SetParent(parent);
        return _object.GetComponent<Ball>();
    }
}