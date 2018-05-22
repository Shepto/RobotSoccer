using System;
using System.Collections.Generic;
using System.Text;

namespace Core
{
    public class GameSetting
    {
        public int NUMBER_OF_ROBOTS {get; set;}     // pocet robotu
        public double AREA_X {get; set;}            // sirka hriste
        public double AREA_Y { get; set; }            // vyska hriste
        public double AREA_NET_X { get; set; }        // hloubka branky
        public double AREA_NET_Y { get; set; }        // delka branky
        public double ROBOT_EDGE_SIZE { get; set; }   // velikost hrany robota
        public double ROBOT_MAX_VEL { get; set; }     // maximalni rychlost robota
        public double timestep { get; set; }          // tick
        public double BALL_FRICTION { get; set; }     // treni micku
    }
}
