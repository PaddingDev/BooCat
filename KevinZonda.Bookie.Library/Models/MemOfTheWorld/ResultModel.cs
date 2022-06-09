using System.Text.Json.Serialization;

namespace KevinZonda.Bookie.Library.Models.MemOfTheWorld;

internal class ResultModel
{
    [JsonPropertyName("_items")]
    public List<ResultItemModel> Items { get; set; }
}

internal class ResultItemModel
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

internal class ResultItemFormatModel
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