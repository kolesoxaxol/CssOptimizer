using System;
using System.Collections.Generic;
using System.Text;

namespace CssOptimizerU.DM
{
    public class Selector : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public string GroupId { get; set; }
        public string MediaQuery { get; set; }
        public int FileId { get; set; }
        public File File { get; set; }
        public Usage Usage { get; set; }
    }
}
