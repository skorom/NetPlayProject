using System;
using System.Collections.Generic;
using System.IO;

namespace Logika
{

    public enum FieldState
    {
        None, PlayerOne, PlayerTwo, PlayerThree, PlayerFour
    }

    public class GridModel
    {
        // Az tömb szélessége és magassága lekérhetõ, de csak külön metodus során állítható
        public int Width { get; private set; }
        public int Height { get; private set; }

        // Az amõba tábla
        private FieldState[,] fields;

        public GridModel(int width = 20, int height = 20)
        {
            this.Width = width;
            this.Height = height;
            this.fields = new FieldState[Height, Width];
            ResetCellsToDefault();
        }

        // a Grid classból a tábla x, y helyen lévõ eleme megkaphatók
        // (nem változtatható) [x,y] színtaktikával
        /* például:
            Grid testGrid = new Grid(); testGrid[2,2]==FieldState.PlayerOne
        */
        public FieldState this[int iPos, int jPos]
        {
            get
            {
                return fields[iPos, jPos];
            }
        }

        // hamis értékkel tér vissza ha szabálytalan a lépés
        public bool MakeMove(int iPos, int jPos, FieldState _fieldState)
        {
            if (IsValidMove(iPos, jPos))
            {
                fields[iPos, jPos] = _fieldState;
                return true;
            }
            else
            {
                return false;
            }
        }

        // visszaadja a iPos, jPos koordinátákon tárolt mezõt
        public FieldState GetFieldAtPosition(int iPos, int jPos)
        {
            return fields[iPos, jPos];
        }

        // Minden mezõt semlegesre állít (egy játékoshoz sem tartozik)
        private void ResetCellsToDefault()
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    this.fields[i, j] = FieldState.None;
                }
            }
        }

        // szabályos lépés feltételei
        private bool IsValidMove(int iPos, int jPos)
        {
            // ha a cella foglalt, vagy nem létezik akkor a lépés hamis
            return (iPos >= 0 && iPos < this.Height &&
                jPos >= 0 && jPos < this.Width &&
                fields[iPos, jPos] == FieldState.None);
        }

    }

    class Program
    {

        static void Main(string[] args)
        {
            GridModel table = new GridModel(20, 20);
            int players, px, py;
            bool end = false;
            FieldState CPU = FieldState.None;

            Console.WriteLine("Add meg a játékosok számát! (minimum 2, maximum 4)");
            do
            {
                players = Convert.ToInt32(Console.ReadLine());
            } while ((players < 2) || (players > 4));

            do
            {
                do
                {
                    kiir(table);
                    Console.WriteLine("Add meg annak a sornak és oszlopnak a számát, ahová lépésedet helyezni szeretnéd!");
                    Console.WriteLine("(Kettő külön sorba add meg az értékeket!)");
                    px = Convert.ToInt32(Console.ReadLine());
                    py = Convert.ToInt32(Console.ReadLine());
                } while (table.MakeMove(px, py, FieldState.PlayerOne) == false);

                end = GameEnd(table, px, py, 5);

                for (int i = 0; i < players - 1; i++)
                {
                    switch (i)
                    {
                        case 0:
                            CPU = FieldState.PlayerTwo;
                            break;
                        case 1:
                            CPU = FieldState.PlayerThree;
                            break;
                        case 2:
                            CPU = FieldState.PlayerFour;
                            break;
                    }
                    AI(table, CPU, ref px, ref py);
                    table.MakeMove(px, py, CPU);
                    end = GameEnd(table, px, py, 5);
                }

            } while (end == false);
        }

        static void beolvas(GridModel table)
        {
            string[] proba = File.ReadAllLines("Test.txt");

            for (int i = 0; i < proba.Length; i++)
            {
                for (int j = 0; j < proba[i].Length; j++)
                {
                    switch (proba[i][j])
                    {
                        case '0':
                            table.MakeMove(i, j, FieldState.None);
                            break;
                        case '1':
                            table.MakeMove(i, j, FieldState.PlayerOne);
                            break;
                        case '2':
                            table.MakeMove(i, j, FieldState.PlayerTwo);
                            break;
                        case '3':
                            table.MakeMove(i, j, FieldState.PlayerThree);
                            break;
                        case '4':
                            table.MakeMove(i, j, FieldState.PlayerFour);
                            break;
                    }
                }
            }
        }

        static void kiir(GridModel table)
        {
            Console.Clear();

            for (int i = 0; i < table.Height; i++)
            {
                for (int j = 0; j < table.Width; j++)
                {
                    switch (table[i, j])
                    {
                        case FieldState.None:
                            Console.Write("0");
                            break;
                        case FieldState.PlayerOne:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("1");
                            break;
                        case FieldState.PlayerTwo:
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.Write("2");
                            break;
                        case FieldState.PlayerThree:
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("3");
                            break;
                        case FieldState.PlayerFour:
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write("4");
                            break;
                    }
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        /*
       Az AI függvény a megkapott tábla alapján a referencia szerint érkező aix, aiy pozíciók értékét meghatározza,
       így ad választ a játékosok lépéseire.
       Paraméterei:
       -CPU: az a FieldState változó ami az AIhoz tartozik.
       */
        static void AI(GridModel table, FieldState CPU, ref int aix, ref int aiy)
        {
            List<int> lx = new List<int>();
            List<int> ly = new List<int>();
            int[] testdirx = { 0, -1, -1, -1, 0, 1, 1, 1 };
            int[] testdiry = { -1, -1, 0, 1, 1, 1, 0, -1 };
            Random rnd = new Random();
            int choice;

            //Összegyűjti a már meglévő lépéseket a táblán.
            for (int i = 1; i < table.Height - 1; i++)
            {
                for (int j = 1; j < table.Width - 1; j++)
                {
                    if (table[i, j] != FieldState.None)
                    {
                        for (int k = 0; k < 8; k++)
                        {
                            if (table[i + testdirx[k], j + testdiry[k]] == FieldState.None)
                            {
                                lx.Add(i + testdirx[k]);
                                ly.Add(j + testdiry[k]);
                            }
                        }
                    }
                }
            }

            for (int i = 1; i < table.Height - 1; i++)
            {
                if (table[i, 0] != FieldState.None && table[i, 1] == FieldState.None)
                {
                    lx.Add(i);
                    ly.Add(1);
                }
                if (table[i, table.Width - 1] != FieldState.None && table[i, table.Width - 2] == FieldState.None)
                {
                    lx.Add(i);
                    ly.Add(table.Width - 2);
                }

            }

            for (int i = 1; i < table.Width - 1; i++)
            {
                if (table[0, i] != FieldState.None && table[1, i] == FieldState.None)
                {
                    lx.Add(0);
                    ly.Add(i);
                }
                if (table[table.Height - 1, i] != FieldState.None && table[table.Height - 2, i] == FieldState.None)
                {
                    lx.Add(table.Height - 2);
                    ly.Add(i);
                }
            }

            if (table[0, 0] != FieldState.None)
            {
                for (int i = 4; i <= 6; i++)
                {
                    if (table[0 + testdirx[i], 0 + testdiry[i]] == FieldState.None)
                    {
                        lx.Add(0 + testdirx[i]);
                        ly.Add(0 + testdiry[i]);
                    }
                }
            }
            if (table[0, table.Width - 1] != FieldState.None)
            {
                for (int i = 6; i <= 8; i++)
                {
                    if (table[0 + testdirx[i % 8], table.Width - 1 + testdiry[i % 8]] == FieldState.None)
                    {
                        lx.Add(0 + testdirx[i % 8]);
                        ly.Add(table.Width - 1 + testdiry[i % 8]);
                    }
                }
            }
            if (table[table.Height - 1, 0] != FieldState.None)
            {
                for (int i = 2; i <= 4; i++)
                {
                    if (table[table.Height - 1 + testdirx[i], 0 + testdiry[i]] == FieldState.None)
                    {
                        lx.Add(table.Height - 1 + testdirx[i]);
                        ly.Add(0 + testdiry[i]);
                    }
                }
            }
            if (table[table.Height - 1, table.Width - 1] != FieldState.None)
            {
                for (int i = 0; i <= 2; i++)
                {
                    if (table[table.Height - 1 + testdirx[i], table.Width - 1 + testdiry[i]] == FieldState.None)
                    {
                        lx.Add(table.Height - 1 + testdirx[i]);
                        ly.Add(table.Width - 1 + testdiry[i]);
                    }
                }
            }

            //Ha nem üres a tábla, random választ egy elemet az összegyűjtött pozíciók listájából.
            //Ha üres akkor a tábla közepére helyezi lépését.
            if (lx.Count == 0)
            {
                aix = table.Height / 2;
                aiy = table.Width / 2;
            }
            else
            {
                choice = rnd.Next(0, lx.Count);
                aix = lx[choice];
                aiy = ly[choice];
            }

        }

        /*
        A GameEnd függyvény megvizsgálja a beérkező x, y pozíciók alapján, hogy a lépést végrehajtó játékosnak összegyűlt-e a szükséges mennyiségű szomszédos lépése.
        Igaz értéket ad vissza ha összegyűlt, tehát a játéknak vége.
        Hamis értéket ad vissza ha még nem gyűlt össze.
        Paraméterei:
           x: Amely sorba a játékos megtette lépését.
           y: Amely oszlopba a játékos megtette lépését.
           step: Ahány szomszédos lépésnek össze kell gyűlnie a nyereséghez.
       */
        static bool GameEnd(GridModel table, int x, int y, int step)
        {
            int testx = x, testy = y, borderx = 0, bordery = 0;
            int sum = 0;
            int[] testdirx = { 0, 0, 1, -1, -1, 1, 1, -1 };
            int[] testdiry = { 1, -1, 0, 0, 1, -1, 1, -1 };

            FieldState prev = table[x, y];

            for (int i = 0; i < 8; i++)
            {

                switch (testdirx[i])
                {
                    case -1:
                        borderx = -1;
                        break;
                    case 0:
                        borderx = -2;
                        break;
                    case 1:
                        borderx = table.Height;
                        break;
                }
                switch (testdiry[i])
                {
                    case -1:
                        bordery = -1;
                        break;
                    case 0:
                        bordery = -2;
                        break;
                    case 1:
                        bordery = table.Width;
                        break;
                }

                while (!(testy == bordery || testx == borderx || table[testx, testy] != prev || sum == step))
                {
                    sum++;
                    testx += testdirx[i];
                    testy += testdiry[i];
                }

                if ((i + 1) % 2 == 0 && (sum - 1) >= step)
                {
                    return true;
                }
                else if ((i + 1) % 2 == 0 && (sum - 1) < step)
                {
                    sum = 0;
                }

                testx = x;
                testy = y;
            }

            return false;
        }
    }
}
