using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    public bool isReady = false;
    [SerializeField] private Ball loadedBall;
    [SerializeField] private Ball nextBall;
    [SerializeField] private Transform transformLoadBall;
    [SerializeField] private Transform transformNextBall;

    public void Initialization()
    {
        loadedBall = Ball.Clone(transformLoadBall.position);
        nextBall = Ball.Clone(transformNextBall.position);
        nextBall.gameObject.SetActive(true);
        loadedBall.gameObject.SetActive(true);
    }
    public void Load()
    {
        
    }

    public void FIre()
    {
        
    }
    void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 delta = mousePos - new Vector2(transform.position.x, transform.position.y);

        float clampValue = Mathf.Clamp(-Mathf.Rad2Deg * Mathf.Atan2(delta.x, delta.y), -60, 60);
        transform.rotation = Quaternion.Euler(0f, 0f, clampValue);
        
        if (!isReady && Input.GetMouseButtonDown(0))
        {
            isReady = true;
            loadedBall.transform.rotation = transform.rotation;
            loadedBall.Launching();
        }
    }
}
