using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace SwitchStartupProject
{
    public class ConfigurationFileOpener
    {
        private readonly DTE dte;
        private readonly string configurationFilename;
        private readonly string oldConfigurationFilename;
        private readonly ConfigurationLoader configurationLoader;

        public ConfigurationFileOpener(DTE dte, string configurationFilename, string oldConfigurationFilename, ConfigurationLoader configurationLoader)
        {
            this.dte = dte;
            this.configurationFilename = configurationFilename;
            this.oldConfigurationFilename = oldConfigurationFilename;
            this.configurationLoader = configurationLoader;
        }

        public void Open()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (!File.Exists(configurationFilename))
            {
                try
                {
                    configurationLoader.CreateDefaultConfigurationFile();
                }
                catch (Exception e)
                {
                    Logger.LogActive("\nERROR: Could not create default configuration file {0}", configurationFilename);
                    Logger.LogException(e);
                }

                if (File.Exists(oldConfigurationFilename))
                {
                    try
                    {
                        dte.ItemOperations.OpenFile(oldConfigurationFilename, EnvDTE.Constants.vsViewKindCode);
                        Logger.LogActive("\nINFO: Found old configuration file!");
                        Logger.Log("You may want to transfer your existing multi-project startup configurations to the new configuration file.");
                    }
                    catch (Exception e)
                    {
                        Logger.LogActive("\nERROR: Could not open configuration file {0}", configurationFilename);
                        Logger.LogException(e);
                    }
                }
            }

            try
            {
                dte.ItemOperations.OpenFile(configurationFilename, EnvDTE.Constants.vsViewKindCode);
            }
            catch (Exception e)
            {
                Logger.LogActive("\nERROR: Could not open configuration file {0}", configurationFilename);
                Logger.LogException(e);
            }
        }
    }
}
