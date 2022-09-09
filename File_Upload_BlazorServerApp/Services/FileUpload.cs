using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace File_Upload_BlazorServerApp.Services
{
    public interface IFileUpload
    {
        Task UploadFile(IBrowserFile file);
        Task<string> GeneratePreviewUrl(IBrowserFile file);

        void JsInterOp();
    }

    public class FileUpload : IFileUpload
    {
        private IWebHostEnvironment _webHostEnvironment;

        private readonly ILogger<FileUpload> _logger;

        private readonly IJSRuntime _jsRuntime;

        public FileUpload(
            IWebHostEnvironment webHostEnvironment,
            ILogger<FileUpload> logger,
            IJSRuntime jsRuntime)
        {
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
            _jsRuntime = jsRuntime;
        }

        public async Task UploadFile(IBrowserFile file)
        {
            if (file is not null)
            {
                try
                {
                    var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", file.Name);

                    using (var stream = file.OpenReadStream())
                    {
                        var fileStream = File.Create(uploadPath);
                        await stream.CopyToAsync(fileStream);
                        fileStream.Close();
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e.ToString());
                }
            }
        }

        public async Task<string> GeneratePreviewUrl(IBrowserFile file)
        {
            if (!file.ContentType.Contains("image"))
            {
                if (file.ContentType.Contains("pdf"))
                {
                    return "images/pdf_logo.png";
                }
            }

            var resizedImage = await file.RequestImageFileAsync(file.ContentType, 100, 100);
            var buffer = new byte[resizedImage.Size];
            await resizedImage.OpenReadStream().ReadAsync(buffer);
            return $"data:{file.ContentType};base64,{Convert.ToBase64String(buffer)}";
        }

        public void JsInterOp()
        {
            _jsRuntime.InvokeVoidAsync("JsIntroOp");
        }
    }
}
