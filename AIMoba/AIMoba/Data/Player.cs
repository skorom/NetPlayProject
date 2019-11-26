using AIMoba.Data;
using AIMoba.Models;

namespace AIMoba.Data
{
    public class Player : IPlayer
    {
        public int ID { get; set; }
        public FieldState Mark { get; set; }

        public bool IsComputer => false;

        public int Index { get; set; }

        public Player(FieldState mark, int ID = 0)
        {
            this.ID = ID;
            this.Mark = mark;
        }

        // visszatérési értéke a lépés helyességétől függ
        public bool MakeMove(GridModel grid, Position pos)
        {
            return grid.MakeMove(pos.IPos, pos.JPos, this.Mark);
        }

    }
}