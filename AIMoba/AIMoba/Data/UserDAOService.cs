using AIMoba.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AIMoba.Data;

namespace AIMoba.Data
{
    //Data Acces Object (Adat elérési objektum)
    public class UserDAOService
    {
        public User Register(string name, string password)
        {
            if (name.StartsWith("Robot"))
            {
                return null;
            }
            using (UserContext db = new UserContext())
            {
                if (!db.Users.Select(u => u.Name).Contains(name))
                {
                    User user = new User { Name = name, Password = Hash.ComputeSha256Hash(password), Score = 1200 };
                    db.Users.Add(user);
                    db.SaveChanges();
                    return user;
                }
                else
                {
                    return null;
                }
                
            }
        }
        public bool Authenticate(string name, string password)
        {
            string hashedPwd = Hash.ComputeSha256Hash(password);
            using (UserContext db = new UserContext())
            {
                return db.Users.FirstOrDefault(u => u.Name == name && u.Password == hashedPwd) != null;                                
            }
        }
        public void Delete(User user)
        {
            using (var db = new UserContext())
            {
                db.Users.Remove(user);
                db.SaveChanges();
            }
        }

        public void RemoveAll()
        {
            using (var db = new UserContext())
            {
                db.Users.RemoveRange(FindAll());
                db.SaveChanges();
            }
        }

        public void UpdateScore(string name, int newScore)
        {
            using(var db = new UserContext())
            {
                var current = db.Users.FirstOrDefault(u => u.Name == name);
                if(current != null)
                {
                    current.Score = newScore;
                    db.SaveChanges();
                }
            }
        }
        public User FindUserById(int id)
        {
            using (var db = new UserContext())
            {
                return db.Users.FirstOrDefault(u=>u.Id==id);
            }
        }
        public User FindUserByName(string name)
        {
            using (var db = new UserContext())
            {
                return db.Users.FirstOrDefault(u => u.Name == name);
            }
        }
        public List<User> FindAll()
        {
            using (var db = new UserContext())
            {
                return db.Users.ToList();                 
            }
        }
    }
}
