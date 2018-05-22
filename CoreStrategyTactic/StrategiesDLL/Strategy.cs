using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core;


namespace StrategiesDLL
{
    public abstract class Strategy
    {
        protected GridStrategy gStrategy; // instance gridove strategie

        protected GameSetting gameSetting;

        public GridStrategy GStrategy
        {
            get
            {
                return gStrategy;
            }
        }

        public Strategy(GameSetting gameSetting)
        {
            this.gameSetting = gameSetting;
        }

        public abstract void TickStrategy(StrategyStorage storage);
        public abstract void AdaptStrategy(List<object> rules);
    }
}
