namespace PhotoAlbum.Services;

public class LocalPhotoStorageService : IPhotoStorageService
{
    private readonly string _uploadPath;

    public LocalPhotoStorageService(string uploadPath)
    {
        _uploadPath = uploadPath;
    }

    public async Task UploadAsync(string fileName, Stream content, string contentType)
    {
        Directory.CreateDirectory(_uploadPath);
        var path = Path.Combine(_uploadPath, fileName);
        await using var destination = new FileStream(path, FileMode.Create, FileAccess.Write);
        await content.CopyToAsync(destination);
    }

    public async Task<byte[]?> DownloadAsync(string fileName)
    {
        var path = Path.Combine(_uploadPath, fileName);
        return File.Exists(path) ? await File.ReadAllBytesAsync(path) : null;
    }

    public Task DeleteAsync(string fileName)
    {
        var path = Path.Combine(_uploadPath, fileName);
        if (File.Exists(path))
        {
            File.Delete(path);
        }

        return Task.CompletedTask;
    }
}
