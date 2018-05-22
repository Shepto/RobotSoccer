using System;
using System.Collections.Generic;
using System.Text;

namespace StrategiesDLL.Regresion
{
    public class RobotDistance
    {
        public Bod bod;
        public double vzdalenost;

        public RobotDistance()
        {
        }

        public RobotDistance(Bod bod, double vzdalenost)
        {
            this.bod = bod;
            this.vzdalenost = vzdalenost;
        }
    }
}
