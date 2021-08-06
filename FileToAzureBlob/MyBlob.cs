using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FileToAzureBlob
{
	// v12 reference: https://docs.microsoft.com/en-us/azure/storage/blobs/storage-quickstart-blobs-dotnet
	public sealed class MyBlob
    {
		private readonly BlobServiceClient _blobServiceClient;
		private readonly BlobContainerClient _blobContainer;
		
		public MyBlob(string connectionString, string blobContainer)
		{
			_blobServiceClient = new BlobServiceClient(connectionString);
			_blobContainer = _blobServiceClient.GetBlobContainerClient(blobContainer);
			_blobContainer.CreateIfNotExists();
		}

		/* Uncomment to explicity create a blob container 
		  public async Task CreateContainerAsync(string blobContainer)
		{
			// Create the container and return a container client object
			await _blobServiceClient.CreateBlobContainerAsync(blobContainer);
		}*/

		public async Task UploadFileAsync(string name, string path, CancellationToken cancellationToken)
		{
			Console.WriteLine($"Uploading file {path} as {name}");
			BlobClient newBlob = _blobContainer.GetBlobClient(name);
			await newBlob.UploadAsync(path, true, cancellationToken);
		}

		public async Task DeleteFileAsync(string name, string filePath, CancellationToken cancellationToken)
		{
			Console.WriteLine($"Deleting file {filePath} as {name}");
			BlobClient blob = _blobContainer.GetBlobClient(name);
			await blob.DeleteIfExistsAsync(Azure.Storage.Blobs.Models.DeleteSnapshotsOption.IncludeSnapshots, null, cancellationToken);
		}

		/* Uncomment to test if a path is a folder or file
		private static bool IsDirectory(string path)
		{
			FileAttributes fa = File.GetAttributes(path);
			return (fa & FileAttributes.Directory) != 0;
		}*/

		public async Task ListBlobsAsync()
		{
			// List all blobs in the container
			await foreach (BlobItem blobItem in _blobContainer.GetBlobsAsync())
			{
				Console.WriteLine("\t" + blobItem.Name);
			}
		}

		public async Task<IEnumerable<BlobItem>> GetBlobsAsync()
		{
			var blobs = new List<BlobItem>();
			// List all blobs in the container
			await foreach (BlobItem blobItem in _blobContainer.GetBlobsAsync())
			{
				blobs.Add(blobItem);
			}

			return blobs;
		}

		public async Task<Azure.Response> DownloadBlob(string name, string destination)
		{
			// Download the blob to a local file, using the reference created earlier.
			// Append the string "_DOWNLOADED" before the .txt extension so that you 
			// can see both files in MyDocuments.
			BlobClient getBlob = _blobContainer.GetBlobClient(name);
			return await getBlob.DownloadToAsync(destination);
		}
	}
}
