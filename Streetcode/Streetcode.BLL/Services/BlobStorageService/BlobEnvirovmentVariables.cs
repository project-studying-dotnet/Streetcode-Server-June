namespace Streetcode.BLL.Services.BlobStorageService;

public class BlobEnvironmentVariables
{
    public string BlobStoreKey { get; set; } = string.Empty;
    public string BlobStorePath { get; set; } = string.Empty;
    public string BlobServiceEndpoint { get; set; } = string.Empty;
    public string StorageAccountName { get; set; } = string.Empty;
    public string StorageAccountKey { get; set; } = string.Empty;
    public string ContainerName { get; set; } = string.Empty;
    public string BlobStorageLocalConnectionString { get; set; } = string.Empty;
}