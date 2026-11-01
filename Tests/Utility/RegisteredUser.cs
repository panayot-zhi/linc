using Microsoft.Playwright;

namespace linc.E2ETests.Utility
{
    /// <summary>
    /// A plain user registered through the UI during this run, still signed in on <see cref="Page"/>.
    /// The email is not confirmed, so the user cannot log in again after logging out.
    /// </summary>
    public record RegisteredUser(
        string Token,
        string FirstName,
        string LastName,
        string Email,
        string UserName,
        string Password,
        DateTimeOffset RegisteredAt,
        IPage Page)
    {
        public string Names => $"{FirstName} {LastName}";
    }
}
