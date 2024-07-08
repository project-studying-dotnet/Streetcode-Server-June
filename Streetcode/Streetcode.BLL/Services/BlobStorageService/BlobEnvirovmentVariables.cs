namespace Streetcode.BLL.Services.BlobStorageService;

public class BlobEnvironmentVariables
{
    public string BlobStoreKey { get; set; } = string.Empty;
    public string BlobStorePath { get; set; } = string.Empty;
    public string ContainerName { get; set; } = string.Empty;
    public string BlobStorageConnectionString { get; set; } = string.Empty;
}