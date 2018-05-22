using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using Core;
using System.IO;
using System.Reflection;

public class PositionsHolder
    {
        public PositionsHolder(int lr, int rr, Vector2D ball, Vector2D r1, Vector2D r2, Vector2D r3, Vector2D r4, Vector2D r5, Vector2D l1, Vector2D l2, Vector2D l3, Vector2D l4, Vector2D l5, string score, string time, int control) 
        {
            ballPosition = new Vector2D(0.0, 0.0);
            leftPlayerRobots = new Vector2D[5];
            rightPlayerRobots = new Vector2D[5];

            lRule = lr;
            rRule = rr;
            ballPosition = ball;

            leftPlayerRobots[0] = l1;
            leftPlayerRobots[1] = l2;
            leftPlayerRobots[2] = l3;
            leftPlayerRobots[3] = l4;
            leftPlayerRobots[4] = l5;

            rightPlayerRobots[0] = r1;
            rightPlayerRobots[1] = r2;
            rightPlayerRobots[2] = r3;
            rightPlayerRobots[3] = r4;
            rightPlayerRobots[4] = r5;

            this.score = score;
            this.time = time;
            this.control = control;
        }

        public int lRule;
        public int rRule;
        public Vector2D ballPosition;
        public Vector2D[] leftPlayerRobots;
        public Vector2D[] rightPlayerRobots;
        public string score;
        public string time;
        public int control;
    }

    

public class LogHolder
    {
        public List<PositionsHolder> positions;


    public LogHolder()
        {
            positions = new List<PositionsHolder>(); 
        }

        public void LogToTextFileFromHolder(string _logFile)
        {
            string filename = "Logs\\" + _logFile + ".log";
            using (StreamWriter sw = new StreamWriter(filename))
            {
                sw.Write("leftRule;");
                sw.Write("rightRule;");
                sw.Write("ball.x;");
                sw.Write("ball.y;");
                for (int i = 0; i < 5; i++)
                {
                    sw.Write("lr" + i + ".x;");
                    sw.Write("lr" + i + ".y;");
                }
                for (int i = 0; i < 5; i++)
                {
                    sw.Write("rr" + i + ".x;");
                    sw.Write("rr" + i + ".y;");
                }
                sw.Write("score;");
                sw.Write("time;");
                sw.Write("control");
                sw.WriteLine();

                foreach (PositionsHolder p in this.positions)
                {
                    sw.Write(p.lRule + ";");
                    sw.Write(p.rRule + ";");
                    sw.Write(p.ballPosition.X + ";");
                    sw.Write(p.ballPosition.Y + ";");
                    for (int i = 0; i < 5; i++)
                    {
                        sw.Write(p.leftPlayerRobots[i].X + ";");
                        sw.Write(p.leftPlayerRobots[i].Y + ";");
                    }
                    for (int i = 0; i < 5; i++)
                    {
                        sw.Write(p.rightPlayerRobots[i].X + ";");
                        sw.Write(p.rightPlayerRobots[i].Y + ";");
                    }
                    sw.Write(p.score + ";");
                    sw.Write(p.time + ";");
                    sw.Write(p.control);
                    sw.WriteLine();
                }
            }
            this.positions.Clear();
        }

        public void LogToXmlFileFromHolder(string _logFile)
        {
            string filename = _logFile + ".xml";
            using (StreamWriter sw = new StreamWriter(filename))
            {
                sw.WriteLine("<?xml version=" + "\"1.0\"" + " encoding=" + "\"utf-8\"" + "?>");
                sw.WriteLine("<ticks>");
                foreach (PositionsHolder p in this.positions)
                {
                    sw.WriteLine("<tick>");

                    sw.WriteLine("<ruleL>");
                    sw.WriteLine(p.lRule);
                    sw.WriteLine("</ruleL>");
                    sw.WriteLine("<ruleR>");
                    sw.WriteLine(p.rRule);
                    sw.WriteLine("</ruleR>");

                    sw.WriteLine("<ball>");
                    WriteVector(sw, p.ballPosition);
                    sw.WriteLine("</ball>");
                    
                    for (int i = 0; i < 5; i++)
                    {
                        sw.WriteLine("<robotL>");
                        WriteVector(sw, p.leftPlayerRobots[i]);
                        sw.WriteLine("</robotL>");
                    }
                    for (int i = 0; i < 5; i++)
                    {
                        sw.WriteLine("<robotR>");
                        WriteVector(sw, p.rightPlayerRobots[i]);
                        sw.WriteLine("</robotR>");
                    }
                    sw.WriteLine("</tick>");
                }
                sw.WriteLine("</ticks>");
            }
            this.positions.Clear();
        }

            private void WriteVector(StreamWriter sw, Vector2D vector2D)
            {
                sw.WriteLine("\t<x>" + vector2D.X + "</x>");
                sw.WriteLine("\t<y>" + vector2D.Y + "</y>");
            }
    }

