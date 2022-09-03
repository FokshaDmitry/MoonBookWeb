﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoonBookWeb
{
    public class AddDbContext : DbContext
    {
        IConfiguration configuration;
        public DbSet<User> Users { get; set; }
        public DbSet<Posts> Posts { get; set; }
        public DbSet<Comments> Comments { get; set; }
        public DbSet<Reactions> Reactions { get; set; }
        public DbSet<Subscriptions> Subscriptions { get; set; }
        public DbSet<Books> Books { get; set; }
        public DbSet<SubBook> SubBooks { get; set; }
        public DbSet<DeleteList> DeleteList { get; set; }
        public AddDbContext(DbContextOptions<AddDbContext> options) : base(options)
        {
            Database.EnsureCreated();  
        }
    }
}
