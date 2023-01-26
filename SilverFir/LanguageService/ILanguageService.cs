namespace SilverFir.LanguageService
{
    public interface ILanguageService
    {
        string ClearButtonText { get; }

        string ConnectionErrorText { get; }

        string DaysToMaturityLessParsingErrorText { get; }

        string DaysToMaturityLessText { get; }

        string DaysToMaturityMoreParsingErrorText { get; }

        string DaysToMaturityMoreText { get; }

        string GetBondsButtonText { get; }

        string IncorrectDaysToMaturityValuesText { get; }

        string IncorrectYieldValuesText { get; }

        string IssueVolumeMoreParsingErrorText { get; }

        string IssueVolumeMoreText { get; }

        string NoBondsForSelectedParametersText { get; }

        string YieldLessParsingErrorText { get; }

        string YieldLessText { get; }

        string YieldMoreParsingErrorText { get; }

        string YieldMoreText { get; }
    }
}