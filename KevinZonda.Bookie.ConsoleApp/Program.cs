using KevinZonda.Bookie.Library;
using KevinZonda.Bookie.Library.Provider;

using System.Text;

PrintHello();

Console.OutputEncoding = Encoding.Unicode;
Console.InputEncoding = Encoding.Unicode;

var dic = new Dictionary<string, Provider>();
Provider p;
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
    switch (cmd[0])
    {
        case "z":
            if (!dic.ContainsKey("z"))
                dic["z"] = new ZLibrary();
            p = new ZLibrary();
            break;
        case "m":
            if (!dic.ContainsKey("m"))
                dic["m"] = new MemOfTheWorld();
            p = dic["m"];
            break;
        case "g":
            if (!dic.ContainsKey("g"))
                dic["g"] = new LibGen();
            p = dic["g"];
            break;
        case "o":
            if (!dic.ContainsKey("o"))
                dic["o"] = new OpenLibrary();
            p = dic["o"];
            break;
        default:
            Console.WriteLine("Error: Not valid provider!");
            continue;
    }

    var m = Extension.Try(() => p.SearchBook(cmd[1]).Result);
    if (!m.IsOk)
    {
        Error("Comand run error!\n" + m.Ex);
        continue;
    }

    foreach (var item in m.Value!)
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
    Console.WriteLine("Syntax ::= [lib] [BookName]  |");
    Console.WriteLine("           cls | exit | gc");
    Console.WriteLine("Where [lib] ::= z for Z-Lib  |");
    Console.WriteLine("                g for LibGen |");
    Console.WriteLine("                m for Memory of the World");
    Console.WriteLine("      [BookName] ::= Σ*");
    Console.WriteLine("===============================");
}