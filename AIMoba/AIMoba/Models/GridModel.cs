using AIMoba.Data;

namespace AIMoba.Models
{
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

}