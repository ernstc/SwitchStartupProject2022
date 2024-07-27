using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using Microsoft.VisualStudio.Shell;

namespace SwitchStartupProject
{
    public class ActiveConfigurationLoader
    {
        public static string GetConfigurationFilename(string solutionFilename)
        {
            var solutionPath = Path.GetDirectoryName(solutionFilename);
            return Path.Combine(solutionPath, ".vs", "SwitchStartupProject", "ActiveConfiguration.json");
        }

        private readonly string configurationFilename;

        public ActiveConfigurationLoader(string configurationFilename)
        {
            this.configurationFilename = configurationFilename;
        }

        public JObject Load()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            Logger.Log("Loading active configuration");

            if (!File.Exists(configurationFilename)) return null;
            try
            {
                return JObject.Parse(File.ReadAllText(configurationFilename));
            }
            catch (Exception e)
            {
                Logger.Log("\nERROR: Could not read active configuration file");
                Logger.LogException(e);
                return null;
            }
        }

        public void Save(JObject activeConfiguration)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            Logger.Log("Saving active configuration");
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(configurationFilename));
                File.WriteAllText(configurationFilename, activeConfiguration.ToString());
            }
            catch (Exception e)
            {
                Logger.Log("\nERROR: Could not write active configuration file");
                Logger.LogException(e);
            }
        }

        public IDropdownEntry Load(IEnumerable<IDropdownEntry> entries)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var config = Load();
            if (config == null) return null;
            var type = config["Type"].Value<string>();
            var name = config["Name"].Value<string>();
            if (type == "Single")
            {
                return entries.OfType<SingleProjectDropdownEntry>().SingleOrDefault(entry => entry.Project.Path == name);
            }
            else if (type == "Multi")
            {
                return entries.OfType<MultiProjectDropdownEntry>().SingleOrDefault(entry => entry.Configuration.Name == name);
            }
            return null;
        }

        public void Save(IDropdownEntry entry)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (entry is SingleProjectDropdownEntry singleEntry)
            {
                Save(new JObject(
                    new JProperty("Type", "Single"),
                    new JProperty("Name", singleEntry.Project.Path)));
            }
            else if (entry is MultiProjectDropdownEntry multiEntry)
            {
                Save(new JObject(
                    new JProperty("Type", "Multi"),
                    new JProperty("Name", multiEntry.Configuration.Name)));
            }
        }
    }
}
