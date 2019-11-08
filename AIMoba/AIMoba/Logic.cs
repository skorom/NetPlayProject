using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using AIMoba.Data;
using AIMoba.Models;

namespace AIMoba
{
    public class Logic
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
        public bool GameEnd(GridModel table, int x, int y, int step)
        {
            int testx = x, testy = y, borderx = 0, bordery = 0;
            int sum = 0;

            //A testdirx és testdiry tömbök határozzák meg az "irányt", amerre a függvény számolja a lépéseket.
            int[] testdirx = { 0, 0, 1, -1, -1, 1, 1, -1 };
            int[] testdiry = { 1, -1, 0, 0, 1, -1, 1, -1 };
            FieldState prev = table[x, y];

            for (int i = 0; i < 8; i++)
            {
                //A testdirx és testdiry tömbök által meghatározott iránynak megfelelően megadja, hogy számolást végző while ciklus meddig mehet (borderx és bordery értéke).
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

                //Lépések számolását végző ciklus
                while (!(testy == bordery || testx == borderx || table[testx, testy] != prev || sum == step))
                {
                    sum++;
                    testx += testdirx[i];
                    testy += testdiry[i];
                }

                if ((i + 1) % 2 == 0)
                {
                    if (sum >= step)
                    {
                        return true;
                    }
                    else
                    {
                        sum = 0;
                    }
                }

                testx = x;
                testy = y;
            }

            return false;
        }
    }
}
