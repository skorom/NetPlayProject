using AIMoba.Data;
using AIMoba.Models;
using System.Collections.Generic;
using AIMoba.Logic;

namespace AIMoba.Data
{
    public class Game
    {
        public string ID { get; set; }
        // a játékban eddig megtett lépések száma
        public int Steps { get; set; } = 0;
        // a tábla melyen a játékosok játszanak
        public GridModel grid { get; set; }

        public FieldState LastMark { get; private set; }

        // a játékosokat, robotokat is beleértve tárolja 
        public Dictionary<string, IPlayer> players = new Dictionary<string, IPlayer>();
        public Game(string id)
        {
            grid = new GridModel();
            this.ID = id;
        }

        // új játékos hozzáadása
        public void AddPlayer(string ID)
        {
            players.Add(ID, new Player(MapNumToState(players.Count+1),ID));

            int turnIndex = 0;

            foreach(Player p in players.Values)
            {
                if(p.IsComputer == false)
                {
                    p.Turn = turnIndex++;
                }
            }
            foreach (Player p in players.Values)
            {
                if (p.IsComputer == true)
                {
                    p.Turn = turnIndex++;
                }
            }

        }
        
        public void AddRobot()
        {
            players.Add(""+players.Count , new Robot(MapNumToState(players.Count + 1)));
        }

        // sorszám alapján keresi meg a játékost
        private IPlayer SearchByIndex(int nextTurnIndex)
        {
            foreach (var player in players.Values)
            {
                if (player.Turn == nextTurnIndex)
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
        public bool PlayerMove(string id, Position position)
        {
            // ha létezik a játékos
            if (players.ContainsKey(id))
            {
                IPlayer currentPlayer = players[id];
                // ha a jelenlegi játékos köre következik
                if (Steps % players.Count == currentPlayer.Turn)
                {
                    if (currentPlayer.MakeMove(grid, position))
                    {
                        LastMark = currentPlayer.Mark;
                        Steps++;
                        return true;
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
            }
            return false;
        }

        public bool isGameOver(Position lastMove)
        {
            return GameLogic.GameEnd(grid, lastMove.IPos, lastMove.JPos, 5);
        }

        public bool isRobotNext(Position lastMove)
        {
            return SearchByIndex(Steps % players.Count).IsComputer;
        }

        public Position MoveRobot()
        {
            IPlayer currentPlayer = SearchByIndex(Steps % players.Count);
            if (currentPlayer.IsComputer)
            {
                int aiJPos = 0;
                int aiIPos = 0;
                GameLogic.AI(grid, currentPlayer.Mark, ref aiIPos, ref aiJPos);
                currentPlayer.MakeMove(grid, new Position(aiIPos, aiJPos));
                Steps++;
                LastMark = currentPlayer.Mark;
                return new Position(aiIPos, aiJPos);
            }
            else
            {
                return null;
            }
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
