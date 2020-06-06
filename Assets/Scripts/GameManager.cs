﻿using UnityEngine;

public sealed class GameManager : MonoBehaviour
{
    public Transform spawnFolder;
    public enum GameStateType
    {
        Wait
    }
    public static GameManager instance = null;
    public ResourceManager resourceManager;
    public GridManager gridManager;
    public Launcher launcher;
    
    void Start () {
        if (instance == null) {
            instance = this; 
        } else if(instance == this){
            Destroy(gameObject); 
        }
        DontDestroyOnLoad(gameObject);
        InitializeManager();
    }
    private void InitializeManager()
    {
        resourceManager = GetComponent<ResourceManager>();
        gridManager = GetComponent<GridManager>();
        resourceManager.Initialization();
        gridManager.Initialization();
        launcher.Initialization();
    }

    public void OnTriggered(Ball ball)
    {
        
        gridManager.AddBall(ball);
    }
    
}