using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.DbContexts
{
    public class CityInfoContext:DbContext
    {
        public CityInfoContext(DbContextOptions<CityInfoContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<City>()
                .HasData(
                new City("New York")
                {
                    Id = 1,
                    Description= " The one with that big park"
                },
                 new City("Craiova")
                {
                    Id = 2,
                     Description = "The city I was born in"
                 },
                new City("Paris")
                {
                    Id = 3,
                    Description = "The city with the big tower"
                }
                ) ;
            modelBuilder.Entity<PointOfInterest>()
                .HasData(
                new PointOfInterest("Statue of Liberty")
                {   
                    Id= 1,
                    CityId=1,
                    Description=" Big"

                },
                 new PointOfInterest("Parcul Romanescu")
                 {
                     Id = 2,
                     CityId = 2,
                     Description = " Big"

                 },
                  new PointOfInterest("Gradina botanica")
                  {
                      Id = 3,
                      CityId = 2,
                      Description = " Big"

                  },
                   new PointOfInterest("Tower")
                   {
                       Id = 4,
                       CityId = 3,
                       Description = " Big"

                   },
                    new PointOfInterest("Notre Dame")
                    {
                        Id = 5,
                        CityId = 3,
                        Description = " Big"

                    }
                );
            base.OnModelCreating(modelBuilder);
        }

        internal Task<IEnumerable<City>> ToListAsync()
        {
            throw new NotImplementedException();
        }

        public DbSet<City> Cities { get; set; }
        public DbSet<PointOfInterest> PointOfInterests { get; set; }
       
    }
}
