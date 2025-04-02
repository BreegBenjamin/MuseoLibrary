using MuseoLibrary.ApplicationDomain.DTOs;
using MuseoLibrary.ApplicationDomain.Models;

namespace MuseoLibrary.ApplicationDomain.Interfaces
{
    public interface IImageAnalyzer
    {
        Task<Trip> AnalyzeImageIAAsync(TripDto dto);
    }
}
