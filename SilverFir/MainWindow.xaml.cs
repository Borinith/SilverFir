using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
        private readonly Dictionary<string, Button> _buttons = new()
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
            },
            {
                "Clear", new Button
                {
                    Width = 120,
                    Height = 30,
                    Content = "Clear",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                }
            }
        };

        public MainWindow()
        {
            InitializeComponent();
            CommonWindow.Children.Clear();
            CommonWindow.Children.Add(ResultBox);
            SearchParameters(new InputParameters(), true);
            CreateButtons();
        }

        private async void ButtonClickAsync(object sender, RoutedEventArgs e)
        {
            if (sender is Button senderButton && ResultBox.FindName("OutputBox") is TextBox result)
            {
                var newInputParameters = NewInputParameters();

                switch (senderButton.Content.ToString())
                {
                    case "Get bonds":
                        SearchParameters(newInputParameters, false);
                        var errors = new List<string>();

                        if (newInputParameters.YieldMore > newInputParameters.YieldLess)
                        {
                            errors.Add("Неверные значения доходности");
                        }

                        if (newInputParameters.DaysToMaturityMore > newInputParameters.DaysToMaturityLess)
                        {
                            errors.Add("Неверные значения количества дней до погашения");
                        }

                        if (!errors.Any())
                        {
                            try
                            {
                                var bonds = await SearchBonds.MoexSearchBonds(newInputParameters);

                                result.Text = bonds != null
                                    ? string.Join("\n", bonds.Select(x => x.SecId +
                                                                          "\t   " +
                                                                          x.BondName +
                                                                          "\t   " +
                                                                          x.MaturityDate.ToString("dd.MM.yyyy") +
                                                                          "\t  " +
                                                                          x.BondYield +
                                                                          "\t   " +
                                                                          x.IssueVolume +
                                                                          "\t  " +
                                                                          x.BondTax))
                                    : "Нет облигаций для выбранных параметров";
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

                    case "Clear":
                        result.Text = string.Empty;

                        break;
                }

                SearchParameters(newInputParameters, true);
            }
        }

        private void CreateButtons()
        {
            if (_buttons.TryGetValue("Get bonds", out var getBonds))
            {
                Grid.SetColumnSpan(getBonds, 2);
                Grid.SetRow(getBonds, 6);
                Grid.SetColumn(getBonds, 1);
                getBonds.Click += ButtonClickAsync;
                ResultBox.Children.Add(getBonds);
            }

            if (_buttons.TryGetValue("Clear", out var clearWindow))
            {
                Grid.SetColumnSpan(clearWindow, 2);
                Grid.SetRow(clearWindow, 6);
                Grid.SetColumn(clearWindow, 3);
                clearWindow.Click += ButtonClickAsync;
                ResultBox.Children.Add(clearWindow);
            }
        }

        private InputParameters NewInputParameters()
        {
            var inputParameters = new InputParameters();

            #region Доходность

            int.TryParse((ResultBox.FindName("YieldMoreValue") as IntegerUpDown)?.Text, NumberStyles.Integer,
                CultureInfo.InvariantCulture, out var yieldMoreValue);

            inputParameters.YieldMore = yieldMoreValue;

            int.TryParse((ResultBox.FindName("YieldLessValue") as IntegerUpDown)?.Text, NumberStyles.Integer,
                CultureInfo.InvariantCulture, out var yieldLessValue);

            inputParameters.YieldLess = yieldLessValue;

            #endregion Доходность

            #region Объём эмиссии

            int.TryParse((ResultBox.FindName("IssueVolumeMoreValue") as IntegerUpDown)?.Text, NumberStyles.Integer,
                CultureInfo.InvariantCulture, out var issueVolumeMoreValue);

            inputParameters.IssueVolumeMore = issueVolumeMoreValue;

            #endregion Объём эмиссии

            #region Количество дней до погашения

            int.TryParse((ResultBox.FindName("DaysToMaturityMoreValue") as IntegerUpDown)?.Text, NumberStyles.Integer,
                CultureInfo.InvariantCulture, out var daysToMaturityMoreValue);

            inputParameters.DaysToMaturityMore = daysToMaturityMoreValue;

            int.TryParse((ResultBox.FindName("DaysToMaturityLessValue") as IntegerUpDown)?.Text, NumberStyles.Integer,
                CultureInfo.InvariantCulture, out var daysToMaturityLessValue);

            inputParameters.DaysToMaturityLess = daysToMaturityLessValue;

            #endregion Количество дней до погашения

            return inputParameters;
        }

        private void SearchParameters(InputParameters inputParameters, bool isEnabled)
        {
            #region Доходность

            if (ResultBox.FindName("YieldMoreValue") is IntegerUpDown yieldMoreValue)
            {
                yieldMoreValue.Text = inputParameters.YieldMore.ToString(CultureInfo.InvariantCulture);
                yieldMoreValue.IsEnabled = isEnabled;
            }

            if (ResultBox.FindName("YieldLessValue") is IntegerUpDown yieldLessValue)
            {
                yieldLessValue.Text = inputParameters.YieldLess.ToString(CultureInfo.InvariantCulture);
                yieldLessValue.IsEnabled = isEnabled;
            }

            #endregion Доходность

            #region Объём эмиссии

            if (ResultBox.FindName("IssueVolumeMoreValue") is IntegerUpDown issueVolumeMoreValue)
            {
                issueVolumeMoreValue.Text = inputParameters.IssueVolumeMore.ToString(CultureInfo.InvariantCulture);
                issueVolumeMoreValue.IsEnabled = isEnabled;
            }

            #endregion Объём эмиссии

            #region Количество дней до погашения

            if (ResultBox.FindName("DaysToMaturityMoreValue") is IntegerUpDown daysToMaturityMoreValue)
            {
                daysToMaturityMoreValue.Text = inputParameters.DaysToMaturityMore.ToString(CultureInfo.InvariantCulture);
                daysToMaturityMoreValue.IsEnabled = isEnabled;
            }

            if (ResultBox.FindName("DaysToMaturityLessValue") is IntegerUpDown daysToMaturityLessValue)
            {
                daysToMaturityLessValue.Text = inputParameters.DaysToMaturityLess.ToString(CultureInfo.InvariantCulture);
                daysToMaturityLessValue.IsEnabled = isEnabled;
            }

            #endregion Количество дней до погашения
        }
    }
}