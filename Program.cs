using System;
using System.Threading.Tasks;

namespace CalendarSolver
{
    class Program
    {
        static void Main(string[] args)
        {
            int day = 21; 
            int month = 1;
            if (args.Length == 2)
            {
                day = Convert.ToInt32(args[0]);
                month = Convert.ToInt32(args[1]);
                Console.WriteLine(day + "." + month + " - " + Solve(day, month) + " solutions");
            }
            else
            {
                Parallel.For(1, 13, (month) =>
                {
                    for (int day = 1; day <= 31; day++)
                    {
                        Console.WriteLine(day + "." + month + " - " + Solve(day, month) + " solutions");
                    }
                });
            }

        }

        static int Solve(int day, int month)
        {
            ulong field = 0b00000011_00000011_00000001_00000001_00000001_00000001_00011111_11111111UL;
            if (month > 6)
                month += 2;
            field |= 1UL << (64 - month);
            field |= 1UL << (48 - (day + (day - 1) / 7));

#if !FF
            Tools.PrintBin(field);
#endif
            int cnt = Solver.Solve(field);
            return cnt;
        }
    }

    class FigureFactory
    {
        public int w, h;
        public ulong vector;
        public FigureFactory(int w, int h, ulong binary)
        {
            this.w = w;
            this.h = h;
            this.vector = binary;

        }

        public UInt64 getShifted(int rightShift, int downShift)
        {
            return vector >> (rightShift + 8 * downShift);
        }
    }

    class Tools
    {
        public static void PrintBin(ulong vector)
        {
            Console.WriteLine("dumping " + vector);
            for (int i = 0; i < 8; i++)
            {
                ulong line = (vector >> (8 * (7 - i))) & 255UL;
                for (int j = 0; j < 8; j++)
                {
                    Console.Write(line >> (7 - j) & 1);
                }
                Console.Write('\n');
            }
        }
    }

    static class Solver
    {
        static FigureFactory[][] figures;
        static Solver()
        {
            figures = new FigureFactory[8][];

            figures[0] = new FigureFactory[2]; // 3x2 rect
            figures[0][0] = new FigureFactory(3, 2, 0b11100000_11100000UL << 48);
            figures[0][1] = new FigureFactory(2, 3, 0b11000000_11000000_11000000UL << 40);

            figures[1] = new FigureFactory[4]; // 3x2 C
            figures[1][0] = new FigureFactory(3, 2, 0b11100000_10100000UL << 48);
            figures[1][1] = new FigureFactory(3, 2, 0b10100000_11100000UL << 48);
            figures[1][2] = new FigureFactory(2, 3, 0b11000000_01000000_11000000UL << 40);
            figures[1][3] = new FigureFactory(2, 3, 0b11000000_10000000_11000000UL << 40);

            figures[2] = new FigureFactory[4]; // 3x3 corner
            figures[2][0] = new FigureFactory(3, 3, 0b11100000_10000000_10000000UL << 40);
            figures[2][1] = new FigureFactory(3, 3, 0b11100000_00100000_00100000UL << 40);
            figures[2][2] = new FigureFactory(3, 3, 0b10000000_10000000_11100000UL << 40);
            figures[2][3] = new FigureFactory(3, 3, 0b00100000_00100000_11100000UL << 40);

            figures[3] = new FigureFactory[4]; // 3x3 S
            figures[3][0] = new FigureFactory(3, 3, 0b11000000_01000000_01100000UL << 40);
            figures[3][1] = new FigureFactory(3, 3, 0b01100000_01000000_11000000UL << 40);
            figures[3][2] = new FigureFactory(3, 3, 0b10000000_11100000_00100000UL << 40);
            figures[3][3] = new FigureFactory(3, 3, 0b00100000_11100000_10000000UL << 40);

            figures[4] = new FigureFactory[8]; // 3x2 rect - corner
            figures[4][0] = new FigureFactory(3, 2, 0b11100000_11000000UL << 48);
            figures[4][1] = new FigureFactory(3, 2, 0b11000000_11100000UL << 48);
            figures[4][2] = new FigureFactory(3, 2, 0b01100000_11100000UL << 48);
            figures[4][3] = new FigureFactory(3, 2, 0b11100000_01100000UL << 48);
            figures[4][4] = new FigureFactory(2, 3, 0b11000000_11000000_10000000UL << 40);
            figures[4][5] = new FigureFactory(2, 3, 0b11000000_11000000_01000000UL << 40);
            figures[4][6] = new FigureFactory(2, 3, 0b01000000_11000000_11000000UL << 40);
            figures[4][7] = new FigureFactory(2, 3, 0b10000000_11000000_11000000UL << 40);

            figures[5] = new FigureFactory[8]; // 4x2 L shifted
            figures[5][0] = new FigureFactory(4, 2, 0b11110000_01000000UL << 48);
            figures[5][1] = new FigureFactory(4, 2, 0b11110000_00100000UL << 48);
            figures[5][2] = new FigureFactory(4, 2, 0b01000000_11110000UL << 48);
            figures[5][3] = new FigureFactory(4, 2, 0b00100000_11110000UL << 48);
            figures[5][4] = new FigureFactory(2, 4, 0b10000000_11000000_10000000_10000000UL << 32);
            figures[5][5] = new FigureFactory(2, 4, 0b10000000_10000000_11000000_10000000UL << 32);
            figures[5][6] = new FigureFactory(2, 4, 0b01000000_11000000_01000000_01000000UL << 32);
            figures[5][7] = new FigureFactory(2, 4, 0b01000000_01000000_11000000_01000000UL << 32);

            figures[6] = new FigureFactory[8]; // 4x2 Snake
            figures[6][0] = new FigureFactory(4, 2, 0b11100000_00110000UL << 48);
            figures[6][1] = new FigureFactory(4, 2, 0b01110000_11000000UL << 48);
            figures[6][2] = new FigureFactory(4, 2, 0b11000000_01110000UL << 48);
            figures[6][3] = new FigureFactory(4, 2, 0b00110000_11100000UL << 48);
            figures[6][4] = new FigureFactory(2, 4, 0b01000000_01000000_11000000_10000000UL << 32);
            figures[6][5] = new FigureFactory(2, 4, 0b01000000_11000000_10000000_10000000UL << 32);
            figures[6][6] = new FigureFactory(2, 4, 0b10000000_10000000_11000000_01000000UL << 32);
            figures[6][7] = new FigureFactory(2, 4, 0b10000000_11000000_01000000_01000000UL << 32);

            figures[7] = new FigureFactory[8]; // 4x2 L
            figures[7][0] = new FigureFactory(4, 2, 0b11110000_10000000UL << 48);
            figures[7][1] = new FigureFactory(4, 2, 0b11110000_00010000UL << 48);
            figures[7][2] = new FigureFactory(4, 2, 0b10000000_11110000UL << 48);
            figures[7][3] = new FigureFactory(4, 2, 0b00010000_11110000UL << 48);
            figures[7][4] = new FigureFactory(2, 4, 0b11000000_10000000_10000000_10000000UL << 32);
            figures[7][5] = new FigureFactory(2, 4, 0b11000000_01000000_01000000_01000000UL << 32);
            figures[7][6] = new FigureFactory(2, 4, 0b10000000_10000000_10000000_11000000UL << 32);
            figures[7][7] = new FigureFactory(2, 4, 0b01000000_01000000_01000000_11000000UL << 32);
        }

        public static int Solve(ulong field, int figId = 0)
        {
            if (figId >= 8)
                return 1;
            int res = 0;
            for (int t = 0; t < figures[figId].Length; t++)
            {
                for (int i = 0; i < 8 - figures[figId][t].h; i++)
                {
                    for (int j = 0; j < 8 - figures[figId][t].w; j++)
                    {
                        ulong shiftedFig = figures[figId][t].getShifted(j, i);
                        if ((field & shiftedFig) == 0)
                        {
#if DEBUG
                            if (figId == 0)
                            {
                                Console.WriteLine("Trying new pos for FirstFig: ");
                                Tools.PrintBin(field | shiftedFig);
                            }
#endif
                            int curRes = Solve(field | shiftedFig, figId + 1);
                            if (curRes > 0)
                            {
#if !FF
                                Console.WriteLine(String.Format("Figure: {0}, Type: {1}, Row: {2}, Col: {3}", figId, t, i, j));
#endif
                                res += curRes;
                            }
                        }
                        
                    }
                }
            }
            return res;
        }
    }
}
