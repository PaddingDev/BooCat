using KevinZonda.Bookie.Library.Models;

namespace KevinZonda.Bookie.Library.Provider;
public abstract class Provider
{
    protected HttpClient _httpClient = Factory.HttpClientFatory.Produce();
    protected async Task<string> Get(string url)
    {
        var response = await _httpClient.GetAsync(url);
        return await response.Content.ReadAsStringAsync();
    }

    public abstract Task<BookInfo[]> GetBook(string searchText);

}
