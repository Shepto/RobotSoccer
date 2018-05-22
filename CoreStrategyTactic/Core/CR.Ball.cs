using System;
using System.Collections.Generic;
using System.Text;


namespace Core
{
    // trida zajistujici zapouzdreni kruhu jako fyzikalniho objektu do realneho objektu (micku)
    public class Ball
    {
         public Ball()
        {
            Position = new Vector2D(0,0);
            Velocity = new Vector2D(0, 0);
        }
        // pozice micku
        public Vector2D Position {get; set; }
        
        // rychlost micku
        public Vector2D Velocity { get; set; }

        public void setPosition(double x, double y)
        {
            Position.X = x;
            Position.Y = y;
        }
   }
}
