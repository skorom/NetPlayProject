
namespace AIMoba.Models
{
    // Maximum 4 j�t�kos t�mogatott
    public enum FieldState
    {
        None, PlayerOne, PlayerTwo, PlayerThree, PlayerFour
    }

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
        // TODO: IsValidMove met�dust implement�lni
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

        public void SetSize(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this.fields = new FieldState[Width, Height];
            ResetCellsToDefault();
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

        private bool IsValidMove(int iPos, int jPos)
        {

            return true;
        }

    }

}