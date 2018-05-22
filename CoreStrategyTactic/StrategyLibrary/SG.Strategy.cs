using System;
using System.Collections.Generic;
using System.Text;

using Core;

namespace StrategyLibrary
{
    // obecna trida definujici zakladni prvky strategie vcetne inicializace vsech promennych
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

        public abstract void AdaptStrategy(List<GridRule> rules);
    }
}
