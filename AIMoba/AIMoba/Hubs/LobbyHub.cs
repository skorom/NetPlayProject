using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using AIMoba.Controllers;
using AIMoba.Data;
using AIMoba.Models;

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
            await Task.Run(() => {
                if (!nameToConnection.ContainsKey(name))
                {
                    lock (_lock)
                    {
                        nameToConnection.Add(name, Context.ConnectionId);
                    }
                }
            });
        }

        // [name] nevű játékos meghívása a [roomname] nevű szobába
        public async Task InvitePlayer(string roomName, string name)
        {
            await Task.Run(async () => {
                lock (_lock)
                {
                    if (nameToConnection.ContainsKey(name))
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

                if (nameToConnection.ContainsKey(name))
                {
                    /* Ha a meghívott felhasználó rendelkezik szever kapcsolattal, akkor
                       Küldve lesz neki egy meghívás, valamint minden már szobában tartozkodónak 
                       el lesz küldve hogy ki lett meghívva */
                    await Clients.Client(nameToConnection[name]).SendAsync("Invited", roomName);
                    await Clients.Group(roomName).SendAsync("AddPlayer", (new PlayerModel()
                    {
                        Name = name,
                        Role = PlayerRights.Játékos,
                        Score = 999,
                        State = PlayerState.Folyamatban
                    }).Stringify());
                    
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
                                        Score = 999,
                                        State = PlayerState.Csatlakozott
                                    });
                            }

                            // Csatlakozás a szobához és a szoba minden eddigi tagjának információt küldeni a csatlakozásról
                            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
                            await Clients.Group(roomName).SendAsync("EditOrAddPlayer", Lobby.lobbys[roomName].FirstOrDefault(x => x.Name == name).Stringify());
                        }
                    }
                }
                else if (GameController.currentGames.ContainsKey(roomName) && !Lobby.lobbys.ContainsKey(roomName)) 
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
                                    Score = 999,
                                    State = PlayerState.Csatlakozott
                                }
                            });
                    }

                    // Szobához való csatlakozás és információk visszaküldése a Tulajdonosnak
                    await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
                    await Clients.Client(Context.ConnectionId).SendAsync("AddPlayer", Lobby.lobbys[roomName][0].Stringify());
                }
            });
            // TODO: groupok törlése 
            
        }

        // Kész státuszra váltás
        public async Task GetReady(string roomName, string name)
        {
            await Task.Run(async () =>
            {
                if (Lobby.lobbys.ContainsKey(roomName))
                {
                    var current = Lobby.lobbys[roomName].FirstOrDefault(x => x.Name == name);
                    if(current != null)
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
                if(Lobby.lobbys[roomName].All( player => player.State == PlayerState.Kész))
                {
                    // ha a szoba létezik és mindenki kész ,akkor mindenkinek elküldi a linket a játék szobához
                    string link = "/Game/YourGame/" + roomName;
                    await Clients.Group(roomName).SendAsync("StartGame", link);
                }
                else
                {
                    await Clients.Caller.SendAsync("Message", "Mindenkinek késszen kell lennie a játék megkezdéséhez");
                }
            }
        }

        // Ha a kapcsolat megszakad akkor legyen törölve a név id kapcsolat
        public override async Task OnDisconnectedAsync(Exception exception)
        {
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
