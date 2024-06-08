using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiscalFlow.Application.Core.Abstractions.Services;

public interface IImageService
{
    public Task<string> UploadImageAsync(string base64Image);
    public Task<string> DeleteImageAsync(string publicId);
    public Task<string> GetImageAsync(string publicId);
}
