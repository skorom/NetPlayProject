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
                                    GameController.currentGames.Remove(roomName);
                                    await Clients.Group(roomName).SendAsync("GameEnded", "Robot");
                                    return;
                                }

                                lastmove = robotMove;
                            }
                        }
                        else // A játéknak vége van
                        {
                              GameController.currentGames.Remove(roomName);
                              await Clients.Group(roomName).SendAsync("GameEnded", players[Context.ConnectionId]);
                        }

                    }

                }

            });
        }
    }
}
