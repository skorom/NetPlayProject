using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AIMoba.Data;
 
namespace AIMoba.Controllers
{
    public class GameController : Controller
    {
        // a játékok eltárolása
        public static Dictionary<string, Game> currentGames = new Dictionary<string, Game>();

        public IActionResult YourGame(string roomname, string name)
        {
            if (currentGames.ContainsKey(roomname))
            {
                currentGames[roomname].AddPlayer(name);
            }
            ViewBag.roomname = roomname;
            ViewBag.name = name;
            return View();
        }
    }
}