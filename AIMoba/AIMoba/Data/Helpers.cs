using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AIMoba.Data
{
    // ezek a classok a kommunikációt seggíttik a frontend és backend között
    public class Message
    {
        // az üzenet "success" vagy "failed"
        public bool ResponsMessage { get; set; }
        // vége van e a játéknak
        public bool EndOfGame { get; set; }
        // milyen állás volt a tálán mikor végelett a játéknak
        public int EndState { get; set; }
        // a pozíció ahová a játékos lépni szeretne
        public List<Move> Data { get; set; } = new List<Move>();
    }

    public class RequestData
    {
        public Position position { get; set; }
        public int PlayerID { get; set; }
        public int GameID { get; set; }
    }

    public class Move
    {
        public Move() { }
        public Move(Position pos, FieldState mark)
        {
            Pos = pos;
            Mark = mark;
        }

        public Position Pos { get; set; }
        public FieldState Mark { get; set; }
    }
    public class Position
    {
        public Position() { }
        public Position(int iPos, int jPos)
        {
            this.IPos = iPos;
            this.JPos = jPos;
        }

        public int IPos { get; set; }
        public int JPos { get; set; }
    }
}
