using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.Shell;

namespace SwitchStartupProject
{
    public class StartupConfiguration
    {
        public StartupConfiguration(string name, IList<StartupConfigurationProject> projects, string solutionConfiguration, string solutionPlatform)
        {
            this.Name = name;
            this.Projects = projects;
            this.SolutionConfiguration = solutionConfiguration;
            this.SolutionPlatform = solutionPlatform;
        }

        public string Name { get; private set; }
        public IList<StartupConfigurationProject> Projects { get; private set; }
        public string SolutionConfiguration { get; private set; }
        public string SolutionPlatform { get; private set; }
    }

    public class StartupConfigurationProject
    {
        public StartupConfigurationProject(
            SolutionProject project,
            string commandLineArguments,
            string workingDirectory,
            bool? startProject,
            string startExternalProgram,
            string startBrowserWithUrl,
            bool? enableRemoteDebugging,
            string remoteDebuggingMachine,
            string profileName,
            string targetFramework)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            this.Project = project;
            this.CommandLineArguments = project.EvaluateBuildMacros(commandLineArguments);
            this.WorkingDirectory = project.EvaluateBuildMacros(workingDirectory);
            this.StartProject = startProject;
            this.StartExternalProgram = project.EvaluateBuildMacros(startExternalProgram);
            this.StartBrowserWithUrl = project.EvaluateBuildMacros(startBrowserWithUrl);
            this.EnableRemoteDebugging = enableRemoteDebugging;
            this.RemoteDebuggingMachine = project.EvaluateBuildMacros(remoteDebuggingMachine);
            this.ProfileName = project.EvaluateBuildMacros(profileName);
            this.TargetFramework = project.EvaluateBuildMacros(targetFramework);
        }

        public SolutionProject Project { get; private set; }
        public string CommandLineArguments { get; private set; }
        public string WorkingDirectory { get; private set; }
        public bool? StartProject { get; set; }
        public string StartExternalProgram { get; set; }
        public string StartBrowserWithUrl { get; set; }
        public bool? EnableRemoteDebugging { get; private set; }
        public string RemoteDebuggingMachine { get; set; }
        public string ProfileName { get; private set; }
        public string TargetFramework { get; private set; }
    }
}
