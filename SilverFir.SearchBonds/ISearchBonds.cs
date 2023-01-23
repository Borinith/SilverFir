﻿using SilverFir.SearchBonds.MoexClasses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SilverFir.SearchBonds
{
    public interface ISearchBonds
    {
        Task<List<BondsResult>> MoexSearchBonds(InputParameters inputParameters);
    }
}