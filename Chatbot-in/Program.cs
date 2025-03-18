using Chatbot_in.Domain.Repositories;
using Chatbot_in.Domain.Service;
using Chatbot_in.Infrastructure;
using Chatbot_in.Infrastructure.HetznerDbRepository;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;

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

builder.Services.AddDbContext<HetznerDbContext>(options =>
options.UseMySql(secrets["db:connectionstring"], ServerVersion.AutoDetect(secrets["db:connectionstring"]))
);

builder.Services.AddScoped<ITokenRepository, HetznerDbTokenRepository>();
builder.Services.AddScoped<IContextFileInfoRepository, HetznerDbContextFileInfoRepository>();
builder.Services.AddScoped<IFileHandlingTemporary, HetznerFileHandling>();

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
