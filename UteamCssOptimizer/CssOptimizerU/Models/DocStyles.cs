using System.Collections.Generic;

namespace CssOptimizerU.Models
{
	public class DocStyle
    {
        public DocStyle()
        {
            Selectors = new List<DocStyleSelector>();
        }
        public string FileName { get; set; }

        public List<DocStyleSelector> Selectors { get; set; }
    }
}
