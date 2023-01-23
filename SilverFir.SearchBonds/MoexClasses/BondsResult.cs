using System;

namespace SilverFir.SearchBonds.MoexClasses
{
    public record BondsResult
    {
        /// <summary>
        ///     Наименование
        /// </summary>
        public string? BondName { get; init; }

        /// <summary>
        ///     Доходность
        /// </summary>
        public decimal BondYield { get; init; }

        /// <summary>
        ///     Объём эмиссии
        /// </summary>
        public decimal IssueVolume { get; init; }

        /// <summary>
        ///     Дата погашения
        /// </summary>
        public DateTime MaturityDate { get; init; }

        /// <summary>
        ///     Состояние выпуска
        /// </summary>
        public bool ReleaseStatus { get; init; }

        /// <summary>
        ///     Код ценной бумаги
        /// </summary>
        public string? SecId { get; init; }
    }
}