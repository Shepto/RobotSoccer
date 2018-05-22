using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Core;
using Regresion;
using UnityEngine;

namespace RobotSoccerSimulator.StrategyGenerator
{
    public class TacticOffensiveShoot : Tactic
    {
        //private bool mirror;

        private double x;
        private double y;
        private double width;
        private double height;

        #region REGRESE

        public double distanceTwoPoints(Bod a, Bod b)
        {
            return Math.Sqrt(((a.x - b.x) * (a.x - b.x)) + ((a.y - b.y) * (a.y - b.y)));
        }

        public Boolean IsObstacle(Bod robo, Bod prekazka, Bod cil)
        {
            double a, b, c, v;
            b = cil.x - robo.x;
            a = (cil.y - robo.y) * (-1);
            c = -1 * (a * robo.x + b * robo.y);
            v = (a * prekazka.x + b * prekazka.y + c) / (Math.Sqrt((a * a) + (b * b)));
            if (v < 0) v *= -1;
            if (v < 15 && distanceTwoPoints(robo, prekazka) < distanceTwoPoints(robo, cil))
            // && ((prekazka.x > robo.x && cil.x > robo.x) || (prekazka.x < robo.x && cil.x < robo.x)))
            {
                return true;
            }
            else return false;
        }

        public void regrese(Core.Robot[] a, Core.Robot[] b)
        {
            Bod robo;
            Bod prekazka;
            Bod prekazka2;
            Bod cil;
            List<RobotDistance> lis = new List<RobotDistance>();
            for (int j = 0; j < 5; j++)
            {
                robo = new Bod(a[j].Position.X, a[j].Position.Y);
                cil = new Bod(a[j].PositionMove.X, a[j].PositionMove.Y);
                //cil = new Bod(ball.Position.X,ball.Position.Y);
                for (int i = 0; i < 5; i++)
                {
                    prekazka = new Bod(b[i].Position.X, b[i].Position.Y);
                    prekazka2 = new Bod(a[i].Position.X, a[i].Position.Y);
                    try
                    {
                        if (IsObstacle(robo, prekazka, cil))
                        {
                            lis.Add(new RobotDistance(new Bod(prekazka.x, prekazka.y), distanceTwoPoints(robo, prekazka)));
                        }
                        if (IsObstacle(robo, prekazka2, cil) && i != j)
                        {
                            lis.Add(new RobotDistance(new Bod(prekazka2.x, prekazka2.y), distanceTwoPoints(robo, prekazka2)));
                        }
                    }
                    catch { };
                }

                if (lis.Count > 0)
                {
                    try
                    {
                        var sort = from s in lis orderby s.vzdalenost select s;
                        PolynomialRegresion regrese = new PolynomialRegresion(robo, new Bod(sort.ElementAt(0).bod.x, sort.ElementAt(0).bod.y), cil);
                        List<Bod> body = regrese.polynom(true);
                        if (cil.x > robo.x)
                            a[j].PositionMove = new Vector2D(body[Convert.ToInt32(robo.x + 1) + 110].x, body[Convert.ToInt32(robo.x + 1) + 110].y);
                        else a[j].PositionMove = new Vector2D(body[Convert.ToInt32(robo.x - 1) + 110].x, body[Convert.ToInt32(robo.x - 1) + 110].y);
                        lis.RemoveAll(item => item.vzdalenost > 0);
                    }
                    catch { }
                }
            }
        }

        public void RegreseOne(int selRobot, Core.Ball ball, Core.Robot[] a, Core.Robot[] b)
        {
            Bod robo;
            Bod prekazka;
            Bod prekazka2;
            Bod cil;
            List<RobotDistance> lis = new List<RobotDistance>();
            //for (int j = 0; j < 5; j++)
            {
                robo = new Bod(a[selRobot].Position.X, a[selRobot].Position.Y);
                //cil = new Bod(a[selRobot].PositionMove.X, a[selRobot].PositionMove.Y);
                cil = new Bod(ball.Position.X,ball.Position.Y);
                
                //hledani prekazek v ceste robota
                for (int i = 0; i < 5; i++)
                {
                    prekazka = new Bod(b[i].Position.X, b[i].Position.Y);
                    prekazka2 = new Bod(a[i].Position.X, a[i].Position.Y);
                    try
                    {
                        if (IsObstacle(robo, prekazka, cil))
                        {
                            lis.Add(new RobotDistance(new Bod(prekazka.x, prekazka.y), distanceTwoPoints(robo, prekazka)));
                        }
                        if (IsObstacle(robo, prekazka2, cil) && i != selRobot)
                        {
                            lis.Add(new RobotDistance(new Bod(prekazka2.x, prekazka2.y), distanceTwoPoints(robo, prekazka2)));
                        }
                    }
                    catch { };
                }

                int offset = 10;

                //objeti prekazky smerem k cili kterym je micek
                if (lis.Count > 0)
                {
                    //if(!mirror)
                    //    Debug.Log(selRobot + " PREKAZKA");
                    try
                    {
                        var sort = from s in lis orderby s.vzdalenost select s;
                        PolynomialRegresion regrese = new PolynomialRegresion(robo, new Bod(sort.ElementAt(0).bod.x, sort.ElementAt(0).bod.y), cil);
                        List<Bod> body = regrese.polynom(true);

                        if (cil.x > robo.x)
                            a[selRobot].PositionMove = new Vector2D(body[Convert.ToInt32(robo.x + offset) + 110].x, body[Convert.ToInt32(robo.x + offset) + 110].y);
                        else a[selRobot].PositionMove = new Vector2D(body[Convert.ToInt32(robo.x - offset) + 110].x, body[Convert.ToInt32(robo.x - offset) + 110].y);

                        lis.RemoveAll(item => item.vzdalenost > 0);
                    }
                    catch { }
                }
                //mezi mickem a robotem neni prekazka, zmenit cil za branku a prolozit mickem
                else
                {
                    try
                    {
                        PolynomialRegresion regrese;
                        List<Bod> body;

                        if (mirror)
                        {
                            //micek je pred robotem
                            if (robo.x > ball.Position.X)
                            {
                                cil = new Bod(-110, 0);
                                regrese = new PolynomialRegresion(robo, new Bod(ball.Position.X, ball.Position.Y), cil);
                                body = regrese.polynom(false);
                            }
                            //micek je za robotem
                            else
                            {
                                cil = new Bod(110, 0);
                                regrese = new PolynomialRegresion(robo, new Bod(ball.Position.X, ball.Position.Y), cil);
                                body = regrese.polynom(true);
                            }

                            if (cil.x > robo.x)
                                a[selRobot].PositionMove = new Vector2D(body[Convert.ToInt32(robo.x + offset) + 110].x, body[Convert.ToInt32(robo.x + offset) + 110].y);
                            else a[selRobot].PositionMove = new Vector2D(body[Convert.ToInt32(robo.x - offset) + 110].x, body[Convert.ToInt32(robo.x - offset) + 110].y);
                        }
                        else
                        {
                            //micek je za robotem
                            if (robo.x > ball.Position.X)
                            {
                                cil = new Bod(-110, 0);
                                regrese = new PolynomialRegresion(robo, new Bod(ball.Position.X, ball.Position.Y), cil);
                                body = regrese.polynom(true);
                            }
                            //micek je pred robotem
                            else
                            {
                                cil = new Bod(110, 0);
                                regrese = new PolynomialRegresion(robo, new Bod(ball.Position.X, ball.Position.Y), cil);
                                body = regrese.polynom(false);
                            }

                            if (cil.x > robo.x)
                                a[selRobot].PositionMove = new Vector2D(body[Convert.ToInt32(robo.x + offset) + 110].x, body[Convert.ToInt32(robo.x + offset) + 110].y);
                            else a[selRobot].PositionMove = new Vector2D(body[Convert.ToInt32(robo.x - offset) + 110].x, body[Convert.ToInt32(robo.x - offset) + 110].y);
                        }
                    }
                    catch { }
                }
            }
        }

        public void RegreseMove(int selRobot, Vector2D moveTo, Core.Robot[] a, Core.Robot[] b)
        {
            Bod robo;
            Bod prekazka;
            Bod prekazka2;
            Bod cil;
            List<RobotDistance> lis = new List<RobotDistance>();
            
            robo = new Bod(a[selRobot].Position.X, a[selRobot].Position.Y);
            cil = new Bod(moveTo.X, moveTo.Y);

            //hledani prekazek v ceste robota
            for (int i = 0; i < 5; i++)
            {
                prekazka = new Bod(b[i].Position.X, b[i].Position.Y);
                prekazka2 = new Bod(a[i].Position.X, a[i].Position.Y);
                try
                {
                    if (IsObstacle(robo, prekazka, cil))
                    {
                        lis.Add(new RobotDistance(new Bod(prekazka.x, prekazka.y), distanceTwoPoints(robo, prekazka)));
                    }
                    if (IsObstacle(robo, prekazka2, cil) && i != selRobot)
                    {
                        lis.Add(new RobotDistance(new Bod(prekazka2.x, prekazka2.y), distanceTwoPoints(robo, prekazka2)));
                    }
                }
                catch { };
            }

            int offset = 10;

            //objeti prekazky smerem k cili kterym je MoveTo
            if (lis.Count > 0)
            {
                try
                {
                    var sort = from s in lis orderby s.vzdalenost select s;
                    PolynomialRegresion regrese = new PolynomialRegresion(robo, new Bod(sort.ElementAt(0).bod.x, sort.ElementAt(0).bod.y), cil);
                    List<Bod> body = regrese.polynom(true);

                    if (cil.x > robo.x)
                        a[selRobot].PositionMove = new Vector2D(body[Convert.ToInt32(robo.x + offset) + 110].x, body[Convert.ToInt32(robo.x + offset) + 110].y);
                    else a[selRobot].PositionMove = new Vector2D(body[Convert.ToInt32(robo.x - offset) + 110].x, body[Convert.ToInt32(robo.x - offset) + 110].y);

                    lis.RemoveAll(item => item.vzdalenost > 0);
                    //Debug.Log(mirror + " - " + selRobot + " PREKAZKA pri MoveTo");
                }
                catch { }
            }
            
        }

        #endregion

        public TacticOffensiveShoot(bool mirror, GameSetting gameSetting)
            : base(mirror, gameSetting)
        {
            if (mirror)
                x = field_width / 2 - one_third;
            else
                x = -field_width / 2 + one_third;

            y = -gameSetting.AREA_Y / 2;
            width = one_third * 2;
            height = gameSetting.AREA_Y;
        }

        public override bool needOf(Vector2D ballPosition)
        {
            if (mirror)
            {
                if (ballPosition.X > x)
                    return false;
                else
                    return true;
            }
            else
            {
                if (ballPosition.X < x)
                    return false;
                else
                    return true;
            }
        }

        public override void doTactic(Storage storage)
        {
            int robot = findRobot(storage);
            //robotCheck(robot);

            //Vector2D ball_next_pos = Storage.Ball.Position + Storage.ball.Position * Storage.timestep * Storage.BALL_FRICTION;
            // smaz storage.myRobots[robot].tactic = true;


            //VYPOCET REGRESE pro vybraneho robota
            //robot = 4;
            RegreseOne(robot, storage.Ball, storage.LeftRobots, storage.RightRobots);
            //storage.MyRobots[robot].PositionMove=storage.Ball.Position;

            for (int i = 0; i < gameSetting.NUMBER_OF_ROBOTS - 1; i++)
            {
                if (i == robot)
                    continue;
                RegreseMove(i, storage.LeftRobots[i].PositionMove, storage.LeftRobots, storage.RightRobots);
            }

            //storage.LeftRobots[0].PositionMove = new Vector2D(-80, -60);
            //storage.LeftRobots[1].PositionMove = new Vector2D(-80, 60);
            //storage.LeftRobots[2].PositionMove = new Vector2D(-80, -60);
            //storage.LeftRobots[3].PositionMove = new Vector2D(-80, 60);
        }

        protected override int findRobot(Storage storage)
        {
            //int robot = shootingArea(storage);
            int robot = ChooseOffensiveRobot(storage);
            return robot;
        }

        private int ChooseOffensiveRobot(Storage storage)
        {
            int bestRobot = -1;
            double minDist = Double.MaxValue;
            double minAngle = Double.MaxValue;

            for(int i = 0; i < gameSetting.NUMBER_OF_ROBOTS - 1; i++)
            {
                Robot r = storage.LeftRobots[i];

                double dist = storage.Ball.Position.DistanceFrom(r.Position);
                if(dist < minDist)
                {
                    minDist = dist;
                    bestRobot = i;
                }
            }

            return bestRobot;
        }

        private int shootingArea(Storage storage)
        {
            Vector2D area_bottom = new Vector2D(x, y);                // spodni bod hranicni primky
            Vector2D area_top = new Vector2D(x, y + height);          // horni bod hranicni primky

            Vector2D pC = storage.Ball.Position;                                        // micek
            Vector2D pB = getCoordinates(opp_goal_bottom, pC, area_bottom, area_top);   // horni bod
            Vector2D pA = getCoordinates(opp_goal_top, pC, area_bottom, area_top);      // spodni bod

            double min = double.MaxValue;   // pro vyber robota s nejmensi vzdalenosti od mice ve spravnem uhlu
            int robot = -1;                 // nejlepsi robot ve spravnem uhlu
            int robot1 = -1;                // robot nejblize spravneho uhlu
            double angle = -1;              // uhel nejblizsi spravnemu uhlu
            int count = 0;                  // pocitani robotu

            foreach (Robot r in storage.LeftRobots)
            {
                double pB_r = pB.DistanceFrom(r.Position);
                double pB_pC = pB.DistanceFrom(pC);

                double pA_r = pA.DistanceFrom(r.Position);
                double pA_pC = pA.DistanceFrom(pC);

                double pA_pB = pB.DistanceFrom(pA);
                double pC_r = pC.DistanceFrom(r.Position);

                double alfa = Math.Acos((pC_r * pC_r + pA_r * pA_r - pA_pC * pA_pC) / (2 * pC_r * pA_r));
                alfa += Math.Acos((pA_r * pA_r + pB_r * pB_r - pA_pB * pA_pB) / (2 * pA_r * pB_r));
                alfa += Math.Acos((pB_r * pB_r + pC_r * pC_r - pB_pC * pB_pC) / (2 * pB_r * pC_r));
                alfa *= 180 / Math.PI;

                // pC_r ... vzdalenost robota od mice
                if (alfa > 355 && pC_r < min && r != storage.LeftRobots[gameSetting.NUMBER_OF_ROBOTS - 1])
                {
                    robot = count;
                    min = pC_r;
                }

                // alternativni plan - robot nejblize spravneho uhlu
                if (alfa > angle && r != storage.LeftRobots[gameSetting.NUMBER_OF_ROBOTS - 1])
                {
                    angle = alfa;
                    robot1 = count;
                }

                count++;
            }

            // nebyl vybran zaden ve spravnem uhlu, pouzijeme alternativni plan
            if (robot < 0) return robot1;

            return robot;
        }

        /**
         * pomoci parametru t, ziskaneho z funkce param(), vypocita souradnice pruseciku
         */
        private Vector2D getCoordinates(Vector2D pA, Vector2D pB, Vector2D pC, Vector2D pD)
        {
            double t = param(pA, pB, pC, pD);
            double x = pA.X + (pB.X - pA.X) * t;
            double y = pA.Y + (pB.Y - pA.Y) * t;
            return new Vector2D(x, y);
        }

        /**
         * zjisti parametr t pro parametrickou rovnici primky. Parametr t udava misto pruseciku dvou primek.
         */
        private double param(Vector2D pA, Vector2D pB, Vector2D pC, Vector2D pD)
        {
            double d = (pB.X - pA.X) * (pD.Y - pC.Y) - (pB.Y - pA.Y) * (pD.X - pC.X);
            double pom = (pC.X - pA.X) * (pD.Y - pC.Y) - (pC.Y - pA.Y) * (pD.X - pC.X);
            return pom / d;
        }
    }   
}