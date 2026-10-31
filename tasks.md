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
- [ ] 4. Login helper and one browser context per role, with saved storage state.
- [ ] 5. Page objects for the author partial views: add, remove, pick a search suggestion, read the author rows.
- [ ] 6. Fixture PDF for document uploads.
- [ ] 7. Email-dependent suites run one at a time, since they share one inbox.

## Suites
- [ ] Access control: issue creation for head editors only; dossier creation for head editors; source edit and delete for administrators; dossier documents and agreement downloads forbidden for anonymous and plain users.
- [ ] Navigation: "List of indexes" and "List of dossiers" links in both languages, no edit mode toggle, "Create issue" on the source admin page.
- [ ] Dossier authors: create with several authors, validation for missing author or email, edit to add and remove authors and change the language, author search filtered by language and reusing existing authors.
- [ ] Source authors: create with several authors and notes, display on public list, admin list and issue details, no DOI for sections, several sources attached to one dossier.
- [ ] Publication agreement: one email per author, full match accepts, email-only match accepts, names-only and no match are redirected with a warning, download rights after signing, empty form returns to the form, head editor deletes and the author signs again, declaration locks fields on edit.
- [ ] Account and author sync: confirming the email links the user to the matching author, changing profile names updates linked authors. Implement the empty PublicationAgreementAcceptance class.
- [ ] Documents: upload and download a dossier document, replacing or deleting leaves no orphaned file.
- [ ] CI: run the suite from the build workflow against the test server, with credentials as secrets.

## Email matching
| Email | Token to search for in the body |
|---|---|
| Registration confirmation | Run token in the user's names |
| Agreement request | Run token in the author's names |
| Agreement received | Run token in the author's names |
