using System.Text.Json.Serialization;

namespace SilverFir.SearchBonds.MoexClasses
{
    public record BoardGroups
    {
        [JsonPropertyName("securities")]
        public Security? Securities { get; init; }
    }
}