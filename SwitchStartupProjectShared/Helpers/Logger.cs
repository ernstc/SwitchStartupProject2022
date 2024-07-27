using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EnvDTE;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace SwitchStartupProject
{
    public static class Logger
    {
        private static IVsOutputWindowPane pane;
        private static Window window;

        public static void Initialize(IVsOutputWindow outputWindow, Window outputWindow2)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _GetOrCreateOutputWindowPane(outputWindow);
            window = outputWindow2;
        }

        public static void LogActive(string message, params object[] parameters)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _Log(string.Format(message, parameters), activate: true);
        }

        public static void Log(string message, params object[] parameters)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _Log(string.Format(message, parameters), activate: false);
        }

        public static void LogException(Exception e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _Log(_FormatException(e), activate: true);
        }

        private static void _Log(string message, bool activate = false)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (string.IsNullOrEmpty(message)) return;
            try
            {
                if (activate)
                {
                    pane.Activate();
                    window.Activate();
                }
                pane.OutputStringThreadSafe(message + Environment.NewLine);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Write(string.Format("Could not write to output window pane: {0}", _FormatException(e)));
            }
        }

        private static string _FormatException(Exception e)
        {
            return e.Message + Environment.NewLine + e.StackTrace;
        }

        private static void _GetOrCreateOutputWindowPane(IVsOutputWindow outputWindow)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            Guid guid = GuidList.guidOutputWindowPane;
            if (outputWindow.GetPane(ref guid, out pane) == VSConstants.S_OK) return;

            if (outputWindow.CreatePane(ref guid, "SwitchStartupProject", Convert.ToInt32(true), Convert.ToInt32(false)) != VSConstants.S_OK)
            {
                System.Diagnostics.Debug.Write("Could not create output window pane");
                return;
            }
            outputWindow.GetPane(ref guid, out pane);
        }
    }
}
