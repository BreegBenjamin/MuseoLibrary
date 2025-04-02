using Microsoft.AspNetCore.StaticFiles;
using MuseoLibrary.UploadImagesService.Model;

namespace MuseoLibrary.UploadImagesService.Services
{
    public class TypeImageValidator
    {
        private static readonly Dictionary<AdmittedContent, string> ContentTypeMap = new()
        {
                { AdmittedContent.jpeg, "image/jpeg" },
                { AdmittedContent.png, "image/png" },
                { AdmittedContent.jpg, "image/jpg" }
        };
        public static string GetContentType(string fileName)
        {
            var provider = new FileExtensionContentTypeProvider();

            if (provider.TryGetContentType(fileName, out string? contentType))
            {
                return contentType;
            }

            return "application/octet-stream";
        }
        public static bool IsValidContentType(string contentType)
        {
            return ContentTypeMap.ContainsValue(contentType);
        }
    }
}
