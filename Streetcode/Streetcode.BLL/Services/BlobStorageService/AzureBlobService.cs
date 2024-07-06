using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.Services.BlobStorageService
{
    public class AzureBlobService : IBlobService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly BlobContainerClient _containerClient;
        private readonly IRepositoryWrapper _repositoryWrapper;

        public AzureBlobService(IOptions<BlobEnvironmentVariables> options, IRepositoryWrapper? repositoryWrapper = null)
        {
            _blobServiceClient = new BlobServiceClient(new Uri(options.Value.BlobServiceEndpoint), new Azure.Storage.StorageSharedKeyCredential(options.Value.StorageAccountName, options.Value.StorageAccountKey));
            _containerClient = _blobServiceClient.GetBlobContainerClient(options.Value.ContainerName);
            _containerClient.CreateIfNotExists();
            _repositoryWrapper = repositoryWrapper;
        }

        public MemoryStream FindFileInStorageAsMemoryStream(string name)
        {
            var blobClient = _containerClient.GetBlobClient(name);
            var downloadInfo = blobClient.DownloadContent();
            var memoryStream = new MemoryStream(downloadInfo.Value.Content.ToArray());
            return memoryStream;
        }

        public string FindFileInStorageAsBase64(string name)
        {
            var blobClient = _containerClient.GetBlobClient(name);
            var downloadInfo = blobClient.DownloadContent();

            var base64String = Convert.ToBase64String(downloadInfo.Value.Content.ToArray());
            return base64String;
        }

        public string SaveFileInStorage(string base64, string name, string mimeType)
        {
            var blobClient = _containerClient.GetBlobClient(name);

            var bytes = Convert.FromBase64String(base64);
            using (var stream = new MemoryStream(bytes))
            {
                blobClient.Upload(stream, new BlobHttpHeaders { ContentType = mimeType });
            }

            return blobClient.Uri.ToString();
        }

        public void SaveFileInStorageBase64(string base64, string name, string extension)
        {
            var blobClient = _containerClient.GetBlobClient($"{name}.{extension}");
            var bytes = Convert.FromBase64String(base64);

            using (var stream = new MemoryStream(bytes))
            {
                blobClient.Upload(stream, new BlobHttpHeaders { ContentType = $"application/{extension}" });
            }
        }

        public void DeleteFileInStorage(string name)
        {
            var blobClient = _containerClient.GetBlobClient(name);
            blobClient.DeleteIfExists();
        }

        public string UpdateFileInStorage(string previousBlobName, string base64Format, string newBlobName, string extension)
        {
            DeleteFileInStorage(previousBlobName);
            var newBlobUri = SaveFileInStorage(base64Format, newBlobName, extension);
            return newBlobUri;
        }

        public async Task CleanBlobStorage()
        {
            var blobs = _containerClient.GetBlobsAsync();
            var blobNames = new List<string>();

            await foreach (var blob in blobs)
            {
                blobNames.Add(blob.Name);
            }

            var existingImagesInDatabase = await _repositoryWrapper.ImageRepository.GetAllAsync();
            var existingAudiosInDatabase = await _repositoryWrapper.AudioRepository.GetAllAsync();

            List<string> existingMedia = new();
            existingMedia.AddRange(existingImagesInDatabase.Select(img => img.BlobName));
            existingMedia.AddRange(existingAudiosInDatabase.Select(img => img.BlobName));

            var filesToRemove = blobNames.Except(existingMedia).ToList();

            foreach (var file in filesToRemove)
            {
                var blobClient = _containerClient.GetBlobClient(file);
                await blobClient.DeleteIfExistsAsync();
            }
        }
    }
}
