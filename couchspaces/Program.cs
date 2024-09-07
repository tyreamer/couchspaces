using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using couchspaces.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using couchspaces;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

// Register services
builder.Services.AddScoped<FirebaseService>();
builder.Services.AddScoped<TokenManagerService>();
builder.Services.AddScoped<TokenValidationService>();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<CouchspacesAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<CouchspacesAuthenticationStateProvider>());
builder.Services.AddMudServices();
builder.Services.AddAuthorizationCore();

await builder.Build().RunAsync();