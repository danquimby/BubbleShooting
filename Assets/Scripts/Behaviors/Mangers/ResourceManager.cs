using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ResourceManager : BaseBehavior
{
    private Loggger log;
    [SerializeField] private Transform spawnBall;
    [SerializeField] private List<Ball> bubbles;
    [SerializeField] private BallData[] ballData;
    [SerializeField] private GameObject basePrefab;
    [SerializeField] private GameObject ExplosionPrefab;

    public GameObject SpawnExplosionObject()
    {
        return Instantiate(ExplosionPrefab, Vector3.zero, Quaternion.identity);
    }

    protected override void Init()
    {
        /*        
        log = LoggerProvider.get(this);
        bubbles = new List<Ball>();
        for (var index = 0; index < ballData.Length; index++)
        {
            BallData data = ballData[index];
            Ball ball = basePrefab.CloneBall(new Vector3(0,0,-1), spawnBall);
            ball.gameObject.SetActive(false);
            ball.InitBall(data, index);
            bubbles.Add(ball);
        }
        */
    }
}
