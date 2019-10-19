using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AIMoba.Controllers
{
    public class GameController : Controller
    {
        public IActionResult Game()
        {
            return View();  //Megjeleníti az oldalt.
        }
        
        public class Player
        {
            public int ID { get; set; } //a játékos azonosítója
            public int score { get; set; }  //a játékos lépéseinek száma

            public Player(int ID = 0, int score = 0)
            {
                this.ID = ID;
                this.score = score;
            }

            public void NextMove()
            {
                //játékos lépése
            }

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

            public void NextRobotMove()
            {
                //robot lépése
            }
        }

        public static void ShowWinner(int currentPlayer, int score)
        {
            //eldönti, hogy ki nyert, és megjeleníti.
        }

        public static void RefreshPage()
        {
            //TODO: Oldal frissítése
        }

        static void Main(string[] args)
        {
            GridModel grid;

            Player[] players = new Player[playerNumber];
            players[playerNumber] = new Robot();

            for (int i = 0; i < playerNumber - 1; i++)
            {
                players[i].ID = i + 1;
                players[i].score = 0;
            }

            int currentPlayer;

            for (int i = 0; i < grid.Width * grid.Height; i++) //Minden cellán végigmegy, mert ennyi lépés lesz.
            {
                currentPlayer = i % playerNumber;

                players[currentPlayer].NextMove(); //Következő lépés.

                RefreshPage();  //Frissíti az oldalt, hogy látható legyen a változás.  

                if (GameEnd())   //Megnézi, hogy nyert-e valaki, ha igen, akkor kilép a ciklusból.
                {
                    ShowWinner(currentPlayer, players[currentPlayer].score);
                    break;
                }

            }

        }

    }
}