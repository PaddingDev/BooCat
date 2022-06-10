using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using KevinZonda.Bookie.Library;

namespace KevinZonda.Bookie.FunctionApp;

public static class MainFunction
{
    private static ProviderDic dic = new ProviderDic();

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
        var p = dic[v];
        if (p == null)
            return new BadRequestObjectResult("Not Valid Provider");
        return new OkObjectResult(await p.SearchBook(name));
    }
}
