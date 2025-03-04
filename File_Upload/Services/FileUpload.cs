using Microsoft.AspNetCore.Components.Forms; //This provides IBrowserFile interface, which represents a file uploaded via browser

namespace File_Upload.Services
{
    // Any class implementing this interface must provide an UploadFile method that accepts an IBroswerFile and returns Task(asynchronous execution)
    public interface IFileUpload
    {
        Task UploadFile(IBrowserFile file);
        Task<string> GeneratePreviewUrl(IBrowserFile file);
    }

    public class FileUpload : IFileUpload
    {
        private IWebHostEnvironment _webHostEnviroment; // gives access to the servers wwwroot to save uploaded files
        private readonly ILogger<FileUpload> _logger; // used for logging errors

        public FileUpload(IWebHostEnvironment webHostEnviroment, ILogger<FileUpload> logger)
        {
            _logger = logger;
            _webHostEnviroment = webHostEnviroment;
        }
        public async Task UploadFile(IBrowserFile file)
        {
            if (file is not null)
            {
                try
                {                                                  //WebRootPath points to wwwroot/
                                                 //file will be saved in wwwroot/uploads/ under it original name
                    var uploadPath = Path.Combine(_webHostEnviroment.WebRootPath, "uploads", file.Name);
                                       //Opens a stream for reading the file contents
                                      //Stream is a way to read data sequentially(ნიშნავს თანმიმდევრობით)
                                     //OpenReadStream method opens the uploaded file and reads it
                                    // using Syntax in this case waits for OpeanReadStream to read the data
                                   // of the uploaded file once the OpenReadStream reads all of the data
                                  // the using Syntax will close that file.
                                 // the close means that it will release memory and file handels to avoid memory
                                // leaks due to the file staying open too long.
                    using (var stream = file.OpenReadStream())
                    {
                                        //Creates a new file in the uploads folder
                        var fileStream = File.Create(uploadPath);
                                    //Copies the uploaded file's data to the newly created file
                        await stream.CopyToAsync(fileStream);
                        // Releases resources when you're done using the file like memory ot file handles
                        fileStream.Close();
                    }
                } //If error occurs, it logs the error using _logger.LogError()
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString()); 
                }
            }
        }


        public async Task<string> GeneratePreviewUrl(IBrowserFile file)
        {
            if (!file.ContentType.Contains("image"))
            {
                if(file.ContentType.Contains("pdf"))
                {
                    return "images/PDF_file_icon.svg.png";
                }
            }

            var resizedImage = await file.RequestImageFileAsync(file.ContentType, 100, 100);
            var buffer = new byte[resizedImage.Size];
            await resizedImage.OpenReadStream().ReadAsync(buffer);
            //Generating a preview URL for the image
            return $"data:{file.ContentType};base64,{Convert.ToBase64String(buffer)}";
        }
    }
}