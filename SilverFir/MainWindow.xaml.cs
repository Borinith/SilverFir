using SilverFir.LanguageService;
using SilverFir.SearchBonds;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Xceed.Wpf.Toolkit;

namespace SilverFir
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private const string CANNOT_LOAD_LANGUAGE = "Cannot load language";
        private const string LANGUAGE_IMAGES_FOLDER = "LanguageImages";
        private readonly HashSet<string> _constants = new(9);
        private readonly FrozenDictionary<LanguageEnum, string> _imagePaths;
        private readonly ProxyLanguage.ProxyLanguageResolver _resolver;
        private readonly ISearchBonds _searchBonds;
        private LanguageEnum _currentLanguage = LanguageEnum.English;
        private ILanguageService _languageService = null!;
        private TextAlignment _textAlignment = TextAlignment.Left;

        /// <summary>
        ///     Main window
        /// </summary>
        public MainWindow(ProxyLanguage.ProxyLanguageResolver resolver, ISearchBonds searchBonds)
        {
            _resolver = resolver;
            _searchBonds = searchBonds;

            _imagePaths = Enum.GetValues<LanguageEnum>().ToFrozenDictionary(language => language, language => $"{language}.png");

            UpdateLanguage();

            InitializeComponent();
            ChildrenClear();

            DrawMainWindow();
            SearchParameters(new InputParameters(), true);
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

                        if (errors.All(x => x.Value == false))
                        {
                            try
                            {
                                result.Text = await SearchBondsResult(newInputParameters);

                                result.TextAlignment = result.Text == _languageService.NoBondsForSelectedParametersText
                                    ? _textAlignment
                                    : TextAlignment.Left;
                            }
                            catch (Exception)
                            {
                                result.Text = _languageService.ConnectionErrorText;
                                result.TextAlignment = _textAlignment;
                            }
                        }
                        else
                        {
                            result.Text = string.Join("\n", errors.Where(x => x.Value).Select(x => x.Key));
                            result.TextAlignment = _textAlignment;
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

        /// <summary>
        ///     Очищаем поле
        /// </summary>
        private void ChildrenClear()
        {
            CommonWindow.Children.Clear();
            CommonWindow.RowDefinitions.Clear();
            CommonWindow.ColumnDefinitions.Clear();

            var constants = new List<string>(_constants);

            foreach (var constant in constants)
            {
                UnregisterName(constant);
                _constants.Remove(constant);
            }
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

            RegisterNameCustom(RegisterNames.OUTPUT_BOX, outputBox);

            #endregion Output box

            #region Yield more

            var yieldMore = new Label
            {
                Content = new TextBlock
                {
                    Text = _languageService.YieldMoreText,
                    TextAlignment = _textAlignment
                },
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
                Width = 100
            };

            Grid.SetRow(yieldMoreValue, 0);
            Grid.SetColumn(yieldMoreValue, 1);

            CommonWindow.Children.Add(yieldMoreValue);

            RegisterNameCustom(RegisterNames.YIELD_MORE_VALUE, yieldMoreValue);

            #endregion Yield more

            #region Yield less

            var yieldLess = new Label
            {
                Content = new TextBlock
                {
                    Text = _languageService.YieldLessText,
                    TextAlignment = _textAlignment
                },
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
                Width = 100
            };

            Grid.SetRow(yieldLessValue, 0);
            Grid.SetColumn(yieldLessValue, 3);

            CommonWindow.Children.Add(yieldLessValue);

            RegisterNameCustom(RegisterNames.YIELD_LESS_VALUE, yieldLessValue);

            #endregion Yield less

            #region Issue volume more

            var issueVolumeMore = new Label
            {
                Content = new TextBlock
                {
                    Text = _languageService.IssueVolumeMoreText,
                    TextAlignment = _textAlignment
                },
                HorizontalAlignment = HorizontalAlignment.Right,
                HorizontalContentAlignment = HorizontalAlignment.Right,
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
                Width = 100
            };

            Grid.SetRow(issueVolumeMoreValue, 0);
            Grid.SetColumn(issueVolumeMoreValue, 5);

            CommonWindow.Children.Add(issueVolumeMoreValue);

            RegisterNameCustom(RegisterNames.ISSUE_VOLUME_MORE_VALUE, issueVolumeMoreValue);

            #endregion Issue volume more

            #region Days to maturity more

            var daysToMaturityMore = new Label
            {
                Content = new TextBlock
                {
                    Text = _languageService.DaysToMaturityMoreText,
                    TextAlignment = _textAlignment
                },
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
                Width = 100
            };

            Grid.SetRow(daysToMaturityMoreValue, 1);
            Grid.SetColumn(daysToMaturityMoreValue, 1);

            CommonWindow.Children.Add(daysToMaturityMoreValue);

            RegisterNameCustom(RegisterNames.DAYS_TO_MATURITY_MORE_VALUE, daysToMaturityMoreValue);

            #endregion Days to maturity more

            #region Days to maturity less

            var daysToMaturityLess = new Label
            {
                Content = new TextBlock
                {
                    Text = _languageService.DaysToMaturityLessText,
                    TextAlignment = _textAlignment
                },
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
                Width = 100
            };

            Grid.SetRow(daysToMaturityLessValue, 1);
            Grid.SetColumn(daysToMaturityLessValue, 3);

            CommonWindow.Children.Add(daysToMaturityLessValue);

            RegisterNameCustom(RegisterNames.DAYS_TO_MATURITY_LESS_VALUE, daysToMaturityLessValue);

            #endregion Days to maturity less

            #region Date of listing more

            var startDateMoexMore = new Label
            {
                Content = new TextBlock
                {
                    Text = _languageService.StartDateMoexMoreText,
                    TextAlignment = _textAlignment
                },
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center
            };

            Grid.SetRow(startDateMoexMore, 1);
            Grid.SetColumn(startDateMoexMore, 4);

            CommonWindow.Children.Add(startDateMoexMore);

            var startDateMoexMoreValue = new DatePicker
            {
                DisplayDateStart = new DateTime(2000, 1, 1),
                FirstDayOfWeek = DayOfWeek.Monday,
                HorizontalAlignment = HorizontalAlignment.Left,
                Name = RegisterNames.START_DATE_MOEX_MORE_VALUE,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 100
            };

            Grid.SetRow(startDateMoexMoreValue, 1);
            Grid.SetColumn(startDateMoexMoreValue, 5);

            CommonWindow.Children.Add(startDateMoexMoreValue);

            RegisterNameCustom(RegisterNames.START_DATE_MOEX_MORE_VALUE, startDateMoexMoreValue);

            #endregion Date of listing more

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

            RegisterNameCustom(RegisterNames.GET_BONDS, getBondsButton);

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

            RegisterNameCustom(RegisterNames.CLEAR, clearButton);

            #endregion Clear button

            #region Create update language button

            var img = new Image
            {
                Source = new BitmapImage(new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LANGUAGE_IMAGES_FOLDER, _imagePaths[_currentLanguage]), UriKind.RelativeOrAbsolute))
            };

            var stackPnl = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };

            stackPnl.Children.Add(img);

            var createUpdateLanguageButton = new Button
            {
                Content = stackPnl,
                Height = 30,
                HorizontalAlignment = HorizontalAlignment.Center,
                Name = RegisterNames.UPDATE_LANGUAGE,
                Tag = _currentLanguage,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 30
            };

            Grid.SetRow(createUpdateLanguageButton, 6);
            Grid.SetColumn(createUpdateLanguageButton, 0);

            createUpdateLanguageButton.Click += UpdateLanguageButtonClick;
            CommonWindow.Children.Add(createUpdateLanguageButton);

            RegisterNameCustom(RegisterNames.UPDATE_LANGUAGE, createUpdateLanguageButton);

            #endregion Create update language button
        }

        private Dictionary<string, bool> ErrorsInputParameters(InputParameters inputParameters)
        {
            var errors = new Dictionary<string, bool>
            {
                {
                    _languageService.IncorrectYieldValuesText, inputParameters.YieldMore > inputParameters.YieldLess
                },
                {
                    _languageService.IncorrectDaysToMaturityValuesText, inputParameters.DaysToMaturityMore > inputParameters.DaysToMaturityLess
                },
                {
                    _languageService.IncorrectStartDateMoexText, inputParameters.StartDateMoexMore > DateTime.Today
                }
            };

            return errors;
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

            var isIssueVolumeMoreParsed = long.TryParse((CommonWindow.FindName(RegisterNames.ISSUE_VOLUME_MORE_VALUE) as IntegerUpDown)?.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var issueVolumeMoreValue);

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

            #region Дата начала торгов

            var startDateMoexMoreParsed = (CommonWindow.FindName(RegisterNames.START_DATE_MOEX_MORE_VALUE) as DatePicker)?.SelectedDate;

            if (startDateMoexMoreParsed is null || startDateMoexMoreParsed == new DateTime(1, 1, 1))
            {
                errors.Add(_languageService.StartDateMoexMoreParsingErrorText);
                inputParameters.StartDateMoexMore = new DateTime(2000, 1, 1);
            }
            else
            {
                inputParameters.StartDateMoexMore = startDateMoexMoreParsed.Value;
            }

            #endregion Дата начала торгов

            if (errors.Any())
            {
                throw new Exception(string.Join("\n", errors));
            }

            return inputParameters;
        }

        /// <summary>
        ///     Регистрируем имя и добавляем имя в кэш
        /// </summary>
        /// <param name="name"></param>
        /// <param name="scopedElement"></param>
        private void RegisterNameCustom(string name, object scopedElement)
        {
            RegisterName(name, scopedElement);
            _constants.Add(name);
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

            #region Дата начала торгов

            if (CommonWindow.FindName(RegisterNames.START_DATE_MOEX_MORE_VALUE) is DatePicker startDateMoexMoreValue)
            {
                startDateMoexMoreValue.DisplayDate = inputParameters.StartDateMoexMore;
                startDateMoexMoreValue.SelectedDate = inputParameters.StartDateMoexMore;
                startDateMoexMoreValue.IsEnabled = isEnabled;
            }

            # endregion Дата начала торгов

            #region Кнопки

            if (CommonWindow.FindName(RegisterNames.GET_BONDS) is Button getBondsButton)
            {
                getBondsButton.IsEnabled = isEnabled;
            }

            if (CommonWindow.FindName(RegisterNames.CLEAR) is Button clearButton)
            {
                clearButton.IsEnabled = isEnabled;
            }

            if (CommonWindow.FindName(RegisterNames.UPDATE_LANGUAGE) is Button updateLanguageButton)
            {
                updateLanguageButton.IsEnabled = isEnabled;
            }

            #endregion Кнопки
        }

        private void UpdateLanguage()
        {
            var languageService = _resolver(_currentLanguage);

            _languageService = languageService ?? throw new Exception(CANNOT_LOAD_LANGUAGE);
            _textAlignment = _currentLanguage == LanguageEnum.Hebrew ? TextAlignment.Right : TextAlignment.Left;
        }

        private void UpdateLanguageButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                var isParsedCurrentLanguage = Enum.TryParse<LanguageEnum>(button.Tag.ToString(), out var currentLanguage);

                if (isParsedCurrentLanguage == false)
                {
                    throw new Exception(CANNOT_LOAD_LANGUAGE);
                }

                var previousErrorsInput = ErrorsInputParameters(NewInputParameters());
                var noBondsOld = _languageService.NoBondsForSelectedParametersText;
                var connectionErrorOld = _languageService.ConnectionErrorText;

                var allLanguages = Enum.GetValues<LanguageEnum>();
                var currentLanguageIndex = Array.IndexOf(allLanguages, currentLanguage);
                var nextLanguageIndex = (currentLanguageIndex + 1) % allLanguages.Length;

                _currentLanguage = (LanguageEnum)nextLanguageIndex;
                UpdateLanguage();

                var previousErrorsSearchBonds = new Dictionary<string, string>
                {
                    {
                        noBondsOld, _languageService.NoBondsForSelectedParametersText
                    },
                    {
                        connectionErrorOld, _languageService.ConnectionErrorText
                    }
                };

                var inputParameters = NewInputParameters();
                var textBoxOld = (CommonWindow.FindName(RegisterNames.OUTPUT_BOX) as TextBox)?.Text ?? string.Empty;

                ChildrenClear();

                DrawMainWindow();
                SearchParameters(inputParameters, true);

                if (CommonWindow.FindName(RegisterNames.OUTPUT_BOX) is TextBox textBoxNew)
                {
                    if (textBoxOld != string.Empty)
                    {
                        var errors = ErrorsInputParameters(inputParameters);

                        if (errors.Any(x => x.Value))
                        {
                            textBoxNew.Text = string.Join("\n", errors.Where(x => x.Value).Select(x => x.Key));
                            textBoxNew.TextAlignment = _textAlignment;
                        }
                        else if (previousErrorsInput.Select(x => x.Key).Any(textBoxOld.Contains))
                        {
                            textBoxNew.Text = string.Empty;
                        }
                        else if (previousErrorsSearchBonds.TryGetValue(textBoxOld, out var errorSearchBonds))
                        {
                            textBoxNew.Text = errorSearchBonds;
                            textBoxNew.TextAlignment = _textAlignment;
                        }
                        else
                        {
                            textBoxNew.Text = textBoxOld;
                        }
                    }
                    else
                    {
                        textBoxNew.Text = string.Empty;
                    }
                }
            }
        }
    }
}