using AIMoba.Models;

namespace AIMoba.Data
{
    // meghat�rozza hogy mit kell kifejteni egy classnak ahoz hogy j�t�kosk�nt lehessen kezelni
    public interface IPlayer
    {
        public int ID { get; set; }
        // sorsz�m
        public int Index { get; set; }
        // "jel" amit a t�bl�ra tesz
        public FieldState Mark { get; set; }
        public bool IsComputer { get; }
        //maga a l�p�s mehanizmusa
        public bool MakeMove(GridModel grid, Position pos);
    }
}

