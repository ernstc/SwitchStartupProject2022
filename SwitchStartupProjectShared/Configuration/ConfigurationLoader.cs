using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Microsoft.VisualStudio.Shell;

using Newtonsoft.Json.Linq;

using NJsonSchema;

namespace SwitchStartupProject
{

    public class ConfigurationLoader
    {
        private const string versionKey = "Version";
        private const int knownVersion = 3;
        private const string listAllProjectsKey = "ListAllProjects";
        private const string multiProjectConfigurationsKey = "MultiProjectConfigurations";
        private const string beginGroupKey = "BeginGroup";
        private const string projectsKey = "Projects";
        private const string claKey = "CommandLineArguments";
        private const string workingDirKey = "WorkingDirectory";
        private const string startProjectKey = "StartProject";
        private const string startExtProgKey = "StartExternalProgram";
        private const string startBrowserKey = "StartBrowserWithURL";
        private const string enableRemoteDebuggingKey = "EnableRemoteDebugging";
        private const string remoteDebuggingMachineKey = "RemoteDebuggingMachine";
        private const string profileNameKey = "ProfileName";
        private const string targetFrameworkKey = "TargetFramework";
        private const string solutionConfigurationKey = "SolutionConfiguration";
        private const string solutionPlatformKey = "SolutionPlatform";

        private readonly string configurationFilename;

        public static string GetConfigurationFilename(string solutionFilename)
        {
            const string configurationFileExtension = ".startup.json";
            var solutionPath = Path.GetDirectoryName(solutionFilename);
            return Path.Combine(solutionPath, solutionFilename + configurationFileExtension);
        }

        public static string GetOldConfigurationFilename(string solutionFilename)
        {
            const string oldConfigurationFileExtension = ".startup.suo";
            var solutionPath = Path.GetDirectoryName(solutionFilename);
            return Path.Combine(solutionPath, Path.GetFileNameWithoutExtension(solutionFilename) + oldConfigurationFileExtension);
        }

        public ConfigurationLoader(string configurationFilename)
        {
            this.configurationFilename = configurationFilename;
        }

        public async Task<Configuration> LoadAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            Logger.Log("Loading configuration for solution");
            if (!_ConfigurationFileExists())
            {
                Logger.Log("No configuration file found, using default configuration.");
                return _GetDefaultConfiguration();
            }

            JObject settings = null;
            try
            {
                settings = JObject.Parse(File.ReadAllText(configurationFilename));
            }
            catch (Exception e)
            {
                Logger.Log("\nERROR: Could not parse configuration file.");
                Logger.Log("Please check the syntax of your configuration file!");
                Logger.LogException(e);
                Logger.Log("Using default configuration instead.");
                return _GetDefaultConfiguration();
            }
            JsonSchema schema = null;
            try
            {
                var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SwitchStartupProject.Configuration.ConfigurationSchema.json");
                using (var reader = new StreamReader(stream))
                {
                    schema = await JsonSchema.FromJsonAsync(await reader.ReadToEndAsync());
                }
            }
            catch (Exception e)
            {
                Logger.Log("\nERROR: Could not parse schema:");
                Logger.LogException(e);
                Logger.Log("Using default configuration.");
                return _GetDefaultConfiguration();
            }

            var validationErrors = schema.Validate(settings);
            if (validationErrors.Any())
            {
                var messages = string.Join("\n", validationErrors.Select(err => string.Format("{0}: {1}", err.Path, err.Kind)));
                Logger.LogActive("\nERROR: Could not validate schema of configuration file.");
                Logger.Log("Please check your configuration file!");
                Logger.LogActive(messages);
                Logger.Log("Using default configuration instead.");
                return _GetDefaultConfiguration();
            }

            var version = _GetVersion(settings);
            if (version != knownVersion)
            {
                Logger.LogActive("\nERROR: Configuration file has unknown version {0}. Version should be {1}.", version , knownVersion);
                Logger.Log("Using default configuration instead.");
                return _GetDefaultConfiguration();
            }

            var listAllProjects = _GetListAllProjects(settings);
            var multiProjectConfigurations = _GetMultiProjectConfigurations(settings);
            return new Configuration(listAllProjects, multiProjectConfigurations.ToList());
        }

        public void CreateDefaultConfigurationFile()
        {
            var sb = new StringBuilder();
            sb.AppendLine("/*");
            sb.AppendLine("    This is a configuration file for the SwitchStartupProject Visual Studio Extension");
            sb.AppendLine("    See https://heptapod.host/thirteen/switchstartupproject/blob/branch/current/Configuration.md");
            sb.AppendLine("*/");
            sb.AppendLine("{");
            sb.AppendLine("    /*  Configuration File Version  */");
            sb.AppendLine("    \"" + versionKey + "\": 3,");
            sb.AppendLine("    ");
            sb.AppendLine("    /*  Create an item in the dropdown list for each project in the solution?  */");
            sb.AppendLine("    \"" + listAllProjectsKey + "\": true,");
            sb.AppendLine("");
            sb.AppendLine("    /*");
            sb.AppendLine("        Dictionary of named configurations with one or multiple startup projects");
            sb.AppendLine("        and optional parameters like command line arguments and working directory.");
            sb.AppendLine("        Example:");
            sb.AppendLine("");
            sb.AppendLine("        \"" + multiProjectConfigurationsKey + "\": {");
            sb.AppendLine("          \"A + B (Ext)\": {");
            sb.AppendLine("            \"" + projectsKey + "\": {");
            sb.AppendLine("              \"MyProjectA\": {},");
            sb.AppendLine("              \"MyProjectB\": {");
            sb.AppendLine("                \"" + claKey + "\": \"1234\",");
            sb.AppendLine("                \"" + workingDirKey + "\": \"%USERPROFILE%\\\\test\",");
            sb.AppendLine("                \"" + startExtProgKey + "\": \"c:\\\\myprogram.exe\"");
            sb.AppendLine("              }");
            sb.AppendLine("            }");
            sb.AppendLine("          },");
            sb.AppendLine("          \"A + B\": {");
            sb.AppendLine("            \"" + projectsKey + "\": {");
            sb.AppendLine("              \"MyProjectA\": {},");
            sb.AppendLine("              \"MyProjectB\": {");
            sb.AppendLine("                \"" + claKey + "\": \"\",");
            sb.AppendLine("                \"" + workingDirKey + "\": \"\",");
            sb.AppendLine("                \"" + startProjectKey + "\": true");
            sb.AppendLine("              }");
            sb.AppendLine("            }");
            sb.AppendLine("          },");
            sb.AppendLine("          \"D (Debug x86)\": {");
            sb.AppendLine("            \"" + projectsKey + "\": {");
            sb.AppendLine("              \"MyProjectD\": {}");
            sb.AppendLine("            },");
            sb.AppendLine("            \"" + solutionConfigurationKey + "\": \"Debug\",");
            sb.AppendLine("            \"" + solutionPlatformKey + "\": \"x86\",");
            sb.AppendLine("          },");
            sb.AppendLine("          \"D (Release x64)\": {");
            sb.AppendLine("            \"" + projectsKey + "\": {");
            sb.AppendLine("              \"MyProjectD\": {}");
            sb.AppendLine("            },");
            sb.AppendLine("            \"" + solutionConfigurationKey + "\": \"Release\",");
            sb.AppendLine("            \"" + solutionPlatformKey + "\": \"x64\",");
            sb.AppendLine("          }");
            sb.AppendLine("        }");
            sb.AppendLine("    */");
            sb.AppendLine("    \"" + multiProjectConfigurationsKey + "\": {}");
            sb.AppendLine("}");
            File.WriteAllText(configurationFilename, sb.ToString());
        }

        private Configuration _GetDefaultConfiguration()
        {
            return new Configuration(true, Enumerable.Empty<MultiProjectConfiguration>().ToList());
        }

        private bool _ConfigurationFileExists()
        {
            return File.Exists(configurationFilename);
        }

        private static int _GetVersion(JObject settings)
        {
            return _ExistsKey(settings, versionKey) ? settings[versionKey].Value<int>() : 0;
        }

        private static bool _GetListAllProjects(JObject settings)
        {
            return !_ExistsKey(settings, listAllProjectsKey) || settings[listAllProjectsKey].Value<bool>();
        }

        private static IEnumerable<MultiProjectConfiguration> _GetMultiProjectConfigurations(JObject settings)
        {
            if (!_ExistsKey(settings, multiProjectConfigurationsKey)) return Enumerable.Empty<MultiProjectConfiguration>();
            return from configuration in settings[multiProjectConfigurationsKey].Cast<JProperty>()
                   let beginGroup = configuration.Value[beginGroupKey]?.Value<bool?>() ?? false
                   let projects = (from project in configuration.Value[projectsKey].Cast<JProperty>()
                                   let cla = project.Value[claKey]
                                   let workingDir = project.Value[workingDirKey]
                                   let startProject = project.Value[startProjectKey]
                                   let startExtProg = project.Value[startExtProgKey]
                                   let startBrowser = project.Value[startBrowserKey]
                                   let enableRemote = project.Value[enableRemoteDebuggingKey]
                                   let remoteMachine = project.Value[remoteDebuggingMachineKey]
                                   let profileName = project.Value[profileNameKey]
                                   let targetFramwork = project.Value[targetFrameworkKey]
                                   select new MultiProjectConfigurationProject(
                                       project.Name,
                                       cla?.Value<string>(),
                                       workingDir?.Value<string>(),
                                       startProject?.Value<bool?>(),
                                       startExtProg?.Value<string>(),
                                       startBrowser?.Value<string>(),
                                       enableRemote?.Value<bool?>(),
                                       remoteMachine?.Value<string>(),
                                       profileName?.Value<string>(),
                                       targetFramwork?.Value<string>()))
                   let solutionConfiguration = configuration.Value[solutionConfigurationKey]?.Value<string>()
                   let solutionPlatform = configuration.Value[solutionPlatformKey]?.Value<string>()
                   select new MultiProjectConfiguration(configuration.Name, beginGroup, projects.ToList(), solutionConfiguration, solutionPlatform);
        }

        private static bool _ExistsKey(JObject settings, string key)
        {
            return settings.Children<JProperty>().Any(child => child.Name == key);
        }
    }
}
