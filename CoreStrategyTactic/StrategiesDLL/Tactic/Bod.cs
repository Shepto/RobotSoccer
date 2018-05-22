using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StrategiesDLL.Regresion
{
    public class Bod
    {
        public double x;// { get { return x; } set { x = value; } }
        public double y;// { get { return y; } set { y = value; } }
        public Bod()
        {
            x = 0;
            y = 0;
        }
        public Bod(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
