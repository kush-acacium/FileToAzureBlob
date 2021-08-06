using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

using Xunit;
using FileToAzureBlob;
using System.Collections.Generic;
using Azure.Storage.Blobs.Models;

namespace FileToAzureBlob.Testing.Integration
{
    public class Tests : IClassFixture<AppFixture>
    {
        AppFixture _fixture;

        public Tests(AppFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void Spin_Up_And_Down()
        {
            _fixture.StartConsoleApp();           
            _fixture.InstallAzuriteContainer();
            _fixture.StartAzuriteProcess();

            _fixture.StopAzuriteProcess();
            _fixture.StopAzuriteContainer();
            _fixture.StopConsoleApp();
        }

        [Fact]
        public async Task Copy_Test_Json_File_To_Trigger_Blob_Copy()
        {
            _fixture.StartConsoleApp();
            _fixture.InstallAzuriteContainer();
            _fixture.StartAzuriteProcess();
            
            File.Copy($"{_fixture._cfg.SourceFile}", $"{_fixture._cfg.TargetFile}", true);

            MyBlob blobstorage = new MyBlob(_fixture._cfg.ConnectionString, _fixture._cfg.Container);
            List<BlobItem> blobs = await blobstorage.GetBlobsAsync() as List<BlobItem>;

            _fixture.StopAzuriteProcess();
            _fixture.StopAzuriteContainer();
            _fixture.StopConsoleApp();

            Assert.True(blobs.Count == 1);
        }

        [Fact]
        public async Task Download_Blob_From_Storage()
        {
            _fixture.StartConsoleApp();
            _fixture.InstallAzuriteContainer();
            _fixture.StartAzuriteProcess();

            try
            {
                File.Copy($"{_fixture._cfg.SourceFile}", $"{_fixture._cfg.TargetFile}", true);

                MyBlob blobstorage = new MyBlob(_fixture._cfg.ConnectionString, _fixture._cfg.Container);
                var blob = await blobstorage.DownloadBlob(Path.GetFileName(_fixture._cfg.TargetFile), _fixture._cfg.DownloadFile);
            }
            catch (Exception e)
            {
                Assert.True(false, $"Exception occurred: {e.Message}");
            }
            finally
            {
                _fixture.StopAzuriteProcess();
                _fixture.StopAzuriteContainer();
                _fixture.StopConsoleApp();
            }

            var downloaded = File.Exists(_fixture._cfg.DownloadFile);
            if (downloaded)
                File.Delete(_fixture._cfg.DownloadFile);

            Assert.True(downloaded);
        }       
    }
}