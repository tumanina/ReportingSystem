using Azure;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Files.DataLake;
using Azure.Storage.Files.DataLake.Models;
using Microsoft.Extensions.Options;
using ReportingSystem.Shared.Configuration;
using ReportingSystem.Shared.Interfaces;
using ReportingSystem.Shared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ReportingSystem.AzureStorage
{
    public class AzureFileStorage : IFileStorage
    {
        private readonly AzureStorageConfiguration _storageConfiguration;

        public AzureFileStorage(IOptions<AzureStorageConfiguration> storageConfiguration)
        {
            _storageConfiguration = storageConfiguration.Value;
        }

        public async Task CreateDirectory(string storagePath, string directoryName)
        {
            await Execute(async (sharedKeyCredential) =>
            {
                var serviceClient = new DataLakeServiceClient(new Uri(_storageConfiguration.Url), sharedKeyCredential);
                var filesystem = serviceClient.GetFileSystemClient(storagePath);
                await filesystem.CreateDirectoryAsync(directoryName);
            });
        }

        public async Task<FileModel> GetFile(string storagePath, string fileName)
        {
            return await Execute(async (sharedKeyCredential) =>
            {
                var serviceClient = new DataLakeServiceClient(new Uri(_storageConfiguration.Url), sharedKeyCredential);
                var filesystem = serviceClient.GetFileSystemClient(storagePath);
                var file = filesystem.GetFileClient(fileName);

                Response<FileDownloadInfo> fileContents = await file.ReadAsync();
                if (fileContents.Value != null)
                {
                    return new FileModel { FileStream = fileContents.Value.Content, Name = fileName };
                }

                return null;
            });
        }

        public async Task<IEnumerable<string>> GetFileNames(string storagePath)
        {
            return await Execute<IEnumerable<string>>(async (sharedKeyCredential) =>
            {
                var names = new List<string>();
                var serviceClient = new DataLakeServiceClient(new Uri(_storageConfiguration.Url), sharedKeyCredential);
                var fileSystemClient = serviceClient.GetFileSystemClient(storagePath);

                await foreach (PathItem pathItem in fileSystemClient.GetPathsAsync())
                {
                    names.Add(pathItem.Name);
                }

                return names;
            });
        }

        public async Task MoveFile(string storagePath, string fileName, string destinationDirectoryPath)
        {
            await Execute(async (sharedKeyCredential) =>
            {
                var serviceClient = new DataLakeServiceClient(new Uri(_storageConfiguration.Url), sharedKeyCredential);
                var fileSystem = serviceClient.GetFileSystemClient(storagePath);

                var destinationFileSystem = serviceClient.GetFileSystemClient(destinationDirectoryPath);
                var fileClient = await destinationFileSystem.CreateFileAsync(fileName);
                using (var stream = File.OpenRead(fileName))
                {
                    await fileClient.Value.UploadAsync(stream);
                }

                await fileSystem.DeleteAsync();
            });
        }

        public async Task UploadFile(string fileName, Stream fileContent)
        {
            await Execute(async (sharedKeyCredential) =>
            {
                var client = new BlobClient(new Uri($"{_storageConfiguration?.Url}/{fileName}"), sharedKeyCredential);

                fileContent.Position = 0;
                await client.UploadAsync(fileContent);
            });
        }

        private async Task<T> Execute<T>(Func<StorageSharedKeyCredential, Task<T>> func)
        {
            return await func(GetCredentials());
        }

        private async Task Execute(Func<StorageSharedKeyCredential, Task> func)
        {
            await func(GetCredentials());
        }

        private StorageSharedKeyCredential GetCredentials()
        {
            if (string.IsNullOrEmpty(_storageConfiguration?.Url))
            {
                throw new Exception("AzureStorageConfiguration.Url is empty or null.");
            }

            return new StorageSharedKeyCredential(_storageConfiguration.AccountName, _storageConfiguration.AccountKey);
        }
    }
}

