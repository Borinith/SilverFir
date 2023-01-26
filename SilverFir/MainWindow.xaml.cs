using SilverFir.LanguageService;
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
        private readonly ILanguageService _languageService;
        private readonly ISearchBonds _searchBonds;

        /// <summary>
        ///     Main window
        /// </summary>
        public MainWindow(ILanguageService languageService, ISearchBonds searchBonds)
        {
            _languageService = languageService;
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
                Name = RegisterNames.OUTPUT_BOX,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };

            Grid.SetRow(outputBox, 2);
            Grid.SetColumn(outputBox, 1);
            Grid.SetRowSpan(outputBox, 4);
            Grid.SetColumnSpan(outputBox, 4);

            CommonWindow.Children.Add(outputBox);

            RegisterName(RegisterNames.OUTPUT_BOX, outputBox);

            #endregion Output box

            #region Yield more

            var yieldMore = new Label
            {
                Content = _languageService.YieldMoreText,
                HorizontalAlignment = HorizontalAlignment.Right,
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
                Name = RegisterNames.YIELD_MORE_VALUE,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 90
            };

            Grid.SetRow(yieldMoreValue, 0);
            Grid.SetColumn(yieldMoreValue, 1);

            CommonWindow.Children.Add(yieldMoreValue);

            RegisterName(RegisterNames.YIELD_MORE_VALUE, yieldMoreValue);

            #endregion Yield more

            #region Yield less

            var yieldLess = new Label
            {
                Content = _languageService.YieldLessText,
                HorizontalAlignment = HorizontalAlignment.Right,
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
                Name = RegisterNames.YIELD_LESS_VALUE,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 90
            };

            Grid.SetRow(yieldLessValue, 0);
            Grid.SetColumn(yieldLessValue, 3);

            CommonWindow.Children.Add(yieldLessValue);

            RegisterName(RegisterNames.YIELD_LESS_VALUE, yieldLessValue);

            #endregion Yield less

            #region Issue volume more

            var issueVolumeMore = new Label
            {
                Content = _languageService.IssueVolumeMoreText,
                HorizontalAlignment = HorizontalAlignment.Right,
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
                Name = RegisterNames.ISSUE_VOLUME_MORE_VALUE,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 90
            };

            Grid.SetRow(issueVolumeMoreValue, 0);
            Grid.SetColumn(issueVolumeMoreValue, 5);

            CommonWindow.Children.Add(issueVolumeMoreValue);

            RegisterName(RegisterNames.ISSUE_VOLUME_MORE_VALUE, issueVolumeMoreValue);

            #endregion Issue volume more

            #region Days to maturity more

            var daysToMaturityMore = new Label
            {
                Content = _languageService.DaysToMaturityMoreText,
                HorizontalAlignment = HorizontalAlignment.Right,
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
                Name = RegisterNames.DAYS_TO_MATURITY_MORE_VALUE,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 90
            };

            Grid.SetRow(daysToMaturityMoreValue, 1);
            Grid.SetColumn(daysToMaturityMoreValue, 1);

            CommonWindow.Children.Add(daysToMaturityMoreValue);

            RegisterName(RegisterNames.DAYS_TO_MATURITY_MORE_VALUE, daysToMaturityMoreValue);

            #endregion Days to maturity more

            #region Days to maturity less

            var daysToMaturityLess = new Label
            {
                Content = _languageService.DaysToMaturityLessText,
                HorizontalAlignment = HorizontalAlignment.Right,
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
                Name = RegisterNames.DAYS_TO_MATURITY_LESS_VALUE,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 90
            };

            Grid.SetRow(daysToMaturityLessValue, 1);
            Grid.SetColumn(daysToMaturityLessValue, 3);

            CommonWindow.Children.Add(daysToMaturityLessValue);

            RegisterName(RegisterNames.DAYS_TO_MATURITY_LESS_VALUE, daysToMaturityLessValue);

            #endregion Days to maturity less

            #region Get bonds button

            var getBondsButton = new Button
            {
                Content = _languageService.GetBondsButtonText,
                Height = 30,
                HorizontalAlignment = HorizontalAlignment.Center,
                Name = RegisterNames.GET_BONDS,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 120
            };

            Grid.SetRow(getBondsButton, 6);
            Grid.SetColumn(getBondsButton, 1);
            Grid.SetColumnSpan(getBondsButton, 2);

            getBondsButton.Click += ButtonClickAsync;
            CommonWindow.Children.Add(getBondsButton);

            RegisterName(RegisterNames.GET_BONDS, getBondsButton);

            #endregion Get bonds button

            #region Clear button

            var clearButton = new Button
            {
                Content = _languageService.ClearButtonText,
                Height = 30,
                HorizontalAlignment = HorizontalAlignment.Center,
                Name = RegisterNames.CLEAR,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 120
            };

            Grid.SetRow(clearButton, 6);
            Grid.SetColumn(clearButton, 3);
            Grid.SetColumnSpan(clearButton, 2);

            clearButton.Click += ButtonClickAsync;
            CommonWindow.Children.Add(clearButton);

            RegisterName(RegisterNames.CLEAR, clearButton);

            #endregion Clear button
        }

        private async void ButtonClickAsync(object sender, RoutedEventArgs e)
        {
            if (sender is Button senderButton && CommonWindow.FindName(RegisterNames.OUTPUT_BOX) is TextBox result)
            {
                InputParameters newInputParameters;

                try
                {
                    newInputParameters = NewInputParameters();
                }
                catch (Exception ex)
                {
                    result.Text = ex.Message;

                    return;
                }

                switch (senderButton.Name)
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
                                result.Text = _languageService.ConnectionErrorText;
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
            var errors = new List<string>();

            #region Доходность

            var isYieldMoreParsed = int.TryParse((CommonWindow.FindName(RegisterNames.YIELD_MORE_VALUE) as IntegerUpDown)?.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var yieldMoreValue);

            if (!isYieldMoreParsed)
            {
                errors.Add(_languageService.YieldMoreParsingErrorText);
            }

            inputParameters.YieldMore = yieldMoreValue;

            var isYieldLessParsed = int.TryParse((CommonWindow.FindName(RegisterNames.YIELD_LESS_VALUE) as IntegerUpDown)?.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var yieldLessValue);

            if (!isYieldLessParsed)
            {
                errors.Add(_languageService.YieldLessParsingErrorText);
            }

            inputParameters.YieldLess = yieldLessValue;

            #endregion Доходность

            #region Объём эмиссии

            var isIssueVolumeMoreParsed = int.TryParse((CommonWindow.FindName(RegisterNames.ISSUE_VOLUME_MORE_VALUE) as IntegerUpDown)?.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var issueVolumeMoreValue);

            if (!isIssueVolumeMoreParsed)
            {
                errors.Add(_languageService.IssueVolumeMoreParsingErrorText);
            }

            inputParameters.IssueVolumeMore = issueVolumeMoreValue;

            #endregion Объём эмиссии

            #region Количество дней до погашения

            var isDaysToMaturityMoreParsed = int.TryParse((CommonWindow.FindName(RegisterNames.DAYS_TO_MATURITY_MORE_VALUE) as IntegerUpDown)?.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var daysToMaturityMoreValue);

            if (!isDaysToMaturityMoreParsed)
            {
                errors.Add(_languageService.DaysToMaturityMoreParsingErrorText);
            }

            inputParameters.DaysToMaturityMore = daysToMaturityMoreValue;

            var isDaysToMaturityLessParsed = int.TryParse((CommonWindow.FindName(RegisterNames.DAYS_TO_MATURITY_LESS_VALUE) as IntegerUpDown)?.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var daysToMaturityLessValue);

            if (!isDaysToMaturityLessParsed)
            {
                errors.Add(_languageService.DaysToMaturityLessParsingErrorText);
            }

            inputParameters.DaysToMaturityLess = daysToMaturityLessValue;

            #endregion Количество дней до погашения

            if (errors.Any())
            {
                throw new Exception(string.Join("\n", errors));
            }

            return inputParameters;
        }

        private void SearchParameters(InputParameters inputParameters, bool isEnabled)
        {
            #region Доходность

            if (CommonWindow.FindName(RegisterNames.YIELD_MORE_VALUE) is IntegerUpDown yieldMoreValue)
            {
                yieldMoreValue.Text = inputParameters.YieldMore.ToString(CultureInfo.InvariantCulture);
                yieldMoreValue.IsEnabled = isEnabled;
            }

            if (CommonWindow.FindName(RegisterNames.YIELD_LESS_VALUE) is IntegerUpDown yieldLessValue)
            {
                yieldLessValue.Text = inputParameters.YieldLess.ToString(CultureInfo.InvariantCulture);
                yieldLessValue.IsEnabled = isEnabled;
            }

            #endregion Доходность

            #region Объём эмиссии

            if (CommonWindow.FindName(RegisterNames.ISSUE_VOLUME_MORE_VALUE) is IntegerUpDown issueVolumeMoreValue)
            {
                issueVolumeMoreValue.Text = inputParameters.IssueVolumeMore.ToString(CultureInfo.InvariantCulture);
                issueVolumeMoreValue.IsEnabled = isEnabled;
            }

            #endregion Объём эмиссии

            #region Количество дней до погашения

            if (CommonWindow.FindName(RegisterNames.DAYS_TO_MATURITY_MORE_VALUE) is IntegerUpDown daysToMaturityMoreValue)
            {
                daysToMaturityMoreValue.Text = inputParameters.DaysToMaturityMore.ToString(CultureInfo.InvariantCulture);
                daysToMaturityMoreValue.IsEnabled = isEnabled;
            }

            if (CommonWindow.FindName(RegisterNames.DAYS_TO_MATURITY_LESS_VALUE) is IntegerUpDown daysToMaturityLessValue)
            {
                daysToMaturityLessValue.Text = inputParameters.DaysToMaturityLess.ToString(CultureInfo.InvariantCulture);
                daysToMaturityLessValue.IsEnabled = isEnabled;
            }

            #endregion Количество дней до погашения

            #region Кнопки

            if (CommonWindow.FindName(RegisterNames.GET_BONDS) is Button getBondsButton)
            {
                getBondsButton.IsEnabled = isEnabled;
            }

            if (CommonWindow.FindName(RegisterNames.CLEAR) is Button clearButton)
            {
                clearButton.IsEnabled = isEnabled;
            }

            #endregion Кнопки
        }

        private List<string> ErrorsInputParameters(InputParameters inputParameters)
        {
            var errors = new List<string>();

            if (inputParameters.YieldMore > inputParameters.YieldLess)
            {
                errors.Add(_languageService.IncorrectYieldValuesText);
            }

            if (inputParameters.DaysToMaturityMore > inputParameters.DaysToMaturityLess)
            {
                errors.Add(_languageService.IncorrectDaysToMaturityValuesText);
            }

            return errors;
        }

        private async Task<string> SearchBondsResult(InputParameters inputParameters)
        {
            var bonds = await _searchBonds.MoexSearchBonds(inputParameters);

            return bonds.Any()
                ? string.Join("\n", bonds.Select(x => (x.SecId ?? string.Empty) +
                                                      "\t   " +
                                                      (x.BondName ?? string.Empty) +
                                                      "\t   " +
                                                      x.MaturityDate.ToString("dd.MM.yyyy") +
                                                      "\t  " +
                                                      x.BondYield +
                                                      "\t   " +
                                                      x.IssueVolume))
                : _languageService.NoBondsForSelectedParametersText;
        }
    }
}