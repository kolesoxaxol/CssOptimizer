using System.Collections.Generic;

namespace CssOptimizerU.Models
{
	public class CssUsingDataModel
    {
        public CssUsingDataModel()
        {
            DocStyles = new List<DocStyle>();
            UsageStyles = new List<DocStyle>();
        }

        public string PageUrl { get; set; }
        public List<DocStyle> DocStyles { get; set; }
        public List<DocStyle> UsageStyles { get; set; }
    }
}
