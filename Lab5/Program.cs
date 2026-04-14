using Lab5.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<ILibraryService, LibraryService>();

var app = builder.Build();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<Lab5.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
