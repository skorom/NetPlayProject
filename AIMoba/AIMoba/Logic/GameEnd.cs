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
        /*
         A GameEnd függyvény megvizsgálja a beérkező x, y pozíciók alapján, hogy a lépést végrehajtó játékosnak összegyűlt-e a szükséges mennyiségű szomszédos lépése.
         Igaz értéket ad vissza ha összegyűlt, tehát a játéknak vége.
         Hamis értéket ad vissza ha még nem gyűlt össze.

         Paraméterei:
            x: Amely sorba a játékos megtette lépését.
            y: Amely oszlopba a játékos megtette lépését.
            step: Ahány szomszédos lépésnek össze kell gyűlnie a nyereséghez.
        */
        public static bool GameEnd(GridModel table, int x, int y, int step)
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

                while (!(testy == bordery || testx == borderx || table[testx, testy] != prev || sum == step + 1))
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
