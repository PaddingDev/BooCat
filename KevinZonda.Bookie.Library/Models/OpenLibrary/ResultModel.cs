using System.Text.Json.Serialization;

#pragma warning disable 8618

namespace KevinZonda.Bookie.Library.Models.OpenLibrary;

public class Doc
{
    [JsonPropertyName("key")]
    public string Uri { get; set; }
    [JsonPropertyName("title")]
    public string Title { get; set; }
    [JsonPropertyName("first_publish_year")]
    public int Year { get; set; }
    [JsonPropertyName("publisher")]
    public List<string> Publishers { get; set; }
    [JsonPropertyName("language")]
    public List<string> Languages { get; set; }
    [JsonPropertyName("author_name")]
    public List<string> Authors { get; set; }
}

public class ResultModel
{
    [JsonPropertyName("docs")]
    public List<Doc> Docs { get; set; }
}

