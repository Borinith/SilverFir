using System;

namespace SilverFir.MoexClasses
{
    public class BondsResult
    {
        /// <summary>
        ///     Наименование
        /// </summary>
        public string BondName { get; set; }

        /// <summary>
        ///     Налоговые льготы для корпоративных облигаций, выпущенных с 1 января 2017 года
        /// </summary>
        public bool BondTax { get; set; }

        /// <summary>
        ///     Доходность
        /// </summary>
        public decimal BondYield { get; set; }

        /// <summary>
        ///     Объём эмиссии
        /// </summary>
        public decimal IssueVolume { get; set; }

        /// <summary>
        ///     Дата погашения
        /// </summary>
        public DateTime MaturityDate { get; set; }

        /// <summary>
        ///     Состояние выпуска
        /// </summary>
        public bool ReleaseStatus { get; set; }

        /// <summary>
        ///     Код ценной бумаги
        /// </summary>
        public string SecId { get; set; }
    }
}