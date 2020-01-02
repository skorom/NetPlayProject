using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using AIMoba.Controllers;
using AIMoba.Data;

namespace AIMoba.Hubs
{
    public class LobbyHub : Hub
    {
        static Dictionary<string, string> nameToConnection = new Dictionary<string, string>();
        public async Task InvitePlayer(string name, string roomName)
        {
            await Task.Run(() => {
                if (nameToConnection.ContainsKey(name))
                {
                    lock (Lobby.invitations)
                    {
                        if (Lobby.invitations.ContainsKey(name))
                        {
                            Lobby.invitations[nameToConnection[name]].Add(roomName);
                        }
                        else
                        {
                            Lobby.invitations.Add(nameToConnection[name], new List<string>() { roomName });
                        }
                    }
                }
            
            });
            await Clients.Client(nameToConnection[name]).SendAsync("Invited", roomName);
        }

        public async Task StartGame(string roomname)
        {
            if (Lobby.lobbys.ContainsKey(roomname))
            {
                string link = "/Game/YourGame/" + roomname;
                await Clients.Group(roomname).SendAsync("StartGame", link);
            }
        }

        public async Task JoinLobby(string roomname, string name)
        {
            await Task.Run(() =>
            {

                lock (Lobby.lobbys)
                {
                    if (Lobby.lobbys.ContainsKey(roomname))
                    {
                        Lobby.lobbys[roomname].Add(name);
                    }
                    else
                    {
                        Lobby.lobbys.Add(roomname, new List<string>() { name });
                    }
                    Groups.AddToGroupAsync(Context.ConnectionId, roomname);
                }

            });
            // TODO: groupok törlése 
            await Clients.Group(roomname).SendAsync("PlayerJoined", name + "csatlakozott");
            
        }

        public async Task Subscribe(string name)
        {
            await Task.Run(() => {
                if (!nameToConnection.ContainsKey(name))
                {
                    nameToConnection.Add(name, Context.ConnectionId);
                }
            });
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (nameToConnection.Values.Contains(Context.ConnectionId))
            {
                nameToConnection.Remove(nameToConnection.First(x => x.Value == Context.ConnectionId).Key);
            }
            if (Lobby.invitations.ContainsKey(Context.ConnectionId))
            {
                Lobby.invitations.Remove(Context.ConnectionId);
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}
