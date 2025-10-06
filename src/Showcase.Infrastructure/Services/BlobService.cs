using Azure.Core;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Showcase.Application.Interfaces;

namespace Showcase.Infrastructure.Services
{
    public class BlobService : IBlobService
    {
        private readonly BlobContainerClient _container;
        private readonly BlobServiceClient _serviceClient;


        public BlobService(IConfiguration configuration)
        {
            var accountName = configuration["AzureBlobStorage:AccountName"];
            var containerName = configuration["AzureBlobStorage:ContainerName"];

            // Create service client for user delegation key
            _serviceClient = new BlobServiceClient(
                new Uri($"https://{accountName}.blob.core.windows.net"),
                new DefaultAzureCredential()
            );

            _container = _serviceClient.GetBlobContainerClient(containerName);
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

        public async Task<string> UploadAsync(IFormFile file, string fileName)
        {
            var blobClient = _container.GetBlobClient(fileName);
            await using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, overwrite: true);
            return blobClient.Uri.ToString();
        }

        public async Task<string> GetSasUri(string blobName, int expiryMinutes = 60)
        {
            // This is how to get an OAuth token if needed and it's how I found out why I was getting not authorized working from dev
            //var tokenCredential = new DefaultAzureCredential();
            //var token = await tokenCredential.GetTokenAsync(new TokenRequestContext(new[] { "https://storage.azure.com/.default" }));
            //Console.WriteLine($"Access token issued to: {token.Token}");

            // Check if blob exists
            var blobClient = _container.GetBlobClient(blobName);

            // Get a user delegation key with buffer for clock skew
            var keyStart = DateTimeOffset.UtcNow.AddMinutes(-15);
            var keyExpiry = DateTimeOffset.UtcNow.AddHours(2);
            var delegationKey = (await _serviceClient.GetUserDelegationKeyAsync(keyStart, keyExpiry)).Value;

            // Set SAS start and expiry times
            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = _container.Name,
                BlobName = blobName,
                Resource = "b", // blob
                StartsOn = DateTimeOffset.UtcNow.AddMinutes(-1), // small buffer
                ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(expiryMinutes)
            };
            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            // Build SAS URI manually
            var sasUri = new UriBuilder(blobClient.Uri)
            {
                Query = sasBuilder.ToSasQueryParameters(delegationKey, _serviceClient.AccountName).ToString()
            }.Uri;

            return sasUri.ToString();
        }

    }
}
