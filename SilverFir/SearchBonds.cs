using Newtonsoft.Json;
using SilverFir.MoexClasses;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SilverFir
{
    /// <summary>
    ///     Поиск облигаций
    /// </summary>
    internal static class SearchBonds
    {
        // Первоначальная номинальная стоимость
        private const int INITIAL_NOMINAL_VALUE = 1000;

        /// <summary>
        ///     Поиск облигаций по параметрам
        /// </summary>
        internal static async Task<List<BondsResult>> MoexSearchBonds(InputParameters inputParameters)
        {
            var result = new ConcurrentDictionary<string, BondsResult>();

            var boardgroups = new List<int>
            {
                7, // Т0: Основной режим - безадрес.
                58, // Т+: Основной режим - безадрес.
                193 // Т+: Основной режим (USD) - безадрес.
            };

            foreach (var boardgroup in boardgroups)
            {
                var url = $"https://iss.moex.com/iss/engines/stock/markets/bonds/boardgroups/{boardgroup}/securities.json?iss.dp=comma&iss.meta=off&iss.only=securities&securities.columns=SECID,SHORTNAME,ISSUESIZEPLACED,MATDATE,COUPONPERCENT,STATUS";

                var client = new HttpClient();

                using (var response = await client.GetAsync(url))
                using (var content = response.Content)
                {
                    var json = await content.ReadAsStringAsync();

                    var resultBoardGroup = JsonConvert.DeserializeObject<BoardGroups>(json.Replace("\\\"", string.Empty));

                    if (resultBoardGroup != null)
                    {
                        Parallel.ForEach(resultBoardGroup.Securities.Data, data =>
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
                            var issueVolumeCount = Convert.ToInt64(data[2] ?? 0);

                            // Объём эмиссии
                            var issueVolume = INITIAL_NOMINAL_VALUE * issueVolumeCount;

                            // Дней до погашения
                            var daysToMaturity = (maturityDate - DateTime.Today).Days;

                            // Доходность
                            var bondYield = Convert.ToDecimal(data[4] ?? 0);

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
                                var bond = new BondsResult
                                {
                                    BondName = bondName,
                                    BondYield = bondYield,
                                    IssueVolume = issueVolume,
                                    MaturityDate = maturityDate,
                                    ReleaseStatus = true,
                                    SecId = secId
                                };

                                result.TryAdd(secId, bond);
                            }
                        });
                    }
                }
            }

            return result.Values.OrderByDescending(x => x.MaturityDate).ThenBy(x => x.SecId).ToList();
        }
    }
}