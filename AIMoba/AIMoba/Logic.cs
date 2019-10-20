using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using AIMoba.Data;

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
            lepes: Ahány szomszédos lépésnek össze kell gyűlnie a nyereséghez.
        */
        public bool GameEnd(GridModel table, int x, int y, int lepes)
        {
            /*
            sum: szomszédos lépések száma.
            testx, testy: összehasonlításhoz hassznált pozíciók.
            */
            int sum;
            FieldState prev = table[x, y];
            int testx, testy;

            //A megtett lépéstől jobbra és balra eső lépések összeszámolása.
            sum = 0;
            testx = x;
            testy = y + 1;

            while (!(testy == table.Width + 1 || table[testx, testy - 1] != prev || sum == lepes))
            {
                testy++;
                sum++;
            }

            testy = y - 1;

            while (!(testy == -2 || table[testx, testy + 1] != prev || sum == lepes + 1))
            {
                testy--;
                sum++;
            }

            if (sum - 1 == lepes)
            {
                return true;
            }

            //A megtett lépés felett és alatt lévő lépések összeszámolása.

            sum = 0;
            testy = y;
            testx--;

            while (!(testx == -2 || table[testx + 1, testy] != prev || sum == lepes))
            {
                sum++;
                testx--;
            }

            testx = x + 1;

            while (!(testx == table.Height + 1 || table[testx - 1, testy] != prev || sum == lepes + 1))
            {
                sum++;
                testx++;
            }

            if (sum - 1 == lepes)
            {
                return true;
            }

            //A megtett lépéstől átlós irányba eső lépések összeszámolása.
            sum = 0;
            testx = x - 1;
            testy = y + 1;

            while (!(testx == -2 || testy == table.Width + 1 || table[testx + 1, testy - 1] != prev || sum == lepes))
            {
                sum++;
                testx--;
                testy++;
            }

            testy = y - 1;
            testx = x + 1;

            while (!(testx == table.Height + 1 || testy == -2 || table[testx - 1, testy + 1] != prev || sum == lepes + 1))
            {
                sum++;
                testx++;
                testy--;
            }

            if (sum - 1 == lepes)
            {
                return true;
            }

            sum = 0;
            testx = x + 1;
            testy = y + 1;

            while (!(testx == table.Height + 1 || testy == table.Width + 1 || table[testx - 1, testy - 1] != prev || sum == lepes))
            {
                sum++;
                testx++;
                testy++;
            }

            testx = x - 1;
            testy = y - 1;

            while (!(testx == -2 || testy == -2 || table[testx + 1, testy + 1] != prev || sum == lepes + 1))
            {
                sum++;
                testx--;
                testy--;
            }
            if (sum - 1 == lepes)
            {
                return true;
            }

            return false;
        }
    }
}
