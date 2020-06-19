using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class DragObject : Ball
{
    private Vector3 screenPoint;
    private Vector3 offset;
    private Vector3 scanPos;
    private float _sensitivity;
    private Vector3 _mouseReference;
    private bool _isRotating;
    private Vector2 Upper, Bottom;
    [SerializeField] private GameObject cursor;
    
    private float sppedBall = 0;

    public void SetEnable()
    {
        // DragBall = gameObject.CastToBall();
        // DragBall.transform.position = transform.position;
        // scanPos = transform.position;
        // Upper = new Vector2(scanPos.x - 0.5f, scanPos.y -0.5f);
        // Bottom = new Vector2(scanPos.x + 0.5f, scanPos.y);
        // _sensitivity = 40f;
        // DragBall = gameObject.CastToBall();
        // DragBall.transform.position = transform.position;
        // cursor.SetActive(true);
    }
   

    private void Update()
    {
        if(_isRotating)
        {
            transform.Rotate(new Vector3(0,0, (transform.position - _mouseReference).x * _sensitivity));
            _mouseReference = transform.position;
        }
    }
    void OnMouseDown()
    {
        _isRotating = true;
        _mouseReference = transform.position;
        screenPoint = Camera.main.WorldToScreenPoint(scanPos);
        offset = scanPos - Camera.main.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));

    }
    void OnMouseDrag()
    {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);

        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        transform.position = curPosition;

        float posX = Mathf.Clamp (transform.position.x,Upper.x,Bottom.x);
        float posY = Mathf.Clamp (transform.position.y,Upper.y,Bottom.y);
        sppedBall = scanPos.y - posY;
        transform.position = new Vector3 (posX,posY,curPosition.z);
    }
    void OnMouseUp()
    {
        if (sppedBall > 0)
        {
            _isRotating = false;
            //DragBall.Launching(sppedBall*25);
            GetComponent<DragObject>().enabled = false;
            cursor.SetActive(false);
        }
    }
}
