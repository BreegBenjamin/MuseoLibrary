using Azure;
using Azure.AI.Vision.ImageAnalysis;
using Azure.Messaging.ServiceBus;
using FluentValidation;
using Microsoft.Azure.Cosmos;
using MuseoLibrary.ApplicationDomain.Interfaces;
using MuseoLibrary.ApplicationDomain.Repostories;
using MuseoLibrary.Infrastructure.Services;
using MuseoLibrary.Validations;

var builder = WebApplication.CreateBuilder(args);

//variables
string serviceBusConnectionString = builder.Configuration["AzureServiceBus:ConnectionString"]!;

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddValidatorsFromAssemblyContaining<UserValidator>();
builder.Services.AddSingleton(new ServiceBusClient(serviceBusConnectionString));

// Cryptography
builder.Services.AddScoped<ICryptographyPassword, CryptographyPassword>();

// Cosmos DB configuration
builder.Services.AddScoped(sp =>
{
    string connectionString = builder.Configuration["Cosmosdb:connectionString"]!;
    string dataBase = builder.Configuration["Cosmosdb:dataBase"]!;
    string container = builder.Configuration["Cosmosdb:tripContainer"]!;

    var cosmosClient = new CosmosClient(connectionString);
    return cosmosClient.GetContainer(dataBase, container);
});

// ImageAnalizer API
builder.Services.AddScoped(ia => 
{
    string endpoint = builder.Configuration["iaImageAnalizer:endpoint"]!;
    string key = builder.Configuration["iaImageAnalizer:key"]!;

    return new ImageAnalysisClient(new Uri(endpoint), new AzureKeyCredential(key));
});

//azure service bus
builder.Services.AddSingleton(sp =>
{
    string queueName = builder.Configuration["AzureServiceBus:queueName"]!;
    var client = sp.GetRequiredService<ServiceBusClient>();
    return client.CreateProcessor(queueName, new ServiceBusProcessorOptions());
});

builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IImageAnalyzer, ImageAnalizer>();
builder.Services.AddScoped<ITripService, TripService>();
builder.Services.AddHostedService<ServiceBusWorker>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
