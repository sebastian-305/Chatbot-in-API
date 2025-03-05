using Microsoft.AspNetCore.HttpLogging;

var builder = WebApplication.CreateBuilder(args);


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


// Add services to the container.

var secrets = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

Console.WriteLine("Secret:");
Console.WriteLine(secrets["TestSecret"]);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseHttpLogging();

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
