using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

public class GridManager : BaseBehavior
{
    [SerializeField] private GameObject prefabBall;
    [SerializeField] private int columns;
    [SerializeField] private int rows;
    [SerializeField] private Ball[,] gridBall;
    [SerializeField] private Transform rootSpawn;
    private int[] DeltaNeighbors = // x,y
    {
        -1,0,  0,-1, 1,0, 0,1, //square
        -1,-1, 1,-1, 1,1, -1,1 //diagonals
    };
    protected override void Init()
    {
        gridBall = new Ball[columns, rows];
        base.Init();
        //TODO till remove
        LoadLevel("Assets/level1.data");
    }

    public void LoadLevel(string filename)
    {
        using (StreamReader streamReader = new StreamReader(filename))
        {
            int value;
            int count = 0;
            string data = streamReader.ReadToEnd();
            for (int c = 0; c < columns; c++)
            {
                for (int r = 0; r < rows; r++)
                {
                    if (count >= data.Length) break;
                    
                    bool next_line = false;
                    while (data[count] == '\r' || data[count] == '\n')
                    {
                        count++;
                        next_line = true;
                    }
                    if (next_line) break;
                    if (Int32.TryParse(data[count++].ToString(),out value ))
                    {
                        var position = new Position(r, c);
                        GameObject obj = PoolManager.Spawn(prefabBall, Converter.ToViewPosition(position),transform.rotation);
                        Ball ball = obj.toBall();
                        ball.InitBall(value);
                        ball.IsBindingRoot = c == 0; // TODO to think... till set root property 
                        obj.transform.SetParent(rootSpawn);
                        gridBall[c, r] = ball;
                    }
                }
            }
        }

       
        //CheckGridToExcessBalls();
    }
    // TODO delete before 
    public void Foo()
    {
        List<Ball> excessBalls = Seek(gridBall[0, 1]);
        if (excessBalls.Count >= 3)
        {
            foreach (Ball ball in excessBalls)
            {
                gridBall[ball.position.Column, ball.position.Row] = null;
                Destroy(ball.gameObject);
                GameObject o = GameManager.instance.resourceManager.SpawnExplosionObject();
                o.transform.position = ball.transform.position;
            }
            excessBalls.Clear();
        }
    }
    void CheckGridToExcessBalls()
    {
        bool [,] arr = new bool[columns,rows];
        for (var i = 0; i < columns; i++)
            for (int i1 = 0; i1 < rows; i1++)  
                arr[i, i1] = false;
        
        for (int c = columns-1; c >= 0; c--)
        {
            for (int r = 0; r < rows; r++)
            {
                Ball ball = gridBall[c, r];
                if (ball != null && !arr[c,r])
                {
                    bool [,] arrResult = new bool[columns,rows];
                    if (CheckGridToExcessBallsRecursive(ball, ref arrResult))
                    {
                        CopyBitArray(ref arr, ref arrResult);
                        log.e("нашли рута");
                    }
                    else
                    {
                        MassDropped(ref arrResult);
                        log.e("не нашли рута");
                    }
                }

            }
        }
    }

    public List<Ball> Seek(Ball ball)
    {
        List<Ball> result = new List<Ball>();
        result.Add(ball);
        bool[,] check = new bool[columns, rows];
        SeekRecursive(ball, ref check, ref result);
        return result;
    }

    private void SeekRecursive(Ball ball, ref bool[,] check, ref List<Ball> result)
    {
        check[ball.position.Column, ball.position.Row] = true;
        
        for (int i = 0; i < DeltaNeighbors.Length / 2; i++)
        {
            int r = DeltaNeighbors[i * 2] + ball.position.Row;
            int c = DeltaNeighbors[i * 2 + 1] + ball.position.Column;
            
            if (!isPossible(c,r)) continue;
            Ball newBall = gridBall[c, r];
            if (newBall != null && !check[c, r])
            {
                log.i("Нашли шарик");
                if (ball.GetBallId() == newBall.GetBallId())
                {
                    result.Add(newBall);
                    SeekRecursive(newBall, ref check, ref result);
                }
            }
        }
    }
    
    private void MassDropped(ref bool[,] arrResult)
    {
        for (int c = 0; c < arrResult.GetLength(0); c++)
        {
            for (int r = 0; r < arrResult.GetLength(1); r++)
            {
                if (arrResult[c, r])
                {
                    gridBall[c, r].Drop();
                }
            }
        }

    }
    bool CheckGridToExcessBallsRecursive(Ball ball, ref bool[,] check)
    {
        bool result = ball.IsBindingRoot;
        check[ball.position.Column, ball.position.Row] = true;
        
        for (int i = 0; i < DeltaNeighbors.Length / 2; i++)
        {
            int r = DeltaNeighbors[i * 2] + ball.position.Row;
            int c = DeltaNeighbors[i * 2 + 1] + ball.position.Column;
            
            if (!isPossible(c,r)) continue;
            
            Ball newBall = gridBall[c, r];
            if (newBall != null && !check[c, r])
            {
                log.i("Нашли шарик");
                bool newResult = CheckGridToExcessBallsRecursive(newBall, ref check);
                if (!result && newResult)
                {
                    result = true;
                }
            }
        }
        return result;
    }
    #region Tools
    private bool isPossible(int _column, int _row)
    {
        return ((_column >= 0 && _column < this.columns) && (_row >= 0 && _row < this.rows));
    }
    private void CopyBitArray(ref bool[,] dest, ref bool[,] src)
    {
        for (int c = 0; c < dest.GetLength(0); c++)
        {
            for (int r = 0; r < dest.GetLength(1); r++)
            {
                if (src[c, r])
                {
                    dest[c, r] = true;
                }
            }
        }
    }
    #endregion
}