namespace SilverFir.MoexClasses
{
    public class BoardGroups
    {
        public Market MarketData { get; set; }

        public Security Securities { get; set; }

        public class Market : GetMoexData
        {
        }

        public class Security : GetMoexData
        {
        }
    }
}