using AutoMapper;

namespace CityInfo.API.Profiles
{
    public class PointOfInterestProfile : Profile
    {
        public PointOfInterestProfile()
        {

            CreateMap<Entities.PointOfInterest,Models.PointOfInterestDTO>();
            CreateMap<Models.PointOfInterestForCreationDTO,Entities.PointOfInterest>();
            CreateMap<Models.PointOfInterestForUpdatingDTO, Entities.PointOfInterest>();
            CreateMap<Entities.PointOfInterest, Models.PointOfInterestForUpdatingDTO>();

        }
    } 
}
