namespace SilverFir.MoexClasses
{
    public class BondsResult
    {
        public string BondName { get; set; }

        public string SecId { get; set; }

        public decimal BondPrice { get; set; }

        public decimal BondVolume { get; set; }

        public decimal BondYield { get; set; }

        public decimal BondDuration { get; set; }

        public bool BondTax { get; set; }
    }
}