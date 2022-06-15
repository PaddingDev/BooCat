using KevinZonda.BooCat.Library.Models;
using KevinZonda.BooCat.Library.Models.MemOfTheWorld;

using System.Text.Json;

namespace KevinZonda.BooCat.Library.Provider;
public sealed class MemOfTheWorld : Provider
{
    protected override string _searchPrefix => "https://books.memoryoftheworld.org/search/titles/";

    protected override string _baseUrl => throw new NotImplementedException();

    protected override BookInfo[] ParseResponse(string response)
    {
        var result = JsonSerializer.Deserialize<ResultModel>(response);
        var list = new List<BookInfo>();
        if (result == null) return list.ToArray();
        foreach (var item in result.Items)
        {
            var book = new BookInfo
            {
                ID = item.ID,
                Name = item.Title,
                Authors = item.Authors.ToArray(),
                Date = item.PubDate.Year.ToString(),
                Url = Id2Url(item.ID),
            };
            if (item.Formats != null && item.Formats.Count > 0)
            {
                book.FileType = item.Formats[0].Format;
                book.FileSize = string.Format("{0:0.00} MB", 1.0 * item.Formats[0].Size / 1024 / 1024);
                if (item.Formats.Count > 1)
                    book.FileType += "+";
            }

            list.Add(book);
        }
        return list.ToArray();
    }

    private static string Id2Url(string id)
    {
        return "https://library.memoryoftheworld.org/#/book/" + id;
    }
}
