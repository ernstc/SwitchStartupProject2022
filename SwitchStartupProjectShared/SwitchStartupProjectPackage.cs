using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace SwitchStartupProject
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    // This attribute is used to register the informations needed to show the this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", AssemblyInfoVersion.Version, IconResourceID = 400)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string, PackageAutoLoadFlags.BackgroundLoad)]
    [Guid(GuidList.guidSwitchStartupProjectPkgString)]
    public sealed class SwitchStartupProjectPackage : AsyncPackage, IVsSolutionEvents, IVsSolutionEvents4, IVsSolutionLoadEvents, IVsSelectionEvents
    {

        private uint solutionEventsCookie;
        private IVsSolution2 solution = null;
        private uint selectionEventsCookie;
        private IVsMonitorSelection ms = null;
        private uint debuggingCookie;

        private StartupProjectSwitcher switcher;

        protected override void Dispose(bool disposing)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            base.Dispose(disposing);

            // Unadvise all events
            if (solution != null && solutionEventsCookie != 0)
                solution.UnadviseSolutionEvents(solutionEventsCookie);
            if (ms != null && selectionEventsCookie != 0)
                ms.UnadviseSelectionEvents(selectionEventsCookie);
        }

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initilaization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override async System.Threading.Tasks.Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await base.InitializeAsync(cancellationToken, progress);

            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            // Get VS Automation object
            var dte = (EnvDTE.DTE)GetGlobalService(typeof(EnvDTE.DTE));

            IVsOutputWindow outputWindow = await GetServiceAsync(typeof(SVsOutputWindow)) as IVsOutputWindow;
            var outputWindow2 = dte.Windows.Item(EnvDTE.Constants.vsWindowKindOutput);
            Logger.Initialize(outputWindow, outputWindow2);

            // Add our command handlers for menu (commands must exist in the .vsct file)
            var mcs = await GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            if ( null == mcs )
            {
                Logger.LogActive("\nERROR: Could not get OleMenuCommandService");
                return;
            }
            var dropdownService = new DropdownService(mcs);

            // Get solution
            solution = await GetServiceAsync(typeof(SVsSolution)) as IVsSolution2;
            if (solution != null)
            {
                // Register for solution events
                solution.AdviseSolutionEvents(this, out solutionEventsCookie);
            }

            // Get selection monitor
            ms = await GetServiceAsync(typeof(SVsShellMonitorSelection)) as IVsMonitorSelection;
            if (ms != null)
            {
                // Remember debugging UI context cookie for later
                ms.GetCmdUIContextCookie(VSConstants.UICONTEXT.Debugging_guid, out debuggingCookie);
                // Register for selection events
                ms.AdviseSelectionEvents(this, out selectionEventsCookie);
            }

            var fileChangeService = await GetServiceAsync(typeof(SVsFileChangeEx)) as IVsFileChangeEx;

            switcher = new StartupProjectSwitcher(dropdownService, dte, fileChangeService);

            // Package is loaded after a solution was opened
            if (solution != null)
            {
                solution.GetProperty((int)__VSPROPID.VSPROPID_IsSolutionOpen, out var value);
                if (value is bool isSolutionOpen && isSolutionOpen)
                {
                    await switcher.AfterOpenSolutionAsync();
                }
            }
        }

        #region IVsSolutionEvents Members

        public int OnAfterCloseSolution(object pUnkReserved)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            switcher.AfterCloseSolution();
            return VSConstants.S_OK;
        }

        public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            switcher.RepopulateDropdownList();
            switcher.UpdateStartupProject();
            return VSConstants.S_OK;
        }

        public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            switcher.OpenProject(pHierarchy, fAdded == 1);
            return VSConstants.S_OK;
        }


        public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            switcher.AfterOpenSolutionAsync().Wait();
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            switcher.RepopulateDropdownList();
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseSolution(object pUnkReserved)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            switcher.BeforeCloseSolution();
            return VSConstants.S_OK;
        }

        public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        #endregion

        #region IVsSolutionEvents4

        public int OnAfterAsynchOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterChangeProjectParent(IVsHierarchy pHierarchy)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterRenameProject(IVsHierarchy pHierarchy)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            switcher.RepopulateDropdownList();
            return VSConstants.S_OK;
        }

        public int OnQueryChangeProjectParent(IVsHierarchy pHierarchy, IVsHierarchy pNewParentHier, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        #endregion

        #region IVsSolutionLoadEvents Members

        public int OnAfterBackgroundSolutionLoadComplete()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            switcher.RepopulateDropdownList();
            return VSConstants.S_OK;
        }

        public int OnAfterLoadProjectBatch(bool fIsBackgroundIdleBatch)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeBackgroundSolutionLoadBegins()
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeLoadProjectBatch(bool fIsBackgroundIdleBatch)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeOpenSolution(string pszSolutionFilename)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            switcher.BeforeOpenSolution(pszSolutionFilename);
            return VSConstants.S_OK;
        }

        public int OnQueryBackgroundLoadProjectBatch(out bool pfShouldDelayLoadToNextIdle)
        {
            pfShouldDelayLoadToNextIdle = false;
            return VSConstants.S_OK;
        }

        #endregion

        #region IVsSelectionEvents Members

        public int OnCmdUIContextChanged(uint dwCmdUICookie, int fActive)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (dwCmdUICookie == debuggingCookie)
            {
                switcher.ToggleDebuggingActive(fActive != 0);
            }
            return VSConstants.S_OK;
        }

        public int OnElementValueChanged(uint elementid, object varValueOld, object varValueNew)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (elementid == (uint)VSConstants.VSSELELEMID.SEID_StartupProject)
            {
                switcher.UpdateStartupProject();
            }
            return VSConstants.S_OK;
        }

        public int OnSelectionChanged(IVsHierarchy pHierOld, uint itemidOld, IVsMultiItemSelect pMISOld, ISelectionContainer pSCOld, IVsHierarchy pHierNew, uint itemidNew, IVsMultiItemSelect pMISNew, ISelectionContainer pSCNew)
        {
            return VSConstants.S_OK;
        }

        #endregion
    }
}
