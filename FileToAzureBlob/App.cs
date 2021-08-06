using Microsoft.Extensions.Options;

using System;

using System.Threading.Tasks;
using Azure;
using System.IO;
using System.Threading;
using System.Net;

namespace FileToAzureBlob
{
    public class App
    {
        private readonly FileToBlobSettings _appSettings;

        public App(IOptions<FileToBlobSettings> appSettings)
        {
            _appSettings = appSettings?.Value ?? throw new ArgumentNullException(nameof(appSettings));
        }

        public async Task Run(string[] args)
        {
			Console.WriteLine("Initializing");
			var myBlob = new MyBlob(_appSettings.ConnectionString, _appSettings.Container);

			//create container if it does not exist
			/*try
			{
				await myBlob.CreateContainerAsync(_appSettings.Container);
			}
			catch (Azure.RequestFailedException e)
			{
				if ((int)e.Status != 409)
				{
					Console.WriteLine($"The Container {_appSettings.Container} had issues being created.: {e.Message}");
					return;
				}
			}*/
			Console.WriteLine($"Watching directory: {_appSettings.MonitoredDirectory}");
			if (!Directory.Exists(_appSettings.MonitoredDirectory))
			{
				Console.WriteLine($"The Directory {_appSettings.MonitoredDirectory} doesn't exist.");
				return;
			}

			var fileWatcher = new FileWatcher(_appSettings.MonitoredDirectory);
			var cancellationToken = new CancellationToken();

			while (true)
			{
				var fileChange = await fileWatcher.GetFileChangeAsync(cancellationToken);

				try
				{
					switch (fileChange.Type)
					{
						case FileChange.ChangeType.Upload:
							await myBlob.UploadFileAsync(fileChange.Name, fileChange.FilePath, cancellationToken);
							break;
						case FileChange.ChangeType.Delete:
							await myBlob.DeleteFileAsync(fileChange.Name, fileChange.FilePath, cancellationToken);
							break;
					}
				}
				catch (RequestFailedException e)
				{
					Console.Error.WriteLine($"Failed to operate on Azure Blob {e}");
				}
				catch (IOException e)
				{
					Console.Error.WriteLine($"Failed to operate on Azure Blob {e}");
				}
				catch (UnauthorizedAccessException e)
				{
					Console.Error.WriteLine($"Failed to operate on Azure Blob {e}");
				}
			}
		}
    }
}
