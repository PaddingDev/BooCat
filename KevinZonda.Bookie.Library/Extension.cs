using HtmlAgilityPack;

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

    public static void PrintAttribute(this HtmlNode node)
    {
        if (node == null) return;
        var collection = node.Attributes;
        if (collection == null) return;
        foreach (var a in collection)
        {
            Console.Write($"{a.Name}:{a.Value}; ");
        }
        Console.WriteLine();
    }

    public static void AddInnerHtml(this List<string> l, HtmlNodeCollection c)
    {
        if (c == null) return;
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
    public static T? OnlyTry<T>(Func<T> f)
    {
        try
        {
            return f();
        }
        catch (Exception)
        {
            return default;
        }
    }


    public static void AddIfNotNull<T>(this List<T> l, T? item)
    {
        if (item != null)
            l.Add(item);
    }

    public static T[] ToSingleArray<T>(this T? t)
    {
        if (t == null) return Array.Empty<T>();
        return new T[] { t };
    }

    public static string[] TrimSplit(this string s, char c)
    {
        if (string.IsNullOrEmpty(s)) return Array.Empty<string>();
        var n = s.Trim(c);
        if (string.IsNullOrEmpty(n)) return Array.Empty<string>();
        return n.Split(c);
    }

    public static string[] SafeSplit(this string? s, char separator)
    {
        if (s == null) return new string[] { };
        return s.Split(separator);
    }

    public static T? SafeIndex<T>(this T[] arr, int index, T? ifOutOfBound = default)
    {
        if (arr == null) return ifOutOfBound;
        if (index < 0 || index >= arr.Length) return ifOutOfBound;
        return arr[index];
    }

    public static T? SafeIndex<T>(this List<T> arr, int index, T? ifOutOfBound = default)
    {
        if (arr == null) return ifOutOfBound;
        if (index < 0 || index >= arr.Count) return ifOutOfBound;
        return arr[index];
    }

    public static string SafeTrim(this string s)
    {
        if (string.IsNullOrEmpty(s)) return s;
        return s.Trim();
    }

    public static T[] SafeToArray<T>(this List<T> l)
    {
        if (l == null) return new T[] { };
        return l.ToArray();
    }

    public static T IfNullThen<T>(this T t, Func<T> ifNull)
    {
        if (t == null)
            return ifNull();
        return t;
    }
    public static T IfNullThen<T>(this T t, T ifNull)
    {
        return t == null ? ifNull : t;
    }


    public static R IfNullElse<R, T>(this T t, R ifNull, Func<T, R> ifNotNul)
    {
        return t == null ? ifNull : ifNotNul(t);
    }
}
