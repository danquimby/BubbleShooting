using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class BaseItem : BaseBehavior
{
    #region Events controls
    protected Action<int, Touch> onTouchBegin;
    protected Action<int, Touch> onTouchMove;
    protected Action<int, Touch> onTouchEnded;
    protected Action<int> onMouseDown;
    protected Action<int> onMouseUp;
    protected Action onMouseMove;
    protected Action checkInput;
    protected Action onUpdate;
    #endregion

    // TODO I don't like it 
    public bool IsBindingRoot;
    
    [SerializeField] protected float moveSpeed;
    private bool pressed;
    public bool isControl;
    private SpriteRenderer _spriteRenderer;
    public SpriteRenderer SpriteRenderer
    {
        get
        {
            if (this._spriteRenderer == null)
                this._spriteRenderer = GetComponent<SpriteRenderer>();
            return this._spriteRenderer;
        }
    }
    protected override void Init()
    {
        pressed = false;
        isControl = false;
        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
            checkInput = CheckMouseInput;
        else
            checkInput = CheckAndroidInput;
    }
    protected void Update()
    {
        if (isControl)
            checkInput?.Invoke();
        onUpdate?.Invoke();
    }

    #region Check controls
    private void CheckMouseInput()
    {
        for (int i = 0; i < 2; i++)
        {
            if (Input.GetMouseButtonDown(i) && DetectSelfObject(Input.mousePosition))
            {
                onMouseDown?.Invoke(i);
                pressed = true;
            }
            else if (pressed && Input.GetMouseButtonUp(i))
            {
                onMouseUp?.Invoke(i);
                pressed = false;
            } 
            if ((Input.GetAxis("Mouse X") != 0) || (Input.GetAxis("Mouse Y") != 0))
            {
                onMouseMove?.Invoke();
            }
        }
    }
    private void CheckAndroidInput()
    {
        for (int i = 0; i < Input.touchCount; ++i)
        {
            Touch touch = Input.GetTouch(i);
            if (touch.phase == TouchPhase.Began)
            {
                onTouchBegin?.Invoke(i, touch);
            } else if (touch.phase == TouchPhase.Ended)
            {
                onTouchEnded?.Invoke(i, touch);
            } else if (touch.phase == TouchPhase.Moved)
            {
                onTouchMove?.Invoke(i, touch);
            }
        }
    }

    #region Movement object
    /// <summary>
    /// Move Object to position
    /// </summary>
    /// <param name="position">to position</param>
    /// <param name="speed">speed movement</param>
    /// <param name="finish">action finished</param>
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
    #endregion

    protected bool DetectSelfObject(Vector3 position)
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(position);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            
        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
        return hit.collider != null && hit.collider.gameObject.name == gameObject.name;
    }
    #endregion

}
