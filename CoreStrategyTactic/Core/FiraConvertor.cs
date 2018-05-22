using System;
using System.Collections.Generic;
using System.Text;

namespace Core
{
    public class FiraConvertor
    {
        public Storage setRobots(Storage storage)
        {
            for (int i = 0; i < storage.LeftRobots.Length; i++)
            {
                setRobot(storage.LeftRobots[i]);
            }
            return storage;
        }
        public Robot setRobot(Robot robot)
        {
            Vector2D x = new Vector2D(10, 0);
            double rychlost = 70;
            if (robot.Position!=robot.PositionMove)
            {
                Vector2D vCil = robot.PositionMove - robot.Position; //cilovy vektor
                double uCil = x.Angle(vCil); //uhel mezi osouX a cilovym vektorem
                if (vCil.Y < 0) uCil = uCil * (-1);
                double uhel = robot.Rotation - uCil; //uhel mezi natočením robota a smerem kam ma jet (cilovy vektor)

                if (uhel > 0)
                {
                    if (uhel < 45) robot.setVelocity(rychlost, rychlost - uhel);
                    else robot.setVelocity(rychlost/2, (rychlost/2 * (-1)));
                }
                else
                {
                    if (uhel > -45) robot.setVelocity(rychlost + uhel, rychlost);
                    else robot.setVelocity((rychlost/2 * (-1)), rychlost/2);
                }
            }
            else robot.setVelocity(0, 0);
            return robot;
        }
    }
}
