using System;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class WebApiContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public WebApiContext(DbContextOptions<WebApiContext> options) : base(options)
        {
            
        }
        
        public override int SaveChanges()
        {
            //var userEmail = _accessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);
            
            foreach (var entry in ChangeTracker.Entries<IEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        //entry.Entity.CreatedBy = userEmail;
                        entry.Entity.CreatedAt = DateTimeOffset.Now;
                        break;
                    case EntityState.Modified:
                        entry.Property(nameof(IEntity.CreatedBy)).IsModified = false;
                        entry.Property(nameof(IEntity.CreatedAt)).IsModified = false;

                        //entry.Entity.UpdatedBy = userEmail;
                        entry.Entity.UpdatedAt = DateTimeOffset.Now;
                        break;
                }
            }

            var affectedRows = base.SaveChanges();

            return affectedRows;
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(WebApiContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}