
using cat_is_lost_api.Interfaces;
using cat_is_lost_api.Models;
using cat_is_lost_api.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace cat_is_lost_api.Services
{
    public class UserService : IUserService
    {
        private readonly IDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Serilog.ILogger _logger;
        public UserService(IDbContext dbContext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, Serilog.ILogger logger)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<User?> GetUser(int userId)
        {
            try
            {
                return await _dbContext.Users.FindAsync(userId);
            }
            catch(Exception ex) 
            {
                _logger.Error<Exception>(ex.Message, ex);
                return null;
            }
        }

        public async Task<(bool, string)> AddUser(User user)
        {
            try
            {
                // Return reason new user isn't valid
                var userValid = await NewUserValid(user.Email, user.Password);
                if (!userValid.Item1) return (false, userValid.Item2);

                // Hash then store password if new user is valid
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(user.Password);
                var newUser = new User() { Email = user.Email, Password = passwordHash, Name = user.Name, Phone = user.Phone };
                _dbContext.Users.Add(newUser);
                await _dbContext.SaveChangesAsync();

                var token = GenerateJwtToken(newUser);

                return (true, token); 
            }
            catch (Exception ex)
            {
                _logger.Error<Exception>(ex.Message, ex);
                return (false, "");
            }
        }
        
        public async Task<string?> AuthenticateUser(Login login)
        {
            try
            {
                // If user doesn't exist
                var user = await _dbContext.Users.Where(u => u.Email.ToLower() == login.Email.Trim().ToLower()).FirstOrDefaultAsync();
                if (user == null) return null;

                // If password is wrong
                if (!VerifyPassword(user, login.Password)) return null;

                // Return generated token
                var token = GenerateJwtToken(user);
                return token;
            }
            catch (Exception ex)
            {
                _logger.Error<Exception>(ex.Message, ex);
                return null;
            }
        }

        public int? GetUserId()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context?.User?.Identity?.IsAuthenticated == true)
            {
                // Find the user id stored from jwt middleware
                int userId;
                string userIdStr = context.User.Claims.EmptyIfNull().FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdStr, out userId))
                {
                    return null;
                }
                return userId;
            }

            return null;
        }

        private async Task<(bool, string)> NewUserValid(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email)) return (false, "email cannot be empty");

            if (email.Length <= 5) return (false, "email must be at least 6 chars");

            if (string.IsNullOrEmpty(password)) return (false, "Password cannot be empty");

            if (password.Length <= 5) return (false, "Password must be at least 6 chars");

            if (_dbContext.Users.Any(u => u.Email == email)) return (false, "email already exists");

            // New user is valid
            return (true, "");
        }

        private string GenerateJwtToken(User user)
        {
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetConnectionString("JWT_KEY")));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Email),
        };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);

            return token;
        }

        private bool VerifyPassword(User user, string password)
        {
            if (user == null || user.Password == null)
            {
                return false;
            }

            var isValid = BCrypt.Net.BCrypt.Verify(password, user.Password);
            return isValid;
        }
    }
}
