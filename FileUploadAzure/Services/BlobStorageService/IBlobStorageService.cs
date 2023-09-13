namespace FileUploadAzure.Services
{
    public interface IBlobStorageService
    {
        Task<string> UploadFileToBlobAsync(string strFileName, string contecntType, Stream fileStream, string userEmail);
    }
}
