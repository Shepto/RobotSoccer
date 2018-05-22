using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using UnityEngine;

namespace StrategiesDLL.Regresion  
{
    public class PolynomialRegresion
    {
        public int n; // STUPEN POLYNOMU +1  
        public double[,] a; //MATICE
        public double[] bodX;//= {7,12,20}; //X-OVE SOURADNICE BODU
        public double[] bodY; //= {5,5,5}; //Y-OVE SOURADNICE BODU
        public int sloupce;// = 30,radky=20; //POCET RADKU A SLOUPCU
        public int radky;
        public Bod robot;
        public Bod prekazka;
        public Bod cil;

        public PolynomialRegresion(Bod robot, Bod prekazka, Bod cil)
        {
            n = 3; //Druhy stupen polynomu
            a = new double[4, 5];
            bodX = new double[3];
            bodY = new double[3];
            sloupce = 220;
            radky = 180;
            this.robot = robot;
            this.prekazka = prekazka;
            this.cil = cil;
            bodX[0] = robot.x;
            bodY[0] = robot.y;
            bodX[1] = prekazka.x;
            bodY[1] = prekazka.y;
            bodX[2] = cil.x;
            bodY[2] = cil.y;
        }
        /*
         * POMOCNA FUNKCE A^n
         */
        public double mocninaAnaN(double a, double n)
        {
            double sum = a;

            if (n == 0)
            {
                return 1;
            }
            else
            {

                for (int i = 0; i < n - 1; i++)
                {
                    sum = sum * a;
                }
                return sum;
            }
        }

        /*
         * VYTVORENI MATICE
         */
        public void createMatrix()
        {
            double sum = 0, sumy = 0;
            int moc1 = 0;
            int moc2 = -1;
            int pocetBodu = bodX.Count();

            for (int i = 0; i < n; i++)
            {
                moc2++;
                moc1 = moc2;
                for (int j = 0; j < n; j++)
                {
                    for (int k = 0; k < pocetBodu; k++)
                    {
                        sum = sum + mocninaAnaN(bodX[k], moc1);
                    }
                    a[i, j] = sum;
                    sum = 0;
                    moc1++;
                }
            }

            for (int i = 0; i < pocetBodu; i++)
            {
                sumy = sumy + bodY[i];
            }
            a[0, n] = sumy;
            for (int j = 1; j < n; j++)
            {

                for (int k = 0; k < pocetBodu; k++)
                {
                    sum = sum + (mocninaAnaN(bodX[k], j) * bodY[k]);
                }
                a[j, n] = sum;
                sum = 0;
            }
        }

        /*
         * FUKNCE PRO GAUSOVU ELIMINACI
         */
        public void forwardSubstitution()
        {
            int i, j, k, max;
            double t;
            for (i = 0; i < n; ++i)
            {
                max = i;
                //nalezeni indexu maxima
                for (j = i + 1; j < n; ++j)
                    if (a[j, i] > a[max, i])
                        max = j;
                //prehozeni radku
                for (j = 0; j < n + 1; ++j)
                {
                    t = a[max, j];
                    a[max, j] = a[i, j];
                    a[i, j] = t;
                }
                for (j = n; j >= i; --j)
                    for (k = i + 1; k < n; ++k)
                        a[k, j] -= a[k, i] / a[i, i] * a[i, j];
            }
        }

        /*
         * FUKNCE PRO GAUSOVU ELIMINACI
         */
        public void reverseElimination()
        {
            int i, j;
            for (i = n - 1; i >= 0; --i)
            {
                a[i, n] = a[i, n] / a[i, i];
                a[i, i] = 1;
                for (j = i - 1; j >= 0; --j)
                {
                    a[j, n] -= a[j, i] * a[i, n];
                    a[j, i] = 0;
                }
            }
        }

        /*
         * GAUSOVA ELIMINACE
         */
        public void gauss()
        {
            //int i, j;
            createMatrix();
            forwardSubstitution();
            reverseElimination();

            /*for (i = 0; i < n; ++i) {
                for (j = 0; j < n + 1; ++j)
                    printf("%f\t", a[i][j]);
                printf("\n");
            }*/
        }


        /*
         * VYPOCET HODNOTY Y K DANEMU X
         */
        public double vypocet(int x)
        {
            double y = 0;
            for (int i = n - 1; i >= 0; i--)
            {
                y += (a[i, n] * mocninaAnaN(x, i));
            }
            return y;
        }



        /*
         * VYTVORENI DVOUROZMERNEHO VEKTORU, KTERY PRO KAZDE X OBSAHUJE DANE Y
         */

        public List<Bod> polynom(bool prekazka)
        {
            //pokud se jedna o prekazku, vypocitat bod pro objeti
            if(prekazka)
                tecna();

            gauss();

            List<Bod> pol = new List<Bod>();
            Bod p;

            for (int x = -110; x < 110; x++)
            {
                double y = vypocet(x);
                p = new Bod(x, y);
                pol.Add(p);
            }
            
            return pol;
        }

        /*
         * VYPOCET KVADRATICKE ROVNICE
         */
        public double[] diskriminant(double a, double b, double c)
        {
            //double* pointer; 
            double[] pole = new double[2];
            //pointer = pole;
            double d;
            d = (b * b) - (4 * a * c);
            if (d >= 0)
            {
                pole[0] = ((-1 * b) + (Math.Sqrt(d))) / (2 * a);
                pole[1] = ((-1 * b) - (Math.Sqrt(d))) / (2 * a);
            }
            return pole;
        }

        /*
         *UDELANI TECNY A ZISKANI BODU DOTYKU
         */
        void tecna()
        {
            int r = 15;
            double x1 = 0, v0 = 0, v1 = 0, a1 = 0, b1 = 0, c1 = 0, m = 0;
            double[] poleX = new double[2];
            double[] poleY = new double[2];
            //VYTVORENI ROVNICE POLARY A VYJADRENI X
            x1 = bodX[0] - bodX[1]; //POMOCNY VYPOCET
            //parametry polary v0,v1
            v0 = (bodY[1] - bodY[0]) / x1;
            v1 = (bodX[0] * bodX[1] - bodX[1] * bodX[1] + bodY[0] * bodY[1] - bodY[1] * bodY[1] + r * r) / x1;
            m = v1 - bodX[1]; //POMOCNY VYPOCET
            //DOSAZENI POLARY DO ROVNICE KRUZNICE A VYTVORENI KVADRATICKE ROVNICE
            a1 = v0 * v0 + 1;
            b1 = (2 * v0 * m) - (2 * bodY[1]);
            c1 = m * m + bodY[1] * bodY[1] - r * r;

            //VYPOCET KVADRATICKE ROVNICE A SOURADNIC X,Y
            poleY = diskriminant(a1, b1, c1);
            poleX[0] = v0 * poleY[0] + v1;
            poleX[1] = v0 * poleY[1] + v1;

            //VYPOCET VZDALENOSTI
            b1 = poleX[0] - bodX[0];
            a1 = poleY[0] - bodY[0];
            c1 = (-1 * a1 * bodX[0]) - (b1 * bodY[0]);
            v0 = (a1 * bodX[2] + b1 * bodY[2] + c1) / (Math.Sqrt((a1 * a1) + (b1 * b1)));
            b1 = poleX[1] - bodX[0];
            a1 = poleY[1] - bodY[0];
            c1 = (-1 * a1 * bodX[0]) - (b1 * bodY[0]);
            v1 = (a1 * bodX[2] + b1 * bodY[2] + c1) / (Math.Sqrt((a1 * a1) + (b1 * b1)));

            //VLOZENI BODU DOTYKU MEZI OSTATNI
            if (v0 < 0) v0 = -1 * v0;
            if (v1 < 0) v1 = -1 * v1;

            if (v0 <= v1 && poleY[0] > -90 && poleY[0] < 90)
            {
                bodX[1] = poleX[0];
                bodY[1] = poleY[0];
            }
            else
            {
                bodX[1] = poleX[1];
                bodY[1] = poleY[1];
            }
        }


        /*
         * VYKRESLENI DO CONSOLE
         */
        public void vykresleni()
        {
            List<Bod> poly = new List<Bod>();
            poly = polynom(true);
            double hod = 0;
            for (int i = 0; i < radky; i++)
            {
                for (int j = 0; j < sloupce; j++)
                {
                    hod = radky - poly.ElementAt(j).y;
                    if (i == hod)
                        Console.Write(" *");
                    else
                        Console.Write(" .");
                }
                Console.WriteLine();
            }
        }


        // static void Main(string[] args)
        //  {
        //       Point robot = new Point(2, 19);
        //       Point prekazka = new Point(18, 19);
        //       Point cil = new Point(25, 19);
        //       PolynomialRegresion polynom = new PolynomialRegresion(robot, prekazka, cil);
        //       polynom.vykresleni();
        //   }
    }
}