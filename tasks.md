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

---

# Test Coverage Plan

## Context
- Tests run against the deployed test server, https://test.linc.uni-plovdiv.bg/.
- The test database is restored weekly from production by backup.sh, together with uploaded files. Anything the tests create is wiped at the next reset, so no cleanup steps are needed.
- On the test server every outgoing email is rerouted to one inbox (EmailConfig.OverwriteRecipients). Real authors are never emailed, but the original recipient is lost.
- Roles cannot be granted through the UI. Registration always assigns the default user role.

## Groundwork
- [x] 1. Email checker: stop relying on "exactly one unread flagged message".
  - Wait for a message whose body contains a unique token and that arrived after the test started. Take the newest match.
  - Check the subject on the client side, so Cyrillic subjects need no server-side search.
  - Mark only the matched message as read. Leftover emails, such as unused registration confirmations, no longer break later tests.
  - Expose the confirmation link from the email, so tests can confirm an account.
- [x] 2. Run token: one ASCII identifier per test run, put into the names of every user and author the run creates. Email greetings print the names, which makes the token searchable.
- [ ] 3. Test accounts and read-only fixtures:
  - A server-only seed file, ~/backups/tests/test_seed.sql next to backup.sh, holds three privileged accounts (administrator, head editor, editor) with password hashes, plus one hidden issue, one dossier and one source with known authors. Never committed.
  - backup.sh runs it after test.linc.service starts, once http://localhost:5001/health returns 200, since the app migrates on startup. It also copies the fixture files from ~/backups/tests/files/ into /srv/www/test.linc/test/.
  - The publish workflow runs the same seed over SSH after each deploy.
  - Plaintext passwords only in the TestConfig section of appsettings.Staging.json in the repository root (gitignored) and GitHub Actions secrets passed as environment values.
  - Two plain users are registered through the UI at the start of each run. User A matches author A by names and email. User B does not.
  - Registration signs the user in without email confirmation. Keep that browser context alive and never log it out.
  - Tests run from the GitHub runner against the public address with basic auth.
- [x] 4. Login helper and one browser context per role, with saved storage state (TestBase.OpenAsAsync). Tests needing a seeded account are skipped when its password is not configured.
- [x] 5. Page objects for the author partial views: add, remove, pick a search suggestion, read the author rows (PageObjects/AuthorsEditor, DossierPages, SourcePages).
- [x] 6. Fixture PDF and Word document for uploads (Tests/Fixtures).
- [x] 7. Email-dependent suites run one at a time, since they share one inbox ([NonParallelizable]).

## Suites
Run against the test server on 2026-10-31: 54 tests, 51 pass, 3 fail on the known issues below. Two of those are fixed since (source edit for editors, "Create issue" link); the suite now has 55 tests and needs a run once the fix is deployed. Tests/Suites and Tests/Processes.
- [x] Access control (AccessControlTests): issue and dossier creation by role, hidden seeded issue, dossier list and details, seeded dossier document downloads, source admin, source edit open to editors with delete only for administrators.
- [x] Navigation (NavigationTests): "List of indexes" and "List of dossiers" in both languages, no edit mode toggle, no editor links for plain users, "Create issue" on the source admin page.
- [x] Dossier authors (DossierAuthorsTests): create with two authors, no authors and missing email validation, edit to add and remove authors, unassigned editor refused, author search picks a seeded author, search filtered by language.
- [x] Source authors (SourceAuthorsTests): create with two authors and notes, editor lands on the edit form after creating, library and issue listings, editor replaces an author, administrator deletes a source, create from a dossier pre-fills its authors, one source per language for the same dossier.
- [x] Publication agreement (Processes/PublicationAgreementAcceptance): one email per author, empty submission, full and email-only match sign, names-only and no match refused, download rights, signed author locked on edit, head editor deletes and the author signs again.
- [x] Account and author sync (AccountAuthorSyncTests): confirming the email links the user to the author with the same email, a new author with an existing user's names is linked on create.
- [x] Documents (DossierDocumentsTests): original document downloads, replacing the anonymized document removes the previous one.
- [x] Run the whole suite once the test server is back and the seed has run.
- [ ] CI: add the TEST_ADMINISTRATOR_PASSWORD, TEST_HEAD_EDITOR_PASSWORD and TEST_EDITOR_PASSWORD secrets, until then those tests are skipped.

Not covered on purpose:
- Changing profile names does not update linked authors: the profile page never calls the author sync, so the feature does not exist yet.
- Language change on dossier edit: the edit form has no language field.
- "No DOI for sections": only the source admin list hides it, and that list pages by issue, which puts the seeded issue on its last page.

Fixed after the first run:
- Source edit (open and save) is allowed to editors; the delete button shows only to administrators, who alone may delete.
- "Create issue" on the source admin page shows only to head editors and above.

Known issues the tests expose or work around:
- DossierService.SaveAgreementAsync sets author.UserId = author.UserId (no effect), so an author linked only by email cannot open their own signed agreement. The download test only checks the author linked by names.
- /author/search needs no login and returns author emails to anyone.
- ReplacingAnonymizedDocument_RemovesThePreviousOne fails: uploading a new anonymized document while the dossier is Prepared answers HTTP 500 (DossierService.UpdateDossierAsync, case Prepared). The cause is in the server log.

Things the app does that the tests rely on:
- Authors are normalised on save (ApplicationDbContext): first name capitalised, last name upper case. AuthorInput.Names gives the stored form.
- The dossier edit form keeps the authors in a collapsed panel; AuthorsEditor.ExpandAsync opens it.
- The layout has a logout form, so submit buttons are targeted with "main form button[type='submit']".

## Email matching
| Email | Token to search for in the body |
|---|---|
| Registration confirmation | Run token in the user's names |
| Agreement request | Run token in the author's names |
| Agreement received | Run token in the author's names |
