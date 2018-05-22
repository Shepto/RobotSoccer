using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using Core;
using System.IO;
using System.Reflection;
using StrategiesDLL;

public class Rule
{
    public int ruleNum { get; set; }

    public int ballX { get; set; }
    public int ballY { get; set; }
    public int lr0x { get; set; }
    public int lr0y { get; set; }
    public int lr1x { get; set; }
    public int lr1y { get; set; }
    public int lr2x { get; set; }
    public int lr2y { get; set; }
    public int lr3x { get; set; }
    public int lr3y { get; set; }
    public int lr4x { get; set; }
    public int lr4y { get; set; }
    public int rr0x { get; set; }
    public int rr0y { get; set; }
    public int rr1x { get; set; }
    public int rr1y { get; set; }
    public int rr2x { get; set; }
    public int rr2y { get; set; }
    public int rr3x { get; set; }
    public int rr3y { get; set; }
    public int rr4x { get; set; }
    public int rr4y { get; set; }

    public int lr0mx { get; set; }
    public int lr0my { get; set; }
    public int lr1mx { get; set; }
    public int lr1my { get; set; }
    public int lr2mx { get; set; }
    public int lr2my { get; set; }
    public int lr3mx { get; set; }
    public int lr3my { get; set; }
    public int lr4mx { get; set; }
    public int lr4my { get; set; }
}

public class StrategyAdaptation
{
    static bool ballWasInGate = false;

    static List<Rule> adaptationDefRules = new List<Rule>();
    static List<Rule> adaptationOffRules = new List<Rule>();

    const int distTreshold = 8;

    const int lookBackLines = 150;
    const int ruleOffset = 50; 

    static int discardedDefrules = 0;
    static int discardedOffrules = 0;

    static int ruleNum = 0;

    public StrategyAdaptation()
    {
      
    }

    public List<object> AdaptStrategy(LogHolder log, object strategy)
    {
        discardedDefrules = 0;
        discardedOffrules = 0;
        adaptationDefRules.Clear();
        adaptationOffRules.Clear();

        ruleNum = (int)strategy.GetType().GetMethod("CurrentRuleNumberCount").Invoke(strategy, null);

        //def adapt
        try
        {
            AdaptDefRules(log, (Strategy)strategy);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
        //off adapt
        try
        {
            AdaptOffRules(log, (Strategy)strategy);
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
        }

        //complete list
        List<object> adaptedRules = new List<object>();

        //convert def rules
        foreach (Rule r in adaptationDefRules)
        {
            GridRule gr = new GridRule();

            gr.Number = r.ruleNum;
            gr.Type = GridRule.RuleType.Deffense;
            gr.Name = "AdaptedDef";
            Vector2D[] vmine = new Vector2D[]{
                new Vector2D(Convert.ToDouble(r.lr0x), Convert.ToDouble(r.lr0y)),
                new Vector2D(Convert.ToDouble(r.lr1x), Convert.ToDouble(r.lr1y)),
                new Vector2D(Convert.ToDouble(r.lr2x), Convert.ToDouble(r.lr2y)),
                new Vector2D(Convert.ToDouble(r.lr3x), Convert.ToDouble(r.lr3y))
            };
            gr.Mine = vmine;
            Vector2D[] voppnt = new Vector2D[]{
                new Vector2D(Convert.ToDouble(r.rr0x), Convert.ToDouble(r.rr0y)),
                new Vector2D(Convert.ToDouble(r.rr1x), Convert.ToDouble(r.rr1y)),
                new Vector2D(Convert.ToDouble(r.rr2x), Convert.ToDouble(r.rr2y)),
                new Vector2D(Convert.ToDouble(r.rr3x), Convert.ToDouble(r.rr3y))
            };
            gr.Oppnt = voppnt;
            gr.Ball = new Vector2D(Convert.ToDouble(r.ballX), Convert.ToDouble(r.ballY));
            Vector2D[] vmove = new Vector2D[]{
                new Vector2D(Convert.ToDouble(r.lr0mx), Convert.ToDouble(r.lr0my)),
                new Vector2D(Convert.ToDouble(r.lr1mx), Convert.ToDouble(r.lr1my)),
                new Vector2D(Convert.ToDouble(r.lr2mx), Convert.ToDouble(r.lr2my)),
                new Vector2D(Convert.ToDouble(r.lr3mx), Convert.ToDouble(r.lr3my))
            };
            gr.Move = vmove;

            Debug.Log(gr.ToString());

            gr.fillZOrder();

            if (!RuleCheck(gr, (Strategy)strategy))
                adaptedRules.Add(gr);
            else discardedDefrules++;

            Debug.Log("Discarded def rules: " + discardedDefrules);
        }

        //convert off rules
        foreach (Rule r in adaptationOffRules)
        {
            GridRule gr = new GridRule();

            gr.Number = r.ruleNum;
            gr.Type = GridRule.RuleType.Offense;
            gr.Name = "AdaptedOff";
            Vector2D[] vmine = new Vector2D[]{
                new Vector2D(Convert.ToDouble(r.lr0x), Convert.ToDouble(r.lr0y)),
                new Vector2D(Convert.ToDouble(r.lr1x), Convert.ToDouble(r.lr1y)),
                new Vector2D(Convert.ToDouble(r.lr2x), Convert.ToDouble(r.lr2y)),
                new Vector2D(Convert.ToDouble(r.lr3x), Convert.ToDouble(r.lr3y))
            };
            gr.Mine = vmine;
            Vector2D[] voppnt = new Vector2D[]{
                new Vector2D(Convert.ToDouble(r.rr0x), Convert.ToDouble(r.rr0y)),
                new Vector2D(Convert.ToDouble(r.rr1x), Convert.ToDouble(r.rr1y)),
                new Vector2D(Convert.ToDouble(r.rr2x), Convert.ToDouble(r.rr2y)),
                new Vector2D(Convert.ToDouble(r.rr3x), Convert.ToDouble(r.rr3y))
            };
            gr.Oppnt = voppnt;
            gr.Ball = new Vector2D(Convert.ToDouble(r.ballX), Convert.ToDouble(r.ballY));
            Vector2D[] vmove = new Vector2D[]{
                new Vector2D(Convert.ToDouble(r.lr0mx), Convert.ToDouble(r.lr0my)),
                new Vector2D(Convert.ToDouble(r.lr1mx), Convert.ToDouble(r.lr1my)),
                new Vector2D(Convert.ToDouble(r.lr2mx), Convert.ToDouble(r.lr2my)),
                new Vector2D(Convert.ToDouble(r.lr3mx), Convert.ToDouble(r.lr3my))
            };
            gr.Move = vmove;

            Debug.Log(gr.ToString());

            gr.fillZOrder();

            if (!RuleCheck(gr, (Strategy)strategy))
                adaptedRules.Add(gr);
            else discardedOffrules++;

            Debug.Log("Discarded off rules: " + discardedOffrules);
        }

        return adaptedRules;
    }

    private static bool RuleCheck(GridRule ar, Strategy str)
    {
        GridRule sr;
        double min = Double.MaxValue;

        for (int i = 0; i < str.GStrategy.Rules.Count; i++)
        {
            sr = str.GStrategy.Rules[i];

            double ballRuleSize = ar.Ball.DistanceFrom(sr.Ball);

            double mineSize = 0;
            for (int j = 0; j < ar.Mine.Length; j++)
                mineSize += ar.Mine[ar.ZMine[j]].DistanceFrom(sr.Mine[sr.ZMine[j]]);

            double oppntSize = 0;
            for (int j = 0; j < ar.Oppnt.Length; j++)
                oppntSize += ar.Oppnt[ar.ZOppnt[j]].DistanceFrom(sr.Oppnt[sr.ZOppnt[j]]);

            if (ballRuleSize + mineSize + oppntSize < min)
                min = ballRuleSize + mineSize + oppntSize;
        }

        Debug.Log("Min dist: " + min);

        if (min < distTreshold)
            return true;
        else return false;
    }

    private static void AdaptDefRules(LogHolder log, Strategy strategy)
    {
        int goalIndex = -1;

        int row = log.positions.Count - 1;
        PositionsHolder p = log.positions[row];
        int rule = p.lRule;
        string score = p.score;

        if (row <= 1)
            return;

        if (score != log.positions[row - 1].score)
        {
            //goal
            string[] lastScoreSplit = log.positions[row - 1].score.Split(':');
            int lastGoalsL = Convert.ToInt16(lastScoreSplit[0]);
            int lastGoalsR = Convert.ToInt16(lastScoreSplit[1]);
            string[] scoreSplit = score.Split(':');
            int goalsL = Convert.ToInt16(scoreSplit[0]);
            int goalsR = Convert.ToInt16(scoreSplit[1]);
            //right team scored goal
            if (lastGoalsL == goalsL && lastGoalsR < goalsR)
                goalIndex = row;
        }

        if (goalIndex != -1)
        {
            //extract lookupLines
            int from = -1;
            int to = goalIndex;

            if ((to - lookBackLines) < 0)
                from = 0;
            else from = to - lookBackLines;

            //extract rules
            for (int off = from; off < to; off += ruleOffset)
            {
                int[] sumValues = new int[18];
                double[] avgValues = new double[18];
                for (int i = 0 + off; i < ruleOffset + off; i++)
                {
                    int j = 0;
                    sumValues[j++] += Convert.ToInt16(log.positions[i].ballPosition.X);
                    sumValues[j++] += Convert.ToInt16(log.positions[i].ballPosition.Y);
                    sumValues[j++] += Convert.ToInt16(log.positions[i].leftPlayerRobots[0].X);
                    sumValues[j++] += Convert.ToInt16(log.positions[i].leftPlayerRobots[0].Y);
                    sumValues[j++] += Convert.ToInt16(log.positions[i].leftPlayerRobots[1].X);
                    sumValues[j++] += Convert.ToInt16(log.positions[i].leftPlayerRobots[1].Y);
                    sumValues[j++] += Convert.ToInt16(log.positions[i].leftPlayerRobots[2].X);
                    sumValues[j++] += Convert.ToInt16(log.positions[i].leftPlayerRobots[2].Y);
                    sumValues[j++] += Convert.ToInt16(log.positions[i].leftPlayerRobots[3].X);
                    sumValues[j++] += Convert.ToInt16(log.positions[i].leftPlayerRobots[3].Y);
                    sumValues[j++] += Convert.ToInt16(log.positions[i].rightPlayerRobots[0].X);
                    sumValues[j++] += Convert.ToInt16(log.positions[i].rightPlayerRobots[0].Y);
                    sumValues[j++] += Convert.ToInt16(log.positions[i].rightPlayerRobots[1].X);
                    sumValues[j++] += Convert.ToInt16(log.positions[i].rightPlayerRobots[1].Y);
                    sumValues[j++] += Convert.ToInt16(log.positions[i].rightPlayerRobots[2].X);
                    sumValues[j++] += Convert.ToInt16(log.positions[i].rightPlayerRobots[2].Y);
                    sumValues[j++] += Convert.ToInt16(log.positions[i].rightPlayerRobots[3].X);
                    sumValues[j++] += Convert.ToInt16(log.positions[i].rightPlayerRobots[3].Y);
                }

                //avg from ruleOffset lines
                for (int i = 0; i < 18; i++)
                {
                    avgValues[i] = sumValues[i] / Convert.ToDouble(ruleOffset);
                    avgValues[i] = Math.Round(avgValues[i]);
                }

                //transform to rule
                Rule rl = new Rule();
                int ii = 0;
                rl.ballX = Convert.ToInt16(avgValues[ii++]);
                rl.ballY = Convert.ToInt16(avgValues[ii++]);
                rl.lr0x = Convert.ToInt16(avgValues[ii++]);
                rl.lr0y = Convert.ToInt16(avgValues[ii++]);
                rl.lr1x = Convert.ToInt16(avgValues[ii++]);
                rl.lr1y = Convert.ToInt16(avgValues[ii++]);
                rl.lr2x = Convert.ToInt16(avgValues[ii++]);
                rl.lr2y = Convert.ToInt16(avgValues[ii++]);
                rl.lr3x = Convert.ToInt16(avgValues[ii++]);
                rl.lr3y = Convert.ToInt16(avgValues[ii++]);
                rl.rr0x = Convert.ToInt16(avgValues[ii++]);
                rl.rr0y = Convert.ToInt16(avgValues[ii++]);
                rl.rr1x = Convert.ToInt16(avgValues[ii++]);
                rl.rr1y = Convert.ToInt16(avgValues[ii++]);
                rl.rr2x = Convert.ToInt16(avgValues[ii++]);
                rl.rr2y = Convert.ToInt16(avgValues[ii++]);
                rl.rr3x = Convert.ToInt16(avgValues[ii++]);
                rl.rr3y = Convert.ToInt16(avgValues[ii++]);
                
                int il = adaptationDefRules.FindIndex(f =>
                        f.ballX == rl.ballX && f.ballY == rl.ballY &&
                        f.lr0x == rl.lr0x && f.lr0y == rl.lr0y &&
                        f.lr1x == rl.lr1x && f.lr1y == rl.lr1y &&
                        f.lr2x == rl.lr2x && f.lr2y == rl.lr2y &&
                        f.lr3x == rl.lr3x && f.lr3y == rl.lr3y &&
                        f.rr0x == rl.rr0x && f.rr0y == rl.rr0y &&
                        f.rr1x == rl.rr1x && f.rr1y == rl.rr1y &&
                        f.rr2x == rl.rr2x && f.rr2y == rl.rr2y &&
                        f.rr3x == rl.rr3x && f.rr3y == rl.rr3y
                    );
                if (il == -1)
                {
                    //rule strategy check
                    int istr = strategy.GStrategy.Rules.FindIndex( f => 
                            f.Ball.X == rl.ballX && f.Ball.Y == rl.ballY &&
                            f.Mine[0].X == rl.lr0x && f.Mine[0].Y == rl.lr0y &&
                            f.Mine[1].X == rl.lr1x && f.Mine[1].Y == rl.lr1y &&
                            f.Mine[2].X == rl.lr2x && f.Mine[2].Y == rl.lr2y &&
                            f.Mine[3].X == rl.lr3x && f.Mine[3].Y == rl.lr3y &&
                            f.Oppnt[0].X == rl.rr0x && f.Oppnt[0].Y == rl.rr0y &&
                            f.Oppnt[1].X == rl.rr1x && f.Oppnt[1].Y == rl.rr1y &&
                            f.Oppnt[2].X == rl.rr2x && f.Oppnt[2].Y == rl.rr2y &&
                            f.Oppnt[3].X == rl.rr3x && f.Oppnt[3].Y == rl.rr3y
                        );

                    if (istr == -1)
                    {
                        rl.ruleNum = ++ruleNum;

                        //set moveto
                        rl.lr0mx = rl.lr0x;
                        rl.lr0my = rl.lr0y;
                        rl.lr1mx = rl.lr1x;
                        rl.lr1my = rl.lr1y;
                        rl.lr2mx = rl.ballX;
                        rl.lr2my = rl.ballY;
                        rl.lr3mx = rl.ballX;
                        rl.lr3my = rl.ballY;

                        adaptationDefRules.Add(rl);
                    }
                    else ++discardedDefrules;
                }
                else ++discardedDefrules;
            }
        }
    }

    private static void AdaptOffRules(LogHolder log, Strategy strategy)
    {
        int goalIndex = -1;
        
        int row = log.positions.Count - 1;
        PositionsHolder p = log.positions[row];
       
        int rule = p.lRule;
        string score = p.score;
        int ballX = Convert.ToInt16(p.ballPosition.X);
        int ballY = Convert.ToInt16(p.ballPosition.Y);

        if (row <= 1)
            return;

        //ball is before the opponents gate
        if (ballX == 6 && (ballY == 2 || ballY == 3))
        {
            ballWasInGate = true;
        }
        //somewhere else
        else
        {
            //failed goal atempt
            if (ballWasInGate && score == log.positions[row - 1].score)
            {
                goalIndex = row - 1;
            }
            ballWasInGate = false;
        }

        if (goalIndex != -1)
        {
            //extract lookupLines
            int from = -1;
            int to = goalIndex;

            if ((to - lookBackLines) < 0)
                from = 0;
            else from = to - lookBackLines;

            //extract rules
            for (int off = from; off < to; off += ruleOffset)
            {
                int[] sumValues = new int[18];
                double[] avgValues = new double[18];
                for (int i = 0 + off; i < ruleOffset + off; i++)
                {
                    int j = 0;
                    sumValues[j++] += Convert.ToInt16(log.positions[i].ballPosition.X);
                    sumValues[j++] += Convert.ToInt16(log.positions[i].ballPosition.Y);
                    sumValues[j++] += Convert.ToInt16(log.positions[i].leftPlayerRobots[0].X);
                    sumValues[j++] += Convert.ToInt16(log.positions[i].leftPlayerRobots[0].Y);
                    sumValues[j++] += Convert.ToInt16(log.positions[i].leftPlayerRobots[1].X);
                    sumValues[j++] += Convert.ToInt16(log.positions[i].leftPlayerRobots[1].Y);
                    sumValues[j++] += Convert.ToInt16(log.positions[i].leftPlayerRobots[2].X);
                    sumValues[j++] += Convert.ToInt16(log.positions[i].leftPlayerRobots[2].Y);
                    sumValues[j++] += Convert.ToInt16(log.positions[i].leftPlayerRobots[3].X);
                    sumValues[j++] += Convert.ToInt16(log.positions[i].leftPlayerRobots[3].Y);
                    sumValues[j++] += Convert.ToInt16(log.positions[i].rightPlayerRobots[0].X);
                    sumValues[j++] += Convert.ToInt16(log.positions[i].rightPlayerRobots[0].Y);
                    sumValues[j++] += Convert.ToInt16(log.positions[i].rightPlayerRobots[1].X);
                    sumValues[j++] += Convert.ToInt16(log.positions[i].rightPlayerRobots[1].Y);
                    sumValues[j++] += Convert.ToInt16(log.positions[i].rightPlayerRobots[2].X);
                    sumValues[j++] += Convert.ToInt16(log.positions[i].rightPlayerRobots[2].Y);
                    sumValues[j++] += Convert.ToInt16(log.positions[i].rightPlayerRobots[3].X);
                    sumValues[j++] += Convert.ToInt16(log.positions[i].rightPlayerRobots[3].Y);
                }

                //avg from ruleOffset lines
                for (int i = 0; i < 18; i++)
                {
                    avgValues[i] = sumValues[i] / Convert.ToDouble(ruleOffset);
                    avgValues[i] = Math.Round(avgValues[i]);
                }

                //transform to rule
                Rule rl = new Rule();
                int ii = 0;
                rl.ballX = Convert.ToInt16(avgValues[ii++]);
                rl.ballY = Convert.ToInt16(avgValues[ii++]);
                rl.lr0x = Convert.ToInt16(avgValues[ii++]);
                rl.lr0y = Convert.ToInt16(avgValues[ii++]);
                rl.lr1x = Convert.ToInt16(avgValues[ii++]);
                rl.lr1y = Convert.ToInt16(avgValues[ii++]);
                rl.lr2x = Convert.ToInt16(avgValues[ii++]);
                rl.lr2y = Convert.ToInt16(avgValues[ii++]);
                rl.lr3x = Convert.ToInt16(avgValues[ii++]);
                rl.lr3y = Convert.ToInt16(avgValues[ii++]);
                rl.rr0x = Convert.ToInt16(avgValues[ii++]);
                rl.rr0y = Convert.ToInt16(avgValues[ii++]);
                rl.rr1x = Convert.ToInt16(avgValues[ii++]);
                rl.rr1y = Convert.ToInt16(avgValues[ii++]);
                rl.rr2x = Convert.ToInt16(avgValues[ii++]);
                rl.rr2y = Convert.ToInt16(avgValues[ii++]);
                rl.rr3x = Convert.ToInt16(avgValues[ii++]);
                rl.rr3y = Convert.ToInt16(avgValues[ii++]);

                //set default moveto
                rl.lr0mx = rl.lr0x;
                rl.lr0my = rl.lr0y;
                rl.lr1mx = rl.lr1x;
                rl.lr1my = rl.lr1y;
                rl.lr2mx = rl.lr2x;
                rl.lr2my = rl.lr2y;
                rl.lr3mx = rl.lr3x;
                rl.lr3my = rl.lr3y;

                //send two closest robots to oponnents gate
                int selRobot = -1;
                int selRobot2 = -1;

                int minX = Int16.MaxValue;
                int minY = Int16.MaxValue;

                int[] robotsL = new int[] { rl.lr0x, rl.lr0y, rl.lr1x, rl.lr1y, rl.lr2x, rl.lr2y, rl.lr3x, rl.lr3y };
                for (int i = 0; i < robotsL.Length; i = i + 2)
                {
                    int distX = 6 - robotsL[i];
                    int distY = 3 - robotsL[i + 1];
                    if (distX <= minX && distY <= minY)
                    {
                        minX = distX;
                        minY = distY;
                        selRobot = i / 2;
                    }
                }

                //find second robot
                robotsL[selRobot * 2] = 0;
                robotsL[(selRobot * 2) + 1] = 0;
                minX = Int16.MaxValue;
                minY = Int16.MaxValue;
                for (int i = 0; i < robotsL.Length; i = i + 2)
                {
                    int distX = 6 - robotsL[i];
                    int distY = 2 - robotsL[i + 1];
                    if (distX <= minX && distY <= minY)
                    {
                        minX = distX;
                        minY = distY;
                        selRobot2 = i / 2;
                    }
                }

                //change moveto for selected robot
                switch (selRobot)
                {
                    case 0:
                        {
                            rl.lr0mx = 6;
                            rl.lr0my = 3;
                            break;
                        }
                    case 1:
                        {
                            rl.lr1mx = 6;
                            rl.lr1my = 3;
                            break;
                        }
                    case 2:
                        {
                            rl.lr2mx = 6;
                            rl.lr2my = 3;
                            break;
                        }
                    case 3:
                        {
                            rl.lr3mx = 6;
                            rl.lr3my = 3;
                            break;
                        }
                }

                //change moveto for selected robot
                switch (selRobot2)
                {
                    case 0:
                        {
                            rl.lr0mx = 6;
                            rl.lr0my = 2;
                            break;
                        }
                    case 1:
                        {
                            rl.lr1mx = 6;
                            rl.lr1my = 2;
                            break;
                        }
                    case 2:
                        {
                            rl.lr2mx = 6;
                            rl.lr2my = 2;
                            break;
                        }
                    case 3:
                        {
                            rl.lr3mx = 6;
                            rl.lr3my = 2;
                            break;
                        }
                }

                //check if the rule is not already created
                int il = adaptationOffRules.FindIndex(f =>
                        f.ballX == rl.ballX && f.ballY == rl.ballY &&
                        f.lr0x == rl.lr0x && f.lr0y == rl.lr0y &&
                        f.lr1x == rl.lr1x && f.lr1y == rl.lr1y &&
                        f.lr2x == rl.lr2x && f.lr2y == rl.lr2y &&
                        f.lr3x == rl.lr3x && f.lr3y == rl.lr3y &&
                        f.rr0x == rl.rr0x && f.rr0y == rl.rr0y &&
                        f.rr1x == rl.rr1x && f.rr1y == rl.rr1y &&
                        f.rr2x == rl.rr2x && f.rr2y == rl.rr2y &&
                        f.rr3x == rl.rr3x && f.rr3y == rl.rr3y
                    );
                if (il == -1)
                {
                    //rule strategy check
                    int istr = strategy.GStrategy.Rules.FindIndex(f =>
                            f.Ball.X == rl.ballX && f.Ball.Y == rl.ballY &&
                            f.Mine[0].X == rl.lr0x && f.Mine[0].Y == rl.lr0y &&
                            f.Mine[1].X == rl.lr1x && f.Mine[1].Y == rl.lr1y &&
                            f.Mine[2].X == rl.lr2x && f.Mine[2].Y == rl.lr2y &&
                            f.Mine[3].X == rl.lr3x && f.Mine[3].Y == rl.lr3y &&
                            f.Oppnt[0].X == rl.rr0x && f.Oppnt[0].Y == rl.rr0y &&
                            f.Oppnt[1].X == rl.rr1x && f.Oppnt[1].Y == rl.rr1y &&
                            f.Oppnt[2].X == rl.rr2x && f.Oppnt[2].Y == rl.rr2y &&
                            f.Oppnt[3].X == rl.rr3x && f.Oppnt[3].Y == rl.rr3y
                        );

                    if (istr == -1)
                    {
                        rl.ruleNum = ++ruleNum;
                        adaptationOffRules.Add(rl);
                    }
                    else ++discardedOffrules;
                }
                else ++discardedOffrules;
            }
        }
    }

    public static void WriteFile(string filename, string text)
    {
        File.Delete(filename);

        using (FileStream fs = new FileStream(filename, FileMode.CreateNew, FileAccess.Write))
        using (StreamWriter sw = new StreamWriter(fs))
        {
            sw.Write(text);
            sw.Close();
        }
    }
}

