using System.Collections;
using System.Collections.Generic;

namespace SilverFir.SearchBonds.MoexClasses
{
    public record GetMoexData
    {
        public List<string>? Columns { get; init; }

        // ReSharper disable once CollectionNeverUpdated.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public List<ArrayList>? Data { get; init; }
    }
}