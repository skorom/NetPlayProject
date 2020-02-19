using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AIMoba.Data;
using AIMoba.Models;

namespace AIMoba.Logic
{
    public static partial class GameLogic
    {
        public class Cells
        {
            public int value;
            public int x;
            public int y;
        }

        public static void AI(GridModel grid, FieldState CPU, ref int aix, ref int aiy, int players)
        {
            int[,] valuesTable = new int[grid.Height, grid.Width];

            GetValues(grid, valuesTable, players);

            int tempMax = 0;
            List<Cells> bestValues = new List<Cells>();
            Random rnd = new Random();
            int random = 0;

            for (int x = 0; x < grid.Height; x++)
            {
                for (int y = 0; y < grid.Width; y++)
                {
                    if (valuesTable[x, y] >= tempMax)
                    {
                        tempMax = valuesTable[x, y];
                    }
                }
            }
            for (int x = 0; x < grid.Height; x++)
            {
                for (int y = 0; y < grid.Width; y++)
                {
                    if (valuesTable[x, y] == tempMax)
                    {
                        bestValues.Add(new Cells { value = tempMax, x = x, y = y });
                    }
                }
            }
            random = rnd.Next(0, bestValues.Count - 1);
            aix = bestValues[random].x;
            aiy = bestValues[random].y;

        }
    }
}
