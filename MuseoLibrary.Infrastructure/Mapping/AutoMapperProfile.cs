using AutoMapper;
using MuseoLibrary.ApplicationDomain.DTOs;
using MuseoLibrary.ApplicationDomain.Models;

namespace MuseoLibrary.Mapping
{
    class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<UserTrip, UserDto>();
            CreateMap<UserDto, UserTrip>();
            CreateMap<Trip, TripDto>();
            CreateMap<TripDto, Trip>();
        }
    }
}
