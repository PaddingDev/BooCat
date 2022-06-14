using KevinZonda.Bookie.FunctionApp.Model;
using KevinZonda.Bookie.Library.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KevinZonda.Bookie.FunctionApp;
public static partial class MainFunction
{
    [FunctionName("AllBooks")]
    public static async Task<IActionResult> RunAll(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
    ILogger log)
    {
        string name = req.Query["name"];
        string[] providers = req.Query["provider"];
        if (string.IsNullOrWhiteSpace(name))
            return new BadRequestObjectResult((ErrModel)"Not Valid Name");

        if (providers == null || providers.Length == 0)
            return new BadRequestObjectResult((ErrModel)"Not Valid Provider");

        var _dic = new Dictionary<string, Task<(BookInfo[] Infos, Exception? Err)>>();
        foreach (var provider in providers)
        {
            var p = dic[provider];
            if (p == null) continue;
            _dic.Add(provider, p.SearchBook(name));
        }
        await Task.Factory.StartNew(() => Task.WaitAll(_dic.Values.ToArray(), 10000));

        var _resultDic = new Dictionary<string, object>();
        var _errDic = new Dictionary<string, ErrModel>();
        foreach (var kvp in _dic)
        {
            var rst = kvp.Value;
            if (!rst.IsCompleted)
            {
                _errDic.Add(kvp.Key, (ErrModel)"Not completed.");
                continue;
            }
            var value = rst.Result;
            if (value.Err != null) _errDic.Add(kvp.Key, (ErrModel)value.Err);
            else _resultDic.Add(kvp.Key, value.Infos);
        }
        _resultDic["err"] = _errDic;
        return new OkObjectResult(_resultDic);
    }
}