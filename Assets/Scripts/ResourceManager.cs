using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ResourceManager : MonoBehaviour
{
    private Loggger log;
    [SerializeField] private Transform spawnBall;
    [SerializeField] private List<Ball> bubbles;
    [SerializeField] private BallData[] ballData;
    [SerializeField] private GameObject basePrefab;
    [SerializeField] private GameObject baseHitPrefab;
    
    public void Initialization()
    {
        log = UnityLogProvider.get(this.GetType().Name);
        bubbles = new List<Ball>();
        for (var index = 0; index < ballData.Length; index++)
        {
            BallData data = ballData[index];
            Ball ball = basePrefab.CloneBall(new Vector3(0,0,-1), spawnBall);
            ball.gameObject.SetActive(false);
            ball.InitBall(data, index);
            bubbles.Add(ball);
        }
    }
    public Ball Get(int index = -1)
    {
        Assert.IsTrue(bubbles.Count > 0, "resource manager is empty");
        if (index >= 0 && index >= bubbles.Count) return null;
        return index == -1 ? bubbles[Random.Range(0,bubbles.Count)] : bubbles[index];
    }
}
