using Newtonsoft.Json;
using SilverFir.MoexClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;

namespace SilverFir
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    [SuppressMessage("ReSharper", "ConvertToUsingDeclaration")]
    public partial class MainWindow
    {
        private const decimal YieldMore = 7; //Доходность больше этой цифры
        private const decimal YieldLess = 14; //Доходность меньше этой цифры
        private const decimal PriceMore = 95; //Цена больше этой цифры
        private const decimal PriceLess = 101; //Цена меньше этой цифры
        private const decimal DurationMore = 1; //Дюрация больше этой цифры
        private const decimal DurationLess = 6; //Дюрация меньше этой цифры
        private const decimal VolumeMore = 5000; //Объем сделок за n дней, шт. больше этой цифры
        private static readonly string Conditions = $"<li>${YieldMore}% < Доходность < ${YieldLess}%</li><li>${PriceMore}% < Цена < ${PriceLess}%</li><li>${DurationMore} мес. < Дюрация < ${DurationLess} мес.</li><li>Объем сделок за n дней > ${VolumeMore} шт.</li><li>Поиск в Т0, Т+, Т+ (USD) - Основной режим - безадрес.</li>";

        private readonly Dictionary<string, Button> _buttons = new Dictionary<string, Button>
        {
            {
                "Get bonds", new Button
                {
                    Width = 120,
                    Height = 30,
                    Content = "Get bonds",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                }
            }
        };

        public MainWindow()
        {
            InitializeComponent();
            CommonWindow.Children.Clear();
            CreateButtons();
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            //GridButtons.Children.Clear();

            if (sender is Button senderButton)
            {
                MoexSearchBonds();
            }
        }

        private void CreateButtons()
        {
            CommonWindow.Children.Add(GridButtons);

            var buttons = _buttons;

            buttons.TryGetValue("Get bonds", out var getBonds);
            Grid.SetRow(getBonds ?? throw new InvalidOperationException(), 1);
            Grid.SetColumn(getBonds, 0);
            getBonds.Click += ButtonClick;
            GridButtons.Children.Add(getBonds);
        }

        /// <summary>
        ///     Узнаем boardId любой бумаги по тикеру
        /// </summary>
        private string MoexBoardId(string secId)
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
        private void MoexSearchBonds()
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

                        if (bondYield > YieldMore &&
                            bondYield < YieldLess && //условия выборки
                            bondPrice > PriceMore &&
                            bondPrice < PriceLess &&
                            bondDuration > DurationMore &&
                            bondDuration < DurationLess)
                        {
                            var bondVolume = MoexSearchVolume(secId);

                            if (bondVolume > VolumeMore) //если оборот в бумагах больше этой цифры
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

            if (result.Count == 0)
            {
            }
        }

        /// <summary>
        ///     Налоговые льготы для корпоративных облигаций, выпущенных с 1 января 2017 года
        /// </summary>
        private bool MoexSearchTax(string secId)
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
        private decimal MoexSearchVolume(string secId)
        {
            var now = DateTime.Now;
            var datePrevious = now.AddDays(-15);
            var datePreviousRequest = $"{datePrevious.Year}-{datePrevious.Month}-{datePrevious.Day}";

            var boardId = MoexBoardId(secId);

            if (boardId == null)
            {
                return 0;
            }

            var url = $"https://iss.moex.com/iss/history/engines/stock/markets/bonds/boards/{boardId}/securities/{secId}.json?iss.meta=off&iss.only=history&history.columns=SECID,TRADEDATE,VOLUME,NUMTRADES&limit=20&from={datePreviousRequest}";

            using (var client = new WebClient())
            {
                var history = JsonConvert.DeserializeObject<MoexHistory>(client.DownloadString(url));

                return history.History.Data.Sum(x => Convert.ToDecimal(x[2]));
            }
        }
    }
}