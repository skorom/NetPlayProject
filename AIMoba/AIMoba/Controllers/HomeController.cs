﻿using System;
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
using Microsoft.EntityFrameworkCore;

namespace AIMoba.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private UserDAOService UserDAOService = new UserDAOService();
        public HomeController(ILogger<HomeController> logger)
        {

        }

#if DEBUG
        public string Hash(string roomName)
        {
            return Data.Hash.ComputeSha256Hash(roomName);
        }

        public void del(string roomName)
        {
            var dao = new UserDAOService();
            dao.Delete(dao.FindUserByName(roomName));
        }

        public string DeleteAll(string roomName, string name)
        {
            if(roomName == "superadmin" && name == "admin")
            {
                (new UserDAOService()).RemoveAll();
                return "Access granted";
            }
            return "Access denied";
        }
        public string GetAllUsers()
        {
            string all = "";
            (new UserDAOService()).FindAll().ForEach(u => all += u.ToString() + "\n");
            return all;
        }
#endif

        public IActionResult CreateRoom(string roomName){
            
            ViewBag.name = roomName; // a játákos neve
            return View();
        }

        // roomName = játékos neve
        public IActionResult Ranking(string? roomName)
        {
            ViewBag.name = roomName;
            return View();
        }

        public IActionResult MakeRoom()
        {
            string roomName = HttpContext.Request.Form["roomName"];
            string name = HttpContext.Request.Form["playerName"];

            if (!GameController.currentGames.ContainsKey(roomName) && !Data.Lobby.lobbys.ContainsKey(roomName))
            {
            return RedirectToAction("JoinRoom","Home",new { roomName, name});
            }
            else 
            {
                //TODO: alert hogy van ilyen szoba
                return RedirectToAction("Lobby","Home", new { roomName = name }); //ez csak egy random valami hogy legyen returnolva valami
            }

        }

        public IActionResult RedirectToJoinRoom()
        {
            string roomName = HttpContext.Request.Form["roomName"];
            string name = HttpContext.Request.Form["playerName"];
            if (Data.Lobby.invitations.ContainsKey(name))
            {
                if (Data.Lobby.invitations[name].Contains(roomName))
                {
                    return RedirectToAction("JoinRoom", "Home", new { roomName, name});
                }
            }
            return RedirectToAction("Lobby", "Home", new { roomName=name});
        }

        public IActionResult RedirectToJoinRoomFromGame()
        {
            string roomName = HttpContext.Request.Form["roomName"];
            string name = HttpContext.Request.Form["name"];

            return RedirectToAction("JoinRoom", "Home", new { roomName, name });
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

        public IActionResult Lobby(string roomName)
        {
            ViewBag.name = roomName;
            return View();
        }
        public IActionResult RedirectToLobby()
        {
            string name = HttpContext.Request.Form["name"];
            string password = HttpContext.Request.Form["password"]; 

            if (UserDAOService.Authenticate(name, password))
            {
                return RedirectToAction("Lobby", "Home", new { roomName = name });
            }
            else {
                return RedirectToAction("Login", "Home");
            }
        }

        public IActionResult RedirectToLogin()
        {
            string name = HttpContext.Request.Form["name"];
            string password = HttpContext.Request.Form["password"];
            string repassword = HttpContext.Request.Form["repassword"];
            User currentUser = new User();
            if (password == repassword)
            {
                if (UserDAOService.Register(name, password) != null)
                {
                    //TODO: alert hogy a felhasználónév már foglalt
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    return RedirectToAction("Register", "Home");
                }
            }
            else
            {
                return RedirectToAction("Register", "Home");
            }
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
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
