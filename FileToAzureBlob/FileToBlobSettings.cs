using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileToAzureBlob
{
    public class FileToBlobSettings
    {
        public string Container { get; set; }
        public string ConnectionString { get; set; }
        public string MonitoredDirectory { get; set; }
    }
}
