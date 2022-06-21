using HtmlAgilityPack;

using KevinZonda.BooCat.Library.Models;

using System.Text;

namespace KevinZonda.BooCat.Library.Provider;
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
        book.Authors = ParseAuthors(ns[1].InnerText);

        // 2 -> Publisher
        var publish = ns[2].IfNullElse(null, x => x.InnerText);
        if (!string.IsNullOrWhiteSpace(publish))
        {
            book.Publishers = publish.TrimSplitTrim(';');
        }

        // 3 -> Year
        book.Date = ns[3].IfNullElse(null, x => x.InnerText);

        // 4 -> Lang
        book.Language = ns[4].IfNullElse(null, x =>
                                  x.InnerText.IfNotNull(
                                      y => y.TrimSplit(';')
                                            .SafeIndex(0, null)
                                      )
                                  );

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

    private static string[]? ParseAuthors(string? s)
    {
        if (s == null) return null;
        var n = s.Trim();
        if (string.IsNullOrEmpty(n)) return null;
        n = n.Replace("[...]", "");
        var sep = n.Contains(';') ? ';' : ',';
        var list = new List<string>();
        string tmp;
        var sb = new StringBuilder();
        int bracketLvl = 0;
        foreach (var c in n)
        {
            if (c == sep)
            {
                if (bracketLvl == 0)
                {
                    tmp = sb.ToString().Trim();
                    if (!string.IsNullOrEmpty(tmp)) list.Add(tmp);
                    sb.Clear();
                    continue;
                }
            }
            if (c == '(') ++bracketLvl;
            if (c == ')') --bracketLvl;
            sb.Append(c);
        }
        tmp = sb.ToString().Trim();
        if (!string.IsNullOrEmpty(tmp)) list.Add(tmp);
        return list.ToArray();
    }

    private static (string Name, string? Uri)? ParseBasicInfo(HtmlNode n)
    {
        var bNode = n.SelectSingleNode("b");
        var aNode = n.SelectSingleNode("a");
        var titleNode = bNode.IfNull(aNode);

        if (titleNode == null) return null;
        var name = titleNode.InnerText.SafeTrim();

        var urlNode = bNode.IfNullElse(
            aNode,
            x => x.SelectSingleNode("a"));

        string? url = null;
        if (urlNode != null)
        {
            var (IsContains, Value) = urlNode.ContainsAttribute("href");
            if (IsContains) url = Value;
        }
        return (name, url);
    }
}