using MuseoLibrary.ApplicationDomain.DTOs;
using MuseoLibrary.ApplicationDomain.Models;

namespace MuseoLibrary.ApplicationDomain.Interfaces
{
    public interface ITripService
    {
        Task<ResponseDto<IEnumerable<Trip>>> GetAllTripsAsync();
        Task<ResponseDto<Trip>> GetTripByIdAsync(string id);
        Task<ResponseDto<IEnumerable<Trip>>> GetTripsByUserIdAsync(string userId);
        Task<ResponseDto<Trip>> CreateTripAsync(TripDto trip);
        Task<ResponseDto<Trip>> UpdateTripAsync(Trip trip);
        Task<ResponseDto<bool>> DeleteTripAsync(string id);
    }
}
