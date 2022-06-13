using HtmlAgilityPack;

using KevinZonda.Bookie.Library.Models;

namespace KevinZonda.Bookie.Library.Provider;
public sealed class LibGen : Provider
{
    protected override string _searchPrefix => "https://libgen.li/index.php?req=";
    protected override string _baseUrl => "https://libgen.li";
    public override int MinLength => 3;

    protected override BookInfo[] ParseResponse(string response)
    {
        var list = new List<BookInfo>();
        var html = new HtmlDocument();
        html.LoadHtml(response);

        var node = html.GetElementbyId("tablelibgen");
        if (node == null) return list.ToArray();

        var bookNodes = node.SelectNodes("tbody/tr");
        if (bookNodes == null) return list.ToArray();

        foreach (var bookNode in bookNodes)
        {
            list.AddIfNotNull(ParseBookNode(bookNode));
        }

        //File.WriteAllText("c.html", node.OuterHtml);
        return list.ToArray();
    }

    private BookInfo? ParseBookNode(HtmlNode node)
    {
        var ns = node.SelectNodes("td");
        if (ns.Count < 8) return null;
        var book = new BookInfo();
        // 0 -> Basic
        var basicInfo = ParseBasicInfo(ns[0]);
        if (basicInfo != null)
        {
            book.ID = basicInfo.Value.Uri.SafeSplit('=').SafeIndex(1);
            book.Name = basicInfo.Value.Name;
            book.Url = Uri2Url(basicInfo?.Uri);
        }
        // 1 -> Authors
        var author = ns[1].InnerText.IfNull("").TrimSplit(';');
        book.Authors = author;

        // 2 -> Publisher
        var publish = ns[2].IfNullElse(null, x => x.InnerText);
        book.Publishers = publish.ToSingleArray();
        // 3 -> Year
        var year = ns[3].IfNullElse(null, x => x.InnerText);
        book.Date = year;
        // 4 -> Lang
        book.Language = ns[4].IfNullElse(null, x => x.InnerText.TrimSplit(';').SafeIndex(0, null));
        // 5 -> Pages
        // Ignore

        // 6 -> FileSize
        book.FileSize = ns[6].IfNullElse(null, x => x.InnerText);
        // 7 -> FileType
        book.FileType = ns[7].IfNullElse(null, x => x.InnerText);
        // 8 -> Mirror
        // Ignore
        return book;
    }

    private (string Name, string? Uri)? ParseBasicInfo(HtmlNode n)
    {
        if (n == null) return null;
        var x = n.SelectSingleNode("b/a")
                         .IfNull(() => n.SelectSingleNode("a"));
        if (x == null) return null;

        var linkAttr = x.ContainsAttribute("href");
        return (x.InnerText.SafeTrim(),
            linkAttr.IsContains
            ? linkAttr.Value
            : null);
    }
}