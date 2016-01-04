using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Common
{
    public class FilesStorageService
    {
        private const string ContainerName = "constructionprogressfiles";
        private readonly CloudBlobContainer _container;
        private readonly StorageUri _blobStorageUri;
        private readonly TelemetryClient _telemetry = new TelemetryClient();

        public FilesStorageService(CloudStorageAccount storageAccount)
        {
            _blobStorageUri = storageAccount.BlobStorageUri;
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            _container = blobClient.GetContainerReference(ContainerName);
            _telemetry.InstrumentationKey = TelemetryConfiguration.Active.InstrumentationKey;
        }
        
        public async Task<string> UploadFile(string fileName, Stream file, string mimeType)
        {
            if (!_container.Exists())
            {
                _container.Create();
                _container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            }

            CloudBlockBlob blockBlob = _container.GetBlockBlobReference(fileName);
            blockBlob.Properties.ContentType = mimeType;
            await blockBlob.UploadFromStreamAsync(file);

            _telemetry.TrackTrace(string.Format(CultureInfo.InvariantCulture, "Uploaded file {0}", fileName), SeverityLevel.Information);

            return $"{_blobStorageUri.PrimaryUri}{ContainerName}/{fileName}";
        }
    }
}