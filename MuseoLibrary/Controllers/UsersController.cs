using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MuseoLibrary.ApplicationDomain.DTOs;
using MuseoLibrary.ApplicationDomain.Interfaces;
using MuseoLibrary.ApplicationDomain.Models;

namespace MuseoLibrary.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;
        private readonly IMapper _mapper;
        public UsersController(IUsersService usersService, IMapper mapper)
        {
            _usersService = usersService;
            _mapper = mapper;
        }

        [HttpGet (nameof(GetAllUsers))]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _usersService.GetAllUsersAsync();
            return Ok(result);
        }

        [HttpGet("GetUserById/{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var result = await _usersService.GetUserByIdAsync(id);
            return Ok(result);
        }

        [HttpPost(nameof(CreateUser))]
        public async Task<IActionResult> CreateUser([FromBody] UserDto userDto)
        {
            var user = _mapper.Map<UserTrip>(userDto);
            var result = await _usersService.CreateUserAsync(user);
            return Ok(result);
        }

        [HttpPut(nameof(UpdateUser))]
        public async Task<IActionResult> UpdateUser([FromBody] UserDto userDto)
        {
            var user = _mapper.Map<UserTrip>(userDto);
            var result = await _usersService.UpdateUserAsync(user);
            return Ok(result);
        }

        [HttpPut(nameof(UpdatePassword))]
        public async Task<IActionResult> UpdatePassword([FromBody] PasswordDto password)
        {
            var result = await _usersService.UpdatePasswordAsync(password);
            return Ok(result);
        }

        [HttpDelete("DeleteUser/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await _usersService.DeleteUserAsync(id);
            return Ok(result);
        }
    }
}
