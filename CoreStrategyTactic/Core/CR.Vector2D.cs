using System;
using System.Collections.Generic;
using System.Text;

namespace Core {
    // trida predstavujici 2D vektor se svou souradnici tvorenou dvema realnymi cisly a nekolika pomocnymi metodami
    public class Vector2D : IComparable<Vector2D> {
        private double x;   // x-ova slozka souradnice
        private double y;   // y-ova slozka souradnice
        public Boolean isMapped { get; set; }

        public double X {
            get { return x; }
            set { x = value; }
        }

        public double Y {
            get { return y; }
            set { y = value; }
        }

        public Vector2D(double x, double y) {
            this.x = x;
            this.y = y;
            isMapped = false;
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }

        public override bool Equals(object obj) {
            return base.Equals(obj);
        }

        // operator == porovnava dva vektory
        public static bool operator ==(Vector2D v1, Vector2D v2) {
            return (v1.X == v2.X && v1.Y == v2.Y);
        }

        // operator != zjistuje zda se dva vektory nerovnaji
        public static bool operator !=(Vector2D v1, Vector2D v2) {
            return (v1.X != v2.X || v1.Y != v2.Y);
        }

        // operator + scita dva vektory
        public static Vector2D operator +(Vector2D v1, Vector2D v2) {
            return new Vector2D(v1.X + v2.X, v1.Y + v2.Y);
        }

        // operator - odecita dva vektory
        public static Vector2D operator -(Vector2D v1, Vector2D v2) {
            return new Vector2D(v1.X - v2.X, v1.Y - v2.Y);
        }

        // operator - otaci slozky vektoru
        public static Vector2D operator -(Vector2D v1) {
            return new Vector2D(-v1.X, -v1.Y);
        }

        // operator * nasobi dva vektory
        public static Vector2D operator *(Vector2D v1, Vector2D v2) {
            return new Vector2D(v1.X * v2.X, v1.Y * v2.Y);
        }

        // operator * nasobi vektor realnym cislem
        public static Vector2D operator *(Vector2D v1, double d) {
            return new Vector2D(v1.X * d, v1.Y * d);
        }

        // operator / deli vektor realnym cislem
        public static Vector2D operator /(Vector2D v1, double d) {
            return new Vector2D(v1.X / d, v1.Y / d);
        }

        // funkce ktera zjistuje vzdalenost mezi dvema vektory (body)
        public double DistanceFrom(Vector2D v) {
            return Math.Sqrt((this.X - v.X) * (this.X - v.X) + (this.Y - v.Y) * (this.Y - v.Y));
        }

        // funkce zjistujici velikost vektoru
        public double Length() {
            return Math.Sqrt(X * X + Y * Y);
        }

        // funkce preklapejici vektor kolem dane osy preklopeni
        public Vector2D FlipArround(Vector2D axis) {
            Vector2D _i = new Vector2D(X, Y).Unit();
            Vector2D _axis = new Vector2D(axis.X, axis.Y).Unit();
            double temp
                = (_i.X != 0)
                ? -X / _i.X
                : (_i.Y != 0)
                ? -Y / _i.Y
                : 0;
            return ((_axis * (_i.X * _axis.X + _i.Y * _axis.Y) * 2) - _i) * temp;
        }

        // prevod vektoru na vektor jehoz velikost je jedna (jednotkovy vektor)
        public Vector2D Unit() {
            if (X == 0 && Y == 0)
                return new Vector2D(0, 0);
            return new Vector2D(X / this.Length(), Y / this.Length());
        }

        // funkce vykonavajici skalarni soucin dvou vektoru
        public double Dot(Vector2D v) {
            return (X * v.X + Y * v.Y);
        }

        // funkce zjistujici odchylku dvou vektoru
        public double Angle(Vector2D v) {
            return Math.Acos(this.Dot(v) / (this.Length() * v.Length())) * (180 / Math.PI);
        }

        /// Metoda pro vypocet uhlu natoceni robota        
        public double Rotation(Vector2D v) {
            double res = (Math.Sqrt(this.X * this.X + this.Y * this.Y) * Math.Sqrt(v.X * v.X + v.Y * v.Y));
            if (res == 0) {
                return 0;
            } else {
                return Math.Acos((this.X * v.X + this.Y * v.Y) / res);
            }
        }

        public int CompareTo(Vector2D obj) {
            if (y < obj.y) return -1;
            else if (y == obj.y && x == obj.x) return 0;
            else if (y == obj.y && x < obj.x) return -1;
            else if (y == obj.y && x > obj.x) return 1;
            else return 1;
        }

        // funkce na vypsání hodnot vektoru
        public String print() {
            return "[" + X + " ; " + Y + "]";
        }
    }
}
