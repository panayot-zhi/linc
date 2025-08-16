using System.Security.Claims;
using linc.Data;
using linc.Models.Enumerations;
using linc.Models.ViewModels.Dossier;
using linc.Utility;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace linc.Services
{
    public partial class DossierService
    {
        /*private static bool CanAttachAgreement(ApplicationDossier dossier)
        {
            return dossier.Agreement is null &&
                   dossier.Status is not ApplicationDossierStatus.Accepted or
                       ApplicationDossierStatus.AcceptedWithCorrections;
        }

        private static bool CanDeleteAgreement(ApplicationDossier dossier)
        {
            return dossier.Agreement is not null &&
                   dossier.Status is not ApplicationDossierStatus.Accepted or
                       ApplicationDossierStatus.AcceptedWithCorrections;
        }*/

        private async Task<ApplicationDocument> SaveDossierDocumentAsync(IFormFile inputFile, int dossierId, ApplicationDocumentType type)
        {
            if (inputFile == null)
            {
                return null;
            }

            var fileExtension = inputFile.Extension();
            var fileName = Guid.NewGuid().ToString();

            var rootFolderPath = Path.Combine(_config.RepositoryPath, SiteConstant.DossiersFolderName, dossierId.ToString());
            var filePath = Path.Combine(rootFolderPath, $"{fileName}.{fileExtension}");

            Directory.CreateDirectory(rootFolderPath);

            var relativePath = Path.Combine(SiteConstant.DossiersFolderName, dossierId.ToString(), $"{fileName}.{fileExtension}");

            await using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await inputFile.CopyToAsync(fileStream);
            }

            var entry = new ApplicationDocument()
            {
                DocumentType = type,
                Extension = fileExtension,
                FileName = fileName,
                MimeType = inputFile.ContentType,
                OriginalFileName = inputFile.FileName,
                RelativePath = relativePath
            };

            var entityEntry = await _context.Documents.AddAsync(entry);
            await _context.SaveChangesAsync();
            return entityEntry.Entity;
        }

        /*private async Task DeleteDossierDocument(ApplicationDossier dossier, ApplicationDocument document)
        {
            var filePath = Path.Combine(_config.RepositoryPath, document.RelativePath);
            if (!File.Exists(filePath))
            {
                _logger.LogWarning(
                    "Could not find a physical file path for dossier {DossierId} document {DocumentId}, deleting only from the database...",
                    dossier.Id, document.Id);
            }
            else
            {
                File.Delete(filePath);
            }

            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();
        }*/

        private ClaimsPrincipal GetCurrentUser()
        {
            return _httpContextAccessor.HttpContext?.User;
        }

        private string GetCurrentUserId()
        {
            return GetCurrentUser()?.GetUserId();
        }

        private List<SelectListItem> GetReviewers()
        {
            var selectList = new List<SelectListItem>()
            {
                new(_localizationService["DossierEdit_Reviewer_Prompt"].Value, string.Empty)
            };

            var reviewers = _context.Users
                .Where(x => x.IsReviewer)
                .Select(x => new SelectListItem(x.Names, x.Id))
                .ToList();

            selectList.AddRange(reviewers);

            return selectList;
        }

        private List<SelectListItem> GetEditors(string assigneeId)
        {
            var editors = _context.GetAllByRole(HelperFunctions.ToScreamingSnakeCase(SiteRole.Editor.ToString()));
            var headEditors = _context.GetAllByRole(HelperFunctions.ToScreamingSnakeCase(SiteRole.HeadEditor.ToString()));

            var selectList = new List<SelectListItem>()
            {
                new(_localizationService["DossierEdit_AssignEditor_Prompt"].Value, string.Empty)
            };

            selectList.AddRange(headEditors.Select(x => new SelectListItem(text: x.Names, x.Id, x.Id == assigneeId)));
            selectList.AddRange(editors.Select(x => new SelectListItem(text: x.Names, x.Id, x.Id == assigneeId)));

            return selectList;
        }

        private async Task<string> FindReviewerAsync(string inputEmail, string inputFirstName, string inputLastName)
        {
            var user = await _applicationUserStore.FindUserByEmailAsync(inputEmail);
            if (user == null)
            {
                user = await _applicationUserStore.FindUserByNamesAsync(inputFirstName, inputLastName);
            }

            if (user is null)
            {
                return null;
            }

            _context.Users.Attach(user);

            user.IsReviewer = true;

            await _context.SaveChangesAsync();

            return user.Id;
        }
    }
}
