using HtmlAgilityPack;

using KevinZonda.Bookie.Library.Models;

namespace KevinZonda.Bookie.Library.Provider;
public sealed class TheOnlineBooks : Provider
{
    protected override string _searchPrefix => "https://onlinebooks.library.upenn.edu/webbin/book/search?title=";

    protected override string _baseUrl => "https://onlinebooks.library.upenn.edu";

    protected override BookInfo[] ParseResponse(string response)
    {
        var html = new HtmlDocument();
        html.LoadHtml(response);
        var list = new List<BookInfo>();
        var root = html.DocumentNode;
        var n = root.SelectNodes("html/body/ul[@class=\"nodot\"]/li");
        if (n == null) return list.ToArray();

        foreach (var m in n)
        {
            var bookUrlNode = m.FirstChild;
            var bookUrl = bookUrlNode.Attributes["href"].Value;
            var nameNode = m.SelectSingleNode("cite");
            if (nameNode == null)
            {
                var innerNodes = m.SelectNodes("a");
                foreach (var inNo in innerNodes)
                {
                    nameNode = inNo.SelectSingleNode("cite");
                    if (nameNode != null)
                        break;
                }
            }
            string? name = nameNode == null ? null : nameNode.InnerText;
            string? author = null;

            foreach (var a in m.ChildNodes)
            {
                var ouT = a.OuterHtml.SafeTrim();
                var inT = a.InnerText.SafeTrim(); ;
                if (inT == null || ouT.Length == 0 ||
                    ouT == null || inT.Length == 0)
                    continue;
                if (inT == ouT)
                {
                    author = ParseAuthor(inT);
                }

            }
            list.Add(new BookInfo()
            {
                Url = bookUrl,
                Name = name,
                Authors = author.ToSingleArray(),
            });
        }
        return list.ToArray();
    }

    private string? ParseAuthor(string v)
    {
        var m = v.Split(new string[] { "by" }, StringSplitOptions.None);
        if (m.Length < 2)
            return null;
        var author = m[1];
        var n = author.Split('(');
        if (n.Length < 2)
            return author.Trim();
        else
            return n[0].Trim();
    }
}
