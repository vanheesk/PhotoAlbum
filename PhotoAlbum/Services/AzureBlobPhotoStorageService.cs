using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace PhotoAlbum.Services;

public class AzureBlobPhotoStorageService : IPhotoStorageService
{
    private readonly BlobContainerClient _container;

    public AzureBlobPhotoStorageService(BlobContainerClient container)
    {
        _container = container;
    }

    public async Task UploadAsync(string fileName, Stream content, string contentType)
    {
        await _container.CreateIfNotExistsAsync(PublicAccessType.None);
        var blob = _container.GetBlobClient(fileName);
        await blob.UploadAsync(content, new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders { ContentType = contentType }
        });
    }

    public async Task<byte[]?> DownloadAsync(string fileName)
    {
        try
        {
            var response = await _container.GetBlobClient(fileName).DownloadContentAsync();
            return response.Value.Content.ToArray();
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return null;
        }
    }

    public async Task DeleteAsync(string fileName)
    {
        await _container.GetBlobClient(fileName).DeleteIfExistsAsync();
    }
}
