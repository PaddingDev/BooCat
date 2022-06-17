using KevinZonda.BooCat.AspNetCoreWebAPI.Controllers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Map("/api/{provider}", async (string provider, string name) =>
{
    return await BooCatController.ProviderRequest(provider, name);
});

app.Run();
