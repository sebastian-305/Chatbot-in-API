using Microsoft.AspNetCore.HttpLogging;

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
