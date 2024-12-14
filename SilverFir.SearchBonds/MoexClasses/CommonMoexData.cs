using System.Collections;
using System.Collections.Generic;

namespace SilverFir.SearchBonds.MoexClasses
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public abstract record CommonMoexData(List<string>? Columns, List<ArrayList>? Data);
}