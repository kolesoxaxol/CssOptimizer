using System;
using System.Collections.Generic;
using System.Text;

namespace CssOptimizerU.DM
{
    public class Usage : BaseEntity
    {
        public int Id { get; set; }
        public string PageUrl { get; set; }
        public int SelectorId { get; set; }
        public string FileName { get; set; }
        public Selector Selector { get; set; }       
    }
}
