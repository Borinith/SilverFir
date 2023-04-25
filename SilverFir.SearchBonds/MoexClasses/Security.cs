using System.Collections;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SilverFir.SearchBonds.MoexClasses
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public record Security
    {
        [JsonPropertyName("columns")]
        public List<string>? Columns { get; init; }

        [JsonPropertyName("data")]
        // ReSharper disable once CollectionNeverUpdated.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public List<ArrayList>? Data { get; init; }
    }
}