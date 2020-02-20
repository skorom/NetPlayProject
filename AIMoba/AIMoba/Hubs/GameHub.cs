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
        public async Task Subscribe(string roomname, string name)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomname);
            await Task.Run(() => players.Add(Context.ConnectionId, name));

        }

        public async Task Move(string roomname, Position moveTo)
        {
            await Task.Run(async () =>
            {
                if (GameController.currentGames.ContainsKey(roomname))
                {
                    Game current = GameController.currentGames[roomname];
                    bool isPossibleMove = current.PlayerMove(players[Context.ConnectionId], moveTo);

                    if (isPossibleMove)
                    {

                        await Clients.Group(roomname).SendAsync("WaitForMove", moveTo, current.LastMark);
                        
                        if (!current.isGameOver(moveTo))
                        {
                            Position lastmove = moveTo;
                            while (current.isRobotNext(lastmove))
                            {
                                await Task.Delay(500);
                                Position robotMove = current.MoveRobot();

                                await Clients.Group(roomname).SendAsync("WaitForMove", robotMove, current.LastMark);

                                if (current.isGameOver(robotMove))
                                {
                                    await Clients.Group(roomname).SendAsync("GameEnded", "Robot");
                                    return;
                                }

                                lastmove = robotMove;
                            }
                        }
                        else // A játéknak vége van
                        {
                            await Clients.Group(roomname).SendAsync("GameEnded", players[Context.ConnectionId]);
                        }

                    }

                }

            });
        }
    }
}
