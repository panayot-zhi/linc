using linc.Models.Enumerations;
using linc.Utility;

namespace linc.Models.ConfigModels
{
    public class ApplicationDocumentDescriptor
    {
        public string FileName { get; set; }

        public string Extension => HelperFunctions.GetFileExtension(FileName);

        public ApplicationDocumentType Type { get; set; }

        public string ContentType { get; set; }

        public byte[] Content { get; set; }
    }
}
