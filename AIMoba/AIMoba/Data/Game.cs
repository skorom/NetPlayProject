using AIMoba.Data;
using AIMoba.Models;
using System.Collections.Generic;

namespace AIMoba.Data
{
    public class Game
    {
        public int ID { get; set; }
        // a játékban eddig megtett lépések száma
        public int Steps { get; set; } = 0;
        // a tábla melyen a játékosok játszanak
        public GridModel grid { get; set; }

        // a játékosokat, robotokat is beleértve tárolja 
        private Dictionary<int,IPlayer> players = new Dictionary<int, IPlayer>();
        public Game(int id=0, int numberOfRobots=1)
        {
            grid = new GridModel();
            for (int i = 0; i < numberOfRobots; i++)
                players.Add(id,new Robot(MapNumToState(players.Count+1)));
            this.ID = id;
            // sorszám megállapíttása
            ReIndex();
        }

        // új játékos hozzáadása
        public void AddPlayer(int ID)
        {
            players.Add(ID, new Player(MapNumToState(players.Count+1),ID));
            // sorszám megállapíttása
            ReIndex();
        }

        private void ReIndex()
        {
            int i = 1;
            // végig megy a játékosokon és sorszámot ad nekik, fontos hogy a sorszám nem lehet 0
            foreach (var player in players.Values)
            {
                if(!player.IsComputer)
                    player.Index = i++;
            }
            foreach (var player in players.Values)
            {
                if (player.IsComputer)
                    player.Index = i++;
            }
        }

        // sorszám alapján keresi meg a játékost
        private IPlayer SearchByIndex(int index)
        {
            foreach (var player in players.Values)
            {
                if (player.Index == index)
                {
                    return player;
                }
            }
            return null;
        }

        // vissza adja a játékosok számát
        public int NumberOfPlayers()
        {
            return players.Count;
        }

        // kezeli az adat feldolgozását ( az adat a kontrollertől való ) 
        // visszatérési értéke akkor true ha a lépés szabályos és a saját körében történt
        public bool Update(RequestData data)
        {
            // ha létezik a játékos
            if (players.ContainsKey(data.PlayerID))
            {
                IPlayer currentPlayer = players[data.PlayerID];
                // ha a jelenlegi játékos köre következik
                if (Steps % players.Count == 0)
                {
                    if(currentPlayer.MakeMove(grid, data.position))
                    {
                        Steps++;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
                // ameddig robot kvetkezik a robot lép
                while (SearchByIndex(Steps % players.Count)!= null && SearchByIndex(Steps % players.Count).IsComputer)
                {
                    currentPlayer = SearchByIndex(Steps % currentPlayer.Index);
                    currentPlayer.MakeMove(grid, null);
                    Steps++;
                }
            }
            return true;
        }
        public static FieldState MapNumToState(int num)
        {
            switch (num)
            {
                case 1:
                    return FieldState.PlayerOne;
                case 2:
                    return FieldState.PlayerTwo;
                case 3:
                    return FieldState.PlayerThree;
                case 4:
                    return FieldState.PlayerFour;
                default:
                    return FieldState.None;
            }
        }


    }

}
