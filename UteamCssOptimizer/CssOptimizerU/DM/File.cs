using System.Collections.Generic;

namespace CssOptimizerU.DM
{
	public class File : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Selector> Selectors { get; set; }
        public List<Usage> Usages { get; set; }
    }
}
