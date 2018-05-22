using System;
using System.Collections.Generic;
using System.Text;


namespace Core
{
    // trida zajistujici zapouzdreni ctverce jako fyzikalniho objektu do realneho objektu (robota)
    public class Robot
    {
        public Robot()
        {
            Position = new Vector2D(0,0);
            Velocity = new Vector2D(0, 0);
            PositionMove = new Vector2D(0, 0);
            Rotation = 0;
        }

        // pozice robota
        public Vector2D Position {  get; set;} //z tehle metody bude dedit Simulator a chce tyhle vlastnosti prepsat
        
        // rychlost robota
        public Vector2D Velocity {get; set; }

        // pozice robota
        public Vector2D PositionMove {  get; set; }
        public double Rotation { get; set; }

        public void setPosition(double x, double y)
        {
            Position.X = x;
            Position.Y = y;
        }
        public void setPositionMove(double x, double y)
        {
            PositionMove.X = x;
            PositionMove.Y = y;
        }
        public void setVelocity(double left, double right)
        {
            Velocity.X = left;
            Velocity.Y = right;
        }
    }
}
