using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using EnvDTE;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace SwitchStartupProject
{
    public class SolutionProject
    {
        private bool isWebSiteProject;
        private string solutionFullName;

        public IVsHierarchy Hierarchy { get; private set; }
        public string Name { get; private set; }
        public string Path { get; private set; }
        public Project Project { get; private set; }
        public string StartArgumentsPropertyName { get; private set; }
        public IList<string> SolutionFolders { get; private set; }

        protected SolutionProject(IVsHierarchy hierarchy, Project project, string name, IList<Guid> projectTypeGuids, string solutionFullName)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            this.Hierarchy = hierarchy;
            this.Project = project;
            this.Name = name;
            this.solutionFullName = solutionFullName;
            this.isWebSiteProject = projectTypeGuids.Contains(GuidList.guidWebSite);

            this.Path = _GetProjectPath(Project, solutionFullName, isWebSiteProject);
            this.StartArgumentsPropertyName = projectTypeGuids.Contains(GuidList.guidCPlusPlus) ? "CommandArguments" : "StartArguments";
            this.SolutionFolders = _GetSolutionFolders(hierarchy);
        }

        public static SolutionProject FromHierarchy(IVsHierarchy hierarchy, string solutionFullName)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var name = _GetProjectStringProperty(hierarchy, __VSHPROPID.VSHPROPID_Name);

            var project = _GetProjectFromHierarchy(hierarchy);
            var projectTypeGuids = _GetProjectTypeGuids(hierarchy, project).ToList();

            // Filter out hierarchy elements that don't represent projects (e.g. solution folders)
            if (name == null || projectTypeGuids.Contains(GuidList.guidSolutionFolder) || projectTypeGuids.Contains(GuidList.guidMiscFiles)) return null;

            return new SolutionProject(hierarchy, project, name, projectTypeGuids, solutionFullName);
        }

        private static string _GetProjectPath(Project project, string solutionFullName, bool isWebSiteProject)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var fullPath = project.FullName;
            if (isWebSiteProject) return fullPath;  // Website projects need to be set using the full path
            var solutionPath = System.IO.Path.GetDirectoryName(solutionFullName) + @"\";
            return Paths.GetPathRelativeTo(fullPath, solutionPath);
        }

        private static string _GetProjectStringProperty(IVsHierarchy pHierarchy, __VSHPROPID property)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            object valueObject = null;
            return pHierarchy.GetProperty((uint)VSConstants.VSITEMID.Root, (int)property, out valueObject) == VSConstants.S_OK ? (string)valueObject : null;
        }

        private static Guid? _GetProjectGuidProperty(IVsHierarchy pHierarchy, __VSHPROPID property)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            Guid guid = Guid.Empty;
            return pHierarchy.GetGuidProperty((uint)VSConstants.VSITEMID.Root, (int)property, out guid) == VSConstants.S_OK ? guid : (Guid?)null;
        }

        private static IEnumerable<Guid> _GetProjectTypeGuids(IVsHierarchy pHierarchy, Project project)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            IEnumerable<Guid> projectTypeGuids;
            var aggregatableProject = pHierarchy as IVsAggregatableProject;
            if (aggregatableProject != null)
            {
                string projectTypeGuidString;
                aggregatableProject.GetAggregateProjectTypeGuids(out projectTypeGuidString);
                projectTypeGuids = projectTypeGuidString.Split(';')
                    .Where(guidString => !string.IsNullOrEmpty(guidString))
                    .Select(guidString => new Guid(guidString));
            }
            else
            {
                projectTypeGuids = new[] { new Guid(project.Kind) };
            }
            return projectTypeGuids;
        }

        private static Project _GetProjectFromHierarchy(IVsHierarchy pHierarchy)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            object project;
            ErrorHandler.ThrowOnFailure(pHierarchy.GetProperty(VSConstants.VSITEMID_ROOT,
                                                               (int)__VSHPROPID.VSHPROPID_ExtObject,
                                                               out project));
            return (project as Project);
        }

        private static List<string> _GetSolutionFolders(IVsHierarchy hierarchy)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var parents = new List<string>();
            var current = hierarchy;
            while (true)
            {
                current = _GetParentHierarchy(current);
                if (current == null) break;
                var typeguid = _GetProjectGuidProperty(current, __VSHPROPID.VSHPROPID_TypeGuid);
                var parentName = _GetProjectStringProperty(current, __VSHPROPID.VSHPROPID_Name);
                if (typeguid != VSConstants.GUID_ItemType_VirtualFolder) break;
                parents.Add(parentName);
            }
            parents.Reverse();
            return parents;
        }

        private static IVsHierarchy _GetParentHierarchy(IVsHierarchy pHierarchy)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            object parentHierarchy;
            return (pHierarchy.GetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_ParentHierarchy, out parentHierarchy)) == VSConstants.S_OK ?
                parentHierarchy as IVsHierarchy :
                null;
        }

        public string EvaluateBuildMacros(string input)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var macroInfo = Hierarchy as IVsBuildMacroInfo;
            if (macroInfo == null) return input;
            if (string.IsNullOrEmpty(input)) return input;
            var matches = Regex.Matches(input, @"(?<token>\$\((?<macro>[^\)]+)\))");
            var tokens = matches.OfType<Match>()
                .Select(m => new { Macro = m.Groups["macro"].Value, Token = m.Groups["token"].Value })
                .Distinct();
            foreach (var token in tokens)
            {
                string value;
                if (macroInfo.GetBuildMacroValue(token.Macro, out value) != VSConstants.S_OK) continue;
                input = input.Replace(token.Token, value);
            }
            return input;
        }
    }
}
