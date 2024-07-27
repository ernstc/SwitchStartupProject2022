using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SwitchStartupProject
{
    public class MultiProjectConfigurationProject
    {
        public MultiProjectConfigurationProject(
            string nameOrPath,
            string commandLineArguments,
            string workingDirectory,
            bool? startProject,
            string startExternalProgram,
            string startBrowserWithUrl,
            bool? enableRemoteDebugging,
            string remoteDebuggingMachine,
            string profileName,
            string targetFramwork)
        {
            this.NameOrPath = nameOrPath;
            this.CommandLineArguments = commandLineArguments;
            this.WorkingDirectory = workingDirectory;
            this.StartProject = startProject;
            this.StartExternalProgram = startExternalProgram;
            this.StartBrowserWithUrl = startBrowserWithUrl;
            this.EnableRemoteDebugging = enableRemoteDebugging;
            this.RemoteDebuggingMachine = remoteDebuggingMachine;
            this.ProfileName = profileName;
            this.TargetFramework = targetFramwork;
        }

        public string NameOrPath { get; private set; }
        public string CommandLineArguments { get; private set; }
        public string WorkingDirectory { get; private set; }
        public bool? StartProject { get; private set; }
        public string StartExternalProgram { get; set; }
        public string StartBrowserWithUrl { get; set; }
        public bool? EnableRemoteDebugging { get; private set; }
        public string RemoteDebuggingMachine { get; set; }
        public string ProfileName { get; private set; }
        public string TargetFramework { get; private set; }

    }
}
