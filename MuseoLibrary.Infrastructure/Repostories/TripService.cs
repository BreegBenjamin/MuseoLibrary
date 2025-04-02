using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using MuseoLibrary.ApplicationDomain.DTOs;
using MuseoLibrary.ApplicationDomain.Interfaces;
using MuseoLibrary.ApplicationDomain.Models;
using System.Net;

namespace MuseoLibrary.ApplicationDomain.Repostories
{
    public class TripService : ITripService
    {
        private readonly Container _container;
        private readonly IImageAnalyzer _imageAnalyzer;

        public TripService(Container container, IImageAnalyzer imageAnalyzer)
        {
            _container = container;
            _imageAnalyzer = imageAnalyzer;
        }

        public async Task<ResponseDto<IEnumerable<Trip>>> GetAllTripsAsync()
        {
            var query = _container.GetItemQueryIterator<Trip>("SELECT * FROM c");
            var results = new List<Trip>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response);
            }
            return new ResponseDto<IEnumerable<Trip>> 
            {
                Data = results,
                Message = (results.Any()) ?  "The trips were found" : "no trips registered",
                Success = results.Any()
            };
        }

        public async Task<ResponseDto<Trip>> GetTripByIdAsync(string id)
        {
            try
            {
                var result = await _container.ReadItemAsync<Trip>(id.ToString(), new PartitionKey(id));
                return new ResponseDto<Trip> 
                {
                    Data = result.Resource,
                    Message = (result.StatusCode == HttpStatusCode.Created) ?  "The trip was found"  : "Error",
                    Success = result.StatusCode == HttpStatusCode.Created
                };
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return new ResponseDto<Trip> 
                {
                    Data = new Trip(),
                    Message = "The trip was not found",
                    Success = false
                };
            }
        }

        public async Task<ResponseDto<IEnumerable<Trip>>> GetTripsByUserIdAsync(string userId)
        {
            var query = _container.GetItemQueryIterator<Trip>($"SELECT * FROM c WHERE c.userId = '{userId}'");
            var results = new List<Trip>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response);
            }
            return new ResponseDto<IEnumerable<Trip>>
            {
                Data = results,
                Message = (results.Any()) ? "The trip was found" : "Error",
                Success = results.Any()
            }; ;
        }

        public async Task<ResponseDto<Trip>> CreateTripAsync(TripDto tripDto)
        {
            try
            {
                var trip = await _imageAnalyzer.AnalyzeImageIAAsync(tripDto);

                var result = await _container.CreateItemAsync(trip, new PartitionKey(trip.id));
                return new ResponseDto<Trip>
                {
                    Data = result.Resource,
                    Message = (result.StatusCode == HttpStatusCode.Created) ? "The trip was found" : "Error",
                    Success = result.StatusCode == HttpStatusCode.Created
                };

            }
            catch (Exception ex)
            {
                string ms = ex.Message;
                return new ResponseDto<Trip>
                {
                    Data = new Trip(),
                    Message = "The trip was not fund",
                    Success = false
                };
            }
        }

        public async Task<ResponseDto<Trip>> UpdateTripAsync(Trip trip)
        {
            var result = await _container.UpsertItemAsync(trip, new PartitionKey(trip.id));
            return new ResponseDto<Trip> 
            {
              Data = (result.Resource ?? new Trip()),
              Message = (result.StatusCode == HttpStatusCode.Created) ? "The trip was updated" : "Error to update",
              Success = result.StatusCode == HttpStatusCode.Created
            };
        }

        public async Task<ResponseDto<bool>> DeleteTripAsync(string id)
        {
            try
            {
                await _container.DeleteItemAsync<Trip>(id.ToString(), new PartitionKey(id));
                return new ResponseDto<bool>
                {
                    Message = "The trip was deleted",
                    Data = true,
                    Success = true
                };
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return new ResponseDto<bool>
                {
                    Message = "Error in the proccess",
                    Data = false,
                    Success = false
                };
            }
        }
    }
}
