using System;

namespace CalendarSolver
{
    class Program
    {
        static void Main(string[] args)
        {
            Solver s = new Solver(5, 33);
            s.Solve();
        }
    }

    class Figure
    {
        public int w, h;
        public byte[][] matrix;
        public Figure(int w, int h, uint binary)
        {
            this.w = w;
            this.h = h;
            this.matrix = new byte[h][];
            for (int i = h - 1; i >= 0; i--)
            {
                this.matrix[i] = new byte[w];
                for (int j = w - 1; j >= 0; j--)
                {
                    this.matrix[i][j] = (byte)(binary & 1);
                    binary >>= 1;
                }
            }
        }
    }

    class Solver
    {
        byte[][] field;
        Figure[][] figures;
        public Solver(int id1, int id2)
        {
            field = new byte[7][];
            for (int i = 0; i < 7; i++)
            {
                field[i] = new byte[7];
            }

            field[0][6] = 1;
            field[1][6] = 1;
            field[6][3] = 1;
            field[6][4] = 1;
            field[6][5] = 1;
            field[6][6] = 1;

            field[id1 / 7][id1 % 7] = 1;
            field[id2 / 7][id2 % 7] = 1;

            figures = new Figure[8][];

            figures[0] = new Figure[2]; // 3x2 rect
            figures[0][0] = new Figure(3, 2, 0b111111);
            figures[0][1] = new Figure(2, 3, 0b111111);

            figures[1] = new Figure[4]; // 3x2 C
            figures[1][0] = new Figure(3, 2, 0b111101);
            figures[1][1] = new Figure(3, 2, 0b101111);
            figures[1][2] = new Figure(2, 3, 0b110111);
            figures[1][3] = new Figure(2, 3, 0b111011);

            figures[2] = new Figure[4]; // 3x3 corner
            figures[2][0] = new Figure(3, 3, 0b111100100);
            figures[2][1] = new Figure(3, 3, 0b111001001);
            figures[2][2] = new Figure(3, 3, 0b100100111);
            figures[2][3] = new Figure(3, 3, 0b001001111);

            figures[3] = new Figure[4]; // 3x3 S
            figures[3][0] = new Figure(3, 3, 0b110_010_011);
            figures[3][1] = new Figure(3, 3, 0b011_010_110);
            figures[3][2] = new Figure(3, 3, 0b100_111_001);
            figures[3][3] = new Figure(3, 3, 0b001_111_100);

            figures[4] = new Figure[8]; // 3x2 rect - corner
            figures[4][0] = new Figure(3, 2, 0b111110);
            figures[4][1] = new Figure(3, 2, 0b110111);
            figures[4][2] = new Figure(3, 2, 0b011111);
            figures[4][3] = new Figure(3, 2, 0b111011);
            figures[4][4] = new Figure(2, 3, 0b111110);
            figures[4][5] = new Figure(2, 3, 0b111101);
            figures[4][6] = new Figure(2, 3, 0b011111);
            figures[4][7] = new Figure(2, 3, 0b101111);

            figures[5] = new Figure[8]; // 4x2 L shifted
            figures[5][0] = new Figure(4, 2, 0b11110100);
            figures[5][1] = new Figure(4, 2, 0b11110010);
            figures[5][2] = new Figure(4, 2, 0b01001111);
            figures[5][3] = new Figure(4, 2, 0b00101111);
            figures[5][4] = new Figure(2, 4, 0b10111010);
            figures[5][5] = new Figure(2, 4, 0b10101110);
            figures[5][6] = new Figure(2, 4, 0b01110101);
            figures[5][7] = new Figure(2, 4, 0b01011101);

            figures[6] = new Figure[8]; // 4x2 Snake
            figures[6][0] = new Figure(4, 2, 0b1110_0011);
            figures[6][1] = new Figure(4, 2, 0b0111_1100);
            figures[6][2] = new Figure(4, 2, 0b1100_0111);
            figures[6][3] = new Figure(4, 2, 0b0011_1110);
            figures[6][4] = new Figure(2, 4, 0b01011110);
            figures[6][5] = new Figure(2, 4, 0b01111010);
            figures[6][6] = new Figure(2, 4, 0b10101101);
            figures[6][7] = new Figure(2, 4, 0b10110101);

            figures[7] = new Figure[8]; // 4x2 L
            figures[7][0] = new Figure(4, 2, 0b11111000);
            figures[7][1] = new Figure(4, 2, 0b11110001);
            figures[7][2] = new Figure(4, 2, 0b10001111);
            figures[7][3] = new Figure(4, 2, 0b00011111);
            figures[7][4] = new Figure(2, 4, 0b11101010);
            figures[7][5] = new Figure(2, 4, 0b11010101);
            figures[7][6] = new Figure(2, 4, 0b10101011);
            figures[7][7] = new Figure(2, 4, 0b01010111);
        }

        public bool Solve(int f = 0)
        {
            if (f >= 8)
                return true;
            bool res = false;
            for (int t = 0; t < figures[f].Length; t++)
            {
                for (int i = 0; i < 8 - figures[f][t].h; i++)
                {
                    for (int j = 0; j < 8 - figures[f][t].w; j++)
                    {
                        if (insert(figures[f][t], i, j))
                        {
                            res = Solve(f + 1);
                        }
                        if (res)
                        {
                            Console.WriteLine(String.Format("Figure: {0}, Type: {1}, Row: {2}, Col: {3}", f, t, i, j));
                            return res;
                        }
                        take(figures[f][t], i, j);
                    }
                }
            }
            return res;
        }



        private bool insert(Figure f, int row, int col)
        {
            bool res = true;
            for (int i = 0; i < f.h; i++)
            {
                for (int j = 0; j < f.w; j++)
                {
                    field[row + i][col + j] += f.matrix[i][j];
                    if (field[row + i][col + j] > 1)
                        res = false;
                }
            }
            return res;
        }

        private void take(Figure f, int row, int col)
        {
            for (int i = 0; i < f.h; i++)
            {
                for (int j = 0; j < f.w; j++)
                {
                    field[row + i][col + j] -= f.matrix[i][j];
                }
            }
        }

    }
}
