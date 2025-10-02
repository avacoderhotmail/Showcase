using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Showcase.Application.Interfaces
{
    public interface IBlobService
    {
        Task<string> UploadFileAsync(Stream stream, string fileName);
        Task DeleteFileAsync(string fileName);

        Task<string> GetSasUri(string blobName, int expiryMinutes = 15);

        Task<string> UploadAsync(IFormFile file, string fileName);
    }
}
