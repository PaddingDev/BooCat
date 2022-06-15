using HtmlAgilityPack;

using KevinZonda.BooCat.Library.Models;

namespace KevinZonda.BooCat.Library.Provider;

public sealed class ZLibrary : Provider
{
    protected override string _searchPrefix => "https://b-ok.cc/s/?q=";
    protected override string _baseUrl => "https://b-ok.cc";
    public override int MinLength => 2;

    protected override BookInfo[] ParseResponse(string response)
    {
        var list = new List<BookInfo>();

        var doc = new HtmlDocument();
        doc.LoadHtml(response);
        var rootNode = doc.GetElementbyId("searchResultBox");
        if (rootNode == null) return list.ToArray();

        if (rootNode.HasChildNodes)
        {
            var nodes = rootNode.ChildNodes;
            foreach (var node in nodes)
            {
                var rst = Parse(node);
                list.AddIfNotNull(rst);
            }
        }

        return list.ToArray();
    }
    BookInfo? Parse(HtmlNode node)
    {
        var bookId = node.ContainsAttribute("data-book_id");
        if (!bookId.IsContains)
            return null;
        var hNode = node.SelectNodes("div/table/tr/td");
        if (hNode == null) return null;

        var last = hNode.Last().SelectNodes("table/tr");
        if (last == null) return null;

        var book = new BookInfo()
        {
            ID = bookId.Value,
        };

        var count = last.Count;

        if (count > 0)
        {
            var result = Extension.Try(() => ParseBasic(last[0]));
            if (result.IsOk)
            {
                var basicInfo = result.Value;
                book.Name = basicInfo.BookName;
                book.Authors = basicInfo.Authors;
                book.Publishers = basicInfo.Publishers;
                book.Url = Uri2Url(basicInfo.Uri);
            }
        }

        if (count > 1)
        {
            var result = Extension.Try(() => ParseDetail(last[1]));
            if (result.IsOk)
            {
                var detailedInfo = result.Value;
                book.FileType = detailedInfo.FileType;
                book.FileSize = detailedInfo.FileSize;
                book.Date = detailedInfo.Date;
                book.Language = detailedInfo.Language;
            }
        }
        return book;
    }

    (string BookName, string[] Publishers, string[] Authors, string Uri) ParseBasic(HtmlNode node)
    {
        var bookNameHtml = node.SelectSingleNode("td/h3/a");
        var bookName = bookNameHtml.InnerText;

        var linkAttr = bookNameHtml.ContainsAttribute("href");
        var link = linkAttr.IsContains ? linkAttr.Value : "FAILED";
        var divs = node.SelectNodes("td/div");
        var publisher = new List<string>();
        var author = new List<string>();
        foreach (var div in divs)
        {
            var (IsContains, Value) = div.ContainsAttribute("class");
            if (IsContains && Value!.ToLower() == "authors")
            {
                var authors = div.SelectNodes("a");
                author.AddInnerText(authors);
            }
            else
            {
                var publishers = div.SelectNodes("a");
                publisher.AddInnerText(publishers);
            }

        }
        return (bookName, publisher.ToArray(), author.ToArray(), link)!;
    }

    (string Date, string Language, string FileType, string FileSize) ParseDetail(HtmlNode node)
    {
        string? date = null;
        string? lang = null;
        string? ftype = null;
        string? fsize = null;
        var detailedBox = node.SelectSingleNode("td/div[@class='bookDetailsBox']");
        foreach (var child in detailedBox.ChildNodes)
        {
            var classAttr = child.ContainsAttribute("class");
            if (!classAttr.IsContains)
                continue;
            var classValue = classAttr.Value!;
            if (!classValue.StartsWith("bookProperty "))
                continue;
            var propertyType = classValue.Substring("bookProperty ".Length);
            var valueNode = child.ChildNodes.Find("class", "property_value");
            string? value = valueNode.IfNullElse(null, x => x!.InnerHtml);

            switch (propertyType)
            {
                case "property_year":
                    date = value;
                    break;
                case "property_language":
                    lang = value;
                    break;
                case "property__file":
                    if (value == null) continue;
                    var x = value.Split(',');
                    if (x.Length < 1) continue;
                    ftype = x[0].SafeTrim();
                    if (x.Length < 2) continue;
                    fsize = x[1].SafeTrim();
                    break;
            }
        }
        return (date, lang, ftype, fsize)!;

    }

}
