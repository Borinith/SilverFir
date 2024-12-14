using System;

namespace SilverFir.SearchBonds
{
    public class InputParameters
    {
        /// <summary>
        ///     Доходность больше или равна этой цифре
        /// </summary>
        public decimal YieldMore { get; set; } = 15;

        /// <summary>
        ///     Доходность меньше или равна этой цифре
        /// </summary>
        public decimal YieldLess { get; set; } = 25;

        /// <summary>
        ///     Объём эмиссии больше или равен этой цифре
        /// </summary>
        public long IssueVolumeMore { get; set; } = 2_000_000_000;

        /// <summary>
        ///     Дней до погашения больше этой цифры
        /// </summary>
        public int DaysToMaturityMore { get; set; } = 365;

        /// <summary>
        ///     Дней до погашения меньше этой цифры
        /// </summary>
        public int DaysToMaturityLess { get; set; } = 1500;

        /// <summary>
        ///     Дата начала торгов больше или равна этой дате
        /// </summary>
        public DateTime StartDateMoexMore { get; set; } = new(2024, 7, 1);
    }
}