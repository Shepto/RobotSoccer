using System;
using System.Collections.Generic;
using System.Text;
using Core;

namespace RightStrategiesDLL.Tactic
{
    public class TacticChooser
    {
        private Tactic defensiveShoot;
        private Tactic offensiveShoot;
        private Tactic borders;
        private Tactic goalkeeper;
        private bool mirror;

        public TacticChooser(bool mirror, GameSetting gameSetting)
        {
            defensiveShoot = new TacticDefensiveShoot(mirror, gameSetting);
            offensiveShoot = new TacticOffensiveShoot(mirror, gameSetting);
            borders = new TacticBorder(mirror, gameSetting);
            goalkeeper = new TacticGoalKeeper(mirror, gameSetting);
            this.mirror = mirror;
        }

        public void chooseTactic(Storage storage)
        {
            //offensiveShoot.doTactic(storage);
            if (goalkeeper.needOf(storage.Ball.Position))
            {
                goalkeeper.doTactic(storage);
            }
            else if (borders.needOf(storage.Ball.Position))
            {
                borders.doTactic(storage);
            }
            //else if (defensiveShoot.needOf(storage.Ball.Position))
            //{
            //    defensiveShoot.doTactic(storage);
            //}
            else //if (offensiveShoot.needOf(storage.Ball.Position))
            {
                offensiveShoot.doTactic(storage);
            }
        }
    }
}



