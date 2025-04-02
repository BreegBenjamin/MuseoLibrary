using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using MuseoLibrary.UploadImagesService.Model;
using MuseoLibrary.UploadImagesService.Services;

var builder = WebApplication.CreateBuilder(args);

// variables.
string blobStorageConnectionString = builder.Configuration["AzureBlobStorage:ConnectionString"]!;
string containerName = builder.Configuration["AzureBlobStorage:containerName"]!;
string serviceBusConnectionString = builder.Configuration["AzureServiceBus:ConnectionString"]!;
string queueName = builder.Configuration["AzureServiceBus:queueName"]!;

//services
builder.Services.AddSingleton(new ServiceBusClient(serviceBusConnectionString));
builder.Services.AddSingleton(sp =>
{
    var client = sp.GetRequiredService<ServiceBusClient>();
    return client.CreateProcessor(queueName, new ServiceBusProcessorOptions());
});
builder.Services.AddSingleton(sp =>
{
    var client = sp.GetRequiredService<ServiceBusClient>();
    return client.CreateSender(queueName);
});
builder.Services.AddSingleton(sp => 
{
   return new BlobContainerClient(blobStorageConnectionString, containerName);
});
builder.Services.AddScoped<ContainerImagesUpload>();


builder.Services.AddOpenApi();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();
app.UseAuthentication();

app.UseHttpsRedirection();


// upload image temporal anazyer
app.MapPost("/upload", async ([FromForm] IFormFile file, [FromForm] string userId, [FromServices] ContainerImagesUpload container) 
    => await container.UploadFilesToStorageAsync(file, userId))
.WithName("uploadImage")
.Accepts<IFormFile>("multipart/form-data")
.DisableAntiforgery()
.Produces<string>(StatusCodes.Status200OK);

// send message to service bus
app.MapPost("/sendMessageImage", async ([FromForm]  IFormFileCollection files, [FromForm] string userId,
                                        [FromForm] string tripId, [FromServices] ContainerImagesUpload container) 
    => await container.UploadImageBach(files, userId, tripId))
    .WithName("sendMessage")
    .Accepts<ImageMessage>("multipart/form-data")
    .DisableAntiforgery()
    .Produces<string>(StatusCodes.Status200OK);

app.Run();
