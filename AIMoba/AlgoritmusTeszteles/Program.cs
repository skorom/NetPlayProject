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
       Az AI függvény először minden üres cellának ad egy értéket hasznosság alapján,
       majd ezekből az értékekből választja ki a legjobb lehetőséget.
       */
        static void AI(GridModel table, FieldState CPU, ref int aix, ref int aiy)
        {
            int cellValue=0;
            int currentX=0, currentY=0;
            int cellValueColumnAI=0, cellValueRowAI=0, cellValueAcrossAI=0, AIcellValue=0;
            int cellValueColumnP1=0, cellValueRowP1=0, cellValueAcrossP1=0, P1cellValue=0;

            int[,] valuesTable=new int[table.Height,table.Width];
            getValues(table, CPU, valuesTable, currentX, currentY, AIcellValue, P1cellValue, cellValueRowAI, cellValueColumnAI, cellValueAcrossAI, cellValueRowP1, cellValueColumnP1, cellValueAcrossP1, cellValue);

            int bestMoveX=0, bestMoveY=0, bestValue=0;

            for (int x = 0; x < table.Height; x++)
			{
                for (int y = 0; y < table.Width; y++)
			    {                 
                    if(valuesTable[x,y]==1)
                    {
                        bestMoveX=x;
                        bestMoveY=y;
                        bestValue=10;
                        break;
                    }else
                    {
                        if(valuesTable[x,y]==2)
                        {
                            bestMoveX=x;
                            bestMoveY=y;
                            bestValue=10;
                            break;
                        }else
                        {
                            if(valuesTable[x,y]>=bestValue)
                            {
                                bestValue=valuesTable[x,y];
                                bestMoveX=x;
                                bestMoveY=y;
                            }
                        }                       
                    }
                    
			    }
            }
            aix=bestMoveX;
            aiy=bestMoveY;

            for (int i = 0; i < table.Height; i++)
            {
                for (int j = 0; j < table.Width; j++)
                {
                    Console.Write(valuesTable[i, j]);
                }
                Console.WriteLine();
            }
            Console.ReadKey();
        }

        static void getValues(GridModel table, FieldState CPU, int[,] valuesTable, int currentX, int currentY, int AIcellValue, int P1cellValue, int cellValueRowAI, int cellValueColumnAI,
            int cellValueAcrossAI, int cellValueRowP1, int cellValueColumnP1, int cellValueAcrossP1, int cellValue)
        {
            for (int x = 1; x < table.Height-1; x++)
            {
                for (int y = 1; y < table.Width-1; y++)
                {
                    if (table[x, y] == FieldState.None)
                    {
                        currentX = x;
                        currentY = y;
                        cellValue = 0;
                        AIcellValue = cellValueAI(table, CPU, currentX, currentY, cellValueRowAI, cellValueColumnAI, cellValueAcrossAI);
                        P1cellValue = cellValuePlayerOne(table, currentX, currentY, cellValueRowP1, cellValueColumnP1, cellValueAcrossP1);

                        if (AIcellValue == 1)
                        {
                            cellValue = 1;
                        }
                        else
                        {
                            if (P1cellValue == 2)
                            {
                                cellValue = 2;
                            }
                            else
                            {
                                if (P1cellValue > AIcellValue)
                                {
                                    cellValue = P1cellValue;
                                }
                            }
                        }
                        valuesTable[x, y] = cellValue;
                    }
                }
            }
        }
        
        static int cellValueAI(GridModel table, FieldState CPU, int currentX, int currentY, int cellValueRowAI, int cellValueColumnAI, int cellValueAcrossAI)
        {   
            int value = 0;
            cellValueAcrossAI=cellValueAIAcross(table,CPU,currentX,currentY);
            cellValueRowAI=cellValueAIRow(table,CPU,currentX,currentY);
            cellValueColumnAI=cellValueAIColumn(table,CPU,currentX,currentY);
            
            if(cellValueAcrossAI>=cellValueColumnAI)
            {
                if(cellValueAcrossAI>=cellValueRowAI)
                {
                    value=cellValueAcrossAI;
                }else
                {
                    value=cellValueRowAI;
                }
            }else
            {
                if(cellValueColumnAI>=cellValueRowAI)
                {
                    value=cellValueColumnAI;
                }else
                {
                    value=cellValueRowAI;
                }
            }

            if (value == 4)
            {
                return 1;
            }
            else
            {
                if (value == 3)
                {
                    return 7;
                }
                else
                {
                    if (value == 2)
                    {
                        return 5;
                    }
                    else
                    {
                        if (value == 1)
                        {
                            return 3;
                        }
                        else
                        {
                            if (value == 0)
                            {
                                return 0;
                            }
                            else
                            {
                                return 20;
                            }
                        }
                    }
                }
            }            
        }
        static int cellValueAIRow(GridModel table, FieldState CPU, int currentX, int currentY)
        {
            int right=0, left=0;
            for (int r = currentY+1; r < table.Width; r++)
            {
                if (table[currentX, r] == CPU)
                {
                    right++;
                }
                else
                {
                    break;
                }
            }
            for (int r = currentY-1; r > 0 ; r--)
            {
                if (table[currentX, r] == CPU)
                {
                    left++;
                }
                else
                {
                    break;
                }
            }
            if(right>=left)
            {
                return right;
            }else
            {
                return left;
            }
        }
        static int cellValueAIColumn(GridModel table, FieldState CPU, int currentX, int currentY)
        {
            int up = 0, down = 0;
            for (int c = currentX+1; c < table.Height-1; c++)
            {
                if (table[c, currentY] == CPU)
                {
                    down++;
                }
                else
                {
                    break;
                }
            }
            for (int c = currentX-1; c > 0; c--)
            {
                if (table[c, currentY] == CPU)
                {
                    up++;
                }
                else
                {
                    break;
                }
            }
            if(up>=down)
            {
                return up;
            }else
            {
                return down;
            }
        }
        static int cellValueAIAcross(GridModel table, FieldState CPU, int currentX, int currentY)
        {
            int up = 0, down = 0;
            int y=currentY+1;

            for (int x = currentX+1; x < table.Height; x++)
			{
                if(table[x,y]==CPU)
                {
                    down++;
                }else
                {
                    break;
                }
                y++;
			}
            y=currentY-1;
            for (int x = currentX-1; x > 0; x--)
			{
                if(table[x,y]==CPU)
                {
                    up++;
                }else
                {
                    break;
                }
                y--;
			}
            if(up>=down)
            {
                return up;
            }else
            {
                return down;
            }
        }
        
        static int cellValuePlayerOne(GridModel table, int currentX, int currentY, int cellValueRowP1, int cellValueColumnP1, int cellValueAcrossP1)
        {
            int value = 0;
            cellValueAcrossP1 = 0;
            cellValueColumnP1 = 0;
            cellValueRowP1 = 0;
            cellValueAcrossP1=cellValuePlayerOneAcross(table,currentX,currentY);
            cellValueRowP1=cellValuePlayerOneRow(table,currentX,currentY);
            cellValueColumnP1=cellValuePlayerOneColumn(table,currentX,currentY);
            Console.WriteLine("X: "+currentX+" Y: " + currentY + " AcrossP1: "+cellValueAcrossP1+" RowP1: "+cellValueRowP1+" columnP1: " + cellValueColumnP1);

            if (cellValueAcrossP1>=cellValueColumnP1)
            {
                if(cellValueAcrossP1>=cellValueRowP1)
                {
                    value=cellValueAcrossP1;
                }else
                {
                    value=cellValueRowP1;
                }
            }else
            {
                if(cellValueColumnP1>=cellValueRowP1)
                {
                    value=cellValueColumnP1;
                }else
                {
                    value=cellValueRowP1;
                }
            }

            if (value == 4)
            {
                return 2;
            }
            else
            {
                if (value == 3)
                {
                    return 8;
                }
                else
                {
                    if (value == 2)
                    {
                        return 6;
                    }
                    else
                    {
                        if (value == 1)
                        {
                            return 4;
                        }
                        else
                        {
                            if (value == 0)
                            {
                                return 0;
                            }
                            else
                            {
                                return 20;
                            }
                        }
                    }
                }
            }            
        }
        static int cellValuePlayerOneRow(GridModel table, int currentX, int currentY)
        {
            int right=0, left=0;

            for (int r = currentY+1; r < table.Width; r++)
            {
                if (table[currentX, r] == FieldState.PlayerOne)
                {
                    right++;
                }
                else
                {
                    break;
                }
            }
            for (int r = currentY-1; r > 0 ; r--)
            {
                if (table[currentX, r] == FieldState.PlayerOne)
                {
                    left++;
                }
                else
                {
                    break;
                }
            }
            if(right>=left)
            {
                return right;
            }else
            {
                return left;
            }
        }
        static int cellValuePlayerOneColumn(GridModel table, int currentX, int currentY)
        {
            int up = 0, down = 0;
            for (int c = currentX+1; c < table.Height; c++)
            {
                if (table[c, currentY] == FieldState.PlayerOne)
                {
                    down++;
                }
                else
                {
                    break;
                }
            }
            for (int c = currentX-1; c > 0; c--)
            {
                if (table[c, currentY] == FieldState.PlayerOne)
                {
                    up++;
                }
                else
                {
                    break;
                }
            }
            if(up>=down)
            {
                return up;
            }else
            {
                return down;
            }
        }
        static int cellValuePlayerOneAcross(GridModel table, int currentX, int currentY)
        {
            int up = 0, down = 0;
            int y=currentY+1;

            for (int x = currentX+1; x < table.Height; x++)
			{
                if(table[x,y] == FieldState.PlayerOne)
                {
                    down++;
                }else
                {
                    break;
                }
                y++;
			}
            y=currentY-1;
            for (int x = currentX-1; x > 0; x--)
			{
                if(table[x,y]==FieldState.PlayerOne)
                {
                    up++;
                }else
                {
                    break;
                }
                y--;
			}
            if(up>=down)
            {
                return up;
            }else
            {
                return down;
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
