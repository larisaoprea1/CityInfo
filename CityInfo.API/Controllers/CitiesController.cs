using AutoMapper;
using CityInfo.API.Entities;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using System.Text.Json;
using System.Xml.XPath;

namespace CityInfo.API.Controllers
{
    [ApiController]
    //[Authorize(Policy = "MustBeFromCraiova")]
    [Route("api/cities")]
    public class CitiesController: ControllerBase
    {
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;
        const int maxCitiesPageSize = 20;
        public CitiesController(ICityInfoRepository cityInfoRepository, IMapper mapper)
        {
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityWOPointsOfInterestDTO>>> GetCities([FromQuery] string? name, string? searchQuery, int pageNumber = 1, int pageSize = 10)
        {
            if (pageSize > maxCitiesPageSize)
            {
                pageSize = maxCitiesPageSize;
            }

            var (cityEntities, paginationMetadata) = await _cityInfoRepository
                .GetCitiesAsync(name, searchQuery, pageNumber, pageSize);

            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(paginationMetadata));

            return Ok(_mapper.Map<IEnumerable<CityWOPointsOfInterestDTO>>(cityEntities));

        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCity(int id, bool includePointsOfInterest = false)
        {
            var city = await _cityInfoRepository.GetCityAsync(id, includePointsOfInterest);
            if (city == null)
            {
                return NotFound();
            }
            if (includePointsOfInterest)
            {
                return Ok(_mapper.Map<CityDTO>(city));
            }
            return Ok(_mapper.Map<CityWOPointsOfInterestDTO>(city));
        }
        [HttpPost]
        public async Task<ActionResult<CityDTO>> CreateCity(CityForCreationDTO city)
        {
            var cityToCreate = _mapper.Map<Entities.City>(city);
            await _cityInfoRepository.CreateCityAsync(cityToCreate);
            await _cityInfoRepository.SaveChangesAsync();
            var createdCityToReturn = _mapper.Map<Models.CityDTO>(cityToCreate);
            return Ok(createdCityToReturn);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<CityDTO>> UpdateCity(int id,bool includePointsOfInterest, CityForCreationDTO city)
        {
            if (!await _cityInfoRepository.GetIfCityExistsAsync(id))
            {
                return NotFound();
            }
            var cityFromStore = await _cityInfoRepository.GetCityAsync(id, includePointsOfInterest);
            if(cityFromStore == null)
            {
                return NotFound();
            }
            _mapper.Map(city,cityFromStore);
            await _cityInfoRepository.SaveChangesAsync();

            return Content("The info has been updated!");
        }
        [HttpPatch("{id}")]
        public async Task<ActionResult<CityDTO>> PartiallyUpdateCity(int id, bool includePointsOfInterest, JsonPatchDocument<CityForUpdatingDTO> patchDocument)
        {
            if (!await _cityInfoRepository.GetIfCityExistsAsync(id))
            {
                return NotFound();
            }
            var cityFromStore = await _cityInfoRepository.GetCityAsync(id, includePointsOfInterest);
            if (cityFromStore == null)
            {
                return NotFound();
            }
            var cityToPatch = _mapper.Map<CityForUpdatingDTO>(cityFromStore);

            patchDocument.ApplyTo(cityToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryValidateModel(cityToPatch))
            {
                return BadRequest(ModelState);
            }
            _mapper.Map(cityToPatch, cityFromStore);
            await _cityInfoRepository.SaveChangesAsync();

            return Content("The item has been updated!");
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCityAsync(int id, bool includePointsOfInterest)
        {
            if (!await _cityInfoRepository.GetIfCityExistsAsync(id))
            {
                return NotFound();
            }
            var cityToDelete = await _cityInfoRepository.GetCityAsync(id, includePointsOfInterest);
            if(cityToDelete == null)
            {
                return NotFound();
            }
            _cityInfoRepository.DeleteCity(cityToDelete);
            await _cityInfoRepository.SaveChangesAsync();
            return Content("The item has been deleted!");
        }
    }
}
