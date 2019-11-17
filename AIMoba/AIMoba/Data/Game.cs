using AIMoba.Data;
using AIMoba.Models;
using System.Collections.Generic;
using AIMoba.Logic;

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
        public Dictionary<int,IPlayer> players = new Dictionary<int, IPlayer>();
        public Game(int id=0, int numberOfRobots=1)
        {
            grid = new GridModel();
            for (int i = 0; i < numberOfRobots; i++)
                players.Add(i+1,new Robot(MapNumToState(players.Count+1)));
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
            int i = 0;
            // végig megy a játékosokon és sorszámot ad nekik
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
        public bool Update(RequestData data, Message message)
        {
            // ha létezik a játékos
            if (players.ContainsKey(data.PlayerID))
            {
                IPlayer currentPlayer = players[data.PlayerID];
                // ha a jelenlegi játékos köre következik
                if (Steps % players.Count == currentPlayer.Index)
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

                if (GameLogic.GameEnd(grid, data.position.IPos, data.position.JPos, 5))
                {
                    message.EndOfGame = true;
                    message.EndState = 1;
                    return true;
                }
                // ameddig robot következik a robot lép
                if (SearchByIndex(Steps % players.Count)!= null && SearchByIndex(Steps % players.Count).IsComputer)
                {
                    currentPlayer = SearchByIndex(Steps % players.Count);
                    int aiJPos = 0;
                    int aiIPos = 0;
                    GameLogic.AI(grid, currentPlayer.Mark, ref aiIPos, ref aiJPos);
                    message.Data.Add(new Move(new Position(aiIPos, aiJPos), currentPlayer.Mark));
                    currentPlayer.MakeMove(grid, new Position(aiIPos,aiJPos));
                    Steps++;
                    if (GameLogic.GameEnd(grid, aiIPos, aiJPos, 5))
                    {
                        message.EndOfGame = true;
                        message.EndState = -1;
                    }
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
