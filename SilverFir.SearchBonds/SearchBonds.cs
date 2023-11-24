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

        /// <summary>
        ///     Поиск облигаций по параметрам
        /// </summary>
        public async Task<List<BondResult>> MoexSearchBonds(InputParameters inputParameters)
        {
            var boardgroupResults = new ConcurrentDictionary<int, BoardGroup?>();
            var bondResults = new ConcurrentDictionary<string, BondResult>();

            var boardgroupIds = new[]
            {
                7, // Т0: Основной режим - безадрес.
                58, // Т+: Основной режим - безадрес.
                193 // Т+: Основной режим (USD) - безадрес.
            };

            using (var cts = new CancellationTokenSource())
            {
                await Parallel.ForEachAsync(boardgroupIds, cts.Token, async (boardgroupId, token) =>
                {
                    var url = $"https://iss.moex.com/iss/engines/stock/markets/bonds/boardgroups/{boardgroupId}/securities.json?iss.dp=comma&iss.meta=off&iss.only=securities&securities.columns=SECID,SHORTNAME,ISSUESIZEPLACED,MATDATE,COUPONPERCENT,STATUS";

                    using (var client = new HttpClient())
                    using (var response = await client.GetAsync(url, token))
                    {
                        if (response.StatusCode != HttpStatusCode.OK)
                        {
                            await cts.CancelAsync();
                        }

                        var boardGroup = await response.Content.ReadFromJsonAsync<BoardGroup>(token);
                        boardgroupResults.TryAdd(boardgroupId, boardGroup);
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
                    var bond = new BondResult(bondName, bondYield, issueVolume, maturityDate, releaseStatus, secId);
                    bondResults.TryAdd(secId, bond);
                }
            });

            return bondResults
                .Select(x => x.Value)
                .OrderByDescending(x => x.MaturityDate)
                .ThenBy(x => x.SecId)
                .ToList();
        }
    }
}