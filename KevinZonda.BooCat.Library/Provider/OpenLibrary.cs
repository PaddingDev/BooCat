using KevinZonda.BooCat.Library.Models;
using KevinZonda.BooCat.Library.Models.OpenLibrary;

using System.Text.Json;

namespace KevinZonda.BooCat.Library.Provider;

public class OpenLibrary : Provider
{
    protected override string _searchPrefix => "https://openlibrary.org/search.json?title=";

    protected override string _baseUrl => "https://openlibrary.org";

    protected override BookInfo[] ParseResponse(string response)
    {
        var result = JsonSerializer.Deserialize<ResultModel>(response);
        var list = new List<BookInfo>();
        if (result == null || result.Docs == null)
            return list.ToArray();
        foreach (var item in result.Docs)
        {
            if (item == null) continue;
            var b = new BookInfo()
            {
                Name = item.Title,
                Authors = item.Authors.SafeToArray(),
                Date = item.Year.ToString(),
                Publishers = item.Publishers.SafeToArray(),
                Url = Uri2Url(item.Uri),
                Language = item.Languages.SafeIndex(0),
            };
            list.Add(b);
        }
        return list.ToArray();
    }
}
