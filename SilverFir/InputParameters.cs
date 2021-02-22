namespace SilverFir
{
    public class InputParameters
    {
        /// <summary>
        ///     Доходность больше этой цифры
        /// </summary>
        public decimal YieldMore { get; set; } = 8;

        /// <summary>
        ///     Доходность меньше этой цифры
        /// </summary>
        public decimal YieldLess { get; set; } = 12;

        /// <summary>
        ///     Объём эмиссии больше этой цифры
        /// </summary>
        public decimal IssueVolumeMore { get; set; } = 2_000_000_000;

        /// <summary>
        ///     Дней до погашения больше этой цифры
        /// </summary>
        public int DaysToMaturityMore { get; set; } = 700;

        /// <summary>
        ///     Дней до погашения меньше этой цифры
        /// </summary>
        public int DaysToMaturityLess { get; set; } = 1500;
    }
}