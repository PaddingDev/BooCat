using KevinZonda.Bookie.Library.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KevinZonda.Bookie.Library.Provider;
public class LibGen : Provider
{
    protected override string _searchPrefix => "https://libgen.li/index.php?req=";
    protected override string _baseUrl => "https://libgen.io";

    protected override BookInfo[] ParseRespose(string response)
    {
        throw new NotImplementedException();
    }
}