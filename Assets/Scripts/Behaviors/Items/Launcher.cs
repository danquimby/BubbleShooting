using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : BaseBehavior
{
    public bool isReady = false;
    [SerializeField] private DragObject loadedBall;
    [SerializeField] private DragObject nextBall;
    [SerializeField] private Transform transformLoadBall;
    [SerializeField] private Transform transformNextBall;
    [SerializeField] private GameObject prefabDragObject; 
    
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
        loadedBall = PoolManager.Spawn(prefabDragObject, loaded, transformLoadBall.transform.rotation).toDragObject();
        loadedBall.InitBall(Random.Range(1,6));
        nextBall = PoolManager.Spawn(prefabDragObject, next, transformNextBall.transform.rotation).toDragObject();
        nextBall.InitBall(Random.Range(1,6));
        
        
        //loadedBall = Ball.Clone(loaded, 1, transform);
        // nextBall = Ball.Clone(next, 1, transform);
        // nextBall.gameObject.SetActive(true);
        // loadedBall.gameObject.SetActive(true);
        // loadedBall.drag = true;
        // loadedBall.onLaunched = Reload;
    }
}
