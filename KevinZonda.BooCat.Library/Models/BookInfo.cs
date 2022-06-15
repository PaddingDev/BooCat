using System.Text.Json;

#pragma warning disable 8618

namespace KevinZonda.BooCat.Library.Models;

public sealed class BookInfo
{
    public string? ID { get; set; }
    public string? Name { get; set; }
    public string? Url { get; set; }
    public string[]? Authors { get; set; }
    public string[]? Publishers { get; set; }
    public string? Date { get; set; }
    public string? FileType { get; set; }
    public string? Language { get; set; }
    public string? FileSize { get; set; }

    public override string ToString()
    {
        var option = new JsonSerializerOptions()
        {
            WriteIndented = true
        };

        return JsonSerializer.Serialize(this, option);
    }
}

