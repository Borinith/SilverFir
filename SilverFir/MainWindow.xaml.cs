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
            //GridButtons.Children.Clear();

            var newInputParameters = NewInputParameters();

            if (sender is Button senderButton && ResultBox.FindName("OutputBox") is TextBox result)
            {
                switch (senderButton.Content.ToString())
                {
                    case "Get bonds":
                        SearchParameters(newInputParameters, false);
                        var errors = new List<string>();

                        if (newInputParameters.YieldMore > newInputParameters.YieldLess)
                        {
                            errors.Add("Неверные значения доходности");
                        }

                        if (newInputParameters.PriceMore > newInputParameters.PriceLess)
                        {
                            errors.Add("Неверные значения цены");
                        }

                        if (newInputParameters.DurationMore > newInputParameters.DurationLess)
                        {
                            errors.Add("Неверные значения дюрации");
                        }

                        if (errors.Count == 0)
                        {
                            try
                            {
                                var bonds = await SearchBonds.MoexSearchBonds(newInputParameters);

                                result.Text = bonds != null ? string.Join("\n", bonds.Select(x => x.BondName)) : "Нет облигаций для выбранных параметров";
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
            var buttons = _buttons;

            buttons.TryGetValue("Get bonds", out var getBonds);
            Grid.SetColumnSpan(getBonds ?? throw new InvalidOperationException(), 2);
            Grid.SetRow(getBonds, 6);
            Grid.SetColumn(getBonds, 2);
            getBonds.Click += ButtonClickAsync;
            ResultBox.Children.Add(getBonds);

            buttons.TryGetValue("Clear", out var clearWindow);
            Grid.SetColumnSpan(clearWindow ?? throw new InvalidOperationException(), 2);
            Grid.SetRow(clearWindow, 6);
            Grid.SetColumn(clearWindow, 4);
            clearWindow.Click += ButtonClickAsync;
            ResultBox.Children.Add(clearWindow);
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

            #region Цена

            int.TryParse((ResultBox.FindName("PriceMoreValue") as IntegerUpDown)?.Text, NumberStyles.Integer,
                CultureInfo.InvariantCulture, out var priceMoreValue);

            inputParameters.PriceMore = priceMoreValue;

            int.TryParse((ResultBox.FindName("PriceLessValue") as IntegerUpDown)?.Text, NumberStyles.Integer,
                CultureInfo.InvariantCulture, out var priceLessValue);

            inputParameters.PriceLess = priceLessValue;

            #endregion Цена

            #region Дюрация

            int.TryParse((ResultBox.FindName("DurationMoreValue") as IntegerUpDown)?.Text, NumberStyles.Integer,
                CultureInfo.InvariantCulture, out var durationMoreValue);

            inputParameters.DurationMore = durationMoreValue;

            int.TryParse((ResultBox.FindName("DurationLessValue") as IntegerUpDown)?.Text, NumberStyles.Integer,
                CultureInfo.InvariantCulture, out var durationLessValue);

            inputParameters.DurationLess = durationLessValue;

            #endregion Дюрация

            #region n дней

            int.TryParse((ResultBox.FindName("PreviousDaysCountValue") as IntegerUpDown)?.Text, NumberStyles.Integer,
                CultureInfo.InvariantCulture, out var previousDaysCountValue);

            inputParameters.PreviousDaysCount = previousDaysCountValue;

            int.TryParse((ResultBox.FindName("VolumeMoreValue") as IntegerUpDown)?.Text, NumberStyles.Integer,
                CultureInfo.InvariantCulture, out var volumeMoreValue);

            inputParameters.VolumeMore = volumeMoreValue;

            #endregion n дней

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

            #region Цена

            if (ResultBox.FindName("PriceMoreValue") is IntegerUpDown priceMoreValue)
            {
                priceMoreValue.Text = inputParameters.PriceMore.ToString(CultureInfo.InvariantCulture);
                priceMoreValue.IsEnabled = isEnabled;
            }

            if (ResultBox.FindName("PriceLessValue") is IntegerUpDown priceLessValue)
            {
                priceLessValue.Text = inputParameters.PriceLess.ToString(CultureInfo.InvariantCulture);
                priceLessValue.IsEnabled = isEnabled;
            }

            #endregion Цена

            #region Дюрация

            if (ResultBox.FindName("DurationMoreValue") is IntegerUpDown durationMoreValue)
            {
                durationMoreValue.Text = inputParameters.DurationMore.ToString(CultureInfo.InvariantCulture);
                durationMoreValue.IsEnabled = isEnabled;
            }

            if (ResultBox.FindName("DurationLessValue") is IntegerUpDown durationLessValue)
            {
                durationLessValue.Text = inputParameters.DurationLess.ToString(CultureInfo.InvariantCulture);
                durationLessValue.IsEnabled = isEnabled;
            }

            #endregion Дюрация

            #region n дней

            if (ResultBox.FindName("PreviousDaysCountValue") is IntegerUpDown previousDaysCountValue)
            {
                previousDaysCountValue.Text = inputParameters.PreviousDaysCount.ToString(CultureInfo.InvariantCulture);
                previousDaysCountValue.IsEnabled = isEnabled;
            }

            if (ResultBox.FindName("VolumeMoreValue") is IntegerUpDown volumeMoreValue)
            {
                volumeMoreValue.Text = inputParameters.VolumeMore.ToString(CultureInfo.InvariantCulture);
                volumeMoreValue.IsEnabled = isEnabled;
            }

            #endregion n дней
        }
    }
}