using MuseoLibrary.ApplicationDomain.DTOs;
using MuseoLibrary.ApplicationDomain.Models;

namespace MuseoLibrary.ApplicationDomain.Interfaces
{
    public interface IUsersService
    {
        Task<ResponseDto<IEnumerable<UserTrip>>> GetAllUsersAsync();
        Task<ResponseDto<UserTrip>> GetUserByIdAsync(string id);
        Task<ResponseDto<UserTrip>> GetUserByEmailAsync(string email);
        Task<ResponseDto<UserTrip>> CreateUserAsync(UserTrip user);
        Task<ResponseDto<UserTrip>> UpdateUserAsync(UserTrip user);
        Task<ResponseDto<bool>> DeleteUserAsync(string id);
        Task<ResponseDto<bool>> UpdatePasswordAsync(PasswordDto passwordDto);
    }
}
