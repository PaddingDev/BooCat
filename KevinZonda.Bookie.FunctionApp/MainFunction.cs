using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using KevinZonda.Bookie.Library;
using KevinZonda.Bookie.FunctionApp.Model;

namespace KevinZonda.Bookie.FunctionApp;

public static partial class MainFunction
{
    private static ProviderDic dic = new ProviderDic();

    [FunctionName("ZLib")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
        ILogger log)
    {
        return await ProviderRequest("z", req);
    }

    [FunctionName("Mem")]
    public static async Task<IActionResult> RunMem(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
        ILogger log)
    {
        return await ProviderRequest("m", req);
    }

    [FunctionName("LibGen")]
    public static async Task<IActionResult> RunLibGen(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
        ILogger log)
    {
        return await ProviderRequest("g", req);
    }

    [FunctionName("OnlineBooks")]
    public static async Task<IActionResult> RunOneLineBooks(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
        ILogger log)
    {
        return await ProviderRequest("b", req);
    }

    [FunctionName("OpenLib")]
    public static async Task<IActionResult> RunOneLib(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
    ILogger log)
    {
        return await ProviderRequest("o", req);
    }

    private static async Task<IActionResult> ProviderRequest(string v, HttpRequest req)
    {
        if (!req.Query.ContainsKey("name"))
            return new BadRequestObjectResult((ErrModel)"Not Valid Parameter");

        string name = req.Query["name"];


        if (string.IsNullOrWhiteSpace(name))
            return new BadRequestObjectResult((ErrModel)"Not Valid Name");
        var p = dic[v];
        if (p == null)
            return new BadRequestObjectResult((ErrModel)"Not Valid Provider");
        var r = await p.SearchBook(name);
        if (r.Err != null)
            return new BadRequestObjectResult((ErrModel)r.Err);
        return new OkObjectResult(r.Infos);
    }
}
