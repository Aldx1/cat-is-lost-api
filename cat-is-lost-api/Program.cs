using cat_is_lost_api.Contexts;
using cat_is_lost_api.Interfaces;
using cat_is_lost_api.Services;
using Serilog;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

string dbconnectionstr = builder.Configuration.GetConnectionString("SQLAZURECONNSTR_1"); 

// Add services to the container.
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IPostService, PostService>();
builder.Services.AddTransient<IFileService, FileService>(); 
builder.Services.AddScoped<IDbContext, CatIsLostDbContext>();
builder.Services.AddDbContext<CatIsLostDbContext>(opt => opt.UseSqlServer(dbconnectionstr));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins(["http://localhost:5173", "https://mycatislost.azurewebsites.net"])
                          .AllowAnyHeader()
                          .AllowAnyMethod());
});

builder.Services.AddControllers();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetConnectionString("JWT_KEY"))),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false,
                };
            });

builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthorization();

var logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.File("logs/CatIsLost.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Logging.AddSerilog(logger);
builder.Services.AddSingleton<Serilog.ILogger>(logger);


var app = builder.Build();

app.UseCors("AllowSpecificOrigin");

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<JwtMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

app.MapControllers();

app.Run();

public partial class CatProgram { }