namespace SilverFir.SearchBonds.MoexClasses
{
    /// <summary>
    ///     Статус финансового инструмента
    /// </summary>
    internal enum SecStatus
    {
        /// <summary>
        ///     Операции разрешены
        /// </summary>
        A = 0,

        /// <summary>
        ///     Операции запрещены
        /// </summary>
        S = 1,

        /// <summary>
        ///     Заблокировано для торгов, разрешено исполнение сделок
        /// </summary>
        N = 2
    }
}