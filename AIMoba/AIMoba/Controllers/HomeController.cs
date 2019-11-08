using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AIMoba.Models;
using AIMoba.Data;

namespace AIMoba.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        // a játékok eltárolása
        private static Dictionary<int, Game> dictionary = new Dictionary<int, Game>();
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        // automatikus kiosztása a játék és játékos idk-nek
        public IActionResult Index(int gameID=0, int playerID=0)
        {
            if (gameID == 0)
            {
                gameID = dictionary.Count + 1;
                dictionary.Add(gameID, new Game(gameID));
                return RedirectToAction("index", "home", new { gameID = gameID });
            }

            
            if (playerID == 0)
            {
                playerID = dictionary[gameID].NumberOfPlayers() + 1;
                dictionary[gameID].AddPlayer(playerID);
                return RedirectToAction("index", "home", new { gameID = gameID, playerID = playerID });
            }

            
            return View();
        }

        //egy lépés request fogadása
        [HttpPost]
        public async Task<Message> MakeMove([FromBody] RequestData data)
        {
            // az üzenet melyet majd visszaküld a frontend nek
            Message message = new Message();
            //Jelenlegi játék meghatározása
            Game currentGame;
            if (dictionary.ContainsKey(data.GameID))
            {
                currentGame = dictionary[data.GameID];
            }
            else
            {
                return null;
            }
            // lépés feldolgozása
            bool success = currentGame.Update(data);
            // visszatérési adatok létrehozása
            if (success)
            {
                message.ResponsMessage = "success";
                message.position = data.position;
            }
            else
            {
                message.ResponsMessage = "failed";
            }
            message.EndState = ""; // TODO: endofgame függvény beépítése
            message.EndOfGame = ( currentGame.Steps-1 <= currentGame.grid.Width * currentGame.grid.Height);
            Console.WriteLine("yeeeey");
            return message;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
