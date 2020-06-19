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
        foo();
        // SetBinging();
        // CheckWithoutBinding();
    }

    void foo()
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

                    log.e("Заходим " + ball.position);
                    if (foo_r(ball, ref arrResult))
                    {
                        FillArray(ref arr, ref arrResult);
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
    bool foo_r(Ball ball, ref bool[,] check)
    {
        bool result = ball.IsBindingRoot;
//        log.i("Шарик у нас IsBindingRoot " + result);
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
                bool newResult = foo_r(newBall, ref check);
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
    private void FillArray(ref bool[,] dest, ref bool[,] src)
    {
        for (int c = 0; c < dest.GetLength(0); c++)
        {
            for (int r = 0; r < dest.GetLength(1); r++)
            {
                if (src[c, r])
                {
                    dest[c, r] = true;
                    log.i($" записываем c={c} r={r} ");
                }
            }
        }
    }
    private List<Ball> FindNeighbors(Ball ball)
    {
        Assert.IsNotNull(ball, "Object ball is null !");
        
        List<Ball> neighbors = new List<Ball>();
        for (int i = 0; i < DeltaNeighbors.Length/2; i++)
        {
            int r = DeltaNeighbors[i * 2] + ball.position.Row;
            int c = DeltaNeighbors[i * 2 + 1] + ball.position.Column;
            if (ball.position.EqualPoints(c,r)) continue; // check self. Possible if column equals 0
            if (isPossible(c, r))
            {
                Ball b = gridBall[c, r];
                if (b != null)
                    neighbors.Add(gridBall[c, r]);
            }
        }
        // log.i($"Binding pos={ball.position} len = {neighbors.Count}");
        // foreach (Ball neighbor in neighbors)
        // {
        //     log.i("neighbor " + neighbor.position);
        // }
        return neighbors;
    }

    private void CheckWithoutBinding()
    {
        for (int r = 0; r < rows; r++)
        {
            Ball ball = gridBall[0, r];
            if (ball != null)
            {
                log.i($"root pos={ball}");
                List<Ball> result = new List<Ball>();
                CheckBranch(ball, result);
                /*
                if (!CheckBranch(ball, result))
                {
                    HashSet<Ball> res1 = new HashSet<Ball>(result);
                    foreach (Ball foundBall in res1)
                    {
                        log.e("Find !! " + foundBall.position);
                        //ball.Drop();
                        gridBall[foundBall.position.Column, foundBall.position.Row] = null;
                        //Binding.RemoveAllBindingsFromObject(foundBall);
                    }
                }
                */
            }
        }

    }
    private static int countW = 0;
    private bool CheckBranch(Ball ball, List<Ball> result)
    {
        if (countW++ > 30)
        {
            log.e("err");
            return true;
        }
        log.e("add result  " + ball);
        result.Add(ball);
        List<Ball> balls = Binding.GetBindingItems(ball);
        foreach (Ball bindingBall in balls)
        {
            // if binding object removed
            if (bindingBall == null)
            {
                Binding.RemoveBing(ball, bindingBall);
                continue;
            }
            if (CheckBranch(bindingBall, result))
                return true;
        }
        return false;
    }
    #endregion

}
#if old
public class GridManager : BaseBehavior
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
    
    protected override void Init()
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
#endif