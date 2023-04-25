namespace SilverFir.LanguageService
{
    public class HebLanguageService : ILanguageService
    {
        public string ClearButtonText => "נקה";

        public string ConnectionErrorText => "שגיאת חיבור";

        public string DaysToMaturityLessParsingErrorText => "\"ימים לפדיון פחות מ\" שגיאת ניתוח ערך";

        public string DaysToMaturityLessText => "ימים לפדיון\n:פחות מ";

        public string DaysToMaturityMoreParsingErrorText => "\"ימים לפדיון יותר מ\" שגיאת ניתוח ערך";

        public string DaysToMaturityMoreText => "ימים לפדיון\n:יותר מ";

        public string GetBondsButtonText => "קבל איגרות חוב";

        public string IncorrectDaysToMaturityValuesText => "ערכים שגויים של מספר הימים לפדיון";

        public string IncorrectYieldValuesText => "ערכי תשואה שגויים";

        public string IssueVolumeMoreParsingErrorText => "\"נפח הנושא הוא יותר מ\" שגיאת ניתוח ערך";

        public string IssueVolumeMoreText => "נפח הנושא הוא\n:יותר מ";

        public string NoBondsForSelectedParametersText => "אין אגרות חוב לפרמטרים נבחרים";

        public string YieldLessParsingErrorText => "\"התשואה היא פחות מ\" שגיאת ניתוח ערך";

        public string YieldLessText => "התשואה היא\n:פחות מ";

        public string YieldMoreParsingErrorText => "\"התשואה היא יותר מ\" שגיאת ניתוח ערך";

        public string YieldMoreText => "התשואה היא\n:יותר מ";
    }
}