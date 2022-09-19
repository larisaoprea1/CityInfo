using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityId}/pointsofinterest")]
    [Authorize]
    [ApiController]
    public class PointsOfInterestController : ControllerBase
    {
        private readonly ILogger<PointsOfInterestController> _logger;
        private readonly ILocalMailService _localMailService;
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger,ILocalMailService localMailService, ICityInfoRepository cityInfoRepository, IMapper mapper)
        {
            _logger = logger ??throw new ArgumentNullException(nameof(logger));
            _localMailService = localMailService ?? throw new ArgumentNullException(nameof(localMailService));
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PointOfInterestDTO>>> GetPointsOfInterest(
             int cityId)
        {
            if (!await _cityInfoRepository.GetIfCityExistsAsync(cityId))
            {
                _logger.LogInformation(
                    $"City with id {cityId} wasn't found when accessing points of interest.");
                return NotFound();
            }

            var pointsOfInterestForCity = await _cityInfoRepository
                .GetPointsOfInterestForCityAsync(cityId);

            return Ok(_mapper.Map<IEnumerable<PointOfInterestDTO>>(pointsOfInterestForCity));
        }

        [HttpGet("{pointOfInterestId}", Name ="GetPointOfInterest")]
        public async Task<ActionResult<PointOfInterestDTO>> GetPointOfInterest(
            int cityId, int pointOfInterestId)
        {
            if (!await _cityInfoRepository.GetIfCityExistsAsync(cityId))
            {
                return NotFound();
            }

            var pointOfInterest = await _cityInfoRepository
                .GetPointOfInterestForCityAsync(cityId, pointOfInterestId);

            if (pointOfInterest == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<PointOfInterestDTO>(pointOfInterest));
        }

        [HttpPost]
        public async Task<ActionResult<PointOfInterestDTO>> CreatePointOfInterest(int cityId, PointOfInterestForCreationDTO pointOfInterest)
        {
           if(!await _cityInfoRepository.GetIfCityExistsAsync(cityId))
           {
                return NotFound();
           }
            var finalPointOfInterest = _mapper.Map<Entities.PointOfInterest>(pointOfInterest);
            await _cityInfoRepository.AddPointOfInterestToACityAsync(cityId,
                                                                     finalPointOfInterest);
            await _cityInfoRepository.SaveChangesAsync();
            var createdPointOfInterestToReturn = _mapper.Map<Models.PointOfInterestDTO>(finalPointOfInterest);
            return CreatedAtRoute("GetPointOfInterest",
                new
                {
                    cityId = cityId,
                    pointOfInterestId = createdPointOfInterestToReturn
                }, createdPointOfInterestToReturn);
        }
        [HttpPut("{pointOfInterestId}")]
        public async Task<ActionResult<PointOfInterestDTO>> UpdatePointOfInterest(int cityId, int pointOfInterestId, PointOfInterestForCreationDTO pointOfInterest)
        {
            if (!await _cityInfoRepository.GetIfCityExistsAsync(cityId))
            {
                return NotFound();
            }
            var pointOfInterestFromStore = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
            if (pointOfInterestFromStore == null)
            {
                return NotFound();
            }
            _mapper.Map(pointOfInterest, pointOfInterestFromStore);
            await _cityInfoRepository.SaveChangesAsync();

            return Content("The info has been updated!");
        }

        [HttpPatch("{pointOfInterestId}")]
        public async Task<ActionResult> PartiallyUpdatePointOfInterest(
            int cityId, int pointOfInterestId,
            JsonPatchDocument<PointOfInterestForUpdatingDTO> patchDocument)
        {
            if (!await _cityInfoRepository.GetIfCityExistsAsync(cityId))
            {
                return NotFound();
            }

            var pointOfInterestFromStore =await _cityInfoRepository
                .GetPointOfInterestForCityAsync(cityId, pointOfInterestId);    
            if (pointOfInterestFromStore == null)
            {
                return NotFound();
            }

            var pointOfInterestToPatch = _mapper.Map<PointOfInterestForUpdatingDTO>(pointOfInterestFromStore);

            patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryValidateModel(pointOfInterestToPatch))
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(pointOfInterestToPatch, pointOfInterestFromStore);
            await _cityInfoRepository.SaveChangesAsync();

            return Content("The item has been updated!");
        }

        [HttpDelete("{pointOfInterestId}")]
        public async Task<ActionResult> DeletePointOfInterestAsync(int cityId, int pointOfInterestId)
        {
            if (!await _cityInfoRepository.GetIfCityExistsAsync(cityId))
            {
                return NotFound();
            }
            var pointOfInterestToDelete = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
            if (pointOfInterestToDelete == null)
            {
                return NotFound();
            }
            _cityInfoRepository.DeletePointOfInterest(pointOfInterestToDelete);

            await _cityInfoRepository.SaveChangesAsync();

            _localMailService.Send("Point of interest deleted",
                $"Point of interest {pointOfInterestToDelete.Name} with id {pointOfInterestToDelete.Id}");
            return Content("The item has been deleted!");

        }
    }
}
