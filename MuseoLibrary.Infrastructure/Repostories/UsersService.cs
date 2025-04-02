using Microsoft.Azure.Cosmos;
using MuseoLibrary.ApplicationDomain.DTOs;
using MuseoLibrary.ApplicationDomain.Interfaces;
using MuseoLibrary.ApplicationDomain.Models;
using System.Net;

namespace MuseoLibrary.ApplicationDomain.Repostories
{
    public class UsersService : IUsersService
    {
        private readonly Container _container;
        private readonly ICryptographyPassword _cryptographyPassword;

        public UsersService(Container container , ICryptographyPassword cryptographyPassword)
        {

            _container = container;
            _cryptographyPassword = cryptographyPassword;
        }

        public async Task<ResponseDto<IEnumerable<UserTrip>>> GetAllUsersAsync()
        {
            var query = _container.GetItemQueryIterator<UserTrip>("SELECT * FROM c");
            var results = new List<UserTrip>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response);
            }
            return new ResponseDto<IEnumerable<UserTrip>> 
            {
                Data = results,
                Message = (results.Any()) ? "The users were found" : "no users registered",
                Success = results.Any()
            };
        }

        public async Task<ResponseDto<UserTrip>> GetUserByIdAsync(string id)
        {
            try
            {
                var result = await _container.ReadItemAsync<UserTrip>(id, new PartitionKey(id));
                return new ResponseDto<UserTrip> 
                {
                    Data = result.Resource,
                    Message = (result.StatusCode == HttpStatusCode.OK) ? "The user was found" : "Error",
                    Success = result.StatusCode == HttpStatusCode.OK
                };
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return new ResponseDto<UserTrip> 
                {
                    Data = new UserTrip(),
                    Message = "The user was not found",
                    Success = false
                };
            }
        }

        public async Task<ResponseDto<UserTrip>> GetUserByEmailAsync(string email)
        {
            var query = _container.GetItemQueryIterator<UserTrip>($"SELECT * FROM c WHERE c.email = '{email}'");
            var results = new List<UserTrip>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response);
            }

            return new ResponseDto<UserTrip> 
            {
                Data = results.FirstOrDefault(),
                Message = (results.Any()) ? "The user was found" : "Error",
                Success = results.Any()
            };
        }

        public async Task<ResponseDto<UserTrip>> CreateUserAsync(UserTrip user)
        {
            try
            {
                bool userExist = await UserNameExistsAsync(user.UserName);
                bool emailExist = await EmailExistsAsync(user.Email);

                if (userExist) 
                {
                    return new ResponseDto<UserTrip>
                    {
                        Data = new UserTrip(),
                        Message =  "The User Name is usted",
                        Success = false
                    };
                }
                if (emailExist) 
                {
                    return new ResponseDto<UserTrip>
                    {
                        Data = new UserTrip(),
                        Message = "The Email is registed",
                        Success = false
                    };
                }

                user.id = Guid.NewGuid().ToString();
                user.UserDescription = string.Empty;
                user.UserImageProfile = string.Empty;

                user.Password = _cryptographyPassword.Hash(user.Password);
                var result = await _container.CreateItemAsync(user, new PartitionKey(user.id));
                return new ResponseDto<UserTrip> 
                {
                    Data = result.Resource,
                    Message = (result.StatusCode == HttpStatusCode.Created) ? "The user was created" : "Error",
                    Success = result.StatusCode == HttpStatusCode.Created
                };
            }
            catch (Exception ex)
            {
                string ms = ex.Message;
                return new ResponseDto<UserTrip>
                {
                    Data = new UserTrip(),
                    Message = "Error was ocurred",
                    Success = false
                };
            }
        }

        public async Task<ResponseDto<UserTrip>> UpdateUserAsync(UserTrip user)
        {
            var result = await _container.UpsertItemAsync(user, new PartitionKey(user.id));
            return new ResponseDto<UserTrip> 
            {
                Data = result.Resource,
                Message = (result.StatusCode == HttpStatusCode.OK) ? "The user was updated" : "Error",
                Success = result.StatusCode == HttpStatusCode.OK
            };
        }

        public async Task<ResponseDto<bool>> DeleteUserAsync(string id)
        {
            try
            {
                bool isDeleted = false;
                string msResponse = string.Empty;
                var response = await _container.ReadItemAsync<UserTrip>(id, new PartitionKey(id));

                if (response != null)
                {
                    await _container.DeleteItemAsync<UserTrip>(id, new PartitionKey(id));
                    isDeleted = true;
                    msResponse = "User deleted successfully.";
                }
                return  new ResponseDto<bool> 
                {
                    Data = isDeleted,
                    Message = msResponse,
                    Success = isDeleted  
                };
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return new ResponseDto<bool> 
                {
                    Data = false,
                    Message = "The user was not found",
                    Success = false

                };
            }
        }

        public async Task<ResponseDto<bool>> UpdatePasswordAsync(PasswordDto passwordDto)
        {
            var user = GetUserByIdAsync(passwordDto.Id!).Result.Data;
            if (user == null) 
            {
                return new ResponseDto<bool>
                {
                    Data = false,
                    Message = "The user was not found",
                    Success = false
                };
            }
            
            bool passwordIsCorrect = _cryptographyPassword.Verify(passwordDto.OldPassword, user.Password);

            if (passwordIsCorrect)
            {
                user.Password = _cryptographyPassword.Hash(passwordDto.NewPassword);
                await UpdateUserAsync(user);
            }
            return new ResponseDto<bool> 
            {
                Data = passwordIsCorrect,
                Message = (passwordIsCorrect) ? "The password was updated" : "Error",
                Success = passwordIsCorrect
            };
        }

        private async Task<bool> UserNameExistsAsync(string userName) 
        {
            var query = "SELECT * FROM c WHERE c.userName = @userName";
            QueryDefinition queryDefinition = new QueryDefinition(query)
                .WithParameter("@userName", userName);

            FeedIterator<int> resultSetIterator = _container.GetItemQueryIterator<int>(queryDefinition);

            while (resultSetIterator.HasMoreResults)
            {
                FeedResponse<int> response = await resultSetIterator.ReadNextAsync();
                int count = response.FirstOrDefault();
                return count > 0;
            }

            return false;
        }
        private async Task<bool> EmailExistsAsync(string email)
        {
            var queryDefinition = new QueryDefinition("SELECT VALUE COUNT(1) FROM c WHERE c.Email = @email")
                        .WithParameter("@email", email);

            FeedIterator<int> resultSetIterator = _container.GetItemQueryIterator<int>(queryDefinition);
            int count = 0;

            while (resultSetIterator.HasMoreResults)
            {
                FeedResponse<int> response = await resultSetIterator.ReadNextAsync();
                count = response.FirstOrDefault();
            }

            return count > 0;
        }
    }
}
