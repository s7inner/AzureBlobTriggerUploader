using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace FileUploadAzure.Services
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobContainerClient _container;
        private readonly string allowedExtension = ".docx";
        private readonly ILogger<BlobStorageService> _logger;

        public BlobStorageService(IConfiguration configuration, ILogger<BlobStorageService> logger)
        {
            var blobStorageConnection = configuration.GetConnectionString("AzureStorageAccount");
            _container = new BlobContainerClient(blobStorageConnection, "file-upload");
            _logger = logger;
        }

        public async Task<string> UploadFileToBlobAsync(string strFileName, string contentType, Stream fileStream, string userEmail)
        {
            try
            {
                var fileExtension = Path.GetExtension(strFileName);

                if (!string.Equals(fileExtension, allowedExtension, StringComparison.OrdinalIgnoreCase))
                {
                    throw new NotSupportedException("Unsupported file extension.");
                }

                var createResponse = await _container.CreateIfNotExistsAsync();
                if (createResponse != null && createResponse.GetRawResponse().Status == 201)
                    await _container.SetAccessPolicyAsync(PublicAccessType.Blob);

                var blob = _container.GetBlobClient(strFileName);
                await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
                await blob.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = contentType });

                blob.SetMetadata(new Dictionary<string, string>
                {
                    { "email", userEmail }
                });

                var urlString = blob.Uri.ToString();
                return urlString;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex.ToString());
                throw;
            }
        }
    }
}
