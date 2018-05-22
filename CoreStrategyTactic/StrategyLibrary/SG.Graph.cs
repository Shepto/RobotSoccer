using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Core;
using System.IO;

namespace StrategyLibrary
{
    public class Node
    {
        public Dictionary<int, double> neighbours;
        public int key;
        public object data;

        public Node(int key, object data)
        {
            this.key = key;
            this.data = data;
            neighbours = new Dictionary<int, double>();
        }

        public void AddNeighbour(int key, double cost)
        {
            neighbours.Add(key, cost);
        }
    }

    public class NodeList
    {
        private Hashtable data = new Hashtable();

        public virtual void Add(Node n)
        {
            data.Add(n.key, n);
        }

        public virtual void Remove(Node n)
        {
            data.Remove(n.key);
        }

        public virtual int getCount()
        {
            return data.Count;
        }

        public virtual bool ContainsKey(int key)
        {
            return data.ContainsKey(key);
        }

        public virtual void Clear()
        {
            data.Clear();
        }

        public virtual Node this[int key]
        {
            get
            {
                return (Node)data[key];
            }
        }
    }

    public class Graph
    {
        struct Triple
        {
            private int xVal;
            private int yVal;
            private double zVal;

            public int x
            {
                get
                {
                    return xVal;
                }
                set
                {
                    xVal = value;
                }
            }

            public int y
            {
                get
                {
                    return yVal;
                }
                set
                {
                    yVal = value;
                }
            }

            public double z
            {
                get
                {
                    return zVal;
                }
                set
                {
                    zVal = value;
                }
            }

            public Triple(int x, int y, double z)
            {
                this.xVal = x;
                this.yVal = y;
                this.zVal = z;
            }
        }

        private NodeList nodes;

        public Graph()
        {
            this.nodes = new NodeList();
        }

        public virtual void AddNode(Node n)
        {
            if (!nodes.ContainsKey(n.key))
                nodes.Add(n);
            else throw new ArgumentException("There already exists a node in the graph with key " + n.key);
        }

        public virtual void AddEdge(Node u, Node v, double cost)
        {
            if (nodes.ContainsKey(u.key) && nodes.ContainsKey(v.key))
            {
                u.AddNeighbour(v.key, cost);
                v.AddNeighbour(u.key, cost);
            }
            else throw new ArgumentException("One or both of the nodes were not members of the graph.");
        }

        public virtual bool Contains(Node n)
        {
            return Contains(n.key);
        }

        public virtual bool Contains(int key)
        {
            return nodes.ContainsKey(key);
        }

        public virtual NodeList Nodes
        {
            get
            {
                return this.nodes;
            }
        }

        public void Fill(GridStrategy gStrategy, GameSetting gameSetting, double _treshold)
        {
            StringBuilder sb = new StringBuilder();

            //nodes
            for (int i = 0; i < gStrategy.Rules.Count; i++)
            {
                AddNode(new Node(i, gStrategy.Rules[i]));
            }

            //edges
            double max = -1;
            List<Triple> values = new List<Triple>();
            for (int i = 0; i < gStrategy.Rules.Count - 1; i++)
            {
                for (int j = i + 1; j < gStrategy.Rules.Count; j++)
                {
                    GridRule r1 = gStrategy.Rules[i];
                    GridRule r2 = gStrategy.Rules[j];

                    //ball
                    double ballRuleSize = gStrategy.GridToReal(r1.Ball, gameSetting).DistanceFrom(
                            gStrategy.GridToReal(r2.Ball, gameSetting)
                        );

                    //mine
                    double mineSize = 0;
                    for (int k = 0; k < gameSetting.NUMBER_OF_ROBOTS - 1; k++)
                        mineSize += gStrategy.GridToReal(r1.Mine[r1.ZMine[k]], gameSetting).DistanceFrom(
                                gStrategy.GridToReal(r2.Mine[r2.ZMine[k]], gameSetting)
                            );

                    //oppnt
                    double oppntSize = 0;
                    for (int k = 0; k < gameSetting.NUMBER_OF_ROBOTS - 1; k++)
                        oppntSize += gStrategy.GridToReal(r1.Oppnt[r1.ZOppnt[k]], gameSetting).DistanceFrom(
                                gStrategy.GridToReal(r2.Oppnt[r2.ZOppnt[k]], gameSetting)
                            );

                    if (ballRuleSize + mineSize + oppntSize > max)
                        max = ballRuleSize + mineSize + oppntSize;

                    values.Add(new Triple(i, j, ballRuleSize + mineSize + oppntSize));
                }
            }

            for (int i = 0; i < values.Count; i++)
            {
                double norm = 1 - (values[i].z / max);
                //Console.WriteLine("{0} {1} {2} {3}", values[i].x, values[i].y, values[i].z, norm);

                if (norm >= _treshold)
                {
                    AddEdge(nodes[values[i].x], nodes[values[i].y], norm);
                    sb.AppendLine(values[i].x + " " + values[i].y + " " + Math.Round(norm, 3).ToString().Replace(',', '.'));
                }
            }

            /**
            string filename = "GRAPH_" + gStrategy.Name + ".txt";
            File.Delete(filename);
            using (StreamWriter sw = new StreamWriter(new FileStream(filename, FileMode.CreateNew)))
            {
                sw.Write(sb.ToString());
                sw.Close();
            }
            */
        }

        public void Fill_LeftStrg31(GridStrategy gStrategy, GameSetting gameSetting)
        {
            //double _treshold = 0.7;
            double _treshold = 0;

            StringBuilder sb = new StringBuilder();

            //nodes
            for (int i = 0; i < gStrategy.Rules.Count; i++)
            {
                AddNode(new Node(i, gStrategy.Rules[i]));
            }

            GridStrategy offensiveM = new GridStrategy();
            offensiveM.Rules.Add(gStrategy.Rules[0]);
            offensiveM.Rules.Add(gStrategy.Rules[1]);
            offensiveM.Rules.Add(gStrategy.Rules[2]);
            offensiveM.Rules.Add(gStrategy.Rules[3]);
            GridStrategy offensiveR = new GridStrategy();
            offensiveR.Rules.Add(gStrategy.Rules[4]);
            offensiveR.Rules.Add(gStrategy.Rules[5]);
            offensiveR.Rules.Add(gStrategy.Rules[6]);
            offensiveR.Rules.Add(gStrategy.Rules[7]);
            offensiveR.Rules.Add(gStrategy.Rules[8]);
            offensiveR.Rules.Add(gStrategy.Rules[9]);
            GridStrategy offensiveL = new GridStrategy();
            offensiveL.Rules.Add(gStrategy.Rules[10]);
            offensiveL.Rules.Add(gStrategy.Rules[11]);
            offensiveL.Rules.Add(gStrategy.Rules[12]);
            offensiveL.Rules.Add(gStrategy.Rules[13]);
            offensiveL.Rules.Add(gStrategy.Rules[14]);
            offensiveL.Rules.Add(gStrategy.Rules[15]);
            GridStrategy defensiveM = new GridStrategy();
            defensiveM.Rules.Add(gStrategy.Rules[16]);
            defensiveM.Rules.Add(gStrategy.Rules[17]);
            defensiveM.Rules.Add(gStrategy.Rules[18]);
            defensiveM.Rules.Add(gStrategy.Rules[19]);
            GridStrategy defensiveR = new GridStrategy();
            defensiveR.Rules.Add(gStrategy.Rules[20]);
            defensiveR.Rules.Add(gStrategy.Rules[21]);
            defensiveR.Rules.Add(gStrategy.Rules[22]);
            defensiveR.Rules.Add(gStrategy.Rules[23]);
            defensiveR.Rules.Add(gStrategy.Rules[24]);
            defensiveR.Rules.Add(gStrategy.Rules[25]);
            GridStrategy defensiveL = new GridStrategy();
            defensiveL.Rules.Add(gStrategy.Rules[26]);
            defensiveL.Rules.Add(gStrategy.Rules[27]);
            defensiveL.Rules.Add(gStrategy.Rules[28]);
            defensiveL.Rules.Add(gStrategy.Rules[29]);
            defensiveL.Rules.Add(gStrategy.Rules[30]);

            List<GridStrategy> lStrategy = new List<GridStrategy>();
            lStrategy.Add(offensiveM);
            lStrategy.Add(offensiveR);
            lStrategy.Add(offensiveL);
            lStrategy.Add(defensiveM);
            lStrategy.Add(defensiveR);
            lStrategy.Add(defensiveL);
           
            //edges
            double max = -1;
            List<Triple> values = new List<Triple>();
            
            foreach (GridStrategy tempStrg in lStrategy)
            { 
                for (int i = 0; i < tempStrg.Rules.Count - 1; i++)
                {
                    for (int j = i + 1; j < tempStrg.Rules.Count; j++)
                    {
                        GridRule r1 = tempStrg.Rules[i];
                        GridRule r2 = tempStrg.Rules[j];

                        //ball
                        double ballRuleSize = gStrategy.GridToReal(r1.Ball, gameSetting).DistanceFrom(
                                gStrategy.GridToReal(r2.Ball, gameSetting)
                            );

                        //mine
                        double mineSize = 0;
                        for (int k = 0; k < gameSetting.NUMBER_OF_ROBOTS - 1; k++)
                            mineSize += gStrategy.GridToReal(r1.Mine[r1.ZMine[k]], gameSetting).DistanceFrom(
                                    gStrategy.GridToReal(r2.Mine[r2.ZMine[k]], gameSetting)
                                );

                        //oppnt
                        double oppntSize = 0;
                        for (int k = 0; k < gameSetting.NUMBER_OF_ROBOTS - 1; k++)
                            oppntSize += gStrategy.GridToReal(r1.Oppnt[r1.ZOppnt[k]], gameSetting).DistanceFrom(
                                    gStrategy.GridToReal(r2.Oppnt[r2.ZOppnt[k]], gameSetting)
                                );

                        if (ballRuleSize + mineSize + oppntSize > max)
                            max = ballRuleSize + mineSize + oppntSize;

                        int ii = gStrategy.Rules.IndexOf(tempStrg.Rules[i]);
                        int ij = gStrategy.Rules.IndexOf(tempStrg.Rules[j]);

                        values.Add(new Triple(ii, ij, ballRuleSize + mineSize + oppntSize));
                    }
                }
            }

            for (int i = 0; i < values.Count; i++)
            {
                double norm = 1 - (values[i].z / max);
                sb.AppendLine(values[i].x + " " + values[i].y + " " + Math.Round(norm, 3).ToString().Replace(',', '.'));
                //Console.WriteLine("{0} {1} {2} {3}", values[i].x, values[i].y, values[i].z, norm);

                if (norm >= _treshold)
                    AddEdge(nodes[values[i].x], nodes[values[i].y], norm);
            }

            string filename = "GRAPH_" + gStrategy.Name + ".txt";
            File.Delete(filename);
            using (StreamWriter sw = new StreamWriter(new FileStream(filename, FileMode.CreateNew)))
            {
                sw.Write(sb.ToString());
            }
        }

        public void PrintGraph()
        {
            Console.WriteLine("-----PrintGraph-----");
            for (int i = 0; i < nodes.getCount(); i++)
            {
                foreach (int k in nodes[i].neighbours.Keys)
                {
                    Console.WriteLine("node {0}, neighbour {1}, cost {2}", i + 1, k + 1, nodes[i].neighbours[k]);
                }
            }
        }
    }
}
