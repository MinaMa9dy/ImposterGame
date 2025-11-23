using Imposter.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imposter.Infrastructure.Dbcontext
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //Rooms
            modelBuilder.Entity<Room>()
            .HasOne(r => r.Host)
            .WithMany()  // host does NOT have collection of rooms
            .HasForeignKey(r => r.HostId)
            .OnDelete(DeleteBehavior.SetNull);

            //Players
            modelBuilder.Entity<Player>()
            .HasOne(p => p.Room)
            .WithMany(r => r.Players)
            .HasForeignKey(p => p.RoomId)
            .OnDelete(DeleteBehavior.SetNull);
        }
        public DbSet<Player> players { get; set; }
        public DbSet<Room> rooms { get; set; }
        public DbSet<SecretWord> secretWords { get; set; }
        public DbSet<Connection> connections { get; set; }
    }
}
