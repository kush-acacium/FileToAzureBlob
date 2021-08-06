using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileToAzureBlob.Testing.Integration
{
    public class Settings
    {
        public string AppExePath { get; set; }
        public string Container { get; set; }
        public string ConnectionString { get; set; }
        public string DownloadFile { get; set; }
        public string Environment { get; set; }
        public string SourceFile { get; set; }
        public string TargetFile { get; set; }
    }
}
