namespace linc.E2ETests.Utility
{
    /// <summary>
    /// Read-only rows inserted by ~/backups/tests/test_seed.sql on the test server.
    /// Tests may read them but must never change them: the seed only runs again
    /// at the weekly restore, so a change would leak into every later run that week.
    /// </summary>
    public static class SeedData
    {
        /// <summary>Hidden issue (not available), released 2000-01-01, number 99.</summary>
        public const int IssueId = 900001;

        /// <summary>Dossier in status Prepared, created by test-head-editor, assigned to test-editor, language bg.</summary>
        public const int DossierId = 900001;

        /// <summary>Source in the seeded issue, linked to the seeded dossier.</summary>
        public const int SourceId = 900001;

        /// <summary>Original document of the seeded dossier.</summary>
        public const int DossierOriginalDocumentId = 900004;

        public const int LanguageIdBg = 1;
        public const int LanguageIdEn = 2;

        /// <summary>Unique words in the last names of the two seeded authors (language bg).</summary>
        public const string AuthorToken1 = "testseed01";
        public const string AuthorToken2 = "testseed02";

        public const string Author1Names = "Seed Author testseed01";
        public const string Author1Email = "testseed01@example.com";
    }
}
