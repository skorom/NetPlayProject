using AIMoba.Data;
using AIMoba.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AIMoba.Logic
{
    public static partial class GameLogic
    {
        public static void GetValues(GridModel grid, int[,] valuesTable, int players)
        {
            int tempValue = 0, tempMaxValue = 0;
            int currentX = 0, currentY = 0;
            FieldState checkedState = FieldState.None;
            FieldState[] playerFieldStates = { FieldState.PlayerOne, FieldState.PlayerTwo, FieldState.PlayerThree, FieldState.PlayerFour };

            for (int x = 0; x < grid.Height; x++)
            {
                for (int y = 0; y < grid.Width; y++)
                {
                    if (grid[x, y] == FieldState.None)
                    {
                        tempMaxValue = 0;
                        for (int i = 0; i < players; i++)
                        {
                            currentX = x;
                            currentY = y;
                            checkedState = playerFieldStates[i];
                            tempValue = CellValue(grid, currentX, currentY, checkedState);

                            if (tempValue > tempMaxValue)
                            {
                                tempMaxValue = tempValue;
                            }
                        }
                        valuesTable[x, y] = tempMaxValue;
                    }
                }
            }
        }
    }
}
