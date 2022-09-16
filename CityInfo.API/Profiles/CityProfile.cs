using AutoMapper;

namespace CityInfo.API.Profiles
{
    public class CityProfile: Profile
    {
        public CityProfile()
        {
            CreateMap<Entities.City, Models.CityWOPointsOfInterestDTO>();
            CreateMap<Entities.City, Models.CityDTO>();
            
        }
    }
}
