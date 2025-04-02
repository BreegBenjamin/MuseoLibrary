using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using MuseoLibrary.UploadImagesService.Model;
using Newtonsoft.Json;

namespace MuseoLibrary.UploadImagesService.Services
{
    public class ContainerImagesUpload
    {
        private readonly ServiceBusSender _serviceBusSender;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public ContainerImagesUpload(ServiceBusSender serviceBusSender, IConfiguration configuration, 
                                     ILogger<ContainerImagesUpload> logger )
        {
            _configuration = configuration;
            _serviceBusSender = serviceBusSender;
            _logger = logger;
        }
        public async Task<BlobResponse> UploadFilesToStorageAsync(IFormFile file, string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                    throw new Exception("Id is null and is required");

                string connectionString = _configuration["AzureBlobStorage:ConnectionString"]!;
                var client = new BlobContainerClient(connectionString, userId);

                await client.CreateIfNotExistsAsync(PublicAccessType.Blob);

                if (file == null || file.Length == 0)
                    throw new Exception("No files uploaded.");

                var blobClient = client.GetBlobClient(file.FileName);
                using var stream = file.OpenReadStream();

                string contentType = TypeImageValidator.GetContentType(file.FileName);
                if (!TypeImageValidator.IsValidContentType(contentType))
                {
                    throw new Exception("Error in the type of file admitted");
                }

                var headers = new BlobHttpHeaders
                {
                    ContentType = contentType
                };

                await blobClient.UploadAsync(stream, new BlobUploadOptions { HttpHeaders = headers }, cancellationToken: default);
                var response = new BlobResponse 
                { 
                    Url = blobClient.Uri.ToString(), 
                    Message = "OK", 
                    Status = true
                };
                return response;
            }
            catch (Exception ex)
            {
                return new BlobResponse 
                { 
                    Url = string.Empty, 
                    Message = ex.Message, 
                    Status = false 
                };
            }
        }
        public async Task<IResult> UploadImageBach(IFormFileCollection files, string userId, string tripId) 
        {
            try
            {
                List<ImageMessage> messages = new();
                foreach (var file in files)
                {
                    var response = await UploadFilesToStorageAsync(file, userId);

                    if (response.Url == string.Empty)
                        continue;

                    var message = new ImageMessage()
                    {
                        UserId = userId,
                        TripId = tripId,
                        ImageUrl = response.Url!,
                        FileName = file.FileName
                    };
                    messages.Add(message);
                }
                await SendMessage(messages);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { Message = ex.Message, Status = false });
            }

            return Results.Ok(new { Message = "OK", Status = true });
        }
        private async Task<IResult> SendMessage(List<ImageMessage> messages)
        {
            try
            {
                using var messageBatch = await _serviceBusSender.CreateMessageBatchAsync();

                foreach (var message in messages)
                {
                    var jsonMessage = JsonConvert.SerializeObject(message);

                    if (!messageBatch.TryAddMessage(new ServiceBusMessage(jsonMessage)))
                    {
                        return Results.Ok(new { Message = $"The message {message.FileName} is too large to fit in the batch.", Status = false });
                    }
                }

                if (messageBatch.Count > 0)
                {
                    await _serviceBusSender.SendMessagesAsync(messageBatch);
                }
                else
                {
                    return Results.Ok(new { Message = "No messages were added to the batch.", Status = false });
                }
                _logger.LogInformation($"Trying to send {messages.Count} messages...");
                _logger.LogInformation($"Batch size: {messageBatch.Count}");

                return Results.Ok(new { Message = "Messages sent successfully", Status = true });
            }
            catch (Exception ex)
            {
                return Results.Ok(new { Message = $"Error sending messages: {ex.Message}", Status = false });
            }
        }

    }
}