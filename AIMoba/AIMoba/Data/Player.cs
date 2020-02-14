using AIMoba.Data;
using AIMoba.Models;

namespace AIMoba.Data
{
    public class Player : IPlayer
    {
        public string ID { get; set; }

        public FieldState Mark { get; set; }

        public bool IsComputer => false;

        public int Turn { get; set; }

        public Player(FieldState mark, string ID)
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