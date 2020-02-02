using AIMoba.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AIMoba.Data
{
    //Data Acces Object (Adat elérési objektum)
    public class UserDAOService
    {
        public User Register(string name, string password)
        {
            using (UserContext db = new UserContext())
            {
                if (!db.Users.Select(u => u.Name).Contains(name))
                {
                    User user = new User { Name = name, Password = password, Score = 0 };
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
            using (UserContext db = new UserContext())
            {
                return db.Users.FirstOrDefault(u => u.Name == name && u.Password == password) != null;                                
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
        public User FindUserById(int id)
        {
            using (var db = new UserContext())
            {
                return db.Users.FirstOrDefault(u=>u.Id==id);
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
