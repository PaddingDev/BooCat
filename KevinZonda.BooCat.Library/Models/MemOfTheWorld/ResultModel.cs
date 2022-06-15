using System.Text.Json.Serialization;

#pragma warning disable 8618

namespace KevinZonda.BooCat.Library.Models.MemOfTheWorld;

internal sealed class ResultModel
{
    [JsonPropertyName("_items")]
    public List<ResultItemModel> Items { get; set; }
}

internal sealed class ResultItemModel
{
    [JsonPropertyName("_id")]
    public string ID { get; set; }
    [JsonPropertyName("title")]
    public string Title { get; set; }
    [JsonPropertyName("pubdate")]
    public DateTime PubDate { get; set; }
    [JsonPropertyName("authors")]
    public List<string> Authors { get; set; }
    [JsonPropertyName("formats")]
    public List<ResultItemFormatModel> Formats { get; set; }
}

internal sealed class ResultItemFormatModel
{
    [JsonPropertyName("format")]
    public string Format { get; set; }
    [JsonPropertyName("file_name")]
    public string FileName { get; set; }
    [JsonPropertyName("dir_path")]
    public string DirPath { get; set; }
    [JsonPropertyName("size")]
    public long Size { get; set; }
}