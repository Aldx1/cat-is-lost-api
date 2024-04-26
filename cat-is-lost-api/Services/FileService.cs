using Azure.Storage.Files.Shares;
using cat_is_lost_api.Interfaces;

namespace cat_is_lost_api.Services
{
    public class FileService : IFileService
    {

        private readonly IConfiguration _configuration;
        private readonly Serilog.ILogger _logger;
        
        public FileService(IConfiguration configuration, IUserService userService, Serilog.ILogger logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string> SaveFile(IFormFile file, string directoryName, string fileName)
        {

            string connectionString = _configuration.GetConnectionString("FILESCONNSTR_1");
            string fileShare = "cat-is-lost";

            ShareClient share = new ShareClient(connectionString, fileShare);

            // Create the share if it doesn't already exist
            await share.CreateIfNotExistsAsync();

            // Ensure that the share exists
            if (await share.ExistsAsync())
            {
                // Get a reference to the sample directory
                ShareDirectoryClient directory = share.GetDirectoryClient(directoryName);

                // Create the directory if it doesn't already exist
                await directory.CreateIfNotExistsAsync();

                // Ensure that the directory exists
                if (await directory.ExistsAsync())
                {
                    try
                    {
                        // Get a reference to a file object
                        ShareFileClient scFile = directory.GetFileClient(fileName);

                        await scFile.CreateAsync(file.Length);
                        await scFile.UploadAsync(file.OpenReadStream());
                        return $"{fileShare}/{directoryName}/{fileName}";
                    }
                    catch(Exception ex)
                    {
                        _logger.Error<Exception>(ex.Message, ex);
                    }
                }
            }
            else
            {
                Console.WriteLine($"CreateShareAsync failed");
                
            }
            return "";
        }
    }
}
