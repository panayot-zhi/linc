using System.ComponentModel.DataAnnotations;

namespace linc.E2ETests.Configuration
{
    public class TestConfig
    {
        [Required]
        public string ServerBaseUrl { get; set; } = "https://test.linc.uni-plovdiv.bg/";

        [Required]
        public string TestGMailUserName { get; set; } = "pivanov@uni-plovdiv.bg";

        [Required]
        public string TestGMailUserPassword { get; set; } = string.Empty;

        public string? BasicAuthUsername { get; set; } = "admin";

        public string? BasicAuthPassword { get; set; } = null!;

        // privileged accounts exist only in linc_db_test,
        // seeded by backup.sh from ~/backups/tests/test_seed.sql,
        // passwords come from the TestConfig section of appsettings.Staging.json (gitignored) or CI environment values

        public string AdministratorUserName { get; set; } = "test-admin";

        public string? AdministratorPassword { get; set; }

        public string HeadEditorUserName { get; set; } = "test-head-editor";

        public string? HeadEditorPassword { get; set; }

        public string EditorUserName { get; set; } = "test-editor";

        public string? EditorPassword { get; set; }

        public bool PlaywrightHeadlessMode { get; set; } = true;

        public float? PlaywrightSlowMo { get; set; } = null;
    }
}
