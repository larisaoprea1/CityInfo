﻿namespace CityInfo.API.Models
{
    public class CityDTO
    {
        public int  Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int NumberOfPointsOfInterest
        {
            get
            {
                return PointOfInterests.Count;
            }
        }
        public ICollection<PointOfInterestDTO> PointOfInterests { get; set; } = new List<PointOfInterestDTO>();
    }
}
