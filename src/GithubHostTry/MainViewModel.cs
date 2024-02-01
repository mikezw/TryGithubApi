using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace GithubHostTry
{
    internal class MainViewModel
    {
        internal static string GitHubApiUrl = "meta";
        internal static string HostFilePath = @"C:\Windows\System32\drivers\etc\hosts";
        internal static string HostFilePathTemp = @"hosts.temp";
        internal static string HostFilePathBackup = @"hosts.backup";
        internal static string CopyScript = @"RunCopy.bat";
        internal static string GitHubDomain = @"github.com";
        private HttpClient client = new HttpClient();

        public MainViewModel()
        {
            client.BaseAddress = new Uri("https://api.github.com/");
            //client.DefaultRequestHeaders.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse(@"application/vnd.github+json"));
            client.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
            client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
            client.DefaultRequestHeaders.Add("User-Agent", "mikezw");
        }

        public async Task<MetaData?> ReadData()
        {
            MetaData? result = await client.GetFromJsonAsync(GitHubApiUrl, typeof(MetaData)) as MetaData;
            return result;
        }

        public string[] GetIPs(MetaData? metaData)
        {
            List<string> ips = new List<string>();
            if (metaData?.Webs != null)
            {
                ips = metaData.Webs.Select(x => x.Split('/')[0]).Where(ip => ip.Contains('.')).ToList();
            }
            return ips.ToArray();
        }

        public async Task<string[]> ReadHosts()
        {
            string[] hosts = await File.ReadAllLinesAsync(HostFilePath);
            return hosts.Select(line => line.TrimStart()).Where(x => !x.StartsWith('#')).ToArray();
        }

        public void BackHosts()
        {
            File.Copy(HostFilePath, HostFilePathBackup, true);
        }

        public void AddIpToHosts(string[] ip)
        {
            BackHosts();
            string[] hosts = File.ReadAllLinesAsync(HostFilePath).Result;
            List<int> githubIndex = new List<int>();
            for (int i = 0; i < hosts.Length; i++)
            {
                string line = hosts[i].Trim();
                if (!line.StartsWith('#') && line.Contains(GitHubDomain))
                {
                    githubIndex.Add(i);
                }
            }
            List<string> newHosts = hosts.ToList();
            for (int i = 0; i < ip.Length; i++)
            {
                if (i<githubIndex.Count)
                {
                    newHosts[githubIndex[i]] = GetGithubHosts(ip[i]);
                }
                else
                {
                    newHosts.Add(GetGithubHosts(ip[i]));
                }
            }
            File.WriteAllLines(HostFilePathTemp, newHosts);
        }

        public void CopyBackHosts()
        {
            if (File.Exists(HostFilePathTemp))
            {
                Process process = new Process();
                ProcessStartInfo processStartInfo = new ProcessStartInfo();
                processStartInfo.UseShellExecute = true;    
                processStartInfo.FileName = Path.GetFullPath(CopyScript);
                processStartInfo.Verb = "runas";
                process.StartInfo= processStartInfo;
                process.Start();
            }
        }

        private string GetGithubHosts(string ip)
        {
            return $"{ip} {GitHubDomain}";
        }
    }
}
