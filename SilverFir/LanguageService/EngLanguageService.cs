namespace SilverFir.LanguageService
{
    public class EngLanguageService : ILanguageService
    {
        public string ClearButtonText => "Clear";

        public string ConnectionErrorText => "Connection error";

        public string DaysToMaturityLessParsingErrorText => "Parsing error of \"Days to maturity less than\" value";

        public string DaysToMaturityLessText => "Days to maturity\nless than:";

        public string DaysToMaturityMoreParsingErrorText => "Parsing error of \"Days to maturity more than\" value";

        public string DaysToMaturityMoreText => "Days to maturity\nmore than:";

        public string GetBondsButtonText => "Get bonds";

        public string IncorrectDaysToMaturityValuesText => "Incorrect days to maturity values";

        public string IncorrectYieldValuesText => "Incorrect yield values";

        public string IssueVolumeMoreParsingErrorText => "Parsing error of \"Issue volume is more than\" value";

        public string IssueVolumeMoreText => "Issue volume is\nmore than:";

        public string NoBondsForSelectedParametersText => "No bonds for selected parameters";

        public string YieldLessParsingErrorText => "Parsing error of \"Yield is less than\" value";

        public string YieldLessText => "Yield is less\nthan:";

        public string YieldMoreParsingErrorText => "Parsing error of \"Yield is more than\" value";

        public string YieldMoreText => "Yield is more\nthan:";
    }
}