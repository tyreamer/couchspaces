using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using couchspaces.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using couchspaces;
using MudBlazor.Services;
using couchspacesShared.Services;
using MudBlazor;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

// Register HttpClient with the base address of the backend
builder.Services.AddHttpClient("BackendAPI", client =>
{
    client.BaseAddress = new Uri("https://localhost:7160"); // Backend URL
});

// Register services
builder.Services.AddScoped<FirebaseService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<TokenManagerService>();
builder.Services.AddScoped<TokenValidationService>();
builder.Services.AddScoped<AuthenticationStateProvider, CouchspacesAuthenticationStateProvider>();

// Register singleton services using a factory method
builder.Services.AddSingleton(sp => new SignalRService("https://localhost:7160/couchspaceshub"));
builder.Services.AddSingleton<SpaceService>();

// Add local storage
builder.Services.AddBlazoredLocalStorage();

// Add MudBlazor services
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;
    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 10000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
});

builder.Services.AddAuthorizationCore();

await builder.Build().RunAsync();
