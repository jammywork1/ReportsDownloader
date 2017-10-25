using System;
using System.IO;
using System.Linq;
using System.Net;
using HtmlAgilityPack;

namespace ReportsDownloader
{
    internal class Downloader
    {
        private WebClient _client;
        private string _destionationPath;
        private string _domain;
        private string _login;
        private string _password;
        private string _reportingServicesReportsUrl;
        public bool ReplaceIfExists { get; set; } = true;

        public string Domain
        {
            get => _domain;
            set
            {
                if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(nameof(Domain));
                _domain = value;
            }
        }

        public string Login
        {
            get => _login;
            set
            {
                if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(nameof(Login));
                _login = value;
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(nameof(Password));
                _password = value;
            }
        }

        public string DestionationPath
        {
            get => _destionationPath;
            set
            {
                if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(nameof(DestionationPath));
                _destionationPath = value;
            }
        }

        public string ReportingServicesReportsUrl
        {
            get => _reportingServicesReportsUrl;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException(nameof(ReportingServicesReportsUrl));
                _reportingServicesReportsUrl = value;
            }
        }

        public void Run()
        {
            Console.WriteLine("Start");
            using (_client = new WebClient {UseDefaultCredentials = false})
            {
                _client.Credentials = new CredentialCache
                {
                    {new Uri(ReportingServicesReportsUrl), "NTLM", new NetworkCredential(Login, Password, Domain)}
                };
                Console.WriteLine($"Get page wtih reports list");
                var body = _client.DownloadString(ReportingServicesReportsUrl);
                Parse(body);
            }
            Console.WriteLine("End");
        }

        private void Parse(string html)
        {
            if (!Directory.Exists(DestionationPath)) { 
                Directory.CreateDirectory(DestionationPath);
                Console.WriteLine($"Create directory '{DestionationPath}'");
            }
            var uri = new Uri(ReportingServicesReportsUrl);
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            Console.WriteLine($"Parse page");
            var nodes = doc.DocumentNode.Descendants("a")
                //.Select(y => y.Descendants()
                .Where(x => x.Attributes["href"].Value.StartsWith(@"/Reports/Pages/Report.aspx?ItemPath=")
                            && x.Attributes["href"].Value.EndsWith(@"ViewMode=Detail"))
                .Select(y => new {Href = y.Attributes["href"].Value, Name = y.Attributes["title"].Value})
                //)
                .ToArray();
            Console.WriteLine($"Finded {nodes.Length} reports");
            foreach (var node in nodes)
            {
                var build = new UriBuilder(uri)
                {
                    Path = node.Href,
                    Query = "&SelectedTabId=PropertiesTab&Export=true"
                };
                var decodedUrl = Uri.UnescapeDataString(build.Uri.AbsoluteUri);
                Console.WriteLine($"Download report {node.Name}");
                var path = Path.Combine(DestionationPath, $"{node.Name}.rdl");
                if (!File.Exists(path) || ReplaceIfExists)
                    _client.DownloadFile(decodedUrl, path);
            }
        }
    }
}