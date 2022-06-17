using KevinZonda.BooCat.AspNetCoreWebAPI.Controllers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Map("/api/{req}", async (string req, string name, string[]? provider) =>
{
    if (req.ToLower() is not "allbooks" or "all" or "a")
        return await BooCatController.ProviderRequest(req, name);
    return await BooCatController.MultipleProviderRequest(provider, name);
});

app.Run();
