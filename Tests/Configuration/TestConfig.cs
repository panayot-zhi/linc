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
    }
}
