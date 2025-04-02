using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MuseoLibrary.ApplicationDomain.DTOs;
using MuseoLibrary.ApplicationDomain.Interfaces;
using MuseoLibrary.ApplicationDomain.Models;

namespace MuseoLibrary.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripController : ControllerBase
    {
        private readonly ITripService _tripService;
        private readonly IMapper _mapper;
        public TripController(ITripService tripService, IMapper mapper)
        {
            _tripService = tripService;
            _mapper = mapper;
        }

        [HttpGet(nameof(GetAllTrips))]
        public async Task<IActionResult> GetAllTrips()
        {
            var result = await _tripService.GetAllTripsAsync();
            return Ok(result);
        }

        [HttpGet("GetTripById/{id}")]
        public async Task<IActionResult> GetTripById(string id)
        {
            var result = await _tripService.GetTripByIdAsync(id);
            return Ok(result);
        }

        [HttpPost(nameof(RegisterTrip))]
        public async Task<IActionResult> RegisterTrip([FromBody] TripDto tripRequest)
        {
            var result = await _tripService.CreateTripAsync(tripRequest);
            return Ok(result);
        }

        [HttpPut(nameof(UpdaTrip))]
        public async Task<IActionResult> UpdaTrip([FromBody] TripDto tripRequest)
        {
            var trip = _mapper.Map<Trip>(tripRequest);
            var result = await _tripService.UpdateTripAsync(trip);
            return Ok(result);
        }

        [HttpDelete("DeleteTrip{id}")]
        public async Task<IActionResult> DeleteTrip(string id)
        {
            var result = await _tripService.DeleteTripAsync(id);
            return Ok(result);
        }
    }
}
