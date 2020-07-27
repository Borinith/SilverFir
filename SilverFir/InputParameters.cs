namespace SilverFir
{
    public class InputParameters
    {
        /// <summary>
        ///     Доходность больше этой цифры
        /// </summary>
        public decimal YieldMore { get; set; } = 7;

        /// <summary>
        ///     Доходность меньше этой цифры
        /// </summary>
        public decimal YieldLess { get; set; } = 14;

        /// <summary>
        ///     Цена больше этой цифры
        /// </summary>
        public decimal PriceMore { get; set; } = 95;

        /// <summary>
        ///     Цена меньше этой цифры
        /// </summary>
        public decimal PriceLess { get; set; } = 101;

        /// <summary>
        ///     Дюрация больше этой цифры
        /// </summary>
        public decimal DurationMore { get; set; } = 1;

        /// <summary>
        ///     Дюрация меньше этой цифры
        /// </summary>
        public decimal DurationLess { get; set; } = 6;

        /// <summary>
        ///     Поиск за последние n дней
        /// </summary>
        public int PreviousDaysCount { get; set; } = 15;

        /// <summary>
        ///     Объем сделок за n дней, шт. больше этой цифры
        /// </summary>
        public decimal VolumeMore { get; set; } = 5000;
    }
}