using KevinZonda.BooCat.Library.Provider;

namespace KevinZonda.BooCat.Library;

public enum ProviderType
{
    OpenLibrary,
    ZLibrary,
    LibraryGenesis,
    MemoryOfTheWorld,
    TheOnlineBooks
}
public class ProviderDic
{
    private Dictionary<ProviderType, Provider.Provider> _dic = new Dictionary<ProviderType, Provider.Provider>();

    public void LazyInit(ProviderType t)
    {
        if (_dic.ContainsKey(t)) return;

        _dic[t] = t switch
        {
            ProviderType.OpenLibrary => new OpenLibrary(),
            ProviderType.ZLibrary => new ZLibrary(),
            ProviderType.LibraryGenesis => new LibGen(),
            ProviderType.MemoryOfTheWorld => new MemOfTheWorld(),
            ProviderType.TheOnlineBooks => new TheOnlineBooks(),
            _ => throw new ArgumentException("Unknown ProviderType")
        };
    }


    public Provider.Provider this[ProviderType x]
    {
        get
        {
            LazyInit(x);
            return _dic[x];
        }
    }

    public Provider.Provider? this[string x]
    {
        get => x.ToLower() switch
        {
            "z" or "zlib" => this[ProviderType.ZLibrary],
            "g" or "libgen" => this[ProviderType.LibraryGenesis],
            "m" or "mem" => this[ProviderType.MemoryOfTheWorld],
            "o" or "openlib" => this[ProviderType.OpenLibrary],
            "b" or "onlinebooks" => this[ProviderType.TheOnlineBooks],
            _ => null
        };
    }

    public string? Regular(string s)
    {
        return s.ToLower() switch
        {
            "z" or "zlib" => "z",
            "g" or "libgen" => "g",
            "m" or "mem" => "m",
            "o" or "openlib" => "o",
            "b" or "onlinebooks" => "b",
            _ => null
        };
    }
}