using linc.E2ETests.Configuration;

namespace linc.E2ETests.Utility
{
    /// <summary>
    /// The privileged accounts seeded in linc_db_test by backup.sh (~/backups/tests/test_seed.sql).
    /// </summary>
    public enum TestRole
    {
        Editor,
        HeadEditor,
        Administrator
    }

    public record TestAccount(TestRole Role, string UserName, string? Password)
    {
        public static TestAccount For(TestConfig config, TestRole role) => role switch
        {
            TestRole.Editor => new TestAccount(role, config.EditorUserName, config.EditorPassword),
            TestRole.HeadEditor => new TestAccount(role, config.HeadEditorUserName, config.HeadEditorPassword),
            TestRole.Administrator => new TestAccount(role, config.AdministratorUserName, config.AdministratorPassword),
            _ => throw new ArgumentOutOfRangeException(nameof(role), role, null)
        };
    }
}
