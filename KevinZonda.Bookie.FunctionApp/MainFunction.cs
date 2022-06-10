using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using KevinZonda.Bookie.Library.Provider;
using System.Collections.Generic;

namespace KevinZonda.Bookie.FunctionApp;

public static class MainFunction
{
    private static Dictionary<string, Provider> dic = new Dictionary<string, Provider>();

    private static Provider Factory(string v)
    {
        switch (v)
        {
            case "z":
                if (!dic.ContainsKey("z"))
                    dic["z"] = new ZLibrary();
                return dic["z"];
            case "m":
                if (!dic.ContainsKey("m"))
                    dic["m"] = new MemOfTheWorld();
                return dic["m"];
            case "g":
                if (!dic.ContainsKey("g"))
                    dic["g"] = new LibGen();
                return dic["g"];
        }
        return null;
    }
    [FunctionName("ZLib")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
        ILogger log)
    {
        string name = req.Query["name"];
        return await ProviderRequest("z", name);
    }

    [FunctionName("Mem")]
    public static async Task<IActionResult> RunMem(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
        ILogger log)
    {

        string name = req.Query["name"];
        return await ProviderRequest("m", name);
    }

    [FunctionName("LibGen")]
    public static async Task<IActionResult> RunLibGen(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
        ILogger log)
    {

        string name = req.Query["name"];
        return await ProviderRequest("g", name);
    }

    private static async Task<IActionResult> ProviderRequest(string v, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return new BadRequestObjectResult("Not Valid Name");
        var p = Factory(v);
        if (p == null)
            return new BadRequestObjectResult("Not Valid Provider");
        return new OkObjectResult(await p.SearchBook(name));
    }
}
