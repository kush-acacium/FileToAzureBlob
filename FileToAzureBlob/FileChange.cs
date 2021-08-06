using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileToAzureBlob
{
	internal sealed class FileChange
	{
		public enum ChangeType
		{
			Upload,
			Delete,
		}

		public ChangeType Type { get; }
		public string Name { get; }
		public string FilePath { get; }

		public FileChange(ChangeType type, string name, string path)
		{
			this.Type = type;
			this.Name = name;
			this.FilePath = path;
		}
	}
}
