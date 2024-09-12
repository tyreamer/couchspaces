using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using couchspaces.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using couchspaces;
using MudBlazor.Services;
using MudBlazor;
using couchspacesShared.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

// Register scoped services
builder.Services.AddScoped<FirebaseService>();
builder.Services.AddScoped<SpaceService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<TokenManagerService>();
builder.Services.AddScoped<TokenValidationService>();
builder.Services.AddScoped<AuthenticationStateProvider, CouchspacesAuthenticationStateProvider>();

// Register singleton services
builder.Services.AddSingleton<SignalRService>();

// Add local storage
builder.Services.AddBlazoredLocalStorage();

// Add mud services
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

// Configure HttpClient to use the backend's base address
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5015") });

await builder.Build().RunAsync();
