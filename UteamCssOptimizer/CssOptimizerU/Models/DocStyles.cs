using System;
using System.Collections.Generic;
using System.Text;

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
