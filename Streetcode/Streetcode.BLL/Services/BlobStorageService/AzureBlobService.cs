using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.Services.BlobStorageService;

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
        // write code for Azure Blob Storage
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

    public Task CleanBlobStorage()
    {
        throw new NotImplementedException();
    }
}
