using System.Security.Cryptography;

namespace linc.E2ETests.Utility
{
    /// <summary>
    /// Identifies one test run. Put a token into the names of every user and author
    /// the run creates: emails print the names, so the token makes them searchable.
    /// </summary>
    internal static class TestRun
    {
        private static int _counter;

        /// <summary>
        /// Digits only, so the value survives any name casing rules on the server.
        /// </summary>
        public static string Id { get; } = $"{DateTime.UtcNow:MMddHHmm}{RandomNumberGenerator.GetInt32(100, 1000)}";

        /// <summary>
        /// Returns a new token, unique within the run, such as "test10311530123x01".
        /// Use it as a separate word in a name, since Gmail searches whole words.
        /// </summary>
        public static string NewToken()
        {
            var sequence = Interlocked.Increment(ref _counter);
            return $"test{Id}x{sequence:D2}";
        }
    }
}
