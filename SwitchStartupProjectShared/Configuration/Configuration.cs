using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SwitchStartupProject
{
    public class Configuration
    {
        public bool ListAllProjects { get; private set; }
        public IList<MultiProjectConfiguration> MultiProjectConfigurations { get; private set; }

        public Configuration(bool listAllProjects, IList<MultiProjectConfiguration> multiProjectConfigurations)
        {
            this.ListAllProjects = listAllProjects;
            this.MultiProjectConfigurations = multiProjectConfigurations;
        }
    }
}
