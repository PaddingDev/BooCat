using KevinZonda.BooCat.AspNetCoreWebAPI.Controllers;

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// builder.Services.AddMemoryCache();

builder.Services.AddStackExchangeRedisCache(x =>
{
    string addr = builder.Configuration["Redis"];
    if (string.IsNullOrEmpty(addr)) addr = "localhost";

    string absoluteExpire = builder.Configuration["RedisAbsoluteExpire"];
    if (!int.TryParse(absoluteExpire, out int aExp)) aExp = 45; // min

    string slidingExpire = builder.Configuration["RedisSlidingExpire"];
    if (!int.TryParse(slidingExpire, out int sExp)) sExp = 720; // min

    Console.WriteLine($"Redis Addr: {x.Configuration}");
    Console.WriteLine($"Absolute Expire: {absoluteExpire} min");
    Console.WriteLine($"Sliding Expire: {slidingExpire} min");
    BooCatController.InitialiseCachOption(TimeSpan.FromMinutes(aExp), TimeSpan.FromMinutes(sExp));
});

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Map("/api/{req}", async (string req, string name, HttpContext c, IDistributedCache? redis) =>
{
    if (redis == null) System.Console.WriteLine("Redis is null");
    if (!(req.ToLower() is "allbooks" or "all" or "a"))
        return await BooCatController.ProviderRequest(req, name, redis);

    string[]? providers = c.Request.Query["provider"];
    return await BooCatController.MultipleProviderRequest(providers, name, redis);
});

app.Run();
