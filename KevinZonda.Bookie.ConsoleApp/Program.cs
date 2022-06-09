using HtmlAgilityPack;

using KevinZonda.Bookie.Library;
using KevinZonda.Bookie.Library.Provider;

var hc = new HttpClient();
var resp = hc.GetAsync("https://b-ok.cc/s/hello?").Result;

var html = resp.Content.ReadAsStringAsync().Result;
var z = new Zibrary();
var m = z.ParseRespose(html);
foreach (var item in m)
{
    Console.WriteLine(item);
}
