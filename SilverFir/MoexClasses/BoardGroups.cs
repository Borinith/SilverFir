namespace SilverFir.MoexClasses
{
    public class BoardGroups
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public Market MarketData { get; set; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public Security Securities { get; set; }

        // ReSharper disable once ClassNeverInstantiated.Global
        public class Market : GetMoexData
        {
        }

        // ReSharper disable once ClassNeverInstantiated.Global
        public class Security : GetMoexData
        {
        }
    }
}