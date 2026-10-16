using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PhotoAlbum.Services;

namespace PhotoAlbum.Pages;

/// <summary>
/// Page model for serving photo files with indirect access
/// </summary>
public class PhotoFileModel : PageModel
{
    private readonly IPhotoService _photoService;
    private readonly IPhotoStorageService _photoStorage;
    private readonly ILogger<PhotoFileModel> _logger;

    /// <summary>
    /// Initializes a new instance of the PhotoFileModel class
    /// </summary>
    /// <param name="photoService">Service for photo operations</param>
    /// <param name="configuration">Configuration instance</param>
    /// <param name="logger">Logger instance</param>
    public PhotoFileModel(
        IPhotoService photoService,
        IPhotoStorageService photoStorage,
        ILogger<PhotoFileModel> logger)
    {
        _photoService = photoService;
        _photoStorage = photoStorage;
        _logger = logger;
    }

    /// <summary>
    /// Serves a photo file by ID
    /// </summary>
    /// <param name="id">The ID of the photo to serve</param>
    /// <returns>File result with the photo, or NotFound if photo doesn't exist</returns>
    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            _logger.LogWarning("Photo file request with null ID");
            return NotFound();
        }

        try
        {
            var photo = await _photoService.GetPhotoByIdAsync(id.Value);

            if (photo == null)
            {
                _logger.LogWarning("Photo with ID {PhotoId} not found", id);
                return NotFound();
            }

            var fileBytes = await _photoStorage.DownloadAsync(photo.StoredFileName);
            if (fileBytes == null)
            {
                _logger.LogError("Stored photo not found for photo ID {PhotoId}", id);
                return NotFound();
            }

            _logger.LogDebug("Serving photo ID {PhotoId} ({FileName}, {FileSize} bytes)",
                id, photo.OriginalFileName, fileBytes.Length);

            // Return the file with appropriate content type and enable caching
            Response.Headers.CacheControl = "public,max-age=31536000"; // Cache for 1 year
            Response.Headers.ETag = $"\"{photo.Id}-{photo.UploadedAt.Ticks}\"";

            return File(fileBytes, photo.MimeType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error serving photo with ID {PhotoId}", id);
            return StatusCode(500);
        }
    }
}
