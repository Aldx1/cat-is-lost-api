using cat_is_lost_api.Models;
using cat_is_lost_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cat_is_lost_api.Controllers
{
    [ApiController]
    [Route("api/posts")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
   
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;
        public PostsController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> GetAllPosts()
        {
            var posts = await _postService.GetAllPosts();
            if (posts == null || posts.Count() == 0) return NoContent();
            return Ok(posts);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Post>>> GetPost(int id)
        {
            var post = await _postService.GetPost(id);
            if (post == null) return NotFound();
            return Ok(post);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Post>> AddPost([FromForm] Post post)
        {
            var newPost = await _postService.AddPost(post);
            return Ok(newPost);
        }

        [HttpPut]
        [Authorize]
        public async Task<ActionResult> UpdatePost(int id, [FromForm] Post post)
        {
            if (id != post.Id) return BadRequest();
            var newPost = await _postService.UpdatePost(id, post);
            return Ok();
        }

        [HttpDelete]
        [Authorize]
        public async Task<ActionResult<bool>> DeletePost(int id)
        {
            await _postService.DeletePost(id);
            return Ok();
        }
    }
}
