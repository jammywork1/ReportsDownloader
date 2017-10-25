using System.IO;
using ServiceStack.Text;

namespace ReportsDownloader
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var settings = JsonSerializer.DeserializeFromStream<Settings>(File.OpenRead("settings.json"));
            new Downloader
            {
                ReplaceIfExists = settings.ReplaceIfExists,
                DestionationPath = settings.DestionationPath,
                Domain = settings.Domain,
                Login = settings.Login,
                Password = settings.Password,
                ReportingServicesReportsUrl = settings.ReportingServicesReportsUrl
            }.Run();
        }
    }
}