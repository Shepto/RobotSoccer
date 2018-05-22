using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using Core;

namespace StrategyLibrary
{
    // trida predstavujici pravidlo, definujici ze ktereho na ktery grid se maji roboti pohnout
    public class GridRule
    {
        private List<int> zMine;
        private List<int> zOppnt;

        private String name;        // jmeno pravidla
        private int number;         // cislo pravidla
        private RuleType type;      // typ pravidla (defenzivni - ofenzivni)
        private Vector2D[] mine;    // souradnice hracovych robotu
        private Vector2D[] oppnt;   // souradnice protivnikovych robotu
        private Vector2D ball;      // souradnice micku
        private Vector2D[] move;    // souradnice mist, kam se maji hracovy roboti presunout

        public List<int> ZMine
        {
            get { return zMine; }
            set { zMine = value; }
        }

        public List<int> ZOppnt
        {
            get { return zOppnt; }
            set { zOppnt = value; }
        }

        public String Name
        {
            get { return name; }
            set { name = value; }
        }
        public int Number
        {
            get { return number; }
            set { number = value; }
        }
        public RuleType Type
        {
            get { return type; }
            set { type = value; }
        }
        public Vector2D[] Mine
        {
            get { return mine; }
            set { mine = value; }
        }
        public Vector2D[] Oppnt
        {
            get { return oppnt; }
            set { oppnt = value; }
        }
        public Vector2D Ball
        {
            get { return ball; }
            set { ball = value; }
        }
        public Vector2D[] Move
        {
            get { return move; }
            set { move = value; }
        }
        public enum RuleType { Offense, Deffense };

        public GridRule()
        {
            mine = new Vector2D[4];
            oppnt = new Vector2D[4];
            move = new Vector2D[4];
        }

        // konstruktor zajistujici spravne rozparsrovani vsech dat ze souboru a nacteni do promennych
        public GridRule(String line1, String line2, String line3, String line4, String line5)
        {
            CultureInfo nfi = new CultureInfo("en-US", false);

            String[] s_line1 = getValue(line1, "Rule").Split(new char[] {' '});
            this.number = Int32.Parse(s_line1[0]);
            this.type = (s_line1[1]=="o") ? RuleType.Offense : RuleType.Deffense;
            this.name = "";
            for (int i = 2; i < s_line1.Length; i++)
                this.name += s_line1[i] + " ";
            this.name = this.name.Substring(0, this.name.Length - 1);

            String[] s_line2 = getValue(line2, "Mine").Split(new char[] { ' ', ',' });
            mine = new Vector2D[] { 
                new Vector2D(Convert.ToDouble(s_line2[0], nfi), Convert.ToDouble(s_line2[1], nfi)), 
                new Vector2D(Convert.ToDouble(s_line2[3], nfi), Convert.ToDouble(s_line2[4], nfi)), 
                new Vector2D(Convert.ToDouble(s_line2[6], nfi), Convert.ToDouble(s_line2[7], nfi)), 
                new Vector2D(Convert.ToDouble(s_line2[9], nfi), Convert.ToDouble(s_line2[10], nfi)) };
            String[] s_line3 = getValue(line3, "Oppnt").Split(new char[] { ' ', ',' });
            oppnt = new Vector2D[] { 
                new Vector2D(Convert.ToDouble(s_line3[0], nfi), Convert.ToDouble(s_line3[1], nfi)), 
                new Vector2D(Convert.ToDouble(s_line3[3], nfi), Convert.ToDouble(s_line3[4], nfi)), 
                new Vector2D(Convert.ToDouble(s_line3[6], nfi), Convert.ToDouble(s_line3[7], nfi)), 
                new Vector2D(Convert.ToDouble(s_line3[9], nfi), Convert.ToDouble(s_line3[10], nfi)) };
            String[] s_line4 = getValue(line4, "Ball").Split(new char[] { ' ', ',' });
            ball = new Vector2D(Convert.ToDouble(s_line4[0], nfi), Convert.ToDouble(s_line4[1], nfi));
            String[] s_line5 = getValue(line5, "Move").Split(new char[] { ' ', ',' });
            move = new Vector2D[] { 
                new Vector2D(Convert.ToDouble(s_line5[0], nfi), Convert.ToDouble(s_line5[1], nfi)), 
                new Vector2D(Convert.ToDouble(s_line5[3], nfi), Convert.ToDouble(s_line5[4], nfi)), 
                new Vector2D(Convert.ToDouble(s_line5[6], nfi), Convert.ToDouble(s_line5[7], nfi)), 
                new Vector2D(Convert.ToDouble(s_line5[9], nfi), Convert.ToDouble(s_line5[10], nfi)) };

            //seradit az po pripadnem mirroringu
            //fillZOrder();
        }

        // funkce pro jednodussi vytahnuti hodnoty promenne z textu
        private String getValue(String line, String var)
        {
            line = line.Replace("." + var, "");
            line = line.Replace("\"", "");
            line = line.Trim();
            return line;
        }

        // zformatovani a ulozeni pravdila do textove podoby
        public override string ToString()
        {
            String line1 = String.Format(".Rule\t{0} {1} {2}", number, (type == RuleType.Offense) ? "o" : "d", name);
            String line2 = String.Format(".Mine\t{0},{1}  {2},{3}  {4},{5}  {6},{7}", mine[0].X, mine[0].Y, mine[1].X, mine[1].Y, mine[2].X, mine[2].Y, mine[3].X, mine[3].Y);
            String line3 = String.Format(".Oppnt\t{0},{1}  {2},{3}  {4},{5}  {6},{7}", oppnt[0].X, oppnt[0].Y, oppnt[1].X, oppnt[1].Y, oppnt[2].X, oppnt[2].Y, oppnt[3].X, oppnt[3].Y);
            String line4 = String.Format(".Ball\t{0},{1}", ball.X, ball.Y);
            String line5 = String.Format(".Move\t{0},{1}  {2},{3}  {4},{5}  {6},{7}", move[0].X, move[0].Y, move[1].X, move[1].Y, move[2].X, move[2].Y, move[3].X, move[3].Y);
            return line1 + "\r\n" + line2 + "\r\n" + line3 + "\r\n" + line4 + "\r\n" + line5 + "\r\n" + "\r\n";
        }

        // funkce zajistujici zrdcadlove otoceni souradnic pravidla (dulezita pro hrace hrajiciho na prave strane)
        public GridRule Mirror(int x_grids)
        {
            for (int i=0; i<4; i++)
            {
                mine[i] = new Vector2D(x_grids - mine[i].X + 1, mine[i].Y);
                oppnt[i] = new Vector2D(x_grids - oppnt[i].X + 1, oppnt[i].Y);
                move[i] = new Vector2D(x_grids - move[i].X + 1, move[i].Y);
            }
            ball = new Vector2D(x_grids - ball.X + 1, ball.Y);
            return this;
        }

        //naplneni ZOrder
        public void fillZOrder()
        {
            ZOrder zord = new ZOrder();
            int x, y;
            List<int>[,] matrix;

            matrix = new List<int>[6, 4];
            for (int i = 0; i < mine.Length; i++)
            {
                x = Convert.ToInt32(mine[i].X) - 1;
                y = Convert.ToInt32(mine[i].Y) - 1;
                if (matrix[x, y] != null)
                    matrix[x, y].Add(i);
                else matrix[x, y] = new List<int>() { i };
            }
            zMine = zord.zOrderMain(matrix, 2);

            matrix = new List<int>[6, 4];
            for (int i = 0; i < oppnt.Length; i++)
            {
                x = Convert.ToInt32(oppnt[i].X) - 1;
                y = Convert.ToInt32(oppnt[i].Y) - 1;
                if (matrix[x, y] != null)
                    matrix[x, y].Add(i);
                else matrix[x, y] = new List<int>() { i };
            }
            zOppnt = zord.zOrderMain(matrix, 2);
        }
    }
}
