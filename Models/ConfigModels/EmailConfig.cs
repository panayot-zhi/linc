namespace linc.Models.ConfigModels
{
    public class EmailConfig
    {
        public int Port { get; init; }

        public string Host { get; init; }

        public string Sender { get; init; }

        public string Password { get; init; }

        public List<string> BlindCarbonCopies { get; init; } = new();
    }
}
