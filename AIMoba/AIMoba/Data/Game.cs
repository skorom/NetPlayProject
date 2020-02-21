using AIMoba.Data;
using AIMoba.Models;
using System.Collections.Generic;
using AIMoba.Logic;
using System;
using System.Linq;

namespace AIMoba.Data
{
    public class Game
    {
        private object _lock = new object();
        public string ID { get; set; }
        // a játékban eddig megtett lépések száma
        private int Steps { get; set; } = 0;
        // a tábla melyen a játékosok játszanak
        private GridModel grid { get; set; }

        public FieldState LastMark { get; private set; }

        // a játékosokat, robotokat is beleértve tárolja 
        private Dictionary<string, IPlayer> players = new Dictionary<string, IPlayer>();
        public Game(string id)
        {
            grid = new GridModel();
            this.ID = id;
        }

        // új játékos hozzáadása
        public void AddPlayer(string ID)
        {
            lock (_lock)
            {
                players.Add(ID, new Player(MapNumToState(players.Count+1),ID));
            }

            CalculateTurns();
        }
        
        public void AddRobot()
        {
            string id = players.Count.ToString();
            lock (_lock)
            {
                players.Add(id , new Robot(MapNumToState(players.Count + 1),id));
            }
            CalculateTurns();
        }

        private void CalculateTurns()
        {
            int turnIndex = 0;
            List<string> keys = new List<string>(players.Keys);
            foreach(var key in keys)
            {
                if (!players[key].IsComputer)
                {
                    players[key].Turn = turnIndex++;
                }
            }
            foreach (var key in keys)
            {
                if (players[key].IsComputer)
                {
                    players[key].Turn = turnIndex++;
                }
            }
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

        public double GetDifScore()
        {
            int min = 100000, max = -100000;
            var dao = new UserDAOService();
            foreach(var player in players.Values)
            {
                var p = dao.FindUserByName(player.ID);
                int s = p==null?1000:p.Score;
                if(s < min)
                {
                    min = s;
                }
                if (s > max)
                {
                    max = s;
                }
            }
            return max-min;
        }

        public void UpdateScores(string winnerName)
        {
            // Rn = Ro + K(W - We)
            // We = 1 / (exp10(-dr / 400) + 1)
            var dao = new UserDAOService();
            double avg = this.GetDifScore();
            double add = 32 * (1 - (1 / (Math.Pow(10, -avg / 400) + 1)));
            double lose = 32 * (-(1 / (Math.Pow(10, -avg  / 400) + 1)));
            int newScore;
            if (players.ContainsKey(winnerName))
            {
                if(players[winnerName].IsComputer == false)
                {
                    newScore = (int)Math.Round(dao.FindUserByName(winnerName).Score + add);
                    dao.UpdateScore(winnerName, newScore);

                }
            }

            foreach(var p in players.Values)
            {
                if (!p.IsComputer)
                {
                    if(p.ID != winnerName)
                    {
                        newScore = (int)Math.Round(dao.FindUserByName(p.ID).Score + lose);
                        dao.UpdateScore(p.ID, newScore);
                    }
                }
            }
        }

        // vissza adja a játékosok számát
        public int NumberOfPlayers()
        {
            return players.Count;
        }

        public List<string> GetKeysOfPlayers()
        {
            var tmp = new List<string>(players.Keys);
            return tmp.Where(p => players[p].IsComputer == false).ToList();
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
                GameLogic.AI(grid, currentPlayer.Mark, ref aiIPos, ref aiJPos, players.Count);
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
