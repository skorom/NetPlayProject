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
        Az AI függvény a megkapott tábla alapján a referencia szerint érkező aix, aiy pozíciók értékét meghatározza,
        így ad választ a játékosok lépéseire.
        Paraméterei:
        -CPU: az a FieldState változó ami az AIhoz tartozik.
        */
        public static void AI(GridModel table, FieldState CPU, ref int aix, ref int aiy)
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
    }
}
