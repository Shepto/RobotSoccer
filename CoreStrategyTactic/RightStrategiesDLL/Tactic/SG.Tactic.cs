using System;
using System.Collections.Generic;
using System.Text;
using Core;

namespace RightStrategiesDLL.Tactic
{
    public abstract class Tactic
    {
        protected bool mirror;
        protected GameSetting gameSetting;

        protected double one_third;   // jedna tretina hriste
        protected double field_width;     // sirka hriste i s brankami
        protected Vector2D my_goal_bottom;          // spodni bod nasi branky
        protected Vector2D my_goal_top;             // horni bod nasi branky
        protected Vector2D opp_goal_bottom;         // spodni bod protivnikovy branky
        protected Vector2D opp_goal_top;            // horni bod protivnikovy branky

        public Tactic(bool mirror, GameSetting gameSetting)
        {
            this.mirror = mirror;
            this.gameSetting = gameSetting;
            
            one_third = (gameSetting.AREA_X + gameSetting.AREA_NET_X * 2) / 3;   // jedna tretina hriste
            field_width = gameSetting.AREA_X + 2 * gameSetting.AREA_NET_X;     // sirka hriste i s brankami
       
            if (mirror)
            {

                this.opp_goal_bottom = new Vector2D(-gameSetting.AREA_X / 2, -(gameSetting.AREA_NET_Y / 2));
                this.opp_goal_top = new Vector2D(-gameSetting.AREA_X / 2, (gameSetting.AREA_NET_Y / 2));
                this.my_goal_bottom = new Vector2D(gameSetting.AREA_X / 2, -(gameSetting.AREA_NET_Y / 2));
                this.my_goal_top = new Vector2D(gameSetting.AREA_X / 2, (gameSetting.AREA_NET_Y / 2));
            }
            else
            {
                this.opp_goal_bottom = new Vector2D(gameSetting.AREA_X / 2, -(gameSetting.AREA_NET_Y / 2));
                this.opp_goal_top = new Vector2D(gameSetting.AREA_X / 2, (gameSetting.AREA_NET_Y / 2));
                this.my_goal_bottom = new Vector2D(-gameSetting.AREA_X / 2, -(gameSetting.AREA_NET_Y / 2));
                this.my_goal_top = new Vector2D(-gameSetting.AREA_X / 2, (gameSetting.AREA_NET_Y / 2));
            }

        }

        /**
         * Zrusime oznaceni robotu, kteri meli vykonat taktiku v minulem kroku
         */
        protected void robotCheck(int robot)
        {
            int count = 0;
            //foreach (Robot r in storage.MyRobots)
            //{
            //    if (count != robot)
            //        r.tactic = false;
            //    count++;
            //}
            
        }

        public abstract bool needOf(Vector2D ballPosition);
        public abstract void doTactic(Storage storage);
        protected abstract int findRobot(Storage storage);
    }
}
