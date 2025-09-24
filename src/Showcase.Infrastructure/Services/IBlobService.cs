using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Showcase.Infrastructure.Services
{
    public interface IBlobService
    {
        Task<string> UploadFileAsync(Stream stream, string fileName);
        Task DeleteFileAsync(string fileName);
    }
}
