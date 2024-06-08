using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using FiscalFlow.Application.Core.Abstractions.Services;
using Microsoft.Extensions.Configuration;

namespace FiscalFlow.Infrastructure.Services;

internal sealed class ImageService : IImageService
{
    private IConfiguration _config;
    private readonly Cloudinary _cloudinary;
    public ImageService(IConfiguration config)
    {
        _config = config;
        _cloudinary = new Cloudinary(_config["Cloudinary:Url"]);
        _cloudinary.Api.Secure = true;
    }

    public async Task<string> DeleteImageAsync(string publicId)
    {
        var deletionParams = new DeletionParams(publicId);
        var deletionResult = await _cloudinary.DestroyAsync(deletionParams);

        if (deletionResult.Result == "ok")
        {
            return $"Image with public ID {publicId} deleted successfully.";
        }
        else
        {
            throw new Exception($"Failed to delete image with public ID {publicId}. Error: {deletionResult.Error.Message}");
        }
    }

    public async Task<string> GetImageAsync(string publicId)
    {
        var url = _cloudinary.Api.UrlImgUp.BuildUrl(publicId);
        return await Task.FromResult(url);
    }

    public async Task<string> UploadImageAsync(string base64Image)
    {
        var uploadParams = new ImageUploadParams()
        {
            File = new FileDescription(@$"data:image/png;base64,{base64Image}"),
            Transformation = new Transformation().Quality("auto").FetchFormat("auto")
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
        {
            return uploadResult.SecureUrl.ToString();
        }
        else
        {
            throw new Exception($"Failed to upload image. Error: {uploadResult.Error.Message}");
        }
    }
}
