using System;
using System.Collections.Generic;
using System.Text;

namespace Core
{
    // trida ktera popisuje situaci ve hre. Instance se predava mezi simulatorem, strategiemi, apod.
    public class Storage
    {
        public Ball Ball { get; set; }                 // micek
        public Robot[] LeftRobots {get; set;}            // nase roboty
        public Robot[] RightRobots { get; set; }         // roboty protivnika

        public Storage( ){
            Ball = new Ball();
            LeftRobots = new Robot[5];
            RightRobots = new Robot[5];
            for (int i = 0; i < LeftRobots.Length; i++) {
                LeftRobots[i] = new Robot();
                RightRobots[i] = new Robot();
            }

        }

        public Storage(int countOfRobot)
        {
            Ball = new Ball();
            LeftRobots = new Robot[countOfRobot];
            RightRobots = new Robot[countOfRobot];
            for (int i = 0; i < LeftRobots.Length; i++)
            {
                LeftRobots[i] = new Robot();
                RightRobots[i] = new Robot();
            }
        }

        public Storage changeRobots()
        {
            Storage newStr = new Storage();
            newStr.LeftRobots = this.RightRobots;
            newStr.RightRobots = this.LeftRobots;
            newStr.Ball = this.Ball;
            return newStr;
        }

        public void changeFromFiraCoordinates(GameSetting gs)
        {

            Vector2D vector = new Vector2D((gs.AREA_X / 2), (gs.AREA_Y / 2));
            
            
            this.Ball.Position = this.Ball.Position - vector;
            for (int i = 0; i < LeftRobots.Length; i++)
            {
                this.LeftRobots[i].Position = this.LeftRobots[i].Position - vector;
                this.RightRobots[i].Position = this.RightRobots[i].Position - vector;

                this.LeftRobots[i].PositionMove = this.LeftRobots[i].PositionMove - vector;
                this.RightRobots[i].PositionMove = this.RightRobots[i].PositionMove - vector;
            }
            
        }
        public void changeToFiraCoordinates(GameSetting gs)
        {
            Vector2D vector = new Vector2D((gs.AREA_X / 2), (gs.AREA_Y / 2));

            this.Ball.Position = this.Ball.Position + vector;
            for (int i = 0; i < LeftRobots.Length; i++)
            {
                this.LeftRobots[i].Position = this.LeftRobots[i].Position + vector;
                this.RightRobots[i].Position = this.RightRobots[i].Position + vector;

                this.LeftRobots[i].PositionMove = this.LeftRobots[i].PositionMove + vector;
                this.RightRobots[i].PositionMove = this.RightRobots[i].PositionMove + vector;
            }
            
        }

        public void print()
        {
            
            Console.WriteLine("Ball.position :" + Ball.Position.print());
            Console.WriteLine("Ball.velocity :" + Ball.Velocity.print());
            
            for (int i = 0; i < LeftRobots.Length; i++)
            {
                Console.WriteLine("MyRobot.position :" + LeftRobots[i].Position.print());
                Console.WriteLine("MyRobot.positionmove :" + LeftRobots[i].PositionMove.print());
                Console.WriteLine("MyRobot.velocity :" + LeftRobots[i].Velocity.print());

               
            }
            for (int i = 0; i < RightRobots.Length; i++)
            {
                Console.WriteLine("OppRobot.position :" + RightRobots[i].Position.print());
                Console.WriteLine("OppRobot.positionmove :" + RightRobots[i].PositionMove.print());
                Console.WriteLine("OppRobot.velocity :" + RightRobots[i].Velocity.print());
            }
        }
    }
}
