using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SwitchStartupProject
{
    public class Solution
    {
        public Solution()
        {
            this.IsOpening = false;
        }

        public ConfigurationFileTracker ConfigurationFileTracker { get; set; }
        public ConfigurationLoader ConfigurationLoader { get; set; }
        public ActiveConfigurationLoader ActiveConfigurationLoader { get; set; }
        public Configuration Configuration { get; set; }
        public bool IsOpening { get; set; }

    }
}
