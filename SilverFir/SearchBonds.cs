using Newtonsoft.Json;
using SilverFir.MoexClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SilverFir
{
    /// <summary>
    ///     Поиск облигаций
    /// </summary>
    [SuppressMessage("ReSharper", "ConvertToUsingDeclaration")]
    internal static class SearchBonds
    {
        /// <summary>
        ///     Узнаем boardId любой бумаги по тикеру
        /// </summary>
        private static string MoexBoardId(string secId)
        {
            var url = $"https://iss.moex.com/iss/securities/{secId}.json?iss.meta=off&iss.only=boards&boards.columns=secid,boardid,is_primary";

            using (var client = new WebClient())
            {
                var board = JsonConvert.DeserializeObject<MoexBoards>(client.DownloadString(url));

                return board.Boards.Data.Where(x => x[2]?.ToString() == "1").Select(x => x[1]).FirstOrDefault()?.ToString();
            }
        }

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
                    7, //Т0: Основной режим - безадрес.
                    58, //Т+: Основной режим - безадрес.
                    193 //Т+: Основной режим (USD) - безадрес.
                };

                foreach (var boardgroup in boardgroups)
                {
                    var url = $"https://iss.moex.com/iss/engines/stock/markets/bonds/boardgroups/{boardgroup}/securities.json?iss.dp=comma&iss.meta=off&iss.only=securities,marketdata&securities.columns=SECID,SECNAME,PREVLEGALCLOSEPRICE&marketdata.columns=SECID,YIELD,DURATION";

                    using (var client = new WebClient())
                    {
                        var resultBoardGroup = JsonConvert.DeserializeObject<BoardGroups>(client.DownloadString(url).Replace("\\\"", ""));

                        for (var data = 0; data < resultBoardGroup.Securities.Data.Count; data++)
                        {
                            var bondName = resultBoardGroup.Securities.Data[data][1]?.ToString() ?? string.Empty;
                            var secId = resultBoardGroup.Securities.Data[data][0]?.ToString() ?? string.Empty;
                            var bondPrice = Convert.ToDecimal(resultBoardGroup.Securities.Data[data][2] ?? 0);
                            var bondYield = Convert.ToDecimal(resultBoardGroup.MarketData.Data[data][1] ?? 0);
                            var bondDuration = Math.Floor(Convert.ToDecimal(resultBoardGroup.MarketData.Data[data][2] ?? 0) / 30 * 100) / 100; // Количество оставшихся месяцев

                            if (bondYield > inputParameters.YieldMore &&
                                bondYield < inputParameters.YieldLess && //условия выборки
                                bondPrice > inputParameters.PriceMore &&
                                bondPrice < inputParameters.PriceLess &&
                                bondDuration > inputParameters.DurationMore &&
                                bondDuration < inputParameters.DurationLess)
                            {
                                var bondVolume = MoexSearchVolume(secId, inputParameters.PreviousDaysCount);

                                if (bondVolume > inputParameters.VolumeMore) //если оборот в бумагах больше этой цифры
                                {
                                    var bondTax = MoexSearchTax(secId);

                                    result.Add(new BondsResult
                                    {
                                        BondName = bondName,
                                        SecId = secId,
                                        BondPrice = bondPrice,
                                        BondVolume = bondVolume,
                                        BondYield = bondYield,
                                        BondDuration = bondDuration,
                                        BondTax = bondTax
                                    });
                                }
                            }
                        }
                    }
                }

                return result.Count == 0 ? null : result;
            });

            return task;
        }

        /// <summary>
        ///     Налоговые льготы для корпоративных облигаций, выпущенных с 1 января 2017 года
        /// </summary>
        private static bool MoexSearchTax(string secId)
        {
            var url = $"https://iss.moex.com/iss/securities/{secId}.json?iss.meta=off&iss.only=description";

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

        /// <summary>
        ///     Суммирование оборотов по корпоративной облигации за последние n дней
        /// </summary>
        private static decimal MoexSearchVolume(string secId, int previousDaysCount)
        {
            var now = DateTime.Now;
            var datePrevious = now.AddDays(-previousDaysCount);
            var datePreviousRequest = $"{datePrevious.Year}-{datePrevious.Month}-{datePrevious.Day}";

            var boardId = MoexBoardId(secId);

            if (boardId == null)
            {
                return 0;
            }

            var url = $"https://iss.moex.com/iss/history/engines/stock/markets/bonds/boards/{boardId}/securities/{secId}.json?iss.meta=off&iss.only=history&history.columns=SECID,TRADEDATE,VOLUME,NUMTRADES&from={datePreviousRequest}";

            using (var client = new WebClient())
            {
                var history = JsonConvert.DeserializeObject<MoexHistory>(client.DownloadString(url));

                return history.History.Data.Sum(x => Convert.ToDecimal(x[2]));
            }
        }
    }
}