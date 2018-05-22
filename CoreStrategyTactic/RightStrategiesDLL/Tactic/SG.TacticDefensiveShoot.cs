using System;
using System.Collections.Generic;
using System.Text;
using Core;

namespace RightStrategiesDLL.Tactic
{
    public class TacticDefensiveShoot : Tactic
    {
        private double x;
        private double y;
        private double width;
        private double height;

        public TacticDefensiveShoot(bool mirror, GameSetting gameSetting)
            : base(mirror, gameSetting)
        {
            if (mirror)
            {
                x = field_width / 2;
            }
            else
            {
                x = -field_width / 2;
            }

            y = -gameSetting.AREA_Y / 2;
            width = one_third;
            height = gameSetting.AREA_Y;
        }

        public override bool needOf(Vector2D ballPosition)
        {
            if (mirror)
            {
                if (ballPosition.X < x - width)
                    return false;
                else
                    return true;
            }
            else
            {
                if (ballPosition.X > x + width)
                    return false;
                else
                    return true;
            }
        }

        public override void doTactic(Storage storage)
        {
            int robot = findRobot(storage);
            robotCheck(robot); //Zrusime oznaceni robotu, kteri meli vykonat taktiku v minulem kroku (definovana v Tactic)

            /*Vector2D ball_next_pos = Storage.ball.Position +
                Storage.ball.Position * Storage.timestep * Storage.BALL_FRICTION;

            storage.myRobots[robot].GoTo(ball_next_pos);*/

            storage.LeftRobots[robot].PositionMove = storage.Ball.Position;
            // smaz storage.myRobots[robot].tactic = true;
        }

        protected override int findRobot(Storage storage)
        {
            int robot = shootingArea(storage);
            return robot;
        }

        private int shootingArea(Storage storage)
        {
            Vector2D area_bottom = new Vector2D(x + width, y);         // spodni bod hranicni primky
            Vector2D area_top = new Vector2D(x + width, y + height);   // horni bod hranicni primky

            Vector2D pC = storage.Ball.Position;                                           // micek
            Vector2D pB = getCoordinates(my_goal_bottom, pC, area_bottom, area_top);       // horni bod
            Vector2D pA = getCoordinates(my_goal_top, pC, area_bottom, area_top);          // spodni bod

            double min = double.MaxValue;
            int robot = -1;
            int count = 0;

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
                if (alfa < 360 && pC_r < min && r != storage.LeftRobots[gameSetting.NUMBER_OF_ROBOTS - 1])
                {
                    robot = count;
                    min = pC_r;
                }

                count++;
            }

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
