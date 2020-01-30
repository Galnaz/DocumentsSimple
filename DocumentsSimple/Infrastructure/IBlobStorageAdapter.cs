using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DocumentsSimple.Infrastructure
{
    public interface IBlobStorageAdapter
    {
        Task<Stream> DownloadFile(string name);
        Task<string> Upload(IFormFile file, string name);
    }
}