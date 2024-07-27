using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.ProjectSystem.Debug;

namespace SwitchStartupProject
{
    public class WritableLaunchProfile : ILaunchProfile
    {
        public string Name { get; set; }
        public string CommandName { get; set; }
        public string ExecutablePath { get; set; }
        public string CommandLineArgs { get; set; }
        public string WorkingDirectory { get; set; }
        public bool LaunchBrowser { get; set; }
        public string LaunchUrl { get; set; }
        public ImmutableDictionary<string, string> EnvironmentVariables { get; set; }
        public ImmutableDictionary<string, object> OtherSettings { get; set; }

        public WritableLaunchProfile(ILaunchProfile launchProfile)
        {
            Name = launchProfile.Name;
            ExecutablePath = launchProfile.ExecutablePath;
            CommandName = launchProfile.CommandName;
            CommandLineArgs = launchProfile.CommandLineArgs;
            WorkingDirectory = launchProfile.WorkingDirectory;
            LaunchBrowser = launchProfile.LaunchBrowser;
            LaunchUrl = launchProfile.LaunchUrl;
            EnvironmentVariables = launchProfile.EnvironmentVariables;
            OtherSettings = launchProfile.OtherSettings;
        }
    }
}
