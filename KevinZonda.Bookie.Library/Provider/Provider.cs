using KevinZonda.Bookie.Library.Models;

using System.Web;

#pragma warning disable 8618

namespace KevinZonda.Bookie.Library.Provider;

public abstract class Provider
{
    protected abstract string _searchPrefix { get; }
    protected abstract string _baseUrl { get; }

    protected HttpClient _httpClient;

    public Provider()
    {
        Init();
    }

    public virtual void Init()
    {
        _httpClient = Factory.HttpClientFatory.Produce();
    }

    protected async Task<string> HttpGet(string url)
    {
        var response = await _httpClient.GetAsync(url);
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<BookInfo[]> SearchBook(string searchText)
    {
        var html = await HttpGet(ConstructSearchUrl(searchText));
        return ParseRespose(html);
    }

    protected abstract BookInfo[] ParseRespose(string response);

    protected virtual string ConstructSearchUrl(string searchText)
    {
        return _searchPrefix + HttpUtility.UrlEncode(searchText);
    }

    protected virtual string? Uri2Url(string uri)
    {
        if (uri == null) return null;
        if (uri.StartsWith("/")) return _baseUrl + uri;
        else return _baseUrl + "/" + uri;
    }

}
