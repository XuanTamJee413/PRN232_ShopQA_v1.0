using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using CloudinaryDotNet.Actions;
namespace Business.Iservices
{
    public interface ICloudinaryService
    {
        Task<ImageUploadResult> UploadImageAsync(IFormFile file, string folderName);
        Task<DeletionResult> DeleteImageAsync(string publicId); // Optional: if you need to delete images
    }
}
