using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

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

    public class Cells
    {
        public int value;
        public int x;
        public int y;
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
                    AI(table, CPU, ref px, ref py, players);
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
       Az AI függvény először minden üres cellának ad egy értéket hasznosság alapján,
       majd ezekből az értékekből választja ki a legjobb lehetőséget.
       */
        static void AI(GridModel table, FieldState CPU, ref int aix, ref int aiy, int players)
        {
            int currentX = 0, currentY = 0;
            int[,] valuesTable = new int[table.Height, table.Width];

            getValues(table, valuesTable, currentX, currentY, players);

            int tempMax = 0;
            List<Cells> bestValues = new List<Cells>();
            Random rnd = new Random();
            int random=0;

            for (int x = 0; x < table.Height; x++)
            {
                for (int y = 0; y < table.Width; y++)
                {
                    if (valuesTable[x, y] >= tempMax)
                    {
                        tempMax = valuesTable[x, y];
                    }
                }
            }
            for (int x = 0; x < table.Height; x++)
            {
                for (int y = 0; y < table.Width; y++)
                {
                    if (valuesTable[x, y] == tempMax)
                    {                        
                        bestValues.Add(new Cells { value = tempMax, x = x, y = y });                        
                    }
                }
            }            
            random = rnd.Next(0, bestValues.Count-1);       
            aix = bestValues[random].x;
            aiy = bestValues[random].y;

        }

        static void getValues(GridModel table, int[,] valuesTable, int currentX, int currentY, int players)
        {
            int P1cellValue = 0, P2cellValue = 0, P3cellValue = 0, AIcellValue = 0;
            FieldState checkedState = FieldState.None;
            for (int x = 1; x < table.Height - 1; x++)
            {
                for (int y = 1; y < table.Width - 1; y++)
                {
                    if (table[x, y] == FieldState.None)
                    {
                        currentX = x;
                        currentY = y;
                        int[] tempArray = new int[players];
                        switch (players)
                        {
                            case 2:
                                checkedState = FieldState.PlayerOne;
                                P1cellValue = cellValue(table, currentX, currentY, checkedState);
                                checkedState = FieldState.PlayerTwo;
                                AIcellValue = cellValue(table, currentX, currentY, checkedState);

                                if (AIcellValue >= P1cellValue)
                                {
                                    valuesTable[x, y] = AIcellValue;
                                }
                                else
                                {
                                    valuesTable[x, y] = P1cellValue;
                                }


                                break;
                            case 3:
                                checkedState = FieldState.PlayerOne;
                                P1cellValue = cellValue(table, currentX, currentY, checkedState);
                                checkedState = FieldState.PlayerTwo;
                                P2cellValue = cellValue(table, currentX, currentY, checkedState);
                                checkedState = FieldState.PlayerThree;
                                AIcellValue = cellValue(table, currentX, currentY, checkedState);

                                int tempMax = 0;

                                tempArray[0] = P1cellValue;
                                tempArray[1] = P2cellValue;
                                tempArray[2] = AIcellValue;

                                for (int i = 0; i < players; i++)
                                {
                                    if (tempArray[i] >= tempMax)
                                    {
                                        tempMax = tempArray[i];
                                    }
                                }
                                valuesTable[x, y] = tempMax;

                                break;
                            case 4:
                                checkedState = FieldState.PlayerOne;
                                P1cellValue = cellValue(table, currentX, currentY, checkedState);
                                checkedState = FieldState.PlayerTwo;
                                P2cellValue = cellValue(table, currentX, currentY, checkedState);
                                checkedState = FieldState.PlayerThree;
                                P3cellValue = cellValue(table, currentX, currentY, checkedState);
                                checkedState = FieldState.PlayerFour;
                                AIcellValue = cellValue(table, currentX, currentY, checkedState);

                                tempMax = 0;

                                tempArray[0] = P1cellValue;
                                tempArray[1] = P2cellValue;
                                tempArray[2] = P3cellValue;
                                tempArray[3] = AIcellValue;

                                for (int i = 0; i < players; i++)
                                {
                                    if (tempArray[i] >= tempMax)
                                    {
                                        tempMax = tempArray[i];
                                    }
                                }
                                valuesTable[x, y] = tempMax;

                                break;
                        }
                    }
                }
            }
        }
           
        static int cellValue(GridModel table, int currentX, int currentY, FieldState checkedState)
        {
            int[] directionsX = { 0, 0, 1, -1, -1, 1, 1, -1 };
            int[] directionsY = { 1, -1, 0, 0, 1, -1, 1, -1 };
            int sumX = 0, sumY = 0;
            int sum = 0;
            int maxValue = 0;

            for (int i = 0; i < 8; i++)
            {                
                sumX = currentX;
                sumY = currentY;
                while(!(sumY == table.Width || sumX == table.Height || table[sumX+directionsX[i], sumY+directionsY[i]] != checkedState || sum == 5))
                {
                    sum++;
                    sumX += directionsX[i];
                    sumY += directionsY[i];
                }
                if (i % 2 == 0)
                {
                    if (sum > maxValue)
                    {
                        maxValue = sum;
                    }
                    sum = 0;
                }
                if (i == 7)
                {
                    if (sum > maxValue)
                    {
                        maxValue = sum;
                    }
                    sum = 0;
                }

            }
            return maxValue;
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
