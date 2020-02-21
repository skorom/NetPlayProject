using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using AIMoba.Controllers;
using AIMoba.Data;

namespace AIMoba.Hubs
{
    public class GameHub:Hub
    {
        static Dictionary<string, string> players = new Dictionary<string, string>();
        public async Task Subscribe(string roomName, string name)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
            await Task.Run(() => players.Add(Context.ConnectionId, name));

        }

        public async Task Move(string roomName, Position moveTo)
        {
            await Task.Run(async () =>
            {
                if (GameController.currentGames.ContainsKey(roomName))
                {
                    Game current = GameController.currentGames[roomName];
                    bool isPossibleMove = current.PlayerMove(players[Context.ConnectionId], moveTo);

                    if (isPossibleMove)
                    {

                        await Clients.Group(roomName).SendAsync("WaitForMove", moveTo, current.LastMark);
                        
                        if (!current.isGameOver(moveTo))
                        {
                            Position lastmove = moveTo;
                            while (current.isRobotNext(lastmove))
                            {
                                Position robotMove = current.MoveRobot();

                                await Clients.Group(roomName).SendAsync("WaitForMove", robotMove, current.LastMark);

                                if (current.isGameOver(robotMove))
                                {
                                    UserDAOService dao = new UserDAOService();
                                    GameController.currentGames[roomName].UpdateScores("Robot");
                                    foreach (var key in GameController.currentGames[roomName].GetKeysOfPlayers())
                                    {
                                            await Clients.Client(players.First(p => p.Value == key).Key).SendAsync("Message", "Az új pontod: "
                                                + dao.FindUserByName(key).Score,
                                                "error");
                                                                         }
                                    GameController.currentGames.Remove(roomName);
                                    await Clients.Group(roomName).SendAsync("GameEnded", "Robot");
                                    return;
                                }

                                lastmove = robotMove;
                            }
                        }
                        else // A játéknak vége van
                        {                            
                            UserDAOService dao = new UserDAOService();
                            GameController.currentGames[roomName].UpdateScores(players[Context.ConnectionId]);
                            await Clients.Caller.SendAsync("Message", "Az új pontod: " +dao.FindUserByName(players[Context.ConnectionId]).Score, "success");
                            foreach(var key in GameController.currentGames[roomName].GetKeysOfPlayers())
                            {
                                if (key != players[Context.ConnectionId])
                                {
                                    await Clients.Client(players.First(p => p.Value == key).Key).SendAsync("Message", "Az új pontod: "
                                        + dao.FindUserByName(key).Score,
                                        "error");
                                }
                            }
                            
                            GameController.currentGames.Remove(roomName);
                            await Clients.Group(roomName).SendAsync("GameEnded", players[Context.ConnectionId]);
                        }

                    }

                }

            });
        }
    }
}
