using linc.Data;

namespace linc.Contracts
{
    public interface IDocumentService
    {
        Task<ApplicationDocument> GetDocumentAsync(int id);

        string GetDocumentFilePath(ApplicationDocument document);
    }
}
