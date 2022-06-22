using HtmlAgilityPack;

using KevinZonda.BooCat.Library.Models;

namespace KevinZonda.BooCat.Library.Provider;

public sealed class ZLibrary : Provider
{
    protected override string _searchPrefix => _prefix;
    protected override string _baseUrl => _base;
    public override int MinLength => 2;

    private string _prefix = "https://1lib.in/s/?q=";
    private string _base = "https://1lib.in";

    private bool _updLock = false;
    public async Task<bool> UpdateUrlAsync()
    {
        if (_updLock) return false;
        _updLock = true;
        var (html, err) = await this.HttpGet("https://z-lib.org/");
        if (err != null) return false;
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        //var n = doc.DocumentNode.SelectSingleNode("/html/body/table/tbody/tr/td/div/div/div/div");
        var n = doc.DocumentNode.SelectSingleNode("//a[contains(., 'Books')]");
        if (n == null) return false;
        var (_, value) = n.ContainsAttribute("href");
        if (value == null) return false;
        var uri = new UriBuilder(value);
        uri.Scheme = Uri.UriSchemeHttps;
        uri.Port = -1;
        _base = uri.ToString().Trim('/');
        _prefix = _base + "/s/?q=";
        _updLock = false;
        return true;
    }

    protected override void Init()
    {
        base.Init();

        try
        {
            if (!UpdateUrlAsync().Result)
                Console.WriteLine("Cannot update zlib's url");
            else
            {
                Console.WriteLine("[ZLib] url update successfully!");
                Console.WriteLine(_base);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Cannot update zlib's url");
            Console.WriteLine(ex);
        }
    }

    protected override BookInfo[] ParseResponse(string response)
    {
        if (response.Contains("Z-Library single sign on"))
        {
            var _ = UpdateUrlAsync();
            throw new Exception("ZLib url is out of date, an update flow has been trigged, please try again");
        }

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

        var authorList = author.ToArray();
        if (authorList.Length == 1)
        {
            var authTxt = authorList[0];
            var auths = authTxt.SplitTrim(new[] { "&amp;",  ";", "&" });
            if (auths.Length > 1)
                authorList = auths;

        }
        return (bookName, publisher.ToArray(), authorList, link)!;
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
