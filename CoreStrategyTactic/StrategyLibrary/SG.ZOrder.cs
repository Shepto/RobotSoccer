using System;
using System.Collections.Generic;
using System.Text;

namespace StrategyLibrary
{
    class ZOrder
    {
        private List<int> retOrder;
        private List<int>[,] matrix;

        private void zOrder_xy(int level, int x, int y)
        {
            if (0 == level)
            {
                Console.WriteLine("{0}, {1}", 0, 0);
                return;
            }

            if (level > 1)
            {
                zOrder_xy(level - 1, 2 * x, 2 * y);
                zOrder_xy(level - 1, 2 * (x + 1) + 1, 2 * y);
                zOrder_xy(level - 1, 2 * x, 2 * (y + 1));
                zOrder_xy(level - 1, 2 * (x + 1) + 1, 2 * (y + 1));
                return;
            }

            List<int[]> lIndx = new List<int[]>();
            lIndx.Add(new int[] { x, y });
            lIndx.Add(new int[] { x + 1, y });
            lIndx.Add(new int[] { x + 2, y });
            lIndx.Add(new int[] { x, y + 1 });
            lIndx.Add(new int[] { x + 1, y + 1 });
            lIndx.Add(new int[] { x + 2, y + 1 });

            foreach (int[] pom in lIndx)
            {
                int ix = pom[0];
                int iy = pom[1];
                if (matrix[ix, iy] != null)
                    foreach (int val in matrix[ix, iy])
                        retOrder.Add(val);
            }

            //Console.WriteLine("{0}, {1}", x,        y);
            //Console.WriteLine("{0}, {1}", x + 1,    y);
            //Console.WriteLine("{0}, {1}", x + 2,    y);
            //Console.WriteLine("{0}, {1}", x,        y + 1);
            //Console.WriteLine("{0}, {1}", x + 1,    y + 1);
            //Console.WriteLine("{0}, {1}", x + 2,    y + 1);
        }

        public List<int> zOrderMain(List<int>[,] matrix, int level)
        {
            this.matrix = matrix;

            retOrder = new List<int>();
            zOrder_xy(level, 0, 0);

            return retOrder;
        }
    }
}
