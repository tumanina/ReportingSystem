using Azure;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Files.DataLake;
using Azure.Storage.Files.DataLake.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ReportingSystem.Shared.Configuration;
using ReportingSystem.Shared.Extensions;
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
        private readonly ILogger<AzureFileStorage> _logger;

        public AzureFileStorage(ILogger<AzureFileStorage> logger, IOptions<AzureStorageConfiguration> storageConfiguration)
        {
            _logger = logger;
            _storageConfiguration = storageConfiguration.Value;
        }

        public async Task<bool> CreateDirectory(string storagePath, string directoryName)
        {
            return await Execute(async (sharedKeyCredential) =>
            {
                var serviceClient = new DataLakeServiceClient(new Uri(_storageConfiguration.Url), sharedKeyCredential);
                var filesystem = serviceClient.GetFileSystemClient(storagePath);
                await filesystem.CreateDirectoryAsync(directoryName);

                return true;
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

        public async Task<bool> MoveFile(string storagePath, string fileName, string destinationDirectoryPath)
        {
            return await Execute(async (sharedKeyCredential) =>
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

                return true;
            });
        }

        public async Task<bool> UploadFile(string fileName, Stream fileContent)
        {
            return await Execute(async (sharedKeyCredential) =>
            {
                var client = new BlobClient(new Uri($"{_storageConfiguration?.Url}/{fileName}"), sharedKeyCredential);

                fileContent.Position = 0;
                await client.UploadAsync(fileContent);

                return true;
            });
        }

        private async Task<T> Execute<T>(Func<StorageSharedKeyCredential, Task<T>> func)
        {
            if (!string.IsNullOrEmpty(_storageConfiguration?.Url))
            {
                try
                {
                    var sharedKeyCredential = new StorageSharedKeyCredential(_storageConfiguration.AccountName, _storageConfiguration.AccountKey);

                    return await func(sharedKeyCredential);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.GetMessage());
                }
            }

            return default(T);
        }
    }
}

