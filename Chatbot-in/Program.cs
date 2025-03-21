using Chatbot_in.Domain.Repositories;
using Chatbot_in.Domain.Service;
using Chatbot_in.Infrastructure.HetznerDbRepository;
using Chatbot_in.Infrastructure.IdentityDbRepository;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Chatbot_in.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Service to log httpRequests and responses
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.RequestMethod
    | HttpLoggingFields.RequestPath
    | HttpLoggingFields.RequestBody 
    | HttpLoggingFields.ResponseBody
    | HttpLoggingFields.ResponseStatusCode;
    logging.ResponseBodyLogLimit = 256;
    //| HttpLoggingFields.Response;
});
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
//Makes our front-end work
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:51767",
                                              "https://localhost:51767");
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

builder.Services.AddDbContext<IdentityDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.User.RequireUniqueEmail = true;
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
}).AddEntityFrameworkStores<IdentityDbContext>();

builder.Services.AddScoped<ITokenRepository, HetznerDbTokenRepository>();


var app = builder.Build();


app.UseHttpLogging();
app.UseCors(MyAllowSpecificOrigins);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
