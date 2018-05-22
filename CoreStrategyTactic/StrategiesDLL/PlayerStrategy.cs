using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core;
using StrategiesDLL.Tactic;
using System.IO;

namespace StrategiesDLL
{
    public class PlayerStrategy : Strategy
    {
        int lastRule = -1;
        Graph graph;
        ZOrder zorder;
        List<int>[,] matrix;

        GridRule currentRule;
        TacticChooser tactic;

        public GridRule CurrentRule
        {
            get
            {
                return currentRule;
            }
        }

        public PlayerStrategy(GameSetting gameSetting)
            : base(gameSetting)
        {
            
        }

        public PlayerStrategy(GameSetting gameSetting, String file, bool mirror)
            : base(gameSetting)
        {
            gStrategy = new GridStrategy(file, mirror);
            tactic = new TacticChooser(mirror, gameSetting);

            zorder = new ZOrder();
            graph = new Graph();
            graph.Fill(gStrategy, gameSetting, 0.7);
        }

        //adapt strategy rules
        public override void AdaptStrategy(List<object> adaptedRules)
        {
            foreach (GridRule rule in adaptedRules)
                gStrategy.Rules.Add(rule);

            zorder = new ZOrder();
            graph = new Graph();
            graph.Fill(gStrategy, gameSetting, 0.7);
        }

        // tik strategie urcujici co se ma pri kazdem kroku vykonat
        public override void TickStrategy(StrategyStorage storage)
        {
            SetGoalkeeperStrategy(storage.myRobots[gameSetting.NUMBER_OF_ROBOTS - 1], storage);

            try
            {
                currentRule = FindBestRuleGraph(storage);
            }
            catch (Exception e)
            {
                currentRule = null;
            }

            SharedMutex.getMutex().WaitOne();
            if (currentRule != null)
            {
                int[] mapping = MapRobotsToMoveTo(storage, currentRule);
                for (int i = 0; i < gameSetting.NUMBER_OF_ROBOTS - 1; i++)
                    storage.myRobots[mapping[i]].PositionMove = gStrategy.GridToReal(currentRule.Move[i], gameSetting);
            }
            SharedMutex.getMutex().ReleaseMutex();

            // tactics
            SharedMutex.getMutex().WaitOne();
            tactic.chooseTactic(storage);
            SharedMutex.getMutex().ReleaseMutex();

        }

        public int CurrentRuleNumber()
        {
            return currentRule.Number;
        }

        #region funkce pro strategie
        // funkce, ktera podle daneho algoritmu najde nejlepsi (nejvyhodnejsi) pravidlo a toto vrati soucasne s mapovacim indexem pro roboty

        private GridRule FindBestRuleGraph(StrategyStorage storage)
        {
            double treshold = 500;
            GridRule bestRule = null;
            int x, y;

            //mine
            matrix = new List<int>[6, 4];
            for (int i = 0; i < storage.myRobots.Length - 1; i++)
            {
                Vector2D v = gStrategy.RealToGrid(storage.myRobots[i].Position, gameSetting);
                x = Convert.ToInt32(v.X);
                y = Convert.ToInt32(v.Y);
                if (matrix[x, y] != null)
                    matrix[x, y].Add(i);
                else matrix[x, y] = new List<int>() { i };
            }
            List<int> actMine = zorder.zOrderMain(matrix, 2);

            //oppnt
            matrix = new List<int>[6, 4];
            for (int i = 0; i < storage.oppntRobots.Length - 1; i++)
            {
                Vector2D v = gStrategy.RealToGrid(storage.oppntRobots[i].Position, gameSetting);
                x = Convert.ToInt32(v.X);
                y = Convert.ToInt32(v.Y);
                if (matrix[x, y] != null)
                    matrix[x, y].Add(i);
                else matrix[x, y] = new List<int>() { i };
            }
            List<int> actOppnt = zorder.zOrderMain(matrix, 2);

            //nejpodobnejsi pravidlo
            double min = Double.MaxValue;

            StringBuilder sb = new StringBuilder();

        jump:

            if (lastRule == -1)
            {
                GridRule rule;
                for (int r = 0; r < gStrategy.Rules.Count; r++)
                {

                    rule = gStrategy.Rules[r];

                    double ballRuleSize = gStrategy.GridToReal(rule.Ball, gameSetting).DistanceFrom(storage.Ball.Position);

                    double mineSize = 0;
                    for (int i = 0; i < actMine.Count; i++)
                        mineSize += gStrategy.GridToReal(rule.Mine[rule.ZMine[i]], gameSetting).DistanceFrom(storage.myRobots[actMine[i]].Position);

                    double oppntSize = 0;
                    for (int i = 0; i < actOppnt.Count; i++)
                        oppntSize += gStrategy.GridToReal(rule.Oppnt[rule.ZOppnt[i]], gameSetting).DistanceFrom(storage.oppntRobots[actOppnt[i]].Position);

                    if (ballRuleSize + mineSize + oppntSize < min)
                    {
                        min = ballRuleSize + mineSize + oppntSize;
                        bestRule = rule;
                        lastRule = r;
                    }

                    sb.AppendLine("rule: " + rule.Number + " dist: " + (ballRuleSize + mineSize + oppntSize));
                }
            }
            else
            {
                Node actNode = graph.Nodes[lastRule];
                GridRule rule;
                List<int> tempKeys = new List<int>(actNode.neighbours.Keys);
                tempKeys.Add(lastRule);

                foreach (int k in tempKeys)
                {
                    rule = (GridRule)graph.Nodes[k].data;

                    double ballRuleSize = gStrategy.GridToReal(rule.Ball, gameSetting).DistanceFrom(storage.Ball.Position);

                    double mineSize = 0;
                    for (int i = 0; i < actMine.Count; i++)
                        mineSize += gStrategy.GridToReal(rule.Mine[rule.ZMine[i]], gameSetting).DistanceFrom(storage.myRobots[actMine[i]].Position);

                    double oppntSize = 0;
                    for (int i = 0; i < actOppnt.Count; i++)
                        oppntSize += gStrategy.GridToReal(rule.Oppnt[rule.ZOppnt[i]], gameSetting).DistanceFrom(storage.oppntRobots[actOppnt[i]].Position);

                    if (ballRuleSize + mineSize + oppntSize < min)
                    {
                        min = ballRuleSize + mineSize + oppntSize;
                        bestRule = rule;
                        lastRule = k;
                    }

                    sb.AppendLine("rule: " + rule.Number + " dist: " + (ballRuleSize + mineSize + oppntSize));
                }

                //pokud je hodnota podobnosti mensi nez nejaky nastaveny treshold, tak bych mel znovu prohledavat vsechna pravidla
                if (min > treshold)
                {
                    sb.AppendLine("-----SEARCH ALL RULES----");
                    lastRule = -1;
                    goto jump;
                }
            }

            if (bestRule != null)
            {
                sb.AppendLine("BESTrule: " + bestRule.Number + " MINdist: " + min);
                sb.AppendLine("----------------------------------------------------------------");
            }

            return bestRule;
        }

        //finds the best mapping between selected moveTo coordinates and the real robots
        private int[] MapRobotsToMoveTo(StrategyStorage storage, GridRule rule)
        {
            int[] bestPerm = new int[4];
            int[][] indexes = { new int[] { 1, 2, 3, 0 },
                                new int[] { 1, 2, 0, 3 },
                                new int[] { 1, 3, 2, 0 },
                                new int[] { 1, 3, 0, 2 },
                                new int[] { 1, 0, 2, 3 },
                                new int[] { 1, 0, 3, 2 },
                                new int[] { 2, 1, 3, 0 },
                                new int[] { 2, 1, 0, 3 },
                                new int[] { 2, 3, 1, 0 },
                                new int[] { 2, 3, 0, 1 },
                                new int[] { 2, 0, 1, 3 },
                                new int[] { 2, 0, 3, 1 },
                                new int[] { 3, 1, 2, 0 },
                                new int[] { 3, 1, 0, 2 },
                                new int[] { 3, 2, 1, 0 },
                                new int[] { 3, 2, 0, 1 },
                                new int[] { 3, 0, 1, 2 },
                                new int[] { 3, 0, 2, 1 },
                                new int[] { 0, 1, 2, 3 },
                                new int[] { 0, 1, 3, 2 },
                                new int[] { 0, 2, 1, 3 },
                                new int[] { 0, 2, 3, 1 },
                                new int[] { 0, 3, 1, 2 },
                                new int[] { 0, 3, 2, 1 } };

            double min = double.MaxValue;

            foreach (int[] i in indexes)
            {
                double moveRuleSize = gStrategy.GridToReal(rule.Move[0], gameSetting).DistanceFrom(storage.myRobots[i[0]].Position)
                    + gStrategy.GridToReal(rule.Move[1], gameSetting).DistanceFrom(storage.myRobots[i[1]].Position)
                    + gStrategy.GridToReal(rule.Move[2], gameSetting).DistanceFrom(storage.myRobots[i[2]].Position)
                    + gStrategy.GridToReal(rule.Move[3], gameSetting).DistanceFrom(storage.myRobots[i[3]].Position);
                if (moveRuleSize < min)
                {
                    min = moveRuleSize;
                    bestPerm = i;
                }
            }

            return bestPerm;
        } 


        // strategie s jakou zvoleny brankar chyta v brane
        private void SetGoalkeeperStrategy(Robot goalkeeper, StrategyStorage storage)
        {
            if(goalkeeper.Position.X < gameSetting.AREA_NET_X / 2)
            {
                double b_y = (storage.Ball.Position.Y < -gameSetting.AREA_NET_Y / 2) ? -gameSetting.AREA_NET_Y / 2 : (storage.Ball.Position.Y > gameSetting.AREA_NET_Y / 2) ? gameSetting.AREA_NET_Y / 2 : storage.Ball.Position.Y;
                goalkeeper.PositionMove.X = -gameSetting.AREA_X / 2 + gameSetting.ROBOT_EDGE_SIZE / 2;
                goalkeeper.PositionMove.Y = b_y;
            }
            else
            {
                double b_y = (storage.Ball.Position.Y < -gameSetting.AREA_NET_Y / 2) ? -gameSetting.AREA_NET_Y / 2 : (storage.Ball.Position.Y > gameSetting.AREA_NET_Y / 2) ? gameSetting.AREA_NET_Y / 2 : storage.Ball.Position.Y;
                goalkeeper.PositionMove.X = gameSetting.AREA_X / 2 - gameSetting.ROBOT_EDGE_SIZE / 2;
                goalkeeper.PositionMove.Y = b_y;
            }
            
        }

        #endregion

        #region deprecated

        //private GridRule FindBestRule4(Storage storage)
        //{
        //    GridRule bestRule = null;
        //    int oppRuleSizeTemp;
        //    int myRuleSizeTemp;
        //    int myRuleSize = int.MaxValue;
        //    int oppRuleSize = int.MaxValue;
        //    double ballRuleSize;
        //    double TotalRuleSize = double.MaxValue;

        //    List<Vector2D> myRobotsCurrent = new List<Vector2D>();
        //    List<Vector2D> oppRobotsCurrent = new List<Vector2D>();
        //    List<Vector2D> myRobotsRule = new List<Vector2D>();
        //    List<Vector2D> oppRobotsRule = new List<Vector2D>();

        //    for (int i = 0; i < 4; i++)
        //    {
        //        myRobotsCurrent.Add(gStrategy.RealToGrid(storage.LeftRobots[i].Position, gameSetting));
        //    }
        //    for (int i = 0; i < 4; i++)
        //    {
        //        oppRobotsCurrent.Add(gStrategy.RealToGrid(storage.RightRobots[i].Position, gameSetting));
        //    }

        //    myRobotsCurrent.Sort();
        //    oppRobotsCurrent.Sort();

        //    foreach (GridRule rule in gStrategy.Rules)
        //    {
        //        ballRuleSize = gStrategy.GridToReal(rule.Ball, gameSetting).DistanceFrom(storage.Ball.Position) * ((double)gStrategy.PriorityBall / 100);

        //        for (int i = 0; i < 4; i++)
        //        {
        //            myRobotsRule.Add(rule.Mine[i]);
        //        }

        //        for (int i = 0; i < 4; i++)
        //        {
        //            oppRobotsRule.Add(rule.Oppnt[i]);
        //        }

        //        myRobotsRule.Sort();
        //        oppRobotsRule.Sort();

        //        myRuleSizeTemp = MatrixDistance(myRobotsCurrent, myRobotsRule);

        //        if (myRuleSizeTemp < myRuleSize)
        //        {
        //            myRuleSize = myRuleSizeTemp;
        //        }

        //        oppRuleSizeTemp = MatrixDistance(oppRobotsCurrent, oppRobotsRule);

        //        if (oppRuleSizeTemp < oppRuleSize)
        //        {
        //            oppRuleSize = oppRuleSizeTemp;
        //        }

        //        if ((myRuleSize + oppRuleSize + ballRuleSize) < TotalRuleSize)
        //        {
        //            TotalRuleSize = myRuleSize + oppRuleSize + ballRuleSize;
        //            bestRule = rule;
        //        }
        //        myRobotsRule.Clear();
        //        oppRobotsRule.Clear();
        //    }
        //    return bestRule;
        //}

        //private GridRule FindBestRule3(Storage storage)
        //{
        //    GridRule bestRule = null;

        //    double oppRuleSizeTemp;
        //    double myRuleSizeTemp;
        //    double myRuleSize = double.MaxValue;
        //    double oppRuleSize = double.MaxValue;
        //    double ballRuleSize;
        //    double TotalRuleSize = double.MaxValue;

        //    List<Vector2D> myRobotsCurrent = new List<Vector2D>();
        //    List<Vector2D> oppRobotsCurrent = new List<Vector2D>();
        //    List<Vector2D> myRobotsRule = new List<Vector2D>();
        //    List<Vector2D> oppRobotsRule = new List<Vector2D>();

        //    //List<int> myRobotsCurrent = new List<int>();
        //    //List<int> oppRobotsCurrent = new List<int>();
        //    //List<int> myRobotsRule = new List<int>();
        //    //List<int> oppRobotsRule = new List<int>();

        //    for (int i = 0; i < 4; i++)
        //    {
        //        myRobotsCurrent.Add(gStrategy.RealToGrid(storage.MyRobots[i].Position, gameSetting));
        //    }
        //    for (int i = 0; i < 4; i++)
        //    {
        //        oppRobotsCurrent.Add(gStrategy.RealToGrid(storage.OppRobots[i].Position, gameSetting));
        //    }
        //    //setridime vectory v listech

        //    myRobotsCurrent.Sort();
        //    oppRobotsCurrent.Sort();

        //    foreach (GridRule rule in gStrategy.Rules)
        //    {
        //        ballRuleSize = gStrategy.GridToReal(rule.Ball, gameSetting).DistanceFrom(storage.Ball.Position) * ((double)gStrategy.PriorityBall / 100);

        //        for (int i = 0; i < 4; i++)
        //        {
        //            myRobotsRule.Add(rule.Mine[i]);
        //        }

        //        for (int i = 0; i < 4; i++)
        //        {
        //            oppRobotsRule.Add(rule.Oppnt[i]);
        //        }

        //        myRobotsRule.Sort();
        //        oppRobotsRule.Sort();

        //        myRuleSizeTemp = EuclideanDistance2(myRobotsCurrent, myRobotsRule, 4);

        //        if (myRuleSizeTemp < myRuleSize)
        //        {
        //            myRuleSize = myRuleSizeTemp;
        //        }

        //        oppRuleSizeTemp = EuclideanDistance2(oppRobotsCurrent, oppRobotsRule, 4);

        //        if (oppRuleSizeTemp < oppRuleSize)
        //        {
        //            oppRuleSize = oppRuleSizeTemp;
        //        }

        //        if ((myRuleSize + oppRuleSize + ballRuleSize) < TotalRuleSize)
        //        {
        //            TotalRuleSize = myRuleSize + oppRuleSize + ballRuleSize;
        //            bestRule = rule;
        //        }

        //        myRobotsRule.Clear();
        //        oppRobotsRule.Clear();
        //    }
        //    return bestRule;
        //}

        //private GridRule FindBestRule2(Storage storage)
        //{
        //    GridRule bestRule = null;

        //    double oppRuleSizeTemp;
        //    double myRuleSizeTemp;
        //    double myRuleSize = double.MaxValue;
        //    double oppRuleSize = double.MaxValue;
        //    double ballRuleSize;
        //    double TotalRuleSize = double.MaxValue;

        //    List<int> myRobotsCurrent = new List<int>();
        //    List<int> oppRobotsCurrent = new List<int>();
        //    List<int> myRobotsRule = new List<int>();
        //    List<int> oppRobotsRule = new List<int>();

        //    for (int i = 0; i < 4; i++)
        //    {
        //        myRobotsCurrent.Add((int)gStrategy.RealToGrid(storage.MyRobots[i].Position, gameSetting).X);
        //        myRobotsCurrent.Add((int)gStrategy.RealToGrid(storage.MyRobots[i].Position, gameSetting).Y);
        //    }
        //    for (int i = 0; i < 4; i++)
        //    {
        //        oppRobotsCurrent.Add((int)gStrategy.RealToGrid(storage.OppRobots[i].Position, gameSetting).X);
        //        oppRobotsCurrent.Add((int)gStrategy.RealToGrid(storage.OppRobots[i].Position, gameSetting).Y);
        //    }

        //    foreach (GridRule rule in gStrategy.Rules)
        //    {
        //        ballRuleSize = gStrategy.GridToReal(rule.Ball, gameSetting).DistanceFrom(storage.Ball.Position) * ((double)gStrategy.PriorityBall / 100);

        //        for (int i = 0; i < 4; i++)
        //        {
        //            myRobotsRule.Add((int)gStrategy.RealToGrid(rule.Mine[i], gameSetting).X);
        //            myRobotsRule.Add((int)gStrategy.RealToGrid(rule.Mine[i], gameSetting).Y);
        //        }

        //        for (int i = 0; i < 4; i++)
        //        {
        //            oppRobotsRule.Add((int)gStrategy.RealToGrid(rule.Oppnt[i], gameSetting).X);
        //            oppRobotsRule.Add((int)gStrategy.RealToGrid(rule.Oppnt[i], gameSetting).Y);
        //        }

        //        myRuleSizeTemp = EuclideanDistance(myRobotsCurrent, myRobotsRule, 8);
        //        if (myRuleSizeTemp < myRuleSize)
        //        {
        //            myRuleSize = myRuleSizeTemp;
        //        }

        //        oppRuleSizeTemp = EuclideanDistance(oppRobotsCurrent, oppRobotsRule, 8);
        //        if (oppRuleSizeTemp < oppRuleSize)
        //        {
        //            oppRuleSize = oppRuleSizeTemp;
        //        }

        //        if ((myRuleSize + oppRuleSize + ballRuleSize) < TotalRuleSize)
        //        {
        //            TotalRuleSize = myRuleSize + oppRuleSize + ballRuleSize;
        //            bestRule = rule;
        //        }

        //        myRobotsRule.Clear();
        //        oppRobotsRule.Clear();

        //    }
        //    return bestRule;
        //}

        //private double EuclideanDistance2(List<Vector2D> p, List<Vector2D> q, int n)
        //{
        //    int i;
        //    double x1;
        //    double y1;
        //    double x2;
        //    double y2;
        //    double result = 0.0;
        //    for (i = 0; i < n; i++)
        //    {
        //        x1 = p[i].X;
        //        y1 = p[i].Y;
        //        x2 = p[i].X;
        //        y2 = p[i].Y;

        //        result += (x1 - x2) * (x1 - x2);
        //        result += (y1 - y2) * (y1 - y2);
        //    }
        //    return Math.Sqrt(result);
        //}
        //private double EuclideanDistance(List<int> p, List<int> q, int n)
        //{
        //    int i;
        //    double result = 0.0;
        //    for (i = 0; i < n; i++)
        //        result += (p[i] - q[i]) * (p[i] - q[i]);
        //    return Math.Sqrt(result);
        //}

        //private int PointDistance(Vector2D a, Vector2D b)
        //{
        //    int tempx;
        //    int tempy;

        //    if (a.X < b.X) tempx = (int)(b.X - a.X);
        //    else if (a.X == b.X) tempx = 0;
        //    else tempx = (int)(a.X - b.X);

        //    if (a.Y < b.Y) tempy = (int)(b.Y - a.Y);
        //    else if (a.Y == b.Y) tempy = 0;
        //    else tempy = (int)(a.Y - b.Y);

        //    if (tempx == tempy) return tempx;
        //    else if (tempx == 0) return tempy;
        //    else if (tempy == 0) return tempx;
        //    else if (tempx > tempy) return tempy + (tempx - tempy);
        //    else return tempx + (tempy - tempx);
        //}

        //private int MatrixDistance(List<Vector2D> a, List<Vector2D> b)
        //{
        //    //celkova "vzdalenost" nebo podobnost tech dvou matic (suma vzdalenosti na sebe namapovanych bodu)
        //    int sumDistance = 0;


        //    /// v tomto cyklu se postupne kontroluji vzdalenosti kazdeho bodu z listu A s kazdym bodem listu B
        //    /// a dochazi k mapovani (tim, ze oznacim property isMapped u bodu na true), tim padem se s temy body uz nepracuje
        //    /// a jejich vzdalenost se pricte k sume vzdalenoti (neboli "jsou namapovany")
        //    ///
        //    ///i = mozne vzdalenosti dvou bodu v nasi matici.. - v nasem pripade od 0 do 5
        //    for (int i = 0; i < 6; i++)
        //    {
        //        //j = pocet bodu v listu A
        //        for (int j = 0; j < 4; j++)
        //        {
        //            //k = pocet bodu v listu B
        //            for (int k = 0; k < 4; k++)
        //            {
        //                //vzdalenost 2 bodu se vypocte jen kdyz jeste oba nejsou namapovane
        //                if (a[j].isMapped == false && b[k].isMapped == false)
        //                {
        //                    //vypocet vzdalenosti
        //                    int tempDistance = PointDistance(a[j], b[k]);
        //                    //pokud je vzdalenost stejna jako soucasna hodnota porovnavane vzdalenosti (tzn, nejprve se mapujou body se vzdalenosti 0, ostatni ne, potom 1, 2..)
        //                    if (tempDistance == i)
        //                    {
        //                        //kdyz uspesne "namapujeme" 2 body, tak jejich vzdalenost se pricte k celkove vzdalenosti techto 2 matic
        //                        sumDistance += tempDistance;
        //                        //a u obou bodu nastavime namapovani, aby se tyto 2 body uz neresily v pristim kroku
        //                        a[j].isMapped = true;
        //                        b[k].isMapped = true;
        //                    }
        //                }
        //            }
        //        }
        //        //kontrola, pokud po nejakem kroku uz jsou vsechny namapovane, nema cenu pokracovat dal.
        //        if (a[0].isMapped && a[1].isMapped && a[2].isMapped && a[3].isMapped)
        //        {
        //            goto navesti;
        //        }
        //    }

        //navesti:

        //    //nastavime isMapped u bodu z A na false, aby se ted mohla porovnavat jedna matice s vice
        //    //ruznymi maticemi, tak jak to bude v simulatoru..
        //    a[0].isMapped = false;
        //    a[1].isMapped = false;
        //    a[2].isMapped = false;
        //    a[3].isMapped = false;

        //    b[0].isMapped = false;
        //    b[1].isMapped = false;
        //    b[2].isMapped = false;
        //    b[3].isMapped = false;


        //    //vracime celkovou sumu vzdalenosti namapovanych bodu.
        //    return sumDistance;
        //}

        //private GridRule FindBestRule(ref int[] perm, Storage storage)
        //{
        //    GridRule bestRule = null;
        //    int[][] indexes = { new int[] { 1, 2, 3, 0 }, 
        //                        new int[] { 1, 2, 0, 3 }, 
        //                        new int[] { 1, 3, 2, 0 }, 
        //                        new int[] { 1, 3, 0, 2 }, 
        //                        new int[] { 1, 0, 2, 3 }, 
        //                        new int[] { 1, 0, 3, 2 }, 
        //                        new int[] { 2, 1, 3, 0 }, 
        //                        new int[] { 2, 1, 0, 3 }, 
        //                        new int[] { 2, 3, 1, 0 }, 
        //                        new int[] { 2, 3, 0, 1 }, 
        //                        new int[] { 2, 0, 1, 3 }, 
        //                        new int[] { 2, 0, 3, 1 }, 
        //                        new int[] { 3, 1, 2, 0 }, 
        //                        new int[] { 3, 1, 0, 2 }, 
        //                        new int[] { 3, 2, 1, 0 }, 
        //                        new int[] { 3, 2, 0, 1 }, 
        //                        new int[] { 3, 0, 1, 2 }, 
        //                        new int[] { 3, 0, 2, 1 }, 
        //                        new int[] { 0, 1, 2, 3 }, 
        //                        new int[] { 0, 1, 3, 2 }, 
        //                        new int[] { 0, 2, 1, 3 }, 
        //                        new int[] { 0, 2, 3, 1 }, 
        //                        new int[] { 0, 3, 1, 2 }, 
        //                        new int[] { 0, 3, 2, 1 } };

        //    double min = double.MaxValue;
        //    foreach (GridRule rule in gStrategy.Rules)
        //    {
        //        double ballRuleSize = gStrategy.GridToReal(rule.Ball, gameSetting).DistanceFrom(storage.Ball.Position) * ((double)gStrategy.PriorityBall / 100);

        //        double oppRuleSize = double.MaxValue;

        //        foreach (int[] i in indexes)
        //        {
        //            int[] p = (rule.Type == GridRule.RuleType.Offense) ? gStrategy.PriorityOpponent : gStrategy.PriorityOpponentD;
        //            double size = gStrategy.GridToReal(rule.Oppnt[0], gameSetting).DistanceFrom(storage.RightRobots[i[0]].Position) * ((double)p[i[0]] / 100) + gStrategy.GridToReal(rule.Oppnt[1], gameSetting).DistanceFrom(storage.RightRobots[i[1]].Position) * ((double)p[i[1]] / 100) + gStrategy.GridToReal(rule.Oppnt[2], gameSetting).DistanceFrom(storage.RightRobots[i[2]].Position) * ((double)p[i[2]] / 100) + gStrategy.GridToReal(rule.Oppnt[3], gameSetting).DistanceFrom(storage.RightRobots[i[3]].Position) * ((double)p[i[3]] / 100);
        //            if (size < oppRuleSize)
        //                oppRuleSize = size;
        //        }

        //        foreach (int[] i in indexes)
        //        {
        //            int[] p = (rule.Type == GridRule.RuleType.Offense) ? gStrategy.PriorityMine : gStrategy.PriorityMineD;
        //            double myRuleSize = gStrategy.GridToReal(rule.Mine[0], gameSetting).DistanceFrom(storage.LeftRobots[i[0]].Position) * ((double)p[i[0]] / 100) + gStrategy.GridToReal(rule.Mine[1], gameSetting).DistanceFrom(storage.LeftRobots[i[1]].Position) * ((double)p[i[1]] / 100) + gStrategy.GridToReal(rule.Mine[2], gameSetting).DistanceFrom(storage.LeftRobots[i[2]].Position) * ((double)p[i[2]] / 100) + gStrategy.GridToReal(rule.Mine[3], gameSetting).DistanceFrom(storage.LeftRobots[i[3]].Position) * ((double)p[i[3]] / 100);
        //            if (ballRuleSize + oppRuleSize + myRuleSize < min)
        //            {
        //                min = ballRuleSize + oppRuleSize + myRuleSize;
        //                bestRule = rule;
        //                perm = i;
        //            }
        //        }
        //    }
        //    return bestRule;
        //}

        // vraci pocet robotu, kteri jsou jiz na pozici, na kterou je dane pravidlo poslalo
        //private int RobotsInDesiredGrid(Storage storage)
        //{
        //    if (currentRule == null)
        //        return Int32.MaxValue;
        //    int count = 0;
        //    for (int i = 0; i < 4; i++)
        //    {
        //        if (gStrategy.RealToGrid(storage.LeftRobots[ruleIndex[i]].Position, gameSetting) == currentRule.Move[i])
        //        {
        //            //TACTIC if (!storage.MyRobots[ruleIndex[i]].tactic)
        //            //    storage.MyRobots[ruleIndex[i]].stop();
        //            count++;
        //        }
        //    }
        //    return count;
        //}

        //#endregion

        //#region goalkeeper & attacker strategy

        // funkce, ktera vrati toho robota, ktery byl funkci vybran jako nejvhodnejsi utocnik
        //private Robot ChooseAttacker(Storage storage)
        //{
        //    Robot attacker = null;
        //    double min = double.MaxValue;

        //    foreach (Robot r in storage.MyRobots)
        //    {
        //        if (r.Position.DistanceFrom(storage.Ball.Position) < min && r != storage.MyRobots[4] && r.Position.DistanceFrom(storage.Ball.Position) < gStrategy.AttackerMin * ((gameSetting.AREA_X + gameSetting.AREA_NET_X * 2) / gStrategy.Size.X))
        //        {
        //            attacker = r;
        //            min = r.Position.DistanceFrom(storage.Ball.Position);
        //        }
        //    }
        //    return attacker;
        //}

        //// strategie, s jakou dany utocnik utoci
        //private void SetAttackerStrategy(Robot attacker, Storage storage)
        //{
        //    if (attacker != null)
        //    {
        //        attacker.PositionMove.X=storage.Ball.Position.X;
        //        attacker.PositionMove.Y = storage.Ball.Position.Y;
        //    }
        //}

        #endregion
    }
}
