using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;

namespace Showcase.Infrastructure.Services
{
    public class BlobService : IBlobService
    {
        private readonly BlobContainerClient _container;

        public BlobService(IConfiguration configuration)
        {
            var connectionString = configuration["AzureBlobStorage:ConnectionString"];
            var containerName = configuration["AzureBlobStorage:ContainerName"];
            _container = new BlobContainerClient(connectionString, containerName);
            _container.CreateIfNotExists();
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName)
        {
            var blobClient = _container.GetBlobClient(fileName);
            await blobClient.UploadAsync(fileStream, overwrite: true);
            return blobClient.Uri.ToString();
        }

        public async Task DeleteFileAsync(string fileName)
        {
            var blobClient = _container.GetBlobClient(fileName);
            await blobClient.DeleteIfExistsAsync();
        }
    }
}
