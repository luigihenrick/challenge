using System.Text.Json.Serialization;
using Infraestructure.Common;

namespace Infraestructure.Models;

public class StoryResponse
{
    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("url")]
    public string Uri { get; set; }

    [JsonPropertyName("by")]
    public string PostedBy { get; set; }

    [JsonConverter(typeof(UnixEpochDateTimeConverter))]
    [JsonPropertyName("time")]
    public DateTime Time { get; set; }

    [JsonPropertyName("score")]
    public int Score { get; set; }

    [JsonPropertyName("descendants")]
    public int CommentCount { get; set; }
}