using couchspacesBackend.Hubs;
using couchspacesBackend.Services;
using couchspacesShared.Services;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

var builder = WebApplication.CreateBuilder(args);

// MUST BE FIRST - Add CORS policy
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

// Initialize Firebase Admin SDK
FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile("D:\\Projects\\couchspaces\\couchspacesBackend\\couchspaces-firebase-adminsdk-5tn0i-90922ab37a.json") //TODO: convert to environment variable
});

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<FirebaseService>();
builder.Services.AddSignalR();

// Configure the URLs and ports based on the environment
if (builder.Environment.IsDevelopment())
{
    var devHttpURI = "http://localhost:5015";
    var devHttpsURI = "https://localhost:7160";

    builder.WebHost.UseUrls(devHttpURI, devHttpsURI);

    // Register HttpClient with the development base address
    builder.Services.AddHttpClient<SpaceService>(client =>
    {
        client.BaseAddress = new Uri(devHttpURI); // Development URL http
    });
}
else
{
    //TODO: update
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
