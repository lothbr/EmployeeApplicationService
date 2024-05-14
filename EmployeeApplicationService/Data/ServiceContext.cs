
using EmployeeApplicationService.DTOs;
using EmployeeApplicationService.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeApplicationService.Data
{
    public  class ServiceContext : DbContext
    {
        private IConfigService _configService;
        public ServiceContext(IConfigService configService)
        {
            _configService = configService;
        }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<ApplicationData> Applications { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseCosmos(_configService.GetCosmosEndpoint(),_configService.GetCosmoskey(),_configService.GetCosmosDbName());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Profile>()
                 .ToContainer("Profiles")
                 .HasPartitionKey(e => e.ProfileId);

            //modelBuilder.Entity<Question>()
            //     .ToContainer("Questions")
            //     .HasPartitionKey(e => e.QuestionId);

            modelBuilder.Entity<ApplicationData>()
                .ToContainer("ApplicationData")
                .HasPartitionKey(e => e.Id);

            modelBuilder.Entity<ApplicationData>().OwnsMany(e => e.Questions);
                
        }
    }
}
