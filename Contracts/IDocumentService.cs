using linc.Data;

namespace linc.Contracts
{
    public interface IDocumentService
    {
        Task<ApplicationDocument> GetFileAsync(int id);
    }
}
