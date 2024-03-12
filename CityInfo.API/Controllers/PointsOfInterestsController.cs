using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityId}/pointsofinterest")]
    [ApiController]
    public class PointsOfInterestsController : ControllerBase
    {
        private readonly ILogger<PointsOfInterestsController> _logger;
        private readonly IMailServices _mailService;
        private readonly CitiesDataStore _citiesDataStore;

        public PointsOfInterestsController(ILogger<PointsOfInterestsController> logger,
            IMailServices mailServices,
            CitiesDataStore citiesDataStore)
        { 
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mailService = mailServices ?? throw new ArgumentNullException(nameof(mailServices));
            _citiesDataStore = citiesDataStore ?? throw new ArgumentNullException(nameof(citiesDataStore));
        }

        [HttpGet]
        public ActionResult<IEnumerable<PointOfInterestDtio>> GetPointsOfInterest(int cityId)
        {
            try {
                //throw new Exception("Exeprion sample.");

                var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);

                if (city == null)
                {
                    _logger.LogInformation($"City with id {cityId} wasn't" +
                        $" found when accessing points of interest.");
                    return NotFound();
                }
                return Ok(city.PointOfInterest);


            }catch (Exception ex)
            {
                _logger.LogCritical($"Exception while getting points of interest" +
                    $"for city with id {cityId}.", ex);
                return StatusCode(500, "A problem happened while handling your request.");
            }
        }

        [HttpGet("{pointofInterestid}")]

        public ActionResult<PointOfInterestDtio> GetPointOfInterest(
            int cityId, int pointOfInterestId)
        {
            try
            {
                throw new Exception("Exception sample.");

                var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
                if (city == null)
                {
                    return NotFound();
                }
                var pointOfInterest = city.PointOfInterest.
                    FirstOrDefault(c => c.Id == pointOfInterestId);
                if (pointOfInterest == null)
                {
                    return NotFound();
                }
                return Ok(pointOfInterest);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while getting points of in" +
                    $"terest for city with id {cityId}.",ex);
                return StatusCode(501, "A problem happened while hadling " +
                    "your request.");
            }
        }
        [HttpPost("{pointofinterestid}", Name = "GetPointOfInterest")]
        public ActionResult<PointOfInterestDtio> CreatePointOfInterest(
            int cityId,
            PointOfInterestForCreationDto pointOfInterest)
        {
           
            var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
            if( city == null)
            {
                return NotFound();
            }
            var maxPointOfInterestId = _citiesDataStore.Cities.SelectMany(
                c => c.PointOfInterest).Max(p => p.Id);

            var finalPointOfInterest = new PointOfInterestDtio()
            {
                Id = ++maxPointOfInterestId,
                Name = pointOfInterest.Name,
                Description = pointOfInterest.Description
            };
            city.PointOfInterest.Add(finalPointOfInterest);

            return CreatedAtRoute("GetPointOfInterest",
                new
                {
                    cityId = cityId,
                    pointOfInterestId = finalPointOfInterest.Id
                },
                finalPointOfInterest);
        }
        [HttpPut("{pointofinterestid}")]
        public ActionResult UpdatePointOfInterest(int cityId, int pointOfInterestId,
            PointOfInterestForUpdateDto pointOfInterest)
        {
            var city = _citiesDataStore.Cities.FirstOrDefault(c =>
            c.Id == cityId);
            if( city == null )
            {
                return NotFound();
            }

            var pointOfInterestFromStore = city.PointOfInterest
                .FirstOrDefault(c => c.Id == pointOfInterestId);
            if(pointOfInterestFromStore == null)
            {
                return NotFound();
            }

            pointOfInterestFromStore.Name = pointOfInterest.Name;
            pointOfInterestFromStore.Description = pointOfInterest.Description; 
            
            return NoContent();
        }

        [HttpPatch("{pointofinterestid}")]
        public ActionResult PartiallyUpdatePointOfInterest(
            int cityId, int pointOfInterestId,
            JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
        {
            var city = _citiesDataStore.Cities.FirstOrDefault(c
                => c.Id == cityId);
            if( city == null )
            {
                return NotFound();
            }

            var pointOfInterestFromStore = city.PointOfInterest
                .FirstOrDefault(c => c.Id == pointOfInterestId);
            if(pointOfInterestFromStore == null)
            {
                return NotFound();
            }

            var pointOfInterestToPatch =
                new PointOfInterestForUpdateDto()
                {
                    Name = pointOfInterestFromStore.Name,
                    Description = pointOfInterestFromStore.Description
                };

            try
            {
                patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                // Log the exception details
                // Return an appropriate error response
                return StatusCode(500, "Internal server error");
            }


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

            return NoContent();
        }

        [HttpDelete("{pointOfInterestId}")]

        public ActionResult DeletePointOfInterest(int cityId, int pointOfInterestId)
        {
            var city = _citiesDataStore.Cities
            .FirstOrDefault(c => c.Id == cityId);  
            if(city == null)
            {
                return NotFound();
            }
            var pointOfInterestFromStore = city.PointOfInterest
                .FirstOrDefault(c => c.Id == pointOfInterestId);
            if(pointOfInterestFromStore == null)
            {
                return NotFound();
            }    

            city.PointOfInterest.Remove(pointOfInterestFromStore);
            _mailService.Send("Point of interest deleted.",
                $"Point of interest {pointOfInterestFromStore.Name} " +
                $"with id {pointOfInterestFromStore.Id} was deleted.");
            return NoContent();
        }
    }
}
