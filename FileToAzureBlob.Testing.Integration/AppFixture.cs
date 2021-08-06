using Microsoft.Extensions.Configuration;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
namespace FileToAzureBlob.Testing.Integration
{
    public class AppFixture : IDisposable
    {
        public readonly Settings _cfg;
        private Process _appProcess;
        private Process _azuriteProcess;

        public AppFixture()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.test.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            IConfiguration config = builder.Build();

            _cfg = new Settings();
            config.GetSection("FileToBlob").Bind(_cfg);

            _appProcess = new Process();
            _azuriteProcess = new Process();
        }

        public void StartConsoleApp()
        {
            _appProcess.Initialise(_cfg.AppExePath, _cfg.Environment);
        }

        public void StopConsoleApp()
        {
            _appProcess.Stop();
        }

        public void InstallAzuriteContainer()
        {
            Process process = new Process();
            process.Initialise("powershell", "docker pull mcr.microsoft.com/azure-storage/azurite", null);
            process.WaitForExit(10000);
            process.Stop();
        }

        public void StartAzuriteProcess()
        {
            _azuriteProcess.Initialise("powershell", "docker run -p 10000:10000 mcr.microsoft.com/azure-storage/azurite azurite-blob --blobHost 0.0.0.0", null);
            _azuriteProcess.WaitForExit(10000);
        }

        public void StopAzuriteProcess()
        {
            _azuriteProcess.Stop();
        }

        public void StopAzuriteContainer()
        {
            Process process = new Process();
            process.Initialise("powershell", "docker kill $(docker ps -q)", null);
            process.WaitForExit(10000);
            process.Stop();
        }        

        public void Dispose()
        {
            _appProcess.Dispose();
            _azuriteProcess.Dispose();
            _appProcess = null;
            _azuriteProcess = null;
        }
    }
}
