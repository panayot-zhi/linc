using linc.Data;
using linc.Models.ConfigModels;

namespace linc.Contracts
{
    public interface IDocumentService
    {
        Task<ApplicationDocument> GetDocumentAsync(int id);

        string GetDocumentFilePath(ApplicationDocument document);

        public Task<ApplicationDocument> SaveDossierDocumentAsync(int dossierId, ApplicationDocumentDescriptor descriptor);

        Task<bool> DeleteDocumentAsync(int documentId);
    }
}
