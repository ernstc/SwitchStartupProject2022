using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SwitchStartupProject
{
    public class MultiProjectConfiguration
    {
        public MultiProjectConfiguration(string name, bool beginGroup, IList<MultiProjectConfigurationProject> projects, string solutionConfiguration, string solutionPlatform)
        {
            this.Name = name;
            this.BeginGroup = beginGroup;
            this.Projects = projects;
            this.SolutionConfiguration = solutionConfiguration;
            this.SolutionPlatform = solutionPlatform;
        }

        public string Name { get; private set; }
        public bool BeginGroup { get; private set; }
        public IList<MultiProjectConfigurationProject> Projects { get; private set; }
        public string SolutionConfiguration { get; private set; }
        public string SolutionPlatform { get; private set; }
    }
}
