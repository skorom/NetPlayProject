using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace AIMoba.Models
{
    public class PlayerModel
    {
        public int ID { get; set; }
        public int score { get; set; }
        public string username { get; set; }
        public string password { get; set; }
    }
}
