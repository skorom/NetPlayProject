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
        public string ResponsMessage { get; set; }
        // vége van e a játéknak
        public bool EndOfGame { get; set; }
        // milyen állás volt a tálán mikor végelett a játéknak
        public string EndState { get; set; }
        // a pozíció ahová a játékos lépni szeretne
        public Position position { get; set; }
    }

    public class RequestData
    {
        public Position position { get; set; }
        public int PlayerID { get; set; }
        public int GameID { get; set; }
    }
    public class Position
    {
        public int IPos { get; set; }
        public int jPos { get; set; }
    }
}
