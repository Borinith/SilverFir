using System;

namespace SilverFir.SearchBonds.MoexClasses
{
    /// <summary>
    ///     Ценная бумага
    /// </summary>
    /// <param name="BondName">Наименование</param>
    /// <param name="BondYield">Доходность</param>
    /// <param name="IssueVolume">Объём эмиссии</param>
    /// <param name="MaturityDate">Дата погашения</param>
    /// <param name="SecId">Код ценной бумаги</param>
    public readonly record struct BondResult(
        string? BondName,
        decimal BondYield,
        long IssueVolume,
        DateTime MaturityDate,
        string? SecId);
}