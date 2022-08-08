
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using VigilantMeerkat.Db.Model;

namespace VigilantMeerkat.Db
{
    public class UserDbContext : DbContext
    {
        public DbSet<User> Users { get; private set; }

        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {

        }
    }
}
