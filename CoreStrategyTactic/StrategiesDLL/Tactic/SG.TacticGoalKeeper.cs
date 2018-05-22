using System;
using System.Collections.Generic;
using System.Text;
using Core;

namespace StrategiesDLL.Tactic
{
    public class TacticGoalKeeper : Tactic
    {
        private double x;
        private double y;
        private double width;
        private double height;

        public TacticGoalKeeper(bool mirror, GameSetting gameSetting)
            : base(mirror, gameSetting)
        {
            width = 0.159 * gameSetting.AREA_X;     // sirka velkeho vapna, 15,9% sirky hriste
            height = 0.444 * gameSetting.AREA_Y;    // vyska velkeho vapna, 44,4% vysky hriste
            x = mirror ? gameSetting.AREA_X / 2 : -gameSetting.AREA_X / 2;
            y = 0 - (height / 2);
        }

        public override bool needOf(Vector2D ballPosition)
        {
            if (mirror)
            {
                if (ballPosition.X < x - width || ballPosition.Y > height / 2 ||
                    ballPosition.Y < y)
                    return false;
                else
                    return true;
            }
            else
            {
                if (ballPosition.X > x + width || ballPosition.Y > height / 2 ||
                    ballPosition.Y < y)
                    return false;
                else
                    return true;
            }
        }

        /**
         * Posleme brankare na pozici mice
         */
        public override void doTactic(StrategyStorage storage)
        {
            int robot = findRobot(storage);
            //robotCheck(robot);
            // smaz storage.myRobots[robot].tactic = true;
            storage.myRobots[robot].PositionMove = storage.Ball.Position;
        }

        /**
         * Vybrany robot je vzdy brankar
         */
        protected override int findRobot(StrategyStorage storage)
        {
            return gameSetting.NUMBER_OF_ROBOTS - 1;
        }
    }
}
