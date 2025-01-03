using SilverFir.SearchBonds;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace SilverFir.Tests
{
    public class SearchBondsTest
    {
        private readonly ISearchBonds _searchBonds;

        public SearchBondsTest(ISearchBonds searchBonds)
        {
            _searchBonds = searchBonds;
        }

        [Fact]
        public async Task PingTest()
        {
            const string url = "https://iss.moex.com/iss/engines";
            var statusCode = await _searchBonds.Ping(url);

            Assert.Equal(HttpStatusCode.OK, statusCode);
        }

        [Fact]
        public async Task MoexSearchBondsNotEmptyTest()
        {
            var inputParameters = new InputParameters
            {
                YieldMore = 15,
                YieldLess = 25
            };

            var bonds = await _searchBonds.MoexSearchBonds(inputParameters);

            Assert.NotEmpty(bonds);
        }

        [Fact]
        public async Task MoexSearchBondsEmptyTest()
        {
            var inputParameters = new InputParameters
            {
                YieldMore = 25,
                YieldLess = 15
            };

            var bonds = await _searchBonds.MoexSearchBonds(inputParameters);

            Assert.Empty(bonds);
        }
    }
}