using System.ComponentModel.DataAnnotations;

namespace linc.E2ETests.Configuration
{
    public class TestConfig
    {
        [Required]
        public string ServerBaseUrl { get; set; } = null!;

        [Required]
        public string TestGMailUserName { get; set; } = null!;

        [Required]
        public string TestGMailUserPassword { get; set; } = null!;

        public string? BasicAuthUsername { get; set; } = null!;

        public string? BasicAuthPassword { get; set; } = null!;
    }
}
