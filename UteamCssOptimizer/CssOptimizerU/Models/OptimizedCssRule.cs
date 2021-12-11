namespace CssOptimizerU.Models
{
    public class OptimizedCssRule
    {
        public string Content { get; set; }
        public string CssRule { get; set; }
        public string ConditionText { get; set; }    
        public int Id { get; set; }
    }
}
