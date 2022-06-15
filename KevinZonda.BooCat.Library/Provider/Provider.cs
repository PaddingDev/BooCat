using KevinZonda.BooCat.Library.Models;

using System.Web;

#pragma warning disable 8618

namespace KevinZonda.BooCat.Library.Provider;

public abstract class Provider
{
    protected abstract string _searchPrefix { get; }
    protected abstract string _baseUrl { get; }
    public virtual int MinLength => 1;

    protected HttpClient _httpClient;

    public Provider()
    {
        Init();
    }

    protected virtual void Init()
    {
        _httpClient = Factory.HttpClientFatory.Produce();
    }

    protected async Task<(string Html, Exception? Err)> HttpGet(string url)
    {
        try
        {
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                return (await response.Content.ReadAsStringAsync(), null);
            }
        }
        catch (Exception ex)
        {
            return ("", ex);
        }
        return ("", new Exception("Upstream API response is not 200"));
    }


    public virtual async Task<(BookInfo[] Infos, Exception? Err)> SearchBook(string searchText)
    {
        if (MinLength > 0 && searchText.Length < MinLength)
            return (Array.Empty<BookInfo>(), new Exception("Search text is too short"));
        var html = await HttpGet(ConstructSearchUrl(searchText));
        if (html.Err != null)
            return (Array.Empty<BookInfo>(), html.Err);
        try
        {
            return (ParseResponse(html.Html), null);
        }
        catch (Exception ex)
        {
            return (Array.Empty<BookInfo>(), ex);
        }
    }

    protected abstract BookInfo[] ParseResponse(string response);

    protected virtual string ConstructSearchUrl(string searchText)
    {
        return _searchPrefix + HttpUtility.UrlEncode(searchText);
    }

    protected virtual string? Uri2Url(string? uri)
    {
        if (uri == null) return null;
        if (uri.StartsWith("/")) return _baseUrl + uri;
        else return _baseUrl + "/" + uri;
    }

}
