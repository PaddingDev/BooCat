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
        this HtmlNode node,
        string attr)
    {
        if (node == null)
            return (false, null);
        var attrNode = node.Attributes;
        return attrNode.ContainsAttribute(attr);
    }

    public static (bool IsContains, string? Value) ContainsAttribute(
    this HtmlAttributeCollection collection,
    string attr)
    {
        if (collection == null)
            return (false, null);
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
            var p = n.ContainsAttribute(propertyName);

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

    public static (bool IsOk, T? Value, Exception? Ex) Try<T>(Func<T> f)
    {
        try
        {
            return (true, f(), null);
        }
        catch (Exception ex)
        {
            return (false, default(T), ex);
        }
    }

    public static void AddIfNotNull<T>(this List<T> l, T? item)
    {
        if (item != null)
            l.Add(item);
    }

    public static T IfNullThen<T>(this T t, T value)
    {
        return t == null ? value : t;
    }

    public static T[] ToSingleArray<T>(this T t)
    {
        if (t == null) return new T[] { };
        return new T[] { t };
    }

    public static string[] TrimSplit(this string s, char c)
    {
        if (string.IsNullOrEmpty(s)) return new string[] { };
        var n = s.Trim(c);
        if (string.IsNullOrEmpty(n)) return new string[] { };
        return n.Split(c);
    }

    public static T SafeIndex<T>(this T[] arr, int index, T ifOutOfBound = default(T))
    {
        if (arr == null) return ifOutOfBound;
        if (index < 0 || index >= arr.Length) return ifOutOfBound;
        return arr[index];
    }

    public static T IfNull<T>(this T t, Func<T> ifNull)
    {
        if (t == null)
            return ifNull();
        return t;
    }
}
