using linc.Models.ConfigModels;
using linc.Models.Enumerations;

namespace linc.Utility
{
    public static class FormFileExtensions
    {
        public static string GetExtension(this IFormFile formFile)
        {
            return HelperFunctions.GetFileExtension(formFile.FileName);
        }

        public static byte[] GetContent(this IFormFile formFile)
        {
            using (var memoryStream = new MemoryStream())
            {
                formFile.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public static async Task<byte[]> GetContentAsync(this IFormFile formFile)
        {
            await using (var memoryStream = new MemoryStream())
            {
                await formFile.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public static ApplicationDocumentDescriptor ToDescriptor(this IFormFile formFile,
            ApplicationDocumentType type)
        {
            if (formFile == null)
            {
                return null;
            }

            return new ApplicationDocumentDescriptor
            {
                FileName = formFile.FileName,
                ContentType = formFile.ContentType,
                Content = formFile.GetContent(),
                Type = type
            };
        }

        public static async Task<ApplicationDocumentDescriptor> ToDescriptorAsync(this IFormFile formFile,
            ApplicationDocumentType type)
        {
            if (formFile == null)
            {
                return null;
            }

            return new ApplicationDocumentDescriptor
            {
                FileName = formFile.FileName,
                ContentType = formFile.ContentType,
                Content = await formFile.GetContentAsync(),
                Type = type
            };
        }
    }
}
