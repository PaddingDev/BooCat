using HtmlAgilityPack;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KevinZonda.Bookie.Library;
public static class Extension
{
    public static (bool IsContains, string? Value) ContainsAttribute(
        this HtmlAttributeCollection collection,
        string attr)
    {
        foreach (var m in collection)
        {
            if (m.Name == attr)
            {
                return (true, m.Value);
            }
        }
        return (false, null);
    }

    public static void PrintAttribute(this HtmlAttributeCollection collection)
    {
        foreach (var a in collection)
        {
            Console.Write($"{a.Name}:{a.Value}");
        }
        Console.WriteLine();
    }

    public static void AddInnerHtml(this List<string> l, HtmlNodeCollection c)
    {
        foreach (var n in c)
        {
            l.Add(n.InnerHtml);
        }
    }

    public static HtmlNode? Find(this HtmlNodeCollection c, string propertyName, string contains)
    {
        bool isNull = string.IsNullOrEmpty(contains);
        foreach (var n in c)
        {
            var p = n.Attributes.ContainsAttribute(propertyName);

            // Contains tis property?
            if (!p.IsContains)
                continue;
            // Can property value be null?
            if (isNull)
                return n;
            // null?
            if (p.Value == null)
                continue;
            if (p.Value.Contains(contains))
                return n;

        }
        return null;
    }

    public static (bool IsOk, T? Value) Try<T>(Func<T> f)
    {
        try
        {
            return (true, f());
        }
        catch (Exception)
        {
            return (false, default(T));
        }
    }
}
