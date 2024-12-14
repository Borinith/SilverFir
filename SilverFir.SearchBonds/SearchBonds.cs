using SilverFir.SearchBonds.MoexClasses;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SilverFir.SearchBonds
{
    /// <summary>
    ///     Поиск облигаций
    /// </summary>
    public class SearchBonds : ISearchBonds
    {
        // Первоначальная номинальная стоимость
        private const int INITIAL_NOMINAL_VALUE = 1000;

        private readonly IHttpClientFactory _httpClientFactory;

        public SearchBonds(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        ///     Поиск облигаций по параметрам
        /// </summary>
        public async Task<List<BondResult>> MoexSearchBonds(InputParameters inputParameters)
        {
            var boardgroupResults = new ConcurrentDictionary<int, Content?>();
            var bondResults = new ConcurrentDictionary<string, BondResult>();

            var boardgroupIds = new[]
            {
                7, // Т0: Основной режим - безадрес.
                58, // Т+: Основной режим - безадрес.
                193 // Т+: Основной режим (USD) - безадрес.
            };

            using (var cts = new CancellationTokenSource())
            using (var client = _httpClientFactory.CreateClient())
            {
                await Parallel.ForEachAsync(boardgroupIds, cts.Token, async (boardgroupId, token) =>
                {
                    var urlSecurities = $"https://iss.moex.com/iss/engines/stock/markets/bonds/boardgroups/{boardgroupId}/securities.json?iss.dp=comma&iss.meta=off&iss.only=securities&securities.columns=SECID,SHORTNAME,ISSUESIZEPLACED,MATDATE,COUPONPERCENT,STATUS";

                    using (var response = await client.GetAsync(urlSecurities, token))
                    {
                        if (response.StatusCode != HttpStatusCode.OK)
                        {
                            await cts.CancelAsync();
                        }

                        var content = await response.Content.ReadFromJsonAsync<Content>(token);
                        boardgroupResults.TryAdd(boardgroupId, content);
                    }
                });
            }

            var allData = boardgroupResults
                .Where(x => x.Value?.Securities?.Data != null)
                .SelectMany(x => x.Value!.Securities!.Data!);

            Parallel.ForEach(allData, data =>
            {
                // Дата погашения
                if (!DateTime.TryParse(data[3]?.ToString() ?? string.Empty, out var maturityDate))
                {
                    return;
                }

                // Код ценной бумаги
                var secId = data[0]?.ToString() ?? string.Empty;

                // Наименование
                var bondName = data[1]?.ToString() ?? string.Empty;

                // Объём выпуска
                var issueVolumeCount = Convert.ToInt64((data[2] ?? 0).ToString());

                // Объём эмиссии
                var issueVolume = INITIAL_NOMINAL_VALUE * issueVolumeCount;

                // Дней до погашения
                var daysToMaturity = (maturityDate - DateTime.Today).TotalDays;

                // Доходность
                var bondYield = Convert.ToDecimal((data[4] ?? 0).ToString());

                // Состояние выпуска - в обращении
                var releaseStatus = (data[5]?.ToString() ?? string.Empty) == SecStatus.A.ToString();

                // Условия выборки
                if (bondYield >= inputParameters.YieldMore &&
                    bondYield <= inputParameters.YieldLess &&
                    issueVolume >= inputParameters.IssueVolumeMore &&
                    daysToMaturity >= inputParameters.DaysToMaturityMore &&
                    daysToMaturity <= inputParameters.DaysToMaturityLess &&
                    releaseStatus
                   )
                {
                    var bond = new BondResult(bondName, bondYield, issueVolume, maturityDate, secId);

                    // ReSharper disable once AccessToModifiedClosure
                    bondResults.TryAdd(secId, bond);
                }
            });

            bondResults = await MoexDescriptionFilter(bondResults, inputParameters.StartDateMoexMore);

            return bondResults
                .Select(x => x.Value)
                .OrderByDescending(x => x.BondYield)
                .ThenBy(x => x.SecId)
                .ToList();
        }

        public async Task<HttpStatusCode> Ping(string url)
        {
            using (var client = _httpClientFactory.CreateClient())
            using (var response = await client.GetAsync(url))
            {
                return response.StatusCode;
            }
        }

        private async Task<ConcurrentDictionary<string, BondResult>> MoexDescriptionFilter(ConcurrentDictionary<string, BondResult> bonds, DateTime startDateMoexMore)
        {
            if (bonds.IsEmpty)
            {
                return bonds;
            }

            const string startDateMoex = "STARTDATEMOEX";
            const string isQualifiedInvestors = "ISQUALIFIEDINVESTORS";

            var bondsFilter = new ConcurrentDictionary<string, BondResult>();

            using (var cts = new CancellationTokenSource())
            using (var client = _httpClientFactory.CreateClient())
            {
                await Parallel.ForEachAsync(bonds.Keys, cts.Token, async (bondId, token) =>
                {
                    var urlDescription = $"https://iss.moex.com/iss/securities/{bondId}.json?iss.dp=comma?iss.meta=off&iss.only=description&description.columns=name,value";

                    using (var response = await client.GetAsync(urlDescription, token))
                    {
                        if (response.StatusCode != HttpStatusCode.OK)
                        {
                            await cts.CancelAsync();
                        }

                        var content = await response.Content.ReadFromJsonAsync<Content>(token);

                        if (content?.Description?.Data is not null)
                        {
                            // Дата начала торгов
                            var startDateMoexRaw = content.Description.Data.FirstOrDefault(x => x[0]?.ToString() == startDateMoex)?[1]?.ToString() ?? string.Empty;

                            if (!DateTime.TryParse(startDateMoexRaw, out var startDate))
                            {
                                return;
                            }

                            // Новая облигация?
                            var isNewBond = startDate > startDateMoexMore;

                            if (!isNewBond)
                            {
                                return;
                            }

                            // Неквалифицированный инвестор?
                            var isQualifiedInvestorsRaw = content.Description.Data.FirstOrDefault(x => x[0]?.ToString() == isQualifiedInvestors)?[1]?.ToString() ?? string.Empty;
                            var isUnqualifiedInvestor = isQualifiedInvestorsRaw == "0";

                            if (isUnqualifiedInvestor)
                            {
                                bondsFilter.TryAdd(bondId, bonds[bondId]);
                            }
                        }
                    }
                });

                return bondsFilter;
            }
        }
    }
}