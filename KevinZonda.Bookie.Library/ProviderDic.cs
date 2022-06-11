using KevinZonda.Bookie.Library.Provider;

namespace KevinZonda.Bookie.Library;

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

    public Provider.Provider this[string x]
    {
        get => x switch
        {
            "z" => this[ProviderType.ZLibrary],
            "g" => this[ProviderType.LibraryGenesis],
            "m" => this[ProviderType.MemoryOfTheWorld],
            "o" => this[ProviderType.OpenLibrary],
            "b" => this[ProviderType.TheOnlineBooks],
            _ => throw new ArgumentException("Unknown ProviderType")
        };
    }
}