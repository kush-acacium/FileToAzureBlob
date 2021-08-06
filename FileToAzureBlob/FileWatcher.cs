using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace FileToAzureBlob
{
	internal sealed class FileWatcher : IDisposable
	{
		private FileSystemWatcher _watcher;
		private BufferBlock<FileChange> _fileChanges;
		private readonly string _baseFullPath;

		public FileWatcher(string monitoredDirectory)
		{
			this._watcher = new FileSystemWatcher(monitoredDirectory);
			this._fileChanges = new BufferBlock<FileChange>();
			this._baseFullPath = Path.GetFullPath(monitoredDirectory);

			this._watcher.NotifyFilter = NotifyFilters.LastWrite |
				NotifyFilters.Size |
				 NotifyFilters.FileName |
				NotifyFilters.DirectoryName;
			this._watcher.IncludeSubdirectories = true;
			this._watcher.Changed += this.OnFileChanged;
			this._watcher.Created += this.OnFileChanged;
			this._watcher.Renamed += this.OnFileRenamed;
			this._watcher.Deleted += this.OnFileDeleted;
			this._watcher.Error += this.OnFileWatchError;
			this._watcher.EnableRaisingEvents = true;
		}

		public void Dispose()
		{
			this._fileChanges.LinkTo(DataflowBlock.NullTarget<FileChange>());
			this._fileChanges.Complete();
			this._watcher.EnableRaisingEvents = false;
			this._watcher.Changed -= this.OnFileChanged;
			this._watcher.Created -= this.OnFileChanged;
			this._watcher.Renamed -= this.OnFileRenamed;
			this._watcher.Deleted -= this.OnFileChanged;
			this._watcher.Error -= this.OnFileWatchError;
		}

		public async Task<FileChange> GetFileChangeAsync(CancellationToken cancellationToken)
		{
			Console.WriteLine("Try to get an item.");
			return await this._fileChanges.ReceiveAsync(cancellationToken);
		}

		// Last access
		private void OnFileChanged(object sender, FileSystemEventArgs e)
		{
			this.FileAction(FileChange.ChangeType.Upload, Path.GetFileName(e.FullPath), e.FullPath);
		}

		private void OnFileDeleted(object sender, FileSystemEventArgs e)
		{
			this.FileAction(FileChange.ChangeType.Delete, Path.GetFileName(e.FullPath), e.FullPath);
		}

		private void OnFileRenamed(object sender, RenamedEventArgs e)
		{
			Console.WriteLine($"File {e.OldFullPath} Renamed to {e.FullPath}");

			this.FileAction(FileChange.ChangeType.Delete, Path.GetFileName(e.OldFullPath), e.OldFullPath);
			this.FileAction(FileChange.ChangeType.Upload, Path.GetFileName(e.FullPath), e.FullPath);
		}

		private void OnFileWatchError(object sender, ErrorEventArgs e)
		{
			Console.Error.WriteLine($"file watch exception: {e.GetException().ToString()}");
		}

		private void FileAction(FileChange.ChangeType changeType, string name, string path)
		{
			Console.WriteLine($"Type: {changeType}, Name: {name}, Path {path}");
			this._fileChanges.Post(new FileChange(changeType, name, path));
		}
	}
	}
