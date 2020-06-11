﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid")]
    [SerializeField] private int columns;
    [SerializeField] private int rows;
    [SerializeField] private float BallStepX = -2;
    [SerializeField] private float offsetX = 0.2f;
//    [SerializeField] private Vector3 compressor.transform.position;
    [SerializeField] private Ball[,] gridBall;
    [Header("Fall down controller")]
    public GameObject compressor;
    
    public void Initialization()
    {
        gridBall = new Ball[columns, rows];
        LoadLevel("Assets/level1.data");
        Debug.Log("loaded");
    }
    public Vector3 Snap(Vector3 position)
    {
        Vector3 objectOffset = position - compressor.transform.position;
        Vector3 objectSnap = new Vector3(
            Mathf.Round(objectOffset.x / BallStepX),
            Mathf.Round(objectOffset.y / BallStepX),
            0
        );
        if ((int)objectSnap.y % 2 != 0)
        {
            if (objectOffset.x > objectSnap.x * BallStepX)			
                objectSnap.x += BallStepX / 2;
        
            else			
                objectSnap.x -= BallStepX / 2;
        }
        return compressor.transform.position + objectSnap * BallStepX;
    }

    private void CalculationDrop()
    {
        List<Ball> balls = new List<Ball>();
        for (int c = 0; c < columns; c++)
        {
            bool find = false;
            balls.Clear();
            for (int r = rows - 1; r >= 0; r--)
            {
                Ball ball = gridBall[c, r];
                // find 1 object
                if (ball != null && !find)
                {
                    find = true;
                }
                // sequence break
                if (ball == null && find)
                {
                    DropBallGroup(balls);
                    balls.Clear();
                    find = false;
                    continue;
                }
                balls.Add(ball);
            }
        }
    }

    private void DropBallGroup(List<Ball> balls)
    {
        foreach (Ball ball in balls)
        {
            ball.Drop();
        }
    }
    private bool Seek(int column, int row, int kind)
    {
        Debug.Log($"kind = " + kind);
        int[] pair = new int[2] { column, row };

        bool[,] visited = new bool[columns, rows];

        visited[column, row] = true;

        int[] deltax = { -1, 0, -1, 0, -1, 1 };
        int[] deltaxprime = { 1, 0, 1, 0, -1, 1 };
        int[] deltay = { -1, -1, 1, 1, 0, 0 };


        Queue<int[]> queue = new Queue<int[]>();
        Queue<Ball> objectQueue = new Queue<Ball>();

        queue.Enqueue(pair);

        int count = 0;
        while (queue.Count != 0)
        {
            int[] top = queue.Dequeue();
            if (!isPossible(top[0], top[1]))
                continue;
            Ball gtop = gridBall[top[0], top[1]];
            if (gtop != null)
            {
                objectQueue.Enqueue(gtop);
            }
            count += 1;
            for (int i = 0; i < 6; i++)
            {
                int[] neighbor = new int[2];
                if (top[1] % 2 == 0)				
                    neighbor[0] = top[0] + deltax[i];				
                else				
                    neighbor[0] = top[0] + deltaxprime[i];
				
                neighbor[1] = top[1] + deltay[i];
                if (!isPossible(neighbor[0], neighbor[1]))
                    continue;
                Ball ball = gridBall[neighbor[0], neighbor[1]];
                if (ball != null)
                {
                
                    if (ball.BallId == kind)
                    {
                        if (!visited[neighbor[0], neighbor[1]])
                        {
                            visited[neighbor[0], neighbor[1]] = true;
                            queue.Enqueue(neighbor);
                        }
                    }
                }
               
            }
        }
        if (count >= 3)
        {
            while (objectQueue.Count != 0)
            {
                Ball ball = objectQueue.Dequeue();
                if (ball != null)
                {
                    if (!isPossible(ball.BallPosition.Column, ball.BallPosition.Row))
                        continue;
                    gridBall[ball.BallPosition.Column, ball.BallPosition.Row] = null;
                    ball.Kill();
                }
            }

            return true;
        }

        return false;
    }
    public void AddBall(Ball ball)
    {
        Vector3 objectSnap = Snap(ball.transform.position);
        Position _pos = GetPosition(objectSnap);
        Vector3 snappedPosition = GetViewPosition(_pos);
        snappedPosition.z = -1;

        ball.BallPosition = GetPosition(snappedPosition);
        ball.MoveTo(snappedPosition, -1, () =>
        {
            GameManager.instance.CompressorStart();
        });
        if (!isPossible(ball.BallPosition.Column, ball.BallPosition.Row))
            return;

        if (gridBall[ball.BallPosition.Column, ball.BallPosition.Row] != null)
        {
            gridBall[ball.BallPosition.Column, ball.BallPosition.Row].Kill();
        }
        gridBall[ball.BallPosition.Column, ball.BallPosition.Row] = ball;
        if (Seek(ball.BallPosition.Column, ball.BallPosition.Row, ball.BallId))
            CalculationDrop();
    }
    private Ball create(Position position, int value)
    {
        Ball ball;
        ball = CloneBall(GetViewPosition(position), value);
        ball.isTrigger = true;
        ball.BallPosition = position;
        if (!isPossible(ball.BallPosition.Column, ball.BallPosition.Row))
            return null;

        gridBall[ball.BallPosition.Column, ball.BallPosition.Row] = ball;
        return ball;
    }

    private Position GetPosition(Vector3 position)
    {
        int row;
        if (position.y > compressor.transform.position.y)
            row = (int)Mathf.Round(( position.y - compressor.transform.position.y) / BallStepX);
        else
            row = (int)Mathf.Round(( compressor.transform.position.y - position.y) / BallStepX);
        int column;
        if (row == 0)
            column = (int)Mathf.Round((position.x - compressor.transform.position.x ) / BallStepX);
        else
            column = (int)Mathf.Round((position.x - compressor.transform.position.x ) / BallStepX - offsetX);
        return new Position(row, column);
    }
    
    private Vector3 GetViewPosition(Position position)
    {
        if (position.Row % 2 == 0)
            return new Vector3(position.Column * BallStepX, (-position.Row) * BallStepX, -1) + compressor.transform.position;
        return new Vector3(position.Column * BallStepX + offsetX, (-position.Row) * BallStepX, -1) + compressor.transform.position;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(new Vector3(compressor.transform.position.x + 1.5f, compressor.transform.position.y, 0), new Vector3(5, .1f, 1));
    }

    private void LoadLevel(string filename)
    {
        using (StreamReader streamReader = new StreamReader(filename))
        {
            int count = 0;
            string data = streamReader.ReadToEnd();
            for (int r = 0; r < columns; r++)
            {
                for (int c = 0; c < rows; c++)
                {
                    if (count >= data.Length) break;
                    //skip system chars 
                    bool next_line = false;
                    while (data[count] == '\r' || data[count] == '\n')
                    {
                        count++;
                        next_line = true;
                    }
                    if (next_line) break;
                    int value;
                    if (Int32.TryParse(data[count++].ToString(),out value ))
                    {
                        Ball ball = create(new Position(r, c), value);
                        ball.drag = false;
                    }
                }
            }
        }
    }
    private Ball CloneBall(Vector3 position, int Id)
    {
        Ball ball =  GameManager.instance.resourceManager.Get(Id);
        GameObject _object = GameObject.Instantiate(ball.gameObject, position, Quaternion.identity);
        _object.SetActive(true);
        _object.transform.SetParent(GameManager.instance.spawnFolder);
        return _object.CastToBall();
    }
    private bool isPossible(int _column, int _row)
    {
        return ((_column >= 0 && _column < this.columns) && (_row >= 0 && _row < this.rows));
    }
}
