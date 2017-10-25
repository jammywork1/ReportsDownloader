namespace ReportsDownloader
{
    internal class Settings
    {
        public bool ReplaceIfExists { get; set; }
        public string DestionationPath { get; set; }
        public string Domain { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string ReportingServicesReportsUrl { get; set; }
    }
}