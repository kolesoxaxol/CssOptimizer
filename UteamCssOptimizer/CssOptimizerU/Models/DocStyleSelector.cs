namespace CssOptimizerU.Models
{
	public class DocStyleSelector
    {
        public string Name { get; set; }
        public string Content { get; set; }
        public string ConditionText { get; set; }
        public bool IsUsed { get; set; }
        public string FullRuleText { get; set; }
    }
}
