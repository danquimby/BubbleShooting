
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public Position cell;
    
    public Ball ball { get; private set; }
    public HashSet<Ball> bindingBall
    {
        get;
        private set;
    }
    public bool empty => ball == null;

    public Cell(Ball ball=null)
    {
        bindingBall = new HashSet<Ball>();
        if (ball != null)
            this.ball = ball;
    }

    public void SetBall(Ball ball)
    {
        this.ball = ball;
    }
    public void AddBinding(Ball ball)
    {
        //Debug.Log("add bindig " + ball.BallPosition);
        bindingBall.Add(ball);
    }

    public void RemoveBinding(Ball ball)
    {
        bindingBall.Remove(ball);
    }

    public void DestroyBall()
    {
        // foreach (Ball ball in bindingBall)
        // {
        //     ball.RemoveBinding(this);
        // }
        // Destroy(this);
    }

    public void ClearBinding()
    {
        bindingBall.Clear();
    }
        
}