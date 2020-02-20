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
        public static int CellValue(GridModel grid, int currentX, int currentY, FieldState checkedState)
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

                do
                {
                    if ((sumX + directionsX[i]) >= 0 && (sumY + directionsY[i]) >= 0 && (sumX + directionsX[i]) <= 19 && (sumY + directionsY[i]) <= 19)
                    {
                        if (grid[sumX + directionsX[i], sumY + directionsY[i]] == checkedState)
                        {
                            sum++;
                            sumX += directionsX[i];
                            sumY += directionsY[i];
                        }
                        else
                        {
                            break;
                        }

                    }
                    else
                    {
                        break;
                    }

                } while (sum != 5);
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
    }
}
