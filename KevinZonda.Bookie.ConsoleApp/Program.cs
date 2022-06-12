using KevinZonda.Bookie.Library;

using System.Text;

PrintHello();

Console.OutputEncoding = Encoding.Unicode;
Console.InputEncoding = Encoding.Unicode;

var dic = new ProviderDic();

while (true)
{
    Console.Write(">");
    var input = Console.ReadLine()!.Trim();
    var cmd = input.Split(' ', 2);
    if (cmd.Length == 1)
    {
        switch (cmd[0])
        {
            case "exit":
                return;
            case "cls":
                Console.Clear();
                PrintHello();
                continue;
            case "gc":
                GC.Collect();
                continue;
            default:
                Error("Not valid cmd!");
                continue;
        }
    }
    var provider = dic[cmd[0]];

    if (provider == null)
    {
        Error("No valid provider!");
        continue;
    }

    var m = provider.SearchBook(cmd[1]).Result;
    if (m.Err != null)
    {
        Error("Command run error!\n" + m.Err);
        continue;
    }

    foreach (var item in m.Infos!)
    {
        Console.WriteLine(item);
    }
}

void Error(string msg)
{
    Console.WriteLine("Error: " + msg);
}

void PrintHello()
{
    Console.WriteLine("===============================");
    Console.WriteLine("Welcome to Bookie by KevinZonda");
    Console.WriteLine("Powered by .NET");
    Console.WriteLine("===============================");
    Console.WriteLine("Syntax ::= [lib] [BookName]   |");
    Console.WriteLine("           cls | exit | gc");
    Console.WriteLine("Where [lib] ::= z for Z-Lib   |");
    Console.WriteLine("                g for LibGen  |");
    Console.WriteLine("                o for OpenLib |");
    Console.WriteLine("                b for The Online Books |");
    Console.WriteLine("                m for Memory of the World");
    Console.WriteLine("      [BookName] ::= Σ*");
    Console.WriteLine("===============================");
}