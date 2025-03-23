using Chatbot_in.Domain.Repositories;
using Chatbot_in.Domain.Service;
using Chatbot_in.Infrastructure;
using Chatbot_in.Infrastructure.HetznerDbRepository;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Chatbot_in.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Service to log httpRequests and responses
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.RequestMethod
    | HttpLoggingFields.RequestHeaders
    | HttpLoggingFields.RequestPath
    | HttpLoggingFields.RequestBody
    | HttpLoggingFields.ResponseBody
    | HttpLoggingFields.ResponseStatusCode;
    logging.ResponseBodyLogLimit = 384;
    //| HttpLoggingFields.Response;
});
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
//Makes our front-end work
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy
                          .SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                      });
});

//For providing the Database connection information
var secrets = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register GeminiService
builder.Services.AddSingleton<GeminiService>();

builder.Services.AddDbContext<HetznerDbContext>(options =>
options.UseMySql(secrets["db:connectionstring"], ServerVersion.AutoDetect(secrets["db:connectionstring"]))
);

//builder.Services.AddDbContext<AuthDbContext>(
//options => options.UseMySql(secrets["db:connectionstring"], ServerVersion.AutoDetect(secrets["db:connectionstring"]))
//);
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.User.RequireUniqueEmail = true;
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
}).AddEntityFrameworkStores<HetznerDbContext>();

builder.Services.AddAuthentication(a =>
{
    a.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    a.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    a.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(opt =>
{
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(
    Encoding.ASCII.GetBytes(
    builder.Configuration.GetSection("JwtSettings")["Secret"]!
    )),
        ValidateIssuer = false,
        ValidateAudience = false,
        RequireExpirationTime = false,
        ValidateLifetime = true
    };
});

builder.Services.AddSwaggerGen(opt =>
{
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer "
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
{
{
new OpenApiSecurityScheme
{
Reference = new OpenApiReference
{
Type = ReferenceType.SecurityScheme,
Id = "Bearer"
}
}, new string[] {}
 }
});
});



builder.Services.AddScoped<ITokenRepository, HetznerDbTokenRepository>();
builder.Services.AddScoped<IContextFileInfoRepository, HetznerDbContextFileInfoRepository>();
builder.Services.AddScoped<IFileHandlingTemporary, HetznerFileHandling>();
builder.Services.AddScoped<ITokenService, JWTService>();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpLogging();
app.UseCors(MyAllowSpecificOrigins);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//SeedDB.Initialize(
//    app.Services.GetRequiredService<IServiceProvider>().CreateScope().ServiceProvider​
//    );

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
