using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core
{
    public class StrategyStorage
    {
        public Ball Ball { get; set; }                 // micek
        public Robot[] myRobots { get; set; }            // nase roboty
        public Robot[] oppntRobots { get; set; }         // roboty protivnika

        public StrategyStorage()
        {
            Ball = new Ball();
            myRobots = new Robot[5];
            oppntRobots = new Robot[5];

            for (int i = 0; i < myRobots.Length; i++)
            {
                myRobots[i] = new Robot();
                oppntRobots[i] = new Robot();
            }
        }


        public StrategyStorage(int countOfRobot)
        {
            Ball = new Ball();
            myRobots = new Robot[countOfRobot];
            oppntRobots = new Robot[countOfRobot];
            for (int i = 0; i < myRobots.Length; i++)
            {
                myRobots[i] = new Robot();
                oppntRobots[i] = new Robot();
            }
        }

        public StrategyStorage changeRobots()
        {
            StrategyStorage newStr = new StrategyStorage();
            newStr.myRobots = this.oppntRobots;
            newStr.oppntRobots = this.myRobots;
            newStr.Ball = this.Ball;
            return newStr;
        }


        public void changeFromFiraCoordinates(GameSetting gs)
        {

            Vector2D vector = new Vector2D((gs.AREA_X / 2), (gs.AREA_Y / 2));


            this.Ball.Position = this.Ball.Position - vector;
            for (int i = 0; i < myRobots.Length; i++)
            {
                this.myRobots[i].Position = this.myRobots[i].Position - vector;
                this.oppntRobots[i].Position = this.oppntRobots[i].Position - vector;

                this.myRobots[i].PositionMove = this.myRobots[i].PositionMove - vector;
                this.oppntRobots[i].PositionMove = this.oppntRobots[i].PositionMove - vector;
            }
        }

        public void changeToFiraCoordinates(GameSetting gs)
        {
            Vector2D vector = new Vector2D((gs.AREA_X / 2), (gs.AREA_Y / 2));

            this.Ball.Position = this.Ball.Position + vector;
            for (int i = 0; i < myRobots.Length; i++)
            {
                this.myRobots[i].Position = this.myRobots[i].Position + vector;
                this.oppntRobots[i].Position = this.oppntRobots[i].Position + vector;

                this.myRobots[i].PositionMove = this.myRobots[i].PositionMove + vector;
                this.oppntRobots[i].PositionMove = this.oppntRobots[i].PositionMove + vector;
            }
        }

        public void print()
        {

            Console.WriteLine("Ball.position :" + Ball.Position.print());
            Console.WriteLine("Ball.velocity :" + Ball.Velocity.print());

            for (int i = 0; i < myRobots.Length; i++)
            {
                Console.WriteLine("MyRobot.position :" + myRobots[i].Position.print());
                Console.WriteLine("MyRobot.positionmove :" + myRobots[i].PositionMove.print());
                Console.WriteLine("MyRobot.velocity :" + myRobots[i].Velocity.print());


            }
            for (int i = 0; i < oppntRobots.Length; i++)
            {
                Console.WriteLine("OppRobot.position :" + oppntRobots[i].Position.print());
                Console.WriteLine("OppRobot.positionmove :" + oppntRobots[i].PositionMove.print());
                Console.WriteLine("OppRobot.velocity :" + oppntRobots[i].Velocity.print());
            }
        }

    }
}
