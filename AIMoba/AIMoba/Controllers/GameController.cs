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

        public IActionResult YourGame(string roomName, string name)
        {
            if (currentGames.ContainsKey(roomName))
            {
                currentGames[roomName].AddPlayer(name);
            }
            ViewBag.roomName = roomName;
            ViewBag.name = name;
            return View();
        }        
    }
}