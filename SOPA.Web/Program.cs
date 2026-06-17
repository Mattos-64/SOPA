using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SOPA.Web;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7181/") });

builder.Services.AddScoped<SOPA.Web.Services.AnimalService>();

// Adiciona o suporte ao LocalStorage do navegador
builder.Services.AddBlazoredLocalStorage();

// Ativa o sistema de autorização nativa do Blazor
builder.Services.AddAuthorizationCore();

// Substitui o gerenciador padrão do Blazor pelo nosso Customizador que lê o Token JWT
builder.Services.AddScoped<AuthenticationStateProvider, SOPA.Web.Security.CustomAuthenticationStateProvider>();
builder.Services.AddScoped<SOPA.Web.Security.CustomAuthenticationStateProvider>(sp =>
    (SOPA.Web.Security.CustomAuthenticationStateProvider)sp.GetRequiredService<AuthenticationStateProvider>());

await builder.Build().RunAsync();