using System;
using System.Collections.Generic;
using System.Text;
using Core;

namespace RightStrategiesDLL.Tactic
{
    public class TacticBorder : Tactic
    {
        private double x;
        private double y;
        private double width;
        private double height;

        private double border;    // sirka kraje
        private double step;      // o kolik ma robot objet mic

        public TacticBorder(bool mirror, GameSetting gameSetting) : base(mirror, gameSetting)
        {
            x = -gameSetting.AREA_X / 2;
            y = -gameSetting.AREA_Y / 2;
            width = gameSetting.AREA_X;
            height = gameSetting.AREA_Y;
            border = 0.03 * gameSetting.AREA_X;    // sirka kraje
            step = 0.03 * gameSetting.AREA_X;      // o kolik ma robot objet mic
        }

        public override bool needOf(Vector2D ballPosition)
        {
            if (ballPosition.X > x + border &&
                ballPosition.X < x + width - border &&
                ballPosition.Y > y + border &&
                ballPosition.Y < y + height - border)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public override void doTactic(Storage storage)
        {
            int robot = findRobot(storage);
            robotCheck(robot);
            // smaz storage.myRobots[robot].tactic = true;

            // micek je pri kraji vlevo nebo vpravo
            if (storage.Ball.Position.X < x + border || 
                storage.Ball.Position.X > x + width - border)
            {
        // smaz storage.MyRobots[robot].GoTo(storage.Ball.Position.X, storage.Ball.Position.Y - step);
                storage.LeftRobots[robot].PositionMove = new Vector2D (storage.Ball.Position.X, storage.Ball.Position.Y - step);
                
            }
            // micek je pri kraji na hore nebo dole
            else if (storage.Ball.Position.Y < y + border ||
                storage.Ball.Position.Y > y + height - border)
            {
                if (mirror)
                    storage.LeftRobots[robot].PositionMove = new Vector2D(storage.Ball.Position.X + (3*step), storage.Ball.Position.Y); 
                else
                    storage.LeftRobots[robot].PositionMove = new Vector2D(storage.Ball.Position.X - (3*step), storage.Ball.Position.Y);
            }
        }

        /**
         * Vybere nejvhodnejsiho - nejblizsiho robota 
         */
        protected override int findRobot(Storage storage)
        {
            double distance = double.MaxValue;

            int count = 0;
            int robot = -1;

            foreach (Robot r in storage.LeftRobots)
            {
                double temp = r.Position.DistanceFrom(storage.Ball.Position);

                if (temp < distance && r != storage.LeftRobots[gameSetting.NUMBER_OF_ROBOTS - 1])
                {
                    distance = temp;
                    robot = count;
                }
                count++;
            }

            return robot;
        }
    }
}
