using GDSwithUI.Components;
using GdsApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Read base URL from config/environment, fall back to localhost for local dev
var gdsBaseUrl = builder.Configuration["GdsApi:BaseUrl"] ?? "https://localhost:8081";
var disableSsl = builder.Configuration["GdsApi:DisableSslValidation"] == "true";

// Single named HttpClient with optional SSL bypass (needed for self-signed cert in Docker)
builder.Services.AddHttpClient("GdsApi", client =>
{
    client.BaseAddress = new Uri(gdsBaseUrl);
}).ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler();
    if (disableSsl)
        handler.ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
    return handler;
});

builder.Services.AddScoped<IApplicationsGdsApi>(sp =>
{
    var client = sp.GetRequiredService<IHttpClientFactory>().CreateClient("GdsApi");
    return new ApplicationsGdsApi(client) { BaseUrl = gdsBaseUrl };
});

builder.Services.AddScoped<ICertificateGroupsGdsApi>(sp =>
{
    var client = sp.GetRequiredService<IHttpClientFactory>().CreateClient("GdsApi");
    return new CertificateGroupsGdsApi(client) { BaseUrl = gdsBaseUrl };
});

builder.Services.AddScoped<ILoginGdsApi>(sp =>
{
    var client = sp.GetRequiredService<IHttpClientFactory>().CreateClient("GdsApi");
    return new LoginGdsApi(client) { BaseUrl = gdsBaseUrl };
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
