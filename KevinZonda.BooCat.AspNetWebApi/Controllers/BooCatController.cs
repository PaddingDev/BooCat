using KevinZonda.BooCat.Library;
using KevinZonda.BooCat.Library.Models.WebAPI;

namespace KevinZonda.BooCat.AspNetCoreWebAPI.Controllers;

public static class BooCatController
{
    private static ProviderDic dic = new ProviderDic();

    public static async Task<IResult> ProviderRequest(string v, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Results.BadRequest((ErrModel)"Not Valid Name");
        var p = dic[v];
        if (p == null)
            return Results.BadRequest((ErrModel)"Not Valid Provider");
        var r = await p.SearchBook(name);

        if (r.Err != null)
            return Results.BadRequest((ErrModel)r.Err);
        return Results.Ok(r.Infos);
        
    }
}
