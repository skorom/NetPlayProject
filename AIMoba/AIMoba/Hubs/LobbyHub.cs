using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using AIMoba.Controllers;
using AIMoba.Data;
using AIMoba.Models;
using System.Threading;

namespace AIMoba.Hubs
{
    public class LobbyHub : Hub
    {
        // Egy felhasználó nevéhez hozzárendeli az idjét
        static Dictionary<string, string> nameToConnection = new Dictionary<string, string>();

        // lock for thread safety
        static object _lock = new object();

        // A névhez a megfelelő id hozzárendelése
        public async Task Subscribe(string name)
        {
            await Task.Run(() =>
            {
                if (!nameToConnection.ContainsKey(name))
                {
                    lock (_lock)
                    {
                        nameToConnection.Add(name, Context.ConnectionId);
                    }
                }
            });
        }

        // [name] nevű játékos meghívása a [roomName] nevű szobába
        public async Task InvitePlayer(string roomName, string name)
        {
            await Task.Run(async () =>
            {
                if (!Lobby.lobbys.ContainsKey(roomName))
                { return; }
                
                if (Lobby.lobbys[roomName].Count >= 4)
                {
                    await Clients.Caller.SendAsync("Message", "Figyelem!", "warning", "Egy szobában maximum 4 játékos tartózkodhat.");
                    return;
                }

                bool isInvited = false;
                if (Lobby.invitations.ContainsKey(name))
                {
                    if (Lobby.invitations[name].Contains(roomName))
                    {
                        isInvited = true;
                    }
                }
                if(!isInvited)
                if (nameToConnection.ContainsKey(name))
                {
                    if (nameToConnection[name] == Context.ConnectionId)
                    {
                        await Clients.Caller.SendAsync("Message", "Figyelem!", "success", "De már itt vagy.");
                        return;
                    }
                    bool isAlreadyInGroup = false;

                    foreach(var player in Lobby.lobbys[roomName])
                    {
                        if(player.Name == name)
                        {
                            isAlreadyInGroup = true;
                            break;
                        }
                    }

                    if (isAlreadyInGroup == true)
                    {
                        await Clients.Caller.SendAsync("Message", "Figyelem!", "warning", name + " nevű játékos már a szobában van.");
                        return;
                    }


                    lock (_lock)
                    {
                        // Ha a meghívott felhasználó rendelkezik szever kapcsolattal
                        if (Lobby.invitations.ContainsKey(name))  // ha már meghívták valahová
                        {
                            Lobby.invitations[name].Add(roomName);
                        }
                        else // Ha ez az első hely ahová meghívták
                        {
                            Lobby.invitations.Add(name, new List<string>() { roomName });
                        }
                    }
                }
                else
                {
                    await Clients.Caller.SendAsync("Message", "Figyelem!", "warning", name + " nevű játékos jelenleg nem elérhető.");
                }


                if (nameToConnection.ContainsKey(name))
                {
                    /* Ha a meghívott felhasználó rendelkezik szever kapcsolattal, akkor
                       Küldve lesz neki egy meghívás, valamint minden már szobában tartozkodónak 
                       el lesz küldve hogy ki lett meghívva */
                    await Clients.Client(nameToConnection[name]).SendAsync("Invited", roomName);
                    await Clients.Caller.SendAsync("Message", "Meghívó elküldve.", "success");
                    if (!isInvited)
                    {

                        await Clients.Group(roomName).SendAsync("AddPlayer", (new PlayerModel()
                        {
                            Name = name,
                            Role = PlayerRights.Játékos,
                            Score = (new UserDAOService()).FindUserByName(name).Score, // TODO: Refactor, Singleton 
                            State = PlayerState.Folyamatban
                        }).Stringify());
                    }

                }

            });
        }

        public async Task DeletePlayerFromRoom(string roomName, string name)
        {
            if (Lobby.lobbys.ContainsKey(roomName))
            {
                PlayerModel current = Lobby.lobbys[roomName].FirstOrDefault(p => p.Name == name);
                if (current != null)
                {
                    if (current.Role != PlayerRights.Robot && current.Role != PlayerRights.Tulajdonos)
                    {
                        await Groups.RemoveFromGroupAsync(nameToConnection[name], roomName);
                        await Clients.Client(nameToConnection[name]).SendAsync("kick");
                    }
                    if (current.Role != PlayerRights.Tulajdonos)
                    {
                        Lobby.lobbys[roomName].Remove(current);
                        await Clients.Group(roomName).SendAsync("DeletePlayer", name);
                    }
                    else
                    {
                        await Clients.Caller.SendAsync("Message", "Figyelem!", "warning", "A tulajdonost nem távolíthatod el.");
                    }

                }
                else if (Lobby.invitations.ContainsKey(name))
                {
                    if (Lobby.invitations[name].Contains(roomName))
                    {
                        Lobby.invitations[name].Remove(roomName);
                        await Clients.Group(roomName).SendAsync("DeletePlayer", name);
                    }
                }

            }

        }


        public async Task AddRobot(string roomName)
        {
            await Task.Run(async () =>
           {


               if (Lobby.lobbys.ContainsKey(roomName))
               {
                   if (Lobby.lobbys[roomName].Count >= 4)
                   {
                       await Clients.Caller.SendAsync("Message", "Figyelem!", "warning", "Egy szobában maximum 4 játékos tartózkodhat.");
                       return;
                   }
                   PlayerModel robot = new PlayerModel()
                   {
                       Name = Robot.GetNewName(),
                       Role = PlayerRights.Robot,
                       Score = 1000,
                       State = PlayerState.Kész
                   };

                   Lobby.lobbys[roomName].Add(robot);
                   await Clients.Group(roomName).SendAsync("AddPlayer", robot.Stringify()); ;
               }
           });
        }

        // Egy szobához való csatlakozás
        public async Task JoinLobby(string roomName, string name)
        {
            await Task.Run(async () =>
            {
                // Ha megvolt hívva a játékos
                if (Lobby.invitations.ContainsKey(name))
                {
                    if (Lobby.invitations[name].Contains(roomName))
                    {
                        if (Lobby.lobbys.ContainsKey(roomName))
                        {
                            // Ha a jelenlegi szobába volt meghívva és a szoba létezik
                            lock (_lock)
                            {
                                // játékos hozzáadása a szobához
                                Lobby.lobbys[roomName].Add(
                                    new PlayerModel()
                                    {
                                        Name = name,
                                        Role = PlayerRights.Játékos,
                                        Score = (new UserDAOService()).FindUserByName(name).Score, // TODO: Refactor, Singelton
                                        State = PlayerState.Csatlakozott
                                    });
                                Lobby.invitations[name].Remove(roomName);
                            }

                            // Csatlakozás a szobához és a szoba minden eddigi tagjának információt küldeni a csatlakozásról
                            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
                            await Clients.Group(roomName).SendAsync("EditOrAddPlayer", Lobby.lobbys[roomName].FirstOrDefault(x => x.Name == name).Stringify());
                        }
                    }
                }
                else
                {
                    // Ha nem volt meghívva a játékos, de a játék létezik, akkor a jelenlegi felhasználó a Tulajdonos
                    lock (_lock)
                    {
                        // Szoba létrehozása
                        Lobby.lobbys.Add(roomName,
                            new List<PlayerModel>() {
                                new PlayerModel() {
                                    Name = name,
                                    Role = PlayerRights.Tulajdonos,
                                    Score = (new UserDAOService()).FindUserByName(name).Score, // TODO: Refactor, Singelton
                                    State = PlayerState.Csatlakozott
                                }
                            });
                    }

                    // Szobához való csatlakozás és információk visszaküldése a Tulajdonosnak
                    await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
                    await Clients.Client(Context.ConnectionId).SendAsync("AddPlayer", Lobby.lobbys[roomName][0].Stringify());
                }
            });
        }

        // Kész státuszra váltás
        public async Task GetReady(string roomName, string name)
        {
            await Task.Run(async () =>
            {
                if (Lobby.lobbys.ContainsKey(roomName))
                {
                    var current = Lobby.lobbys[roomName].FirstOrDefault(x => x.Name == name);
                    if (current != null)
                    {
                        current.State = PlayerState.Kész;
                        await Clients.Group(roomName).SendAsync("PlayerReady", name);
                    }
                }
            });
        }

        // Játék elkezdése
        public async Task StartGame(string roomName)
        {
            if (Lobby.lobbys.ContainsKey(roomName))
            {
                if (Lobby.lobbys[roomName].All(player => player.State == PlayerState.Kész))
                {
                    if (Lobby.lobbys[roomName].Count < 2)
                    {
                        await Clients.Caller.SendAsync("Message", "Figyelem!", "warning", "Egyedül elég unalmas...");
                        return;
                    }
                    GameController.currentGames.Add(roomName, new Game(roomName));
                    foreach (var p in Lobby.lobbys[roomName])
                    {
                        if (p.Role == PlayerRights.Robot)
                        {
                            GameController.currentGames[roomName].AddRobot();
                        }
                    }
                    // ha a szoba létezik és mindenki kész ,akkor mindenkinek elküldi a linket a játék szobához
                    string link = "/Game/YourGame/" + roomName;
                    await Clients.Group(roomName).SendAsync("StartGame", link);
                }
                else
                {
                    await Clients.Caller.SendAsync("Message", "Hiba!", "error", "Mindenkinek készen kell lennie a játék elindításához!");
                }
            }
        }

        private async Task DeleteDiscLobby(string name, string roomName)
        {
            if (Lobby.lobbys.ContainsKey(roomName))
            {
                PlayerModel current = Lobby.lobbys[roomName].FirstOrDefault(p => p.Name == name);
                if (current != null)
                {
                    Lobby.lobbys[roomName].Remove(current);
                    if (current.Role != PlayerRights.Robot)
                    {
                        await Groups.RemoveFromGroupAsync(nameToConnection[name], roomName);
                        await Clients.Client(nameToConnection[name]).SendAsync("kick");
                    }

                    await Clients.Group(roomName).SendAsync("DeletePlayer", name);

                    int numberOfRobots = Lobby.lobbys[roomName].Where(p => p.Role == PlayerRights.Robot).Count();
                    if(numberOfRobots == Lobby.lobbys[roomName].Count)
                    {
                        Lobby.lobbys.Remove(roomName);
                    }
                }

            }

        }

        // Ha a kapcsolat megszakad akkor legyen törölve a név id kapcsolat
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var playerName = nameToConnection.FirstOrDefault(p => p.Value == Context.ConnectionId).Key;
            var room = Lobby.lobbys.FirstOrDefault(r => r.Value.Where(p => p.Name == playerName).Count() > 0);
            if(room.Key != null)
            {
                await DeleteDiscLobby(playerName, room.Key);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, room.Key);
            }

            lock (_lock)
            {
                if (nameToConnection.Values.Contains(Context.ConnectionId))
                {
                    nameToConnection.Remove(nameToConnection.First(x => x.Value == Context.ConnectionId).Key);
                }
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}
