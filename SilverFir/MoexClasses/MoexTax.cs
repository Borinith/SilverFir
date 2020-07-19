namespace SilverFir.MoexClasses
{
    public class MoexTax
    {
        public DescriptionData Description { get; set; }

        public class DescriptionData : GetMoexData
        {
        }
    }
}