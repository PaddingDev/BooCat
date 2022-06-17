using KevinZonda.BooCat.AspNetCoreWebAPI.Controllers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Map("/api/{req}", async (string req, string name, HttpContext c) =>
{
    if (req.ToLower() is not "allbooks" or "all" or "a")
        return await BooCatController.ProviderRequest(req, name);

    string[]? providers = c.Request.Query["provider"];
    return await BooCatController.MultipleProviderRequest(providers, name);
});

app.Run();
