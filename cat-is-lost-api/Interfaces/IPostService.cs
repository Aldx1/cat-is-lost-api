using cat_is_lost_api.Models;

namespace cat_is_lost_api.Services
{
    public interface IPostService
    {
        /// <summary>
        /// Find post using id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Post> GetPost(int id);

        /// <summary>
        /// Get all posts
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Post>> GetAllPosts();

        /// <summary>
        /// Add post
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        Task<Post> AddPost(Post post);

        /// <summary>
        /// Update existing post
        /// </summary>
        /// <param name="postId"></param>
        /// <param name="post"></param>
        /// <returns></returns>
        Task<Post> UpdatePost(int postId, Post post);

        /// <summary>
        /// Delete post 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> DeletePost(int id);
    }
}
