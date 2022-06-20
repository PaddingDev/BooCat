using KevinZonda.BooCat.Library;
using KevinZonda.BooCat.Library.Models;
using KevinZonda.BooCat.Library.Models.WebAPI;
using KevinZonda.BooCat.Library.Provider;

using Microsoft.Extensions.Caching.Distributed;

using System.Text.Json;

namespace KevinZonda.BooCat.AspNetCoreWebAPI.Controllers;

public static class BooCatController
{
    private static readonly ProviderDic dic = new ProviderDic();
    private static readonly JsonSerializerOptions JsonDeserialiseDefault = new()
    {
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private static string GetCachedKey(string provider, string name)
    {
        return $"{provider}_{name}";
    }

    public static async Task<BookInfo[]?> GetCached(IDistributedCache? cache, string provider, string name)
    {
        if (cache == null || cache == null) return null;
        try
        {
            var result = await cache.GetStringAsync(GetCachedKey(provider, name));
            if (string.IsNullOrEmpty(result)) return null;
            return JsonSerializer.Deserialize<BookInfo[]>(result, JsonDeserialiseDefault);
        }
        catch
        {
            return null;
        }
    }

    public static async Task<(BookInfo[]? Infos, ErrModel? Err)> GetSearchedBook(string provider, string name, IDistributedCache? cache = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return (null, (ErrModel)"Not Valid Name");
        var p = dic[provider];

        if (p == null)
            return (null, (ErrModel)"Not Valid Provider");

        var cached = await GetCached(cache, provider, name);
        if (cached != null) return (cached, null);

        var (Infos, Err) = await p.SearchBook(name);
        return (Infos, Err == null ? null : (ErrModel)Err);
    }

    public static async Task<IResult> ProviderRequest(string provider, string name)
    {
        var (Infos, Err) = await GetSearchedBook(provider, name);

        if (Err != null)
            return Results.BadRequest(Err);
        return Results.Ok(Infos);
    }

    public static async Task<IResult> MultipleProviderRequest(string[]? providers, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Results.BadRequest((ErrModel)"Not Valid Name");

        if (providers == null || providers.Length == 0)
            return Results.BadRequest((ErrModel)"Not Valid Provider");

        var _dic = new Dictionary<string, Task<(BookInfo[]? Infos, ErrModel? Err)>>();
        foreach (var provider in providers)
        {
            var p = dic[provider];
            if (p == null) continue;
            _dic.Add(provider, GetSearchedBook(provider, name));
        }
        await Task.Factory.StartNew(() => Task.WaitAll(_dic.Values.ToArray(), 10000));

        var _resultDic = new Dictionary<string, ResultModel>();
        foreach (var kvp in _dic)
        {
            var rst = kvp.Value;
            if (!rst.IsCompleted)
            {
                _resultDic.Add(kvp.Key, (ResultModel)(ErrModel)"Not completed.");
                continue;
            }
            var (Infos, Err) = rst.Result;
            if (Err != null) _resultDic.Add(kvp.Key, (ResultModel)Err);
            else if (Infos != null) _resultDic.Add(kvp.Key, (ResultModel)Infos);
            else continue;
        }
        return Results.Ok(_resultDic);
    }
}
