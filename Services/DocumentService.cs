using linc.Contracts;
using linc.Data;
using Microsoft.EntityFrameworkCore;

namespace linc.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly ApplicationDbContext _context;

        public DocumentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApplicationDocument> GetFileAsync(int id)
        {
            return await _context.Documents.FindAsync(id);
        }
    }
}
