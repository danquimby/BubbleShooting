using System;
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
    [SerializeField] private Vector3 InitPosition;
    [SerializeField] private Ball[,] gridBall;

    public void Initialization()
    {
        gridBall = new Ball[columns, rows];
        LoadLevel("Assets/level1.data");
        Debug.Log("loaded");
    }

    public Vector3 Snap(Vector3 position)
    {
        Vector3 objectOffset = position - InitPosition;
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
        return InitPosition + objectSnap * BallStepX;
    }
    private void RecursiveSeek(Position position,int side, int BallId, ref bool[,] visited)
    {
        Ball ball = gridBall[position.Column, position.Row];
        if (ball != null)
        {
                    
            // if (ball.BallId == BallId)
            // {
            //     if (!visited[neighbor[0], neighbor[1]])
            //     {
            //         visited[neighbor[0], neighbor[1]] = true;
            //         queue.Enqueue(neighbor);
            //     }
            // }
        }
    }

    private void Seek(int column, int row, int kind)
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
                if (neighbor[0] < 0 || neighbor[1] < 0)
                {
                    //Debug.Log($"neighbor 0 ={neighbor[0]} 1={neighbor[1]}");
                    //Debug.LogError("negative ");
                    continue;
                }
                //Debug.Log($"neighbor 0 ={neighbor[0]} 1={neighbor[1]}");
                try
                {
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
                catch (System.IndexOutOfRangeException)	{}
               
            }
        }
        if (count >= 3)
        {
            while (objectQueue.Count != 0)
            {
                Ball ball = objectQueue.Dequeue();
                if (ball != null)
                {
                    gridBall[ball.BallPosition.Column, ball.BallPosition.Row] = null;
                    ball.Kill();
                }
            }
        }
        // if win !
        //CheckCeiling(0);
    }

    public void AddBall(Ball ball)
    {
        Vector3 objectSnap = Snap(ball.transform.position);
        Position _pos = GetPosition(objectSnap);
        Vector3 snappedPosition = GetViewPosition(_pos);
        snappedPosition.z = -1;

        ball.BallPosition = GetPosition(snappedPosition);
        ball.MoveTo(snappedPosition);
        Debug.Log("ball.BallPosition " + ball.BallPosition);
        gridBall[ball.BallPosition.Column, ball.BallPosition.Row] = ball;
        Seek(ball.BallPosition.Column, ball.BallPosition.Row, ball.BallId);
    }

    public void Create(Position position, int value)
    {
        Ball ball;
        ball = CloneBall(GetViewPosition(position), value);
        ball.isTrigger = true;
        ball.BallPosition = position;
        gridBall[ball.BallPosition.Column, ball.BallPosition.Row] = ball;
    }

    private Position GetPosition(Vector3 position)
    {
        int row;
        if (position.y > InitPosition.y)
            row = (int)Mathf.Round(( position.y - InitPosition.y) / BallStepX);
        else
            row = (int)Mathf.Round(( InitPosition.y - position.y) / BallStepX);
        int column;
        if (row == 0)
            column = (int)Mathf.Round((position.x - InitPosition.x ) / BallStepX);
        else
            column = (int)Mathf.Round((position.x - InitPosition.x ) / BallStepX - offsetX);
        return new Position(row, column);
    }
    
    private Vector3 GetViewPosition(Position position)
    {
        if (position.Row % 2 == 0)
            return new Vector3(position.Column * BallStepX, (-position.Row) * BallStepX, -1) + InitPosition;
        return new Vector3(position.Column * BallStepX + offsetX, (-position.Row) * BallStepX, -1) + InitPosition;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(new Vector3(InitPosition.x+1.6f, InitPosition.y+0.25f, 0), new Vector3(5, .1f, 1));
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
                        Create(new Position(r, c),value);
                    }
                }
            }
        }
    }
    Ball CloneBall(Vector3 position, int Id)
    {
        Ball ball =  GameManager.instance.resourceManager.Get(Id);
        GameObject _object = GameObject.Instantiate(ball.gameObject, position, Quaternion.identity);
        _object.SetActive(true);
        _object.transform.SetParent(GameManager.instance.spawnFolder);
        return _object.CastToBall();
    }
}
