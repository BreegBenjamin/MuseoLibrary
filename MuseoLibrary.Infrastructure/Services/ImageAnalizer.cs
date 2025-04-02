using Azure.AI.Vision.ImageAnalysis;
using MuseoLibrary.ApplicationDomain.DTOs;
using MuseoLibrary.ApplicationDomain.Interfaces;
using MuseoLibrary.ApplicationDomain.Models;
using Polly;

namespace MuseoLibrary.Infrastructure.Services
{
    public class ImageAnalizer : IImageAnalyzer
    {
        private readonly ImageAnalysisClient _clientIA;
        public ImageAnalizer(ImageAnalysisClient clientIA)
        {
            _clientIA = clientIA;
        }
        public async Task<Trip> AnalyzeImageIAAsync(TripDto tripDto)
        {
			try
			{
                var trip = new Trip();
                int retries = 3;
                ImageAnalysisResult? analysisResult = null;

                //Implement Polly
                var policyResult = await Policy.Handle<Exception>()
                    .WaitAndRetryAsync
                    (
                        retryCount: retries,
                        sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                        onRetry: (exception, timeSpan, retryCount, context) =>
                        {
                            // Log
                        }
                    ).ExecuteAndCaptureAsync( async ()=>
                    {
                        analysisResult  = await _clientIA.AnalyzeAsync(
                        new Uri(tripDto.ImageURL),
                        VisualFeatures.DenseCaptions,
                        new ImageAnalysisOptions
                        {
                           GenderNeutralCaption = true
                        });
                    });

                if (analysisResult!.DenseCaptions.Values != null) 
                {
                    trip.Images.Add(tripDto.ImageURL);

                    trip.DetectedObjects = analysisResult.DenseCaptions.Values
                    .Select(o => new DetectedObjectInfo()
                    {
                        ObjectText = o.Text,
                        Confidence = o.Confidence
                    }).ToList();
                }

                if (analysisResult.Caption != null) 
                {
                    trip.Confidence = analysisResult.Caption.Confidence;
                    trip.Description = analysisResult.Caption.Text;
                }

                if (analysisResult.Tags != null)
                {
                    trip.Tags = (analysisResult.Tags.Values != null) ? analysisResult.Tags.Values
                        .Where(t => t.Confidence > 0.6)
                        .Select(t => t.Name).ToList() : new List<string>();
                }
                else 
                {
                    trip.Tags = new List<string>();
                }

                // llamar segundo servicio IA para validar el mensaje

                return trip;
            }
			catch (Exception ex) 
			{
                string ms = ex.Message;
                return new Trip();
            }
        }
    }
}
