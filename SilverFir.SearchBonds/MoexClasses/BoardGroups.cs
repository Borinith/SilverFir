namespace SilverFir.SearchBonds.MoexClasses
{
    public record BoardGroups
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public Security? Securities { get; init; }

        // ReSharper disable once ClassNeverInstantiated.Global
        public record Security : GetMoexData
        {
        }
    }
}