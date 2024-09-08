using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using couchspaces.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using couchspaces;
using MudBlazor.Services;
using MudBlazor;
using Google.Api;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

// Register services
builder.Services.AddScoped<SpaceService>();
builder.Services.AddScoped<FirebaseService>();
builder.Services.AddSingleton<SignalRService>();
builder.Services.AddScoped<TokenManagerService>();
builder.Services.AddScoped<TokenValidationService>();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<AuthenticationStateProvider, CouchspacesAuthenticationStateProvider>();
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
