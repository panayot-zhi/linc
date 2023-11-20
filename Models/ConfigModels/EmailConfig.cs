namespace linc.Models.ConfigModels
{
    public class EmailConfig
    {
        public int Port { get; init; }

        public string Host { get; init; }

        public string Username { get; init; }

        public string Password { get; init; }

        public bool Debug { get; init; }
    }
}
