using AIMoba.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AIMoba.Models
{
    public class PlayerModel
    {
        public string Name { get; set; }
        public int Score { get; set; }
        public PlayerRights Role { get; set; }
        public PlayerState State { get; set; }

        public PlayerModelString Stringify()
        {
            return new PlayerModelString()
            {
                Name = Name,
                Score = Score,
                Role = Role.ToString(),
                State = State.ToString(),
            };
        }
    }

    public class PlayerModelString
    {
        public string Name { get; set; }
        public int Score { get; set; }
        public string Role { get; set; }
        public string State { get; set; }
    }
}
