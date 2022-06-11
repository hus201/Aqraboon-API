using ModelsRepository.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace DataAccessRepository.Context
{
    public class MainContext:DbContext
    {
        public MainContext(DbContextOptions<MainContext> context):base(context)
        {

        }

        public DbSet<User> User { get; set; }
        public DbSet<ServiceType> ServiceType { get; set; }
        public DbSet<Service> Service { get; set; }
        public DbSet<Request> Request { get; set; }
        public DbSet<Report> Report { get; set; }
        public DbSet<AcceptedRequest> AcceptedRequest { get; set; }
        public DbSet<DeliveredRequest> DeliveredRequest { get; set; }
        public DbSet<FailedRequest> FailedRequest { get; set; }
        public DbSet<RequestReceivers> RequestReceivers { get; set; }
        public DbSet<ServiceAttachment> ServiceAttachment { get; set; }
        public DbSet<UserRating> UserRating { get; set; }
        public DbSet<VolunteerInfo> VolunteerInfo { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder dbContextOptionsBuilder)
        {
            base.OnConfiguring(dbContextOptionsBuilder);
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfigurationRoot config = builder.Build();
            var conString = config.GetConnectionString("DefaultConnection");
            dbContextOptionsBuilder.UseSqlServer(conString);
        }

        //
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

    }
}
