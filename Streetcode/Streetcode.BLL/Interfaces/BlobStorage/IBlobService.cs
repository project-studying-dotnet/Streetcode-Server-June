namespace Streetcode.BLL.Interfaces.BlobStorage;

public interface IBlobService
{
    public string SaveFileInStorage(string base64, string name, string mimeType);
    public void SaveFileInStorageBase64(string base64, string name, string extension);
    public MemoryStream FindFileInStorageAsMemoryStream(string name);
    public string UpdateFileInStorage(
        string previousBlobName,
        string base64Format,
        string newBlobName,
        string extension);
    public string FindFileInStorageAsBase64(string name);
    public void DeleteFileInStorage(string name);
    public Task CleanBlobStorage();
}
