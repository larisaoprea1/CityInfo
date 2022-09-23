using AutoMapper;

namespace CityInfo.API.Profiles
{
    public class CityProfile: Profile
    {
        public CityProfile()
        {
            CreateMap<Entities.City, Models.CityWOPointsOfInterestDTO>();
            CreateMap<Models.CityDTO, Entities.City>();
            CreateMap<Entities.City, Models.CityDTO>();
            CreateMap<Models.CityForCreationDTO, Entities.City>();
            CreateMap<Models.CityForUpdatingDTO, Entities.City>();

        }
    }
}
