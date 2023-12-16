using SilverFir.SearchBonds.MoexClasses;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace SilverFir.SearchBonds
{
    public interface ISearchBonds
    {
        Task<List<BondResult>> MoexSearchBonds(InputParameters inputParameters);

        Task<HttpStatusCode> Ping(string url);
    }
}