using Microsoft.AspNetCore.StaticFiles;
using Microsoft.JSInterop;

namespace File_Upload.Services
{
    public interface IFileDownload
    {
        Task<List<String>> GetUploadedFiles();
        Task DownloadFile(string url);
    }
    public class FileDownload : IFileDownload
    {
       private IWebHostEnvironment _webHostEnviroment;
        private readonly IJSRuntime _js;

        public FileDownload(IWebHostEnvironment webHostEnvironment, IJSRuntime js)
        {
                _webHostEnviroment = webHostEnvironment;
                _js = js;   
        }
        public async Task<List<string>> GetUploadedFiles()
        {
            var base64Urls = new List<string>();
            var uploadPath = Path.Combine(_webHostEnviroment.WebRootPath, "uploads");
            var files = Directory.GetFiles(uploadPath);

            if(files is not null && files.Length > 0)
            {
                foreach (var file in files)
                {
                    using(var fileInput = new FileStream(file, FileMode.Open, FileAccess.Read))
                    {
                        var memoryStream = new MemoryStream();
                        await fileInput.CopyToAsync(memoryStream);

                        var buffer = memoryStream.ToArray();
                        var fileType = GetMimeTypeForFileExtension(file);
                        base64Urls.Add($"data:{fileType}; base64Urls,{Convert.ToBase64String(buffer)}");
                    }
                }
            }
            return base64Urls;
        }

        public async Task DownloadFile(string url)
        {
            await _js.InvokeVoidAsync("downloadFile", url);
        }

        //Mime type is for example png or jpeg
        private string GetMimeTypeForFileExtension(string filePath)
        {
            //This Basically means that if some files doesnt have a content type, it will return the default content type octet-stream.
            const string DefaultContentType = "application/octet-stream";

            var provider = new FileExtensionContentTypeProvider();
            //If the file extension is not found in the provider, it will return DefaultContentType octet-stream.
            //So it will return DefaultContentType if for example the file is a .txt file.
            if (!provider.TryGetContentType(filePath, out string contentType))
            {
                return DefaultContentType;
            }

            return contentType;
        }

       
    }
}
