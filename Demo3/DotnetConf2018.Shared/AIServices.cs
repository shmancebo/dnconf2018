using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DotnetConf2018.Services
{
    public class AIServices
    {
        public static List<PredictionModel> PredictionImage(string base64)
        {
            string predictionKey = "c5bda34631dc4e399c3114f35fc7f980";
            var projectId = new Guid("9ad813c1-4f29-432a-9c9e-e0c53d51a1e9");

            // Create a prediction endpoint, passing in the obtained prediction key
            PredictionEndpoint endpoint = new PredictionEndpoint() { ApiKey = predictionKey};
            byte[] data = System.Convert.FromBase64String(base64);
            MemoryStream ms = new MemoryStream(data);
            var result = endpoint.PredictImage(projectId, ms);
            List<PredictionModel> predictions = new List<PredictionModel>();
            // Loop over each prediction and write out the results
            var filterResult = result.Predictions.Where(p => p.Probability > 0.7).ToList();
            foreach (var p in filterResult)
            {
                predictions.Add(p);
            }
            return predictions;

        }

        public static List<PredictionModel> PredictionImageByUrl(string url)
        {
            string predictionKey = "c5bda34631dc4e399c3114f35fc7f980";
            var projectId = new Guid("9ad813c1-4f29-432a-9c9e-e0c53d51a1e9");

            // Create a prediction endpoint, passing in the obtained prediction key
            PredictionEndpoint endpoint = new PredictionEndpoint() { ApiKey = predictionKey };
            var result = endpoint.PredictImageUrl(projectId,new ImageUrl() { Url = url } );
            List<PredictionModel> predictions = new List<PredictionModel>();
            // Loop over each prediction and write out the results
            var filterResult = result.Predictions.Where(p => p.Probability > 0.7).ToList();
            foreach (var p in filterResult)
            {
                predictions.Add(p);
            }
            return predictions;

        }
    }
}
