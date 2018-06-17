using System;
using System.IO;
using System.Net.Sockets;
using System.Reflection;
using Ngonzalez.Telnet;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Net;
using System.Text;

namespace BackendMate
{
    class Program
    {
        static void Main(string[] args)
        {
            StringBuilder sb = new StringBuilder();
            string machineIP = GetLocalIPAddress();
            LogResult($"***** Source IP ---> {machineIP} *****", ref sb);

            string basePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            string configsPath = Path.Combine(basePath, "config.txt");
            var configLine = System.IO.File.ReadAllLines(configsPath).ToList();
            configLine.RemoveAll(x => x.StartsWith("#"));

            if (!configLine.Any())
            {
                LogResult("Invalid config file", ref sb);
                return;
            }

            List<KeyValuePair<string, int>> configs = new List<KeyValuePair<string, int>>();
            configLine.ForEach(x => 
            {
                try
                {
                    var config = Regex.Replace(x, @"\s+", "|");
                    var c = config.Split('|');
                    configs.Add(new KeyValuePair<string, int>(c[0], int.Parse(c[1])));
                }
                catch
                {
                    LogResult($"Invalid config file: {x}", ref sb);
                }
            });

            foreach (var config in configs)
            {
                string result = String.Empty;
                try
                {
                    using (Client client = new Client(config.Key, config.Value, new System.Threading.CancellationToken()))
                    {
                        result = client.IsConnected ? "PASS" : "FAIL";
                    }
                }
                catch (SocketException ex)
                {
                    result = "CONNECTION FAIL";
                }
                catch (Exception ex)
                {
                    result = ex.Message;
                }
                finally
                {
                    result = $"{DateTime.Now.ToString("s")} - {config.Key} {config.Value} ----> {result}";
                    LogResult(result, ref sb);
                }
            }

            File.WriteAllText(Path.Combine(basePath, DateTime.Now.ToString("TestResult-yyyyMMdd_HHmmss") + ".txt"), sb.ToString());
            Console.ReadLine();

        }

        private static void LogResult(string message, ref StringBuilder sb)
        {
            sb.Append($"{message}{Environment.NewLine}");
            Console.WriteLine(message);
        }

        private static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    }
}
