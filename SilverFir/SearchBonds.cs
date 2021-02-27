using Newtonsoft.Json;
using SilverFir.MoexClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SilverFir
{
    /// <summary>
    ///     Поиск облигаций
    /// </summary>
    internal static class SearchBonds
    {
        /// <summary>
        ///     Поиск облигаций по параметрам
        /// </summary>
        internal static Task<List<BondsResult>> MoexSearchBonds(InputParameters inputParameters)
        {
            var task = Task.Run(() =>
            {
                var result = new List<BondsResult>();

                var boardgroups = new List<int>
                {
                    7, // Т0: Основной режим - безадрес.
                    58, // Т+: Основной режим - безадрес.
                    193 // Т+: Основной режим (USD) - безадрес.
                };

                foreach (var boardgroup in boardgroups)
                {
                    var url = $"https://iss.moex.com/iss/engines/stock/markets/bonds/boardgroups/{boardgroup}/securities.json?iss.dp=comma&iss.meta=off&iss.only=securities,marketdata&securities.columns=SECID,SHORTNAME,ISSUESIZEPLACED,MATDATE,COUPONPERCENT&marketdata.columns=SECID,TRADINGSTATUS";

                    // ReSharper disable once ConvertToUsingDeclaration
                    using (var client = new WebClient())
                    {
                        var resultBoardGroup = JsonConvert.DeserializeObject<BoardGroups>(client.DownloadString(url).Replace("\\\"", ""));

                        Parallel.For(0, resultBoardGroup.Securities.Data.Count, data =>
                        {
                            // Код ценной бумаги
                            var secId = resultBoardGroup.Securities.Data[data][0]?.ToString() ?? string.Empty;

                            // Наименование
                            var bondName = resultBoardGroup.Securities.Data[data][1]?.ToString() ?? string.Empty;

                            // Первоначальная номинальная стоимость
                            var initialNominalValue = 1000;

                            // Объём выпуска
                            var issueVolumeCount = Convert.ToInt64(resultBoardGroup.Securities.Data[data][2] ?? 0);

                            // Объём эмиссии
                            var issueVolume = initialNominalValue * issueVolumeCount;

                            // Дата погашения
                            if (!DateTime.TryParse(resultBoardGroup.Securities.Data[data][3]?.ToString() ?? string.Empty, out var maturityDate))
                            {
                                return;
                            }

                            // Дней до погашения
                            var daysToMaturity = (maturityDate - DateTime.Today).Days;

                            // Доходность
                            var bondYield = Convert.ToDecimal(resultBoardGroup.Securities.Data[data][4] ?? 0);

                            // Состояние выпуска - в обращении
                            var releaseStatus = (resultBoardGroup.MarketData.Data[data][1]?.ToString() ?? string.Empty) == "N";

                            // Условия выборки
                            if (bondYield >= inputParameters.YieldMore &&
                                bondYield <= inputParameters.YieldLess &&
                                issueVolume >= inputParameters.IssueVolumeMore &&
                                daysToMaturity >= inputParameters.DaysToMaturityMore &&
                                daysToMaturity <= inputParameters.DaysToMaturityLess &&
                                releaseStatus
                            )
                            {
                                var bondTax = MoexSearchTax(secId);

                                var bond = new BondsResult
                                {
                                    BondName = bondName,
                                    BondTax = bondTax,
                                    BondYield = bondYield,
                                    IssueVolume = issueVolume,
                                    MaturityDate = maturityDate,
                                    ReleaseStatus = true,
                                    SecId = secId
                                };

                                lock (result)
                                {
                                    result.Add(bond);
                                }
                            }
                        });
                    }
                }

                return result.Count == 0 ? null : result.OrderByDescending(x => x.MaturityDate).ThenBy(x => x.SecId).ToList();
            });

            return task;
        }

        /// <summary>
        ///     Налоговые льготы для корпоративных облигаций, выпущенных с 1 января 2017 года
        /// </summary>
        private static bool MoexSearchTax(string secId)
        {
            var url = $"https://iss.moex.com/iss/securities/{secId}.json?iss.meta=off&iss.only=description";

            // ReSharper disable once ConvertToUsingDeclaration
            using (var client = new WebClient())
            {
                var tax = JsonConvert.DeserializeObject<MoexTax>(client.DownloadString(url));

                var startDateMoex = tax.Description.Data.Where(x => x[0]?.ToString() == "STARTDATEMOEX").Select(x => x[2]).FirstOrDefault()?.ToString();

                if (DateTime.TryParse(startDateMoex, out var startDate))
                {
                    return startDate > new DateTime(2017, 1, 1);
                }

                return false;
            }
        }
    }
}