using HtmlAgilityPack;

using KevinZonda.Bookie.Library.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KevinZonda.Bookie.Library.Provider;
public class LibGen : Provider
{
    protected override string _searchPrefix => "https://libgen.li/index.php?req=";
    protected override string _baseUrl => "https://libgen.li";

    protected override BookInfo[] ParseRespose(string response)
    {
        var list = new List<BookInfo>();
        var html = new HtmlDocument();
        html.LoadHtml(response);
        var node = html.GetElementbyId("tablelibgen");
        node = node.SelectSingleNode("tbody");
        var bookNodes = node.SelectNodes("tr");
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
        var basicNode = ns[0];
        // 1 -> Authors
        var author = ns[1].InnerText.IfNullThen("").TrimSplit(';');

        // 2 -> Publisher
        var publish = ns[2].InnerText;
        // 3 -> Year
        var year = ns[3].InnerText;
        // 4 -> Lang
        var lang = ns[4].InnerText;
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
            Publishers = publish.ToSingleArray(),
            Authors = author,
            Date = year,
            Language = lang,
            FileSize = size,
            FileType = type
        };
    }
}