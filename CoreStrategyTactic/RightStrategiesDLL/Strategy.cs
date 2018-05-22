using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using Core;


namespace RightStrategiesDLL
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

        // tik strategie urcujici co se ma pri kazdem kroku vykonat
        public abstract void TickStrategy(Storage storage);

        //zmenene na object
        //public abstract void AdaptStrategy(List<GridRule> rules);
        public abstract void AdaptStrategy(List<object> rules);
    }
}
