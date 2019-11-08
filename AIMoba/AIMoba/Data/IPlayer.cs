using AIMoba.Models;

namespace AIMoba.Data
{
    // meghatározza hogy mit kell kifejteni egy classnak ahoz hogy játékosként lehessen kezelni
    public interface IPlayer
    {
        public int ID { get; set; }
        // sorszám
        public int Index { get; set; }
        // "jel" amit a táblára tesz
        public FieldState Mark { get; set; }
        public bool IsComputer { get; }
        //maga a lépés mehanizmusa
        public bool MakeMove(GridModel grid, Position pos);
    }
}

