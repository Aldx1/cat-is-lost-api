namespace cat_is_lost_api.Interfaces
{
    public interface IFileService
    {
        Task<string> SaveFile(IFormFile file, string directoryName, string fileName);
    }
}
