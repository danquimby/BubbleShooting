using System;
using System.Collections;
using UnityEngine;

public sealed class GameManager : BaseBehavior
{
    public Transform spawnFolder;
    public enum GameStateType
    {
        Wait
    }
    public static GameManager instance = null;
    public ResourceManager resourceManager;
    //public GridManager gridManager;
    public Launcher launcher;
    public GameObject Compressor;
    public GridManager gridManager;
    protected override void Start () {
        if (instance == null) {
            instance = this; 
        } else if(instance == this){
            Destroy(gameObject); 
        }
        DontDestroyOnLoad(gameObject);
        base.Start();
    }

    protected override void Init()
    {
        base.Init();
        //PoolManager.Spawn()
        resourceManager = GetComponent<ResourceManager>();
        if (gridManager == null)
            gridManager = GetComponent<GridManager>();
        gridManager.LoadLevel("Assets/level1.data");
        
      //  gridManager = GetComponent<GridManager>();
    }


    public void OnTriggered(Ball ball)
    {
        //gridManager.AddBall(ball);
        ball.transform.SetParent(spawnFolder);
        //launcher.Reload();
    }

    public void CompressorStart()
    {
        StartCoroutine(MoveProcess(Compressor,
            new Vector3(Compressor.transform.position.x, Compressor.transform.position.y - 0.1f, 0), null));
    }
    IEnumerator MoveProcess(GameObject target, Vector3 position, Action finished)
    {
        yield return new WaitForSeconds(0.5f);
        while (Vector3.Distance(target.transform.position, position) > 0.001f)
        {
            float step =  2f * Time.deltaTime;
            target.transform.position = Vector3.MoveTowards(target.transform.position, position, step);
            yield return null;
        }
        target.transform.position = position;
        finished?.Invoke();
        yield return null;
    }
    
    
}