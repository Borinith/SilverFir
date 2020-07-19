namespace SilverFir.MoexClasses
{
    public class MoexBoards
    {
        public BoardData Boards { get; set; }

        public class BoardData : GetMoexData
        {
        }
    }
}