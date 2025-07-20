# Dossier Author Separation & UI Refactor Tasks

## 1. Data Model & ViewModel Refactor
- Remove usage of obsolete FirstName, LastName, Email from Dossier logic and views.
- Ensure ApplicationDossier.Authors is used for all author data.
- Update DossierDetailsViewModel, DossierEditViewModel, DossierCreateViewModel to use a list of AuthorViewModels (no Notes, Email required).

## 2. Author ViewModel
- Create DossierAuthorViewModel (Id, FirstName, LastName, Email, UserId, UserName).
- Use this in DossierCreate/Edit/Details view models.

## 3. Service Layer
- Update DossierService to handle create/update of authors via Authors collection, using AuthorService for search/creation.
- On create/update, set Author.LanguageId = Dossier.LanguageId.
- Remove all logic using obsolete fields.

## 4. Views
- Update Dossier Create/Edit views:
  - Remove FirstName, LastName, Email fields from main form.
  - Add partial _Authors (copy from Source, but require Email, remove Notes).
  - Add author search (as in Source).
- Update Dossier Details/Index views to display all authors (as Source does).

## 5. Controller
- Update DossierController to bind Authors collection in Create/Edit actions.
- Pass Authors to/from view models.

## 6. Author Search
- Ensure author search endpoint is available for Dossier (reuse Source logic if possible).

## 7. Testing & Validation
- Test Dossier create/edit with multiple authors, required email, and language sync.
- Test Dossier details/index for correct author display.

## 8. Cleanup
- Remove any remaining usage of obsolete author fields in Dossier codebase.

---

**Questions answered:**
- Multiple authors supported, email required, no notes, language sync, all authors displayed.

---

**Next Steps:**
- Implement above tasks in order, starting with ViewModels and data flow, then UI, then service/controller logic, then cleanup.
