using SilverFir.SearchBonds;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit;

namespace SilverFir
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly ISearchBonds _searchBonds;

        /// <summary>
        ///     Main window
        /// </summary>
        public MainWindow(ISearchBonds searchBonds)
        {
            _searchBonds = searchBonds;

            InitializeComponent();
            CommonWindow.Children.Clear();
            DrawMainWindow();
            SearchParameters(new InputParameters(), true);
        }

        private void DrawMainWindow()
        {
            const int rowCount = 7;
            const int columnCount = 6;

            for (var i = 0; i < rowCount; i++) //Row
            {
                CommonWindow.RowDefinitions.Add(new RowDefinition());
            }

            for (var i = 0; i < columnCount; i++) //Column
            {
                CommonWindow.ColumnDefinitions.Add(new ColumnDefinition());
            }

            #region Output box

            var outputBox = new TextBox
            {
                IsReadOnly = true,
                Name = "OutputBox",
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };

            Grid.SetRow(outputBox, 2);
            Grid.SetColumn(outputBox, 1);
            Grid.SetRowSpan(outputBox, 4);
            Grid.SetColumnSpan(outputBox, 4);

            CommonWindow.Children.Add(outputBox);

            RegisterName("OutputBox", outputBox);

            #endregion Output box

            #region Yield more

            var yieldMore = new Label
            {
                Content = "Доходность\nбольше, чем:",
                HorizontalAlignment = HorizontalAlignment.Right,
                Name = "YieldMore",
                VerticalAlignment = VerticalAlignment.Center
            };

            Grid.SetRow(yieldMore, 0);
            Grid.SetColumn(yieldMore, 0);

            CommonWindow.Children.Add(yieldMore);

            var yieldMoreValue = new IntegerUpDown
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Increment = 1,
                Minimum = 0,
                Name = "YieldMoreValue",
                VerticalAlignment = VerticalAlignment.Center,
                Width = 90
            };

            Grid.SetRow(yieldMoreValue, 0);
            Grid.SetColumn(yieldMoreValue, 1);

            CommonWindow.Children.Add(yieldMoreValue);

            RegisterName("YieldMoreValue", yieldMoreValue);

            #endregion Yield more

            #region Yield less

            var yieldLess = new Label
            {
                Content = "Доходность\nменьше, чем:",
                HorizontalAlignment = HorizontalAlignment.Right,
                Name = "YieldLess",
                VerticalAlignment = VerticalAlignment.Center
            };

            Grid.SetRow(yieldLess, 0);
            Grid.SetColumn(yieldLess, 2);

            CommonWindow.Children.Add(yieldLess);

            var yieldLessValue = new IntegerUpDown
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Increment = 1,
                Minimum = 0,
                Name = "YieldLessValue",
                VerticalAlignment = VerticalAlignment.Center,
                Width = 90
            };

            Grid.SetRow(yieldLessValue, 0);
            Grid.SetColumn(yieldLessValue, 3);

            CommonWindow.Children.Add(yieldLessValue);

            RegisterName("YieldLessValue", yieldLessValue);

            #endregion Yield less

            #region Issue volume more

            var issueVolumeMore = new Label
            {
                Content = "Объём\nэмиссии\nбольше, чем:",
                HorizontalAlignment = HorizontalAlignment.Right,
                Name = "IssueVolumeMore",
                VerticalAlignment = VerticalAlignment.Center
            };

            Grid.SetRow(issueVolumeMore, 0);
            Grid.SetColumn(issueVolumeMore, 4);

            CommonWindow.Children.Add(issueVolumeMore);

            var issueVolumeMoreValue = new IntegerUpDown
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Increment = 1,
                Minimum = 0,
                Name = "IssueVolumeMoreValue",
                VerticalAlignment = VerticalAlignment.Center,
                Width = 90
            };

            Grid.SetRow(issueVolumeMoreValue, 0);
            Grid.SetColumn(issueVolumeMoreValue, 5);

            CommonWindow.Children.Add(issueVolumeMoreValue);

            RegisterName("IssueVolumeMoreValue", issueVolumeMoreValue);

            #endregion Issue volume more

            #region Days to maturity more

            var daysToMaturityMore = new Label
            {
                Content = "Дней до\nпогашения\nбольше, чем:",
                HorizontalAlignment = HorizontalAlignment.Right,
                Name = "DaysToMaturityMore",
                VerticalAlignment = VerticalAlignment.Center
            };

            Grid.SetRow(daysToMaturityMore, 1);
            Grid.SetColumn(daysToMaturityMore, 0);

            CommonWindow.Children.Add(daysToMaturityMore);

            var daysToMaturityMoreValue = new IntegerUpDown
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Increment = 1,
                Minimum = 0,
                Name = "DaysToMaturityMoreValue",
                VerticalAlignment = VerticalAlignment.Center,
                Width = 90
            };

            Grid.SetRow(daysToMaturityMoreValue, 1);
            Grid.SetColumn(daysToMaturityMoreValue, 1);

            CommonWindow.Children.Add(daysToMaturityMoreValue);

            RegisterName("DaysToMaturityMoreValue", daysToMaturityMoreValue);

            #endregion Days to maturity more

            #region Days to maturity less

            var daysToMaturityLess = new Label
            {
                Content = "Дней до\nпогашения\nменьше, чем:",
                HorizontalAlignment = HorizontalAlignment.Right,
                Name = "DaysToMaturityLess",
                VerticalAlignment = VerticalAlignment.Center
            };

            Grid.SetRow(daysToMaturityLess, 1);
            Grid.SetColumn(daysToMaturityLess, 2);

            CommonWindow.Children.Add(daysToMaturityLess);

            var daysToMaturityLessValue = new IntegerUpDown
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Increment = 1,
                Minimum = 0,
                Name = "DaysToMaturityLessValue",
                VerticalAlignment = VerticalAlignment.Center,
                Width = 90
            };

            Grid.SetRow(daysToMaturityLessValue, 1);
            Grid.SetColumn(daysToMaturityLessValue, 3);

            CommonWindow.Children.Add(daysToMaturityLessValue);

            RegisterName("DaysToMaturityLessValue", daysToMaturityLessValue);

            #endregion Days to maturity less

            #region Get bonds button

            var getBondsButton = new Button
            {
                Content = RegisterNames.GET_BONDS,
                Height = 30,
                HorizontalAlignment = HorizontalAlignment.Center,
                Name = "GetBonds",
                VerticalAlignment = VerticalAlignment.Center,
                Width = 120
            };

            Grid.SetRow(getBondsButton, 6);
            Grid.SetColumn(getBondsButton, 1);
            Grid.SetColumnSpan(getBondsButton, 2);

            getBondsButton.Click += ButtonClickAsync;
            CommonWindow.Children.Add(getBondsButton);

            RegisterName("GetBonds", getBondsButton);

            #endregion Get bonds button

            #region Clear button

            var clearButton = new Button
            {
                Content = RegisterNames.CLEAR,
                Height = 30,
                HorizontalAlignment = HorizontalAlignment.Center,
                Name = "Clear",
                VerticalAlignment = VerticalAlignment.Center,
                Width = 120
            };

            Grid.SetRow(clearButton, 6);
            Grid.SetColumn(clearButton, 3);
            Grid.SetColumnSpan(clearButton, 2);

            clearButton.Click += ButtonClickAsync;
            CommonWindow.Children.Add(clearButton);

            RegisterName("Clear", clearButton);

            #endregion Clear button
        }

        private async void ButtonClickAsync(object sender, RoutedEventArgs e)
        {
            if (sender is Button senderButton && CommonWindow.FindName("OutputBox") is TextBox result)
            {
                var newInputParameters = NewInputParameters();

                switch (senderButton.Content.ToString())
                {
                    case RegisterNames.GET_BONDS:
                    {
                        SearchParameters(newInputParameters, false);

                        var errors = ErrorsInputParameters(newInputParameters);

                        if (!errors.Any())
                        {
                            try
                            {
                                result.Text = await SearchBondsResult(newInputParameters);
                            }
                            catch (Exception)
                            {
                                result.Text = "Ошибка подключения";
                            }
                        }
                        else
                        {
                            result.Text = string.Join("\n", errors);
                        }

                        break;
                    }

                    case RegisterNames.CLEAR:
                    {
                        result.Text = string.Empty;

                        break;
                    }
                }

                SearchParameters(newInputParameters, true);
            }
        }

        private InputParameters NewInputParameters()
        {
            var inputParameters = new InputParameters();

            #region Доходность

            int.TryParse((CommonWindow.FindName("YieldMoreValue") as IntegerUpDown)?.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var yieldMoreValue);
            inputParameters.YieldMore = yieldMoreValue;

            int.TryParse((CommonWindow.FindName("YieldLessValue") as IntegerUpDown)?.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var yieldLessValue);
            inputParameters.YieldLess = yieldLessValue;

            #endregion Доходность

            #region Объём эмиссии

            int.TryParse((CommonWindow.FindName("IssueVolumeMoreValue") as IntegerUpDown)?.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var issueVolumeMoreValue);
            inputParameters.IssueVolumeMore = issueVolumeMoreValue;

            #endregion Объём эмиссии

            #region Количество дней до погашения

            int.TryParse((CommonWindow.FindName("DaysToMaturityMoreValue") as IntegerUpDown)?.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var daysToMaturityMoreValue);
            inputParameters.DaysToMaturityMore = daysToMaturityMoreValue;

            int.TryParse((CommonWindow.FindName("DaysToMaturityLessValue") as IntegerUpDown)?.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var daysToMaturityLessValue);
            inputParameters.DaysToMaturityLess = daysToMaturityLessValue;

            #endregion Количество дней до погашения

            return inputParameters;
        }

        private void SearchParameters(InputParameters inputParameters, bool isEnabled)
        {
            #region Доходность

            if (CommonWindow.FindName("YieldMoreValue") is IntegerUpDown yieldMoreValue)
            {
                yieldMoreValue.Text = inputParameters.YieldMore.ToString(CultureInfo.InvariantCulture);
                yieldMoreValue.IsEnabled = isEnabled;
            }

            if (CommonWindow.FindName("YieldLessValue") is IntegerUpDown yieldLessValue)
            {
                yieldLessValue.Text = inputParameters.YieldLess.ToString(CultureInfo.InvariantCulture);
                yieldLessValue.IsEnabled = isEnabled;
            }

            #endregion Доходность

            #region Объём эмиссии

            if (CommonWindow.FindName("IssueVolumeMoreValue") is IntegerUpDown issueVolumeMoreValue)
            {
                issueVolumeMoreValue.Text = inputParameters.IssueVolumeMore.ToString(CultureInfo.InvariantCulture);
                issueVolumeMoreValue.IsEnabled = isEnabled;
            }

            #endregion Объём эмиссии

            #region Количество дней до погашения

            if (CommonWindow.FindName("DaysToMaturityMoreValue") is IntegerUpDown daysToMaturityMoreValue)
            {
                daysToMaturityMoreValue.Text = inputParameters.DaysToMaturityMore.ToString(CultureInfo.InvariantCulture);
                daysToMaturityMoreValue.IsEnabled = isEnabled;
            }

            if (CommonWindow.FindName("DaysToMaturityLessValue") is IntegerUpDown daysToMaturityLessValue)
            {
                daysToMaturityLessValue.Text = inputParameters.DaysToMaturityLess.ToString(CultureInfo.InvariantCulture);
                daysToMaturityLessValue.IsEnabled = isEnabled;
            }

            #endregion Количество дней до погашения

            #region Кнопки

            if (CommonWindow.FindName("GetBonds") is Button getBondsButton)
            {
                getBondsButton.IsEnabled = isEnabled;
            }

            if (CommonWindow.FindName("Clear") is Button clearButton)
            {
                clearButton.IsEnabled = isEnabled;
            }

            #endregion Кнопки
        }

        private static List<string> ErrorsInputParameters(InputParameters inputParameters)
        {
            var errors = new List<string>();

            if (inputParameters.YieldMore > inputParameters.YieldLess)
            {
                errors.Add("Неверные значения доходности");
            }

            if (inputParameters.DaysToMaturityMore > inputParameters.DaysToMaturityLess)
            {
                errors.Add("Неверные значения количества дней до погашения");
            }

            return errors;
        }

        private async Task<string> SearchBondsResult(InputParameters inputParameters)
        {
            var bonds = await _searchBonds.MoexSearchBonds(inputParameters);

            return bonds.Any()
                ? string.Join("\n", bonds.Select(x => x.SecId +
                                                      "\t   " +
                                                      x.BondName +
                                                      "\t   " +
                                                      x.MaturityDate.ToString("dd.MM.yyyy") +
                                                      "\t  " +
                                                      x.BondYield +
                                                      "\t   " +
                                                      x.IssueVolume))
                : "Нет облигаций для выбранных параметров";
        }
    }
}