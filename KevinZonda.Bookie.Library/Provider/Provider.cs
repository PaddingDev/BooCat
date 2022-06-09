using KevinZonda.Bookie.Library.Models;

#pragma warning disable 8618

namespace KevinZonda.Bookie.Library.Provider;

public abstract class Provider
{
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

    protected abstract string ConstructSearchUrl(string searchText);

}
