using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AIMoba.Models;
using AIMoba.Data;
using AIMoba.Logic;
using AIMoba.Hubs;

namespace AIMoba.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult CreateRoom(){
            string name = HttpContext.Request.Form["name"];
            ViewBag.name = name;
            return View();
        }

        public IActionResult MakeRoom()
        {
            string roomName = HttpContext.Request.Form["roomName"];
            string name = HttpContext.Request.Form["playerName"];

            GameController.currentGames.Add(roomName, new Game(roomName));
            return RedirectToAction("JoinRoom","Home",new { roomname = roomName, name = name});
        }
        //TODO refactor these functions because they are doing the same thing
        public IActionResult RedirectToJoinRoom()
        {
            string roomName = HttpContext.Request.Form["room"];
            string name = HttpContext.Request.Form["name"];

            return RedirectToAction("JoinRoom", "Home", new { roomname = roomName, name = name });
        }

        public IActionResult JoinRoom(string roomName, string name){
            ViewBag.roomName = roomName;
            ViewBag.name = name;
            return View();
        }

        public IActionResult FakeAutentication()
        {
            return View();
        }
        /*
        //egy lépés request fogadása
        [HttpPost]
        public async Task<Message> MakeMove([FromBody] RequestData data)
        { 

            // lépés feldolgozása
            bool success = currentGame.Update(data,message);
            // visszatérési adatok létrehozása
            if (success)
            {
                message.ResponsMessage = true;
                message.Data.Add(new Move(data.position, currentGame.players[data.PlayerID].Mark));

            }
            else
            {
                message.ResponsMessage = false;
            }
            if (!message.EndOfGame&&currentGame.Steps >= currentGame.grid.Width * currentGame.grid.Height)
            {
                message.EndState = 0; // TODO: endofgame függvény beépítése

            }
            return message;
        }*/
       
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
