using CityInfo.API.Models;

namespace CityInfo.API
{
    public class CitiesDataStore
    {
        public List<CityDTO> Cities { get; set; }
        public static CitiesDataStore Current { get; }= new CitiesDataStore();
        public CitiesDataStore()
        {
            Cities = new List<CityDTO>()
            {
                new CityDTO()
                {
                    Id = 1, 
                    Name =" New York City",
                    Description = "The one with that big park",
                    PointOfInterests = new List<PointOfInterestDTO>
                    {
                        new PointOfInterestDTO()
                        {
                           Id=1,
                           Name= "Statue of Liberty",
                           Description= " Big "
                        }
                    }
                },
                new CityDTO()
                {
                    Id = 2,
                    Name = "Craiova",
                    Description = "The city I was born in",
                    PointOfInterests = new List<PointOfInterestDTO>
                    {
                        new PointOfInterestDTO()
                        {
                           Id=1,
                           Name= "Parcul Romanescu",
                           Description= " Parcul mare "
                        },
                        new PointOfInterestDTO()
                        {
                           Id=2,
                           Name= "Centrul vechi",
                           Description= " Multe magazine si cafenele "
                        }
                    }

                },
                new CityDTO()
                {
                    Id = 3,
                    Name = "Paris",
                    Description = "The city with the big tower",
                    PointOfInterests = new List<PointOfInterestDTO>
                    {
                        new PointOfInterestDTO()
                        {
                           Id=1,
                           Name= "Eiffle Tower",
                           Description= " Big tower "
                        },
                        new PointOfInterestDTO()
                        {
                           Id=2,
                           Name= "The Louvre",
                           Description= " Amazing art "
                        }
                    }

                }
            };
        }
    }
}
