using HtmlAgilityPack;

using KevinZonda.Bookie.Library;
using KevinZonda.Bookie.Library.Provider;

using System.Text;

PrintHello();

Provider? p = null;

Console.OutputEncoding = Encoding.Unicode;
Console.InputEncoding = Encoding.Unicode;

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
            default:
                Error("Not valid cmd!");
                continue;
        }
    }
    switch (cmd[0])
    {
        case "z":
            p = new ZLibrary();
            break;
        case "m":
            p = new MemOfTheWorld();
            break;
        case "g":
            p = new LibGen();
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
    Console.WriteLine("           cls | exit");
    Console.WriteLine("Where [lib] ::= z for Z-Lib  |");
    Console.WriteLine("                g for LibGen |");
    Console.WriteLine("                m for Memory of the World");
    Console.WriteLine("      [BookName] ::= Σ*");
    Console.WriteLine("===============================");
}