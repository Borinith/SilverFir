﻿using System.Collections;
using System.Collections.Generic;

namespace SilverFir.SearchBonds.MoexClasses
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public record Security(List<string>? Columns, List<ArrayList>? Data) : CommonMoexData(Columns, Data);
}