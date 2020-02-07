using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AIMoba.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public int Score { get; set; }

        public override string ToString()
        {
            return "id: " + Id + ", name: " + Name + ", password: " + Password + ", score: " + Score;
        }
    }

}
