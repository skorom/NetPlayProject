using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AIMoba.Models;

namespace AIMoba.Data
{
    public static class Lobby
    {
        public static Dictionary<string, List<string>> invitations = new Dictionary<string, List<string>>();

        public static Dictionary<string, List<PlayerModel>> lobbys = new Dictionary<string, List<PlayerModel>>();

    }
}
