using AIMoba.Data;

namespace AIMoba.Models
{
    public class GridModel
    {
        // Az t�mb sz�less�ge �s magass�ga lek�rhet�, de csak k�l�n metodus sor�n �ll�that�
        public int Width { get; private set; }
        public int Height { get; private set; }

        // Az am�ba t�bla
        private FieldState[,] fields;

        public GridModel(int width = 20, int height = 20)
        {
            this.Width = width;
            this.Height = height;
            this.fields = new FieldState[Height, Width];
            ResetCellsToDefault();
        }

        // a Grid classb�l a t�bla x, y helyen l�v� eleme megkaphat�k
        // (nem v�ltoztathat�) [x,y] sz�ntaktik�val
        /* p�ld�ul:
            Grid testGrid = new Grid(); testGrid[2,2]==FieldState.PlayerOne
        */
        public FieldState this[int iPos, int jPos]
        {
            get
            {
                return fields[iPos, jPos];
            }
        }

        // hamis �rt�kkel t�r vissza ha szab�lytalan a l�p�s
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

        // visszaadja a iPos, jPos koordin�t�kon t�rolt mez�t
        public FieldState GetFieldAtPosition(int iPos, int jPos)
        {
            return fields[iPos, jPos];
        }

        // Minden mez�t semlegesre �ll�t (egy j�t�koshoz sem tartozik)
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

        // szab�lyos l�p�s felt�telei
        private bool IsValidMove(int iPos, int jPos)
        {
            // ha a cella foglalt, vagy nem l�tezik akkor a l�p�s hamis
            return (iPos >= 0 && iPos < this.Height &&
                jPos >= 0 && jPos < this.Width &&
                fields[iPos, jPos] == FieldState.None);




        }

    }

}