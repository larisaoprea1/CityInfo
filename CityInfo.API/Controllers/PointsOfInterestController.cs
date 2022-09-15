using CityInfo.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityId}/pointsofinterest")]
    [ApiController]
    public class PointsOfInterestController : ControllerBase
    {
        private readonly ILogger<PointsOfInterestController> _logger;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger)
        {
            _logger = logger ??throw new ArgumentNullException(nameof(logger));
        }
        [HttpGet]
        public ActionResult<IEnumerable<PointOfInterestDTO>> GetPointsOfInterest(int cityId)
        { 
            try
            { 
                var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
                if (city == null)
                {
                    _logger.LogInformation($"City with id {cityId} was not found");
                    return NotFound();
                }
                return Ok(city.PointOfInterests); 
            }
            catch(Exception ex)
            {
                _logger.LogCritical($"Exceptions while getting points of interest for city with id {cityId}", ex);
                    throw;
            }
        }
        [HttpGet("{pointOfInterestId}", Name = "GetPointOfInterest")]
        public ActionResult<PointOfInterestDTO> GetPointOfInterest(
            int cityId, int pointOfInterestId)
        {
            var city = CitiesDataStore.Current.Cities
                .FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            // find point of interest
            var pointOfInterest = city.PointOfInterests
                .FirstOrDefault(c => c.Id == pointOfInterestId);
            if (pointOfInterest == null)
            {
                return NotFound();
            }

            return Ok(pointOfInterest);
        }
        [HttpPost]
        public ActionResult<PointOfInterestDTO> CreatePointOfInterest(int cityId, PointOfInterestForCreationDTO pointOfInterest)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c=>c.Id== cityId);
            if(city == null)
            {
                return NotFound();
            }
            var maxPointOfInterestId = CitiesDataStore.Current.Cities.SelectMany(c => c.PointOfInterests).Max(p => p.Id);
            var finalPointOfInterest = new PointOfInterestDTO()
            {
                Id = ++maxPointOfInterestId,
                Name = pointOfInterest.Name,
                Description = pointOfInterest.Description

            };
            city.PointOfInterests.Add(finalPointOfInterest);
            return CreatedAtRoute("GetPointOfInterest", 
                new
                {
                    cityId= cityId,
                    pointOfInterestId = finalPointOfInterest.Id

                },
                finalPointOfInterest);
        }
        [HttpPut("{pointOfInterestId}")]
        public ActionResult<PointOfInterestDTO> UpdatePointOfInterest(int cityId, int pointOfInterestId, PointOfInterestForCreationDTO pointOfInterest)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }
            var pointOfInterestFromStore = city.PointOfInterests.FirstOrDefault(p => p.Id == pointOfInterestId);
            if (pointOfInterestFromStore == null)
            {
                return NotFound();
            }
            pointOfInterestFromStore.Name= pointOfInterest.Name;
            pointOfInterestFromStore.Description= pointOfInterest.Description;

            return Content("The info has been updated!");
        }

        [HttpPatch("{pointOfInterestId}")]
        public ActionResult PartiallyUpdatePointOfInterest(
            int cityId, int pointOfInterestId,
            JsonPatchDocument<PointOfInterestForUpdatingDTO> patchDocument)
        {
            var city = CitiesDataStore.Current.Cities
                .FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterestFromStore = city.PointOfInterests
                .FirstOrDefault(c => c.Id == pointOfInterestId);
            if (pointOfInterestFromStore == null)
            {
                return NotFound();
            }

            var pointOfInterestToPatch =
                   new PointOfInterestForUpdatingDTO()
                   {
                       Name = pointOfInterestFromStore.Name,
                       Description = pointOfInterestFromStore.Description
                   };

            patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryValidateModel(pointOfInterestToPatch))
            {
                return BadRequest(ModelState);
            }

            pointOfInterestFromStore.Name = pointOfInterestToPatch.Name;
            pointOfInterestFromStore.Description = pointOfInterestToPatch.Description;

            return Content("The item has been updated!");
        }
        [HttpDelete("{pointOfInterestId}")]
        public ActionResult DeletePointOfInterest(int cityId, int pointOfInterestId)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c=>c.Id== cityId);
            if(city == null)
            {
                return NotFound();
            }
            var pointOfInterestToDelete = city.PointOfInterests.FirstOrDefault(p => p.Id == pointOfInterestId);
            if(pointOfInterestToDelete == null)
            {
                return NotFound();
            }
            city.PointOfInterests.Remove(pointOfInterestToDelete);
            return Content("The item has been deleted!");
            
        }
    }
}
