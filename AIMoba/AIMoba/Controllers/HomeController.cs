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

        public IActionResult CreateRoom(string roomname){
            
            ViewBag.name = roomname; // a játákos neve
            return View();
        }

        public IActionResult MakeRoom()
        {
            string roomName = HttpContext.Request.Form["roomName"];
            string name = HttpContext.Request.Form["playerName"];

            GameController.currentGames.Add(roomName, new Game(roomName));
            return RedirectToAction("JoinRoom","Home",new { roomName, name});
        }

        public IActionResult RedirectToJoinRoom()
        {
            string roomName = HttpContext.Request.Form["roomName"];
            string name = HttpContext.Request.Form["playerName"];

            return RedirectToAction("JoinRoom", "Home", new { roomName, name});
        }

        public IActionResult JoinRoom(string roomName, string name){
            ViewBag.roomName = roomName;
            ViewBag.name = name;
            if (Data.Lobby.lobbys.ContainsKey(roomName))
            {
                return View(AIMoba.Data.Lobby.lobbys[roomName]);
            }
            else
            {
                return View(null);
            }
        }

        public IActionResult Lobby()
        {
            string name = HttpContext.Request.Form["name"];
            ViewBag.name = name;
            return View();
        }

        public IActionResult FakeAutentication()
        {
            return View();
        }
       
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
