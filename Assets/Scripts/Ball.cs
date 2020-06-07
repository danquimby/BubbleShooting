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
    public int BallId;
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
        Destroy(gameObject);
    }

    public void Stop()
    {
        _rigidbody2D.velocity = Vector2.zero;
    }

    public void Launching()
    {
        gameObject.tag = "hits";
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
            float step =  moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, position, step);
            yield return null;
        }
        transform.position = position;
        finished?.Invoke();
        yield return null;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider != null)
        {
            if (gameObject.tag == "hits" || gameObject.tag == "upper")
            {
                // todo fast kick 
                //Ball ball = collider.gameObject.CastToBall();
                isTrigger = true;
                Stop();
                gameObject.tag = "bubble";
                onTriggeredEvent?.Invoke(this);
                onTriggeredEvent.RemoveListener(GameManager.instance.OnTriggered);
            }
        }
    }

    public static Ball Clone(Vector3 position, int index = -1, Transform parent = null)
    {
        return GameManager.instance.resourceManager.Get(index).gameObject.CloneBall(position, parent);
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
        _object.transform.SetParent(parent == null ? GameManager.instance.spawnFolder : parent);
        return _object.GetComponent<Ball>();
    }
}