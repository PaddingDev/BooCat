using KevinZonda.BooCat.AspNetCoreWebAPI.Controllers;

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddMemoryCache();

builder.Services.AddStackExchangeRedisCache(x =>
{
    x.Configuration = builder.Configuration.GetConnectionString("Redis");
});

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Map("/api/{req}", async (string req, string name, HttpContext c, IMemoryCache? mem, IDistributedCache? redis) =>
{
    if (!(req.ToLower() is "allbooks" or "all" or "a"))
        return await BooCatController.ProviderRequest(req, name);

    string[]? providers = c.Request.Query["provider"];
    return await BooCatController.MultipleProviderRequest(providers, name);
});

app.Run();
