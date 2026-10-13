using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Azure.Identity;
using Azure.Storage.Blobs;
using PhotoAlbum.Data;
using PhotoAlbum.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Authentication/authorization: the gallery is public, but state-changing
// operations (photo delete) require an authenticated admin (CWE-306).
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.AccessDeniedPath = "/Login";
    });
builder.Services.AddAuthorization();

// Add DbContext
builder.Services.AddDbContext<PhotoAlbumContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register PhotoService
builder.Services.AddScoped<IPhotoService, PhotoService>();

var blobEndpoint = builder.Configuration["AzureStorageBlob:Endpoint"];
if (!string.IsNullOrWhiteSpace(blobEndpoint))
{
    var containerName = builder.Configuration["AzureStorageBlob:ContainerName"] ?? "photos";
    builder.Services.AddSingleton(new BlobContainerClient(
        new Uri(blobEndpoint),
        containerName,
        new DefaultAzureCredential()));
    builder.Services.AddSingleton<IPhotoStorageService, AzureBlobPhotoStorageService>();
}
else
{
    var uploadPath = builder.Configuration["FileUpload:UploadPath"] ?? "wwwroot/uploads";
    builder.Services.AddSingleton<IPhotoStorageService>(
        new LocalPhotoStorageService(Path.Combine(builder.Environment.ContentRootPath, uploadPath)));
}

// Configure form options for file uploads
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10485760; // 10MB
    options.ValueLengthLimit = 10485760;
    options.MultipartBoundaryLengthLimit = 128;
});

var app = builder.Build();

// Run database migrations on startup in all environments (skip only when flagged as test)
var isTestEnvironment = app.Configuration.GetValue<bool>("IsTestEnvironment");
if (!isTestEnvironment)
{
    try
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PhotoAlbumContext>();
        await context.Database.MigrateAsync();
    }
    catch (Exception ex)
    {
        // Log and rethrow to fail fast if migrations cannot be applied
        Console.WriteLine($"Database migration failed: {ex}");
        throw;
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

// Enable static files with cache headers
app.UseStaticFiles(new Microsoft.AspNetCore.Builder.StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        // Cache static files for 1 hour
        ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=3600");
    }
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();

// Make the implicit Program class public for testing
public partial class Program { }
