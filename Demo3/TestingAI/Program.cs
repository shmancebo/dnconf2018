using Compartimos.Indexation.Services;
using DotnetConf2018.Services;
using DotnetConf2018.Services.Models;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TestingAI
{
    class Program
    {

        static void Main(string[] args)
        {
            //DoLocal();
            DoBlob();
        }

        static byte[] GetImageAsByteArray(string imageFilePath)
        {
            FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
        }

        public static void DoLocal()
        {
            //imagenes locales
            var files = Directory.GetFiles("../../../imagenes");
            var result = new List<PredictionModel>();

            foreach (var file in files)
            {
                var image = GetImageAsByteArray(file);
                var memoryImage = Convert.ToBase64String(image);
                var prediction = (AIServices.PredictionImage(memoryImage).FirstOrDefault());
                if (prediction != null)
                    result.Add(prediction);
            }

            string json = JsonConvert.SerializeObject(result);
            //write string to file
            System.IO.File.WriteAllText("../../../Resultados/prediction.json", json);
        }

        public static void DoBlob()
        {
            // Imagenes desde un blob
            var imagesUrls = JsonConvert.DeserializeObject<Paths>(System.IO.File.ReadAllText("../../../Ficheros/Path.json"));
            var searchList = new List<AzureSearchModel>();

            foreach (var url in imagesUrls.data)
            {
                var prediction = (AIServices.PredictionImageByUrl(url.Signature).FirstOrDefault());
                if (prediction != null)
                    searchList.Add(new AzureSearchModel() { Tag = prediction.TagName, Scoring = prediction.Probability.ToString(), Url = url.Signature });
            }

            string searchServiceName = "dotnetconfsearch";
            string adminApiKey = "162ED4F1F498A366C066F1A1B1430F5D";

            //Create index
            var azureSearch = new AzureSearchService(searchServiceName, adminApiKey);
            var indexCreate = azureSearch.CreateIndexAsync<AzureSearchModel>("csindex", false, null).Result;

            // Group by year - One document by year
            var uploadDocument = azureSearch.UploadDocuments<AzureSearchModel>("csindex", searchList.ToArray()).Result;
        }
    }

}
