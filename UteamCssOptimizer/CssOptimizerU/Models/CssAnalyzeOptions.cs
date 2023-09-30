namespace CssOptimizerU.Models
{
    public class CssAnalyzeOptions
    {
        public string PageUrl { get; set; }
        public string[] CssProcessFileNames { get; set; }
        public bool IsProcessAllFiles { get; set; }
    }
}
