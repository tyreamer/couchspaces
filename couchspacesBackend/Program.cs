using couchspacesBackend.Hubs;
using couchspacesBackend.Services;
using couchspacesShared.Repositories;
using couchspacesShared.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

// Register singleton services
builder.Services.AddSingleton<FirebaseService>();
builder.Services.AddSingleton(sp => new SignalRService("https://localhost:7160/couchspaceshub"));
builder.Services.AddSingleton<MessageRepository>();
builder.Services.AddSingleton<MessageService>();

builder.Services.AddTransient<SpaceService>();

// Register IHttpClientFactory
builder.Services.AddHttpClient();

// Redis configuration
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = ConfigurationOptions.Parse("localhost:6379", true);
    return ConnectionMultiplexer.Connect(configuration);
});

// Configure the URLs and ports based on the environment
if (builder.Environment.IsDevelopment())
{
    var devHttpURI = "http://localhost:5015";
    var devHttpsURI = "https://localhost:7160";

    builder.WebHost.UseUrls(devHttpURI, devHttpsURI);
}
else
{
    //TODO: update for production
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.UseCors("AllowAll");

app.MapControllers();

app.MapHub<CouchspacesHub>("/couchspaceshub");

app.Run();
