namespace SilverFir.LanguageService
{
    public class RusLanguageService : ILanguageService
    {
        public string ClearButtonText => "Очистить";

        public string ConnectionErrorText => "Ошибка подключения";

        public string DaysToMaturityLessParsingErrorText => "Ошибка парсинга значения \"Дней до погашения меньше, чем\"";

        public string DaysToMaturityLessText => "Дней до\nпогашения\nменьше, чем:";

        public string DaysToMaturityMoreParsingErrorText => "Ошибка парсинга значения \"Дней до погашения больше, чем\"";

        public string DaysToMaturityMoreText => "Дней до\nпогашения\nбольше, чем:";

        public string GetBondsButtonText => "Получить облигации";

        public string IncorrectDaysToMaturityValuesText => "Неверные значения количества дней до погашения";

        public string IncorrectStartDateMoexText => "Дата начала торгов должна быть больше текущего дня";

        public string IncorrectYieldValuesText => "Неверные значения доходности";

        public string IssueVolumeMoreParsingErrorText => "Ошибка парсинга значения \"Объём эмиссии больше, чем\"";

        public string IssueVolumeMoreText => "Объём эмиссии\nбольше, чем:";

        public string NoBondsForSelectedParametersText => "Нет облигаций для выбранных параметров";

        public string StartDateMoexMoreParsingErrorText => "Ошибка парсинга значения \nДата начала торгов больше, чем\n";

        public string StartDateMoexMoreText => "Дата начала торгов\nбольше, чем:";

        public string YieldLessParsingErrorText => "Ошибка парсинга значения \"Доходность меньше, чем\"";

        public string YieldLessText => "Доходность\nменьше, чем:";

        public string YieldMoreParsingErrorText => "Ошибка парсинга значения \"Доходность больше, чем\"";

        public string YieldMoreText => "Доходность\nбольше, чем:";
    }
}