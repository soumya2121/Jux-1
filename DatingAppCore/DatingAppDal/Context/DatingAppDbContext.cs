using DatingAppDal.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;


namespace DatingAppDal.Context
{
    public class DatingAppDbContext:DbContext
    {
        public DatingAppDbContext(DbContextOptions<DatingAppDbContext>options):base(options)
        {

        }
        public DbSet<Value> Values { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Photo> Photos {get;set;}
        public DbSet<Like> Likes{get;set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
           modelBuilder.Entity<Like>().HasKey(u=> new {u.LikeeId,u.LikerId});

            modelBuilder.Entity<Like>().
            HasOne(u => u.Likee)
            .WithMany(u => u.Liker)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Like>()
            .HasOne(u => u.Liker)
            .WithMany(u => u.Likee)
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
