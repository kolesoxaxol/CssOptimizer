namespace CssOptimizerU.DM
{
	public class Selector : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public string ConditionText { get; set; }
        public string FullRuleText { get; set; }
        public File File { get; set; }
        public Usage Usage { get; set; }
    }
}
