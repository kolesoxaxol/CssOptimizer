using System;
using System.Collections.Generic;
using System.Text;

namespace CssOptimizerU.DM
{
    public class File : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Selector Selector { get; set; }
    }
}
