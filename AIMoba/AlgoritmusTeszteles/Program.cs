using AlgorithmTestingConsoleApp.Model;
using System;
using System.Linq;

namespace AlgoritmusTeszteles
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            using (var db = new UsersContext())
            {
                var user = new User { name="Ferike",password="bananfa",score=100};
                db.Users.Add(user);
                db.SaveChanges();
            }
            Console.WriteLine("Beírva");

            using (var db = new UsersContext())
            {
                var users = db.Users
                    .OrderBy(u => u.name)
                    .ToList();
                users.ForEach(u => Console.WriteLine(u.ToString()));
            }
        }
    }
}
