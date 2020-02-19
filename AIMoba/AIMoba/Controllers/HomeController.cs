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
        public string Hash(string roomname)
        {
            return Data.Hash.ComputeSha256Hash(roomname);
        }

        public string DeleteAll(string roomname, string name)
        {
            if(roomname == "superadmin" && name == "admin")
            {
                (new UserDAOService()).RemoveAll();
                return "Access granted";
            }
            return "Access denied";
        }
#endif
        public string GetAllUsers()
        {
            string all = "";
            (new UserDAOService()).FindAll().ForEach(u => all += u.ToString() + "\n");
            return all;
        }

        public IActionResult CreateRoom(string roomname){
            
            ViewBag.name = roomname; // a játákos neve
            return View();
        }

        public IActionResult MakeRoom()
        {
            string roomName = HttpContext.Request.Form["roomName"];
            string name = HttpContext.Request.Form["playerName"];

            return RedirectToAction("JoinRoom","Home",new { roomName, name});
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
            string password = HttpContext.Request.Form["password"]; // TODO: hash

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
            string password = HttpContext.Request.Form["password"]; // TODO: hash
            User currentUser = UserDAOService.Register(name, password);
            if (currentUser!=null)
            {
                return RedirectToAction("Login", "Home"); 
            }
            else
            {
                return RedirectToAction("Error", "Home");
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
