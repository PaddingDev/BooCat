using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using KevinZonda.Bookie.Library;
using System;

namespace KevinZonda.Bookie.FunctionApp;

public static partial class MainFunction
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

    [FunctionName("OnlineBooks")]
    public static async Task<IActionResult> RunOneLineBooks(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
        ILogger log)
    {

        string name = req.Query["name"];
        return await ProviderRequest("b", name);
    }

    private static async Task<IActionResult> ProviderRequest(string v, string name)
    {
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

    private class ErrModel
    {
        public string Msg { get; set; }
        public string? Source { get; set; }

        public ErrModel(string msg, string? source = null)
        {
            Msg = msg;
            Source = source;
        }

        public static explicit operator ErrModel(Exception v)
        {
            return new ErrModel(v.Message, v.Source);
        }

        public static explicit operator ErrModel(string v)
        {
            return new ErrModel(v);
        }
    }
}
