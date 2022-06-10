using HtmlAgilityPack;

using KevinZonda.Bookie.Library.Models;

namespace KevinZonda.Bookie.Library.Provider;
public class LibGen : Provider
{
    protected override string _searchPrefix => "https://libgen.li/index.php?req=";
    protected override string _baseUrl => "https://libgen.li";
    public override int MinLength => 3;

    protected override BookInfo[] ParseRespose(string response)
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
        // 0 -> Basic
        var basicInfo = ParseBasicInfo(ns[0]);
        // 1 -> Authors
        var author = ns[1].InnerText.IfNullThen("").TrimSplit(';');

        // 2 -> Publisher
        var publish = ns[2].InnerText;
        // 3 -> Year
        var year = ns[3].InnerText;
        // 4 -> Lang
        var lang = ns[4].InnerText.TrimSplit(';').SafeIndex(0, null);
        // 5 -> Pages
        // Ignore

        // 6 -> FileSize
        var size = ns[6].InnerText;
        // 7 -> FileType
        var type = ns[7].InnerText;
        // 8 -> Mirror
        // Ignore
        return new BookInfo
        {
            ID = basicInfo?.Uri.Split('=').SafeIndex(1),
            Name = basicInfo?.Name,
            Url = Uri2Url(basicInfo?.Uri),
            Publishers = publish.ToSingleArray(),
            Authors = author,
            Date = year,
            Language = lang,
            FileSize = size,
            FileType = type
        };
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