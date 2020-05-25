using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MiApp.Web.Helpers
{
    public interface IImageHelper
    {
        Task<string> UploadImageAsync(IFormFile imageFile, string folder);

        string UploadImage(byte[] pictureArray, string folder);
    }
}
