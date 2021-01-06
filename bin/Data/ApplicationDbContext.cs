using bin.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bin.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
           // LoadUsers();
        }

        public DbSet<User> Users { get; set; }

        //public void LoadUsers()
        //{
        //    User user = new User() { Username = "Test", Password = "Test" };
        //    Users.Add(user);
        //    user = new User() { Username = "Test2", Password = "Test2" };
        //    Users.Add(user);
        //}

        //public List<User> GetUsers()
        //{
        //    return Users.Local.ToList<User>();
        //}
    }
}
