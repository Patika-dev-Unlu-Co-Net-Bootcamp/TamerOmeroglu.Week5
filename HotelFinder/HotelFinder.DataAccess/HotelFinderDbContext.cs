using HotelFinder.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelFinder.DataAccess
{
    public class HotelFinderDbContext : IdentityDbContext<User>
    {
        public HotelFinderDbContext(DbContextOptions<HotelFinderDbContext> options) : base(options)
        {

        }

        public DbSet<Hotel> Hotels { get; set; }

        public DbSet<Employee> Employees { get; set; }



    }
}
