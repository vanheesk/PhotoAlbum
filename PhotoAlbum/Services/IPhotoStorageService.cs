namespace PhotoAlbum.Services;

public interface IPhotoStorageService
{
    Task UploadAsync(string fileName, Stream content, string contentType);

    Task<byte[]?> DownloadAsync(string fileName);

    Task DeleteAsync(string fileName);
}
