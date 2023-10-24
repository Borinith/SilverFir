using System.Text.Json.Serialization;

namespace SilverFir.SearchBonds.MoexClasses
{
    public record BoardGroup
    {
        [JsonPropertyName("securities")]
        public Security? Securities { get; init; }
    }
}