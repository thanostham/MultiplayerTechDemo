using Microsoft.EntityFrameworkCore;
using RestApi.Models;

namespace RestApi.Data
{
    public class API_Context : DbContext
    {
        public DbSet<UsersDT> users {  get; set; }
        public API_Context(DbContextOptions<API_Context> options) 
            :base(options)
        { 

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UsersDT>().HasKey(u => u.Id);
        }
    }
}
