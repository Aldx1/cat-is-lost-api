
using cat_is_lost_api.Interfaces;
using cat_is_lost_api.Models;
using Microsoft.EntityFrameworkCore;
using cat_is_lost_api.Utils;

namespace cat_is_lost_api.Services
{
    public class PostService : IPostService
    {
        private readonly IDbContext _dbContext;
        private readonly IFileService _fileService;
        private readonly IUserService _userService;
        private readonly Serilog.ILogger _logger;

        public PostService(IDbContext dbContext, IFileService fileService, IUserService userService, Serilog.ILogger logger)
        {
            _dbContext = dbContext;
            _fileService = fileService;
            _userService = userService;
            _logger = logger;
        }


        public async Task<Post> GetPost(int id)
        {
            try
            {
                var post = await _dbContext.Posts.FindAsync(id);
                return post;
            }
            catch(Exception ex)
            {
                _logger.Error<Exception>(ex.Message, ex);
                return null;
            }
        }


        public async Task<Post?> AddPost(Post post)
        {
            try
            {
                var userId = _userService.GetUserId().Value;

                List<string> filePaths = new List<string>();
                string dTicks = DateTime.Now.Ticks.ToString();

                // Save the images to the file storage, store filepath in db on successful write
                for (int i = 0; i < post.Files.EmptyIfNull().Count(); i++)
                {
                    string fileExt = Path.GetExtension(post.Files[i].FileName);
                    string fileName = $"{dTicks}_{i}{fileExt}";

                    var filePath = await _fileService.SaveFile(post.Files[i], userId.ToString(), fileName);

                    if (!string.IsNullOrWhiteSpace(filePath)) { filePaths.Add(filePath); }
                    else { _logger.Error($"Error saving file {post.Files[i].Name}"); }
                }

                post.User_Id = userId;
                post.Pictures = string.Join(',', filePaths);

                _dbContext.Posts.Add(post);
                await _dbContext.SaveChangesAsync();

                return post;
            }
            catch(Exception ex)
            {
                _logger.Error<Exception>(ex.Message, ex);
                return null;
            }
        }

        public async Task<bool> DeletePost(int id)
        {
            try
            {
                var post = await _dbContext.Posts.FindAsync(id);
                if (post != null)
                {
                    _dbContext.Posts.Remove(post);
                    await _dbContext.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error<Exception>(ex.Message, ex);
                return false;
            }
        }

        public async Task<IEnumerable<Post>?> GetAllPosts()
        {
            try
            {
                return await _dbContext.Posts.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.Error<Exception>(ex.Message, ex);
                return null;
            }
        }

        public async Task<Post?> UpdatePost(int postId, Post post)
        {
            try
            {
                var oldPost = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == postId);
                oldPost = post;
                await _dbContext.SaveChangesAsync();
                return post;
            }
            catch (Exception ex)
            {
                _logger.Error<Exception>(ex.Message, ex);
                return null;
            }
        }
    }
}
