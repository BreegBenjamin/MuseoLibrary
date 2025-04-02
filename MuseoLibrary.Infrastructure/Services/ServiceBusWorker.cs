using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MuseoLibrary.ApplicationDomain.DTOs;
using MuseoLibrary.ApplicationDomain.Interfaces;
using MuseoLibrary.ApplicationDomain.Models;
using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace MuseoLibrary.Infrastructure.Services
{
    public class ServiceBusWorker : BackgroundService
    {
        private readonly ServiceBusProcessor _processor;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        private static readonly ConcurrentDictionary<string, List<ImageMessage>> _messageGroups = new();
        private static readonly ConcurrentDictionary<string, CancellationTokenSource> _batchTimers = new();

        public ServiceBusWorker(IServiceScopeFactory serviceScopeFactory, ServiceBusProcessor processor)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _processor = processor;
            _processor.ProcessMessageAsync += MessageHandler;
            _processor.ProcessErrorAsync += ErrorHandler;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _processor.StartProcessingAsync();
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private async Task MessageHandler(ProcessMessageEventArgs args)
        {
            try
            {
                var messageBody = args.Message.Body.ToString();
                var imageMessage = JsonConvert.DeserializeObject<ImageMessage>(messageBody);
                if (imageMessage == null) throw new Exception("Invalid message format");

                string tripId = imageMessage.TripId;

                _messageGroups.AddOrUpdate(tripId,
                    new List<ImageMessage> { imageMessage },
                    (key, list) =>
                    {
                        list.Add(imageMessage);
                        return list;
                    });

                RestartBatchTimer(tripId);

                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error to process menssage: {ex.Message}");
            }
        }

        private void RestartBatchTimer(string tripId)
        {
            if (_batchTimers.TryRemove(tripId, out var existingTimer))
            {
                existingTimer.Cancel();
            }

            var cts = new CancellationTokenSource();
            _batchTimers[tripId] = cts;

            Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(10), cts.Token);
                    if (!cts.Token.IsCancellationRequested)
                    {
                        await ProcessBatchAsync(tripId);
                    }
                }
                catch (TaskCanceledException ex) 
                {
                    Console.WriteLine(ex.Message, "Error en el temporizador de lote");
                }
            });
        }

        private async Task ProcessBatchAsync(string tripId)
        {
            if (_messageGroups.TryRemove(tripId, out var messagesToProcess))
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var tripService = scope.ServiceProvider.GetRequiredService<ITripService>();

                    var result = await tripService.GetTripByIdAsync(tripId);
                    if (result.Data == null) return;

                    Trip trip = result.Data;
                    trip.Images.AddRange(messagesToProcess.Select(x => x.ImageUrl));

                    await tripService.UpdateTripAsync(trip);
                }

                Console.WriteLine($"Procesado lote de {messagesToProcess.Count} imágenes para TripId: {tripId}");
            }

            _batchTimers.TryRemove(tripId, out _);
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine($"Error en Service Bus: {args.Exception.Message}");
            return Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            await _processor.StopProcessingAsync();
        }
    }
}
