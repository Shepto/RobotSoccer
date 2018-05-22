using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core;
using System.Globalization;
using System.IO;

namespace StrategiesDLL
{
    public class GridStrategy
    {
        private String name;                // jmeno strategie
        private String algorithm;           // algoritmus strategie
        private String author;              // autor strategie
        private String date;                // datum strategie
        private Vector2D size;              // rozmer mrizky nad kterou jsou pravidla vytvorena
        private int attackerMin;            // vzdalenost (v gridech) urcujici, jak daleko muze byt max. micek od utocnika, aby zacal utocnik utocit
        private int ruleException;          // cislo, urujici kolik minimalne robotu musi byt na svych cilovych pozicich (danych pravidlem) aby se mohlo zacit pouzivat jine pravidlo
        private int[] priorityMine;         // pole, urcujici jak moc nas bude zajimat kazdy z nasich robotu pri utoceni
        private int[] priorityMineD;        // pole, urcujici jak moc nas bude zajimat kazdy z nasich robotu pri braneni
        private int[] priorityOpponent;     // pole, urcujici jak moc nas bude zajimat kazdy z protivnikovych robotu pri utoceni
        private int[] priorityOpponentD;    // pole, urcujici jak moc nas bude zajimat kazdy z protivnikovych robotu pri braneni
        private int priorityBall;           // hodnota urcujici jak vyznamne pro rozhodnuti pro nas bude pozice micku
        private List<GridRule> rules;       // kolekce pravidel dane strategie

        public String Name
        {
            get { return name; }
            set { name = value; }
        }
        public String Algorithm
        {
            get { return algorithm; }
            set { algorithm = value; }
        }
        public String Author
        {
            get { return author; }
            set { author = value; }
        }
        public String Date
        {
            get { return date; }
            set { date = value; }
        }
        public Vector2D Size
        {
            get { return size; }
            set { size = value; }
        }
        public int AttackerMin
        {
            get { return attackerMin; }
            set { attackerMin = value; }
        }
        public int RuleException
        {
            get { return ruleException; }
            set { ruleException = value; }
        }
        public int[] PriorityMine
        {
            get { return priorityMine; }
            set { priorityMine = value; }
        }
        public int[] PriorityMineD
        {
            get { return priorityMineD; }
            set { priorityMineD = value; }
        }
        public int[] PriorityOpponent
        {
            get { return priorityOpponent; }
            set { priorityOpponent = value; }
        }
        public int[] PriorityOpponentD
        {
            get { return priorityOpponentD; }
            set { priorityOpponentD = value; }
        }
        public int PriorityBall
        {
            get { return priorityBall; }
            set { priorityBall = value; }
        }
        public List<GridRule> Rules
        {
            get { return rules; }
            set { rules = value; }
        }

        public GridStrategy()
        {
            rules = new List<GridRule>();
        }

        // konstruktor zajistujici spravne rozparsrovani vsech dat ze souboru a nacteni do promennych a pripadne x-ove preklopeni pravidel
        public GridStrategy(String fileName, bool mirror)
        {
            CultureInfo nfi = new CultureInfo("en-US", false);

            StreamReader reader = new StreamReader(new FileStream(fileName, FileMode.Open));

            this.name = getValue(reader.ReadLine(), "Strategy");
            this.algorithm = getValue(reader.ReadLine(), "Algorithm");
            this.author = getValue(reader.ReadLine(), "Author");
            this.date = getValue(reader.ReadLine(), "Date");
            String[] s_size = getValue(reader.ReadLine(), "Size").Split(new char[] { ',' });
            this.size = new Vector2D(Convert.ToDouble(s_size[0], nfi), Convert.ToDouble(s_size[1], nfi));
            this.attackerMin = Int32.Parse(getValue(reader.ReadLine(), "AttackerMin"));
            this.ruleException = Int32.Parse(getValue(reader.ReadLine(), "RuleException"));
            String[] s_priorityMine = getValue(reader.ReadLine(), "PriorityMine").Split(new char[] { ' ', '\t' });
            this.priorityMine = new int[] { Int32.Parse(s_priorityMine[0]), Int32.Parse(s_priorityMine[1]), Int32.Parse(s_priorityMine[2]), Int32.Parse(s_priorityMine[3]) };
            String[] s_priorityMineD = getValue(reader.ReadLine(), "PriorityMineD").Split(new char[] { ' ', '\t' });
            this.priorityMineD = new int[] { Int32.Parse(s_priorityMineD[0]), Int32.Parse(s_priorityMineD[1]), Int32.Parse(s_priorityMineD[2]), Int32.Parse(s_priorityMineD[3]) };
            String[] s_priorityOpponent = getValue(reader.ReadLine(), "PriorityOpponent").Split(new char[] { ' ', '\t' });
            this.priorityOpponent = new int[] { Int32.Parse(s_priorityOpponent[0]), Int32.Parse(s_priorityOpponent[1]), Int32.Parse(s_priorityOpponent[2]), Int32.Parse(s_priorityOpponent[3]) };
            String[] s_priorityOpponentD = getValue(reader.ReadLine(), "PriorityOpponentD").Split(new char[] { ' ', '\t' });
            this.priorityOpponentD = new int[] { Int32.Parse(s_priorityOpponentD[0]), Int32.Parse(s_priorityOpponentD[1]), Int32.Parse(s_priorityOpponentD[2]), Int32.Parse(s_priorityOpponentD[3]) };
            this.priorityBall = Int32.Parse(getValue(reader.ReadLine(), "PriorityBall"));

            rules = new List<GridRule>();
            try
            {
                while (!reader.EndOfStream)
                {
                    reader.ReadLine();
                    GridRule r = new GridRule(reader.ReadLine(), reader.ReadLine(), reader.ReadLine(), reader.ReadLine(), reader.ReadLine());
                    if (mirror)
                        r.Mirror((int)size.X);
                    r.fillZOrder();
                    rules.Add(r);
                }
            }
            catch (Exception) { }
            reader.Close();
        }

        // funkce pro jednodussi vytahnuti hodnoty promenne z textu
        private String getValue(String line, String var)
        {
            line = line.Replace("." + var, "");
            line = line.Replace("\"", "");
            line = line.Trim();
            return line;
        }

        // funkce pro prevod souradnice gridu na realnou souradnici
        public Vector2D GridToReal(Vector2D gridPoint, GameSetting gameSetting)
        {
            double x = gridPoint.X * (gameSetting.AREA_X / size.X) - gameSetting.AREA_X / 2 - (gameSetting.AREA_X / size.X) / 2;
            double y = gridPoint.Y * (gameSetting.AREA_Y / size.Y) - gameSetting.AREA_Y / 2 - (gameSetting.AREA_Y / size.Y) / 2;
            return new Vector2D(Math.Round(x, 2), Math.Round(y, 2));
        }

        // funkce pro prevod realne souradnice na souradnici gridu
        public Vector2D RealToGrid(Vector2D realPoint, GameSetting gameSetting)
        {
            double x = (realPoint.X + gameSetting.AREA_X / 2) / (gameSetting.AREA_X / size.X);
            double y = (realPoint.Y + gameSetting.AREA_Y / 2) / (gameSetting.AREA_Y / size.Y);
            return new Vector2D(Math.Ceiling(x) - 1, Math.Ceiling(y) - 1);
        }

        // zformatovani a ulozeni pravdila do textove podoby
        public override string ToString()
        {
            String line1 = String.Format(".Strategy\t\t\"{0}\"", name);
            String line2 = String.Format(".Algorithm\t\t\"{0}\"", algorithm);
            String line3 = String.Format(".Author\t\t\t\"{0}\"", author);
            String line4 = String.Format(".Date\t\t\t\"{0}\"", date);
            String line5 = String.Format(".Size\t\t\t{0},{1}", size.X, size.Y);
            String line6 = String.Format(".AttackerMin\t\t{0}", attackerMin);
            String line7 = String.Format(".RuleException\t\t{0}", ruleException);
            String line8 = String.Format(".PriorityMine\t\t{0} {1} {2} {3}", priorityMine[0], priorityMine[1], priorityMine[2], priorityMine[3]);
            String line9 = String.Format(".PriorityMineD\t\t{0} {1} {2} {3}", priorityMineD[0], priorityMineD[1], priorityMineD[2], priorityMineD[3]);
            String line10 = String.Format(".PriorityOpponent\t{0} {1} {2} {3}", priorityOpponent[0], priorityOpponent[1], priorityOpponent[2], priorityOpponent[3]);
            String line11 = String.Format(".PriorityOpponentD\t{0} {1} {2} {3}", priorityOpponentD[0], priorityOpponentD[1], priorityOpponentD[2], priorityOpponentD[3]);
            String line12 = String.Format(".PriorityBall\t\t{0}", priorityBall);
            String linerules = "";
            foreach (GridRule r in rules)
                linerules += r.ToString();
            return line1 + "\r\n" + line2 + "\r\n" + line3 + "\r\n" + line4 + "\r\n" + line5 + "\r\n" + line6 + "\r\n" + line7 + "\r\n" + line8 + "\r\n" + line9 + "\r\n" + line10 + "\r\n" + line11 + "\r\n" + line12 + "\r\n" + "\r\n" + linerules;
        }
    }
}
