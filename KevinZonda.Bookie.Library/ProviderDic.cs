using KevinZonda.Bookie.Library.Provider;

namespace KevinZonda.Bookie.Library;

public enum ProviderType
{
    OpenLibrary,
    ZLibrary,
    LibraryGenesis,
    MemoryOfTheWorld
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
            _ => throw new ArgumentException("Unknown ProviderType")
        };
    }
}