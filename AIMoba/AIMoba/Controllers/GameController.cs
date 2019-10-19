using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AIMoba.Controllers
{
    public class GameController : Controller
    {
        public IActionResult Page()
        {
            return View();
        }

        public abstract class Player
        {
            public int ID { get; set; }
            public int score { get; set; }

            public Player(int ID = 0, int score = 0)
            {
                this.ID = ID;
                this.score = score;
            }

            public abstract void NextMove();
        }
        public class Robot : Player
        {
            public int difficulty { get; private set; } //a robot nehézségi szintje később állítható lesz

            public Robot(int id = 0, int score = 0, int diff = 1)
            {
                this.ID = id;
                this.score = score;
                this.difficulty = diff;
            }

            public override void NextMove()
            {
                //robot lépése
            }
        }

        public static void ShowWinner(int currentPlayer, int score)
        {
            //TODO: eldönti, hogy ki nyert, és megjeleníti.
        }

        public static void RefreshPage()
        {
            //TODO: Oldal frissítése
        }
        
        public static void Game()
        {
            GridModel grid;
            int playerNumber = 3;

            Player[] players = new Player[playerNumber];
            players[playerNumber] = new Robot();

            for (int i = 0; i < playerNumber - 1; i++)
            {
                players[i].ID = i + 1;
                players[i].score = 0;
            }

            Array.ForEach(players, i => i.ID = Convert.ToInt32(i)+1);
            Array.ForEach(players, i => i.score = 0);

            int currentPlayer;

            for (int i = 0; i < grid.Width * grid.Height; i++)
            {
                currentPlayer = i % playerNumber;

                players[currentPlayer].NextMove();

                RefreshPage();

                if (GameEnd())
                {
                    ShowWinner(currentPlayer, players[currentPlayer].score);
                    break;
                }
            }
        }

        

        


