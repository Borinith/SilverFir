namespace SilverFir.MoexClasses
{
    /// <summary>
    ///     Статус финансового инструмента
    /// </summary>
    internal enum SecStatus
    {
        /// <summary>
        ///     Операции разрешены
        /// </summary>
        A,

        /// <summary>
        ///     Операции запрещены
        /// </summary>
        S,

        /// <summary>
        ///     Заблокировано для торгов, разрешено исполнение сделок
        /// </summary>
        N
    }
}