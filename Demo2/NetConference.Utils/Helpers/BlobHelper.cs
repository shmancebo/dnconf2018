using Microsoft.WindowsAzure.Storage;
using System.Configuration;
using System.IO;

namespace NetConference.Utils.Helpers
{
    public static class BlobHelper
    {
        public static Stream GetStream(string fileName, string container)
        {
            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["AzureWebJobsStorage"]);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var privateContainer = blobClient.GetContainerReference(container);
            var csvBlob = privateContainer.GetBlockBlobReference(fileName);

            var ms = new MemoryStream();
            csvBlob.DownloadToStream(ms);
            ms.Position = 0;

            return ms;
        }
    }
}
