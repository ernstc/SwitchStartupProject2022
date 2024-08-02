// Guids.cs
// MUST match guids.h
using System;

namespace SwitchStartupProject
{
    static class GuidList
    {
        public const string guidSwitchStartupProjectPkgString = "6a40f0fb-1e24-4a5c-b348-a0103ec5f81d";
        public const string guidSwitchStartupProjectCmdSetString = "4a6aacf5-b958-4cdd-94d1-3409ceec3d22";

        public static readonly Guid guidSwitchStartupProjectCmdSet = new Guid(guidSwitchStartupProjectCmdSetString);

        public static readonly Guid guidOutputWindowPane = new Guid("{ab14d0d4-2687-4580-86a4-cb46a5532afe}");

        public static readonly Guid guidCPlusPlus = new Guid("{8bc9ceb8-8b4a-11d0-8d11-00a0c91bc942}");
        public static readonly Guid guidWebApp = new Guid("{349c5851-65df-11da-9384-00065b846f21}");
        public static readonly Guid guidWebSite = new Guid("{e24c65dc-7377-472b-9aba-bc803b73c61a}");
        public static readonly Guid guidVsPackage = new Guid("{82b43b9b-a64c-4715-b499-d71e9ca2bd60}");
        public static readonly Guid guidDatabase = new Guid("{00d1a9c2-b5f0-4af3-8072-f6c62b433612}");

        public static readonly Guid guidSolutionFolder = new Guid("{2150e333-8fdc-42a3-9474-1a3956d46de8}");
        public static readonly Guid guidMiscFiles = new Guid(EnvDTE.Constants.vsProjectKindMisc);
        public static readonly Guid guidSharedProject = new Guid("{d954291e-2a0b-460d-934e-dc6b0785db48}");
    };
}