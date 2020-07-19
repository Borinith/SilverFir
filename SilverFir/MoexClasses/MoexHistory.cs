namespace SilverFir.MoexClasses
{
    public class MoexHistory
    {
        public HistoryData History { get; set; }

        public class HistoryData : GetMoexData
        {
        }
    }
}