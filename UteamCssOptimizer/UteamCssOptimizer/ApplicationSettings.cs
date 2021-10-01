using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UteamCssOptimizer
{
    public class ApplicationSettings
    {
        public const string ApplicationSettingsSectionName = "ApplicationSettings";

        public string DestinationPath { get; set; }
        public string IgnoreFileCondition { get; set; }
    }
}
