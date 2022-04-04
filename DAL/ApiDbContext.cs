using Microsoft.EntityFrameworkCore;
using Play2GetherAPI.Models;

namespace Play2GetherAPI.DAL
{
    public class ApiDbContext:DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
            .UseSqlite("Data Source = Database\\Play2GetherApiDatabase.db;");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Login>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }

        public virtual DbSet<Login> Logins { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Activitie> Activities { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<Place> Places { get; set; }
        public virtual DbSet<PlaceProposition> PlacePropositions { get; set; }
        public virtual DbSet<UserEvent> UserEvents { get; set; }
    }
}
