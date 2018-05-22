using System.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using Core;
using System.IO;
using System.Reflection;

public class PositionsHolder
    {
        public PositionsHolder(int lr, int rr, Vector2D ball, Vector2D r1, Vector2D r2, Vector2D r3, Vector2D r4, Vector2D r5, Vector2D l1, Vector2D l2, Vector2D l3, Vector2D l4, Vector2D l5, string score, string time, int control) 
        {
            ballPosition = new Vector2D(0.0, 0.0);
            leftPlayerRobots = new Vector2D[5];
            rightPlayerRobots = new Vector2D[5];

            lRule = lr;
            rRule = rr;
            ballPosition = ball;

            leftPlayerRobots[0] = l1;
            leftPlayerRobots[1] = l2;
            leftPlayerRobots[2] = l3;
            leftPlayerRobots[3] = l4;
            leftPlayerRobots[4] = l5;

            rightPlayerRobots[0] = r1;
            rightPlayerRobots[1] = r2;
            rightPlayerRobots[2] = r3;
            rightPlayerRobots[3] = r4;
            rightPlayerRobots[4] = r5;

            this.score = score;
            this.time = time;
            this.control = control;
        }

        public int lRule;
        public int rRule;
        public Vector2D ballPosition;
        public Vector2D[] leftPlayerRobots;
        public Vector2D[] rightPlayerRobots;
        public string score;
        public string time;
        public int control;
    }

    

public class LogHolder{

    public List<PositionsHolder> positions;


    public LogHolder()
    {
        positions = new List<PositionsHolder>(); 
    }

    private void WriteVector(StreamWriter sw, Vector2D vector2D)
    {
        sw.WriteLine("\t<x>" + vector2D.X + "</x>");
        sw.WriteLine("\t<y>" + vector2D.Y + "</y>");
    }

}

