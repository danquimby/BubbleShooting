using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : BaseBehavior
{
    public bool isReady = false;
    [SerializeField] private Ball loadedBall;
    [SerializeField] private Ball nextBall;
    [SerializeField] private Transform transformLoadBall;
    [SerializeField] private Transform transformNextBall;
    private Vector3 next, loaded;
    private bool touch = false;

    public void Reload()
    {
        // loadedBall = nextBall;
        // loadedBall.Collider = false;
        // loadedBall.onLaunched = Reload;
        // loadedBall.MoveTo(loaded, 10, () =>
        // {
        //     nextBall = Ball.Clone(next, -1, transform);
        //     nextBall.gameObject.SetActive(true);
        //     loadedBall.Collider = true;
        //     loadedBall.drag = true;
        //     isReady = false;
        // });
    }

    protected override void Init()
    {
        loaded = transformLoadBall.position;
        next = transformNextBall.position;
        // loadedBall = Ball.Clone(loaded, 1, transform);
        // nextBall = Ball.Clone(next, 1, transform);
        // nextBall.gameObject.SetActive(true);
        // loadedBall.gameObject.SetActive(true);
        // loadedBall.drag = true;
        // loadedBall.onLaunched = Reload;
    }
}
