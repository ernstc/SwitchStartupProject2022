# Release notes

## Version 4.4
* Skip Shared Project because they cannot be started.
* Disable the drop down when the solution is closed.

## Version 4.3
* Add support for Visual Studio 2022 for ARM64
* Add boolean property `BeginGroup` in the multi-targeting projects for grouping configurations.

## Version 4.2
* Add support for Visual Studio 2022 (issue #102)
* Do not modify `launchsettings.json` file if configuration only contains profile name (issue #83)

## Version 4.1
* Add support for switching the target framework of multi-targeting projects (issue #89)

## Version 4.0
* Add Output Window pane named "SwitchStartupProject"
* Replace ActivityLog messages and popup messages with log messages in Output Window (issue #60)
* Rename command and tooltip of dropdown to "SwitchStartupProject" to avoid confusion (issue #67)
* To improve Visual Studio responsiveness load the SwitchStartupProject extension in the background when a solution is opened
* Make project names and paths in config file case insensitive (issue #63)
* Simplify VSSDK package management by using the VSSDK meta package (issue #78)
* Refresh configuration of current item when configuration file has changed (issue #76)
* Add support for switching the solution configuration and solution platform (issue #70)

## Version 3.5
* Fix support for configuration parameters of C++ projects (issues #55, #61)
* Fix detection of active configuration
* Add support for configuration parameters of CPS (for example .NET Core) projects (issue #56)
* Add support for switching launch profiles of CPS projects (issues #42, #65, #66)
* Add support for build macros in configuration parameters, thanks to Chris Huseman

## Version 3.4
* Add support for starting an external program or browser (issue #54) thanks to Jon List
* Add support for enabling remote debugging (issue #35)

## Version 3.3
* Add support for configuring working directories of projects (issue #27)
* Add support for SQL Server Integration Services (SSIS) projects (issue #44)
* Improve handling of unloaded projects (issues #48 and #49)

## Version 3.2
* Add support for Visual Studio 2017 RC (issue #47)
* Support solutions with multiple projects of the same name (issue #39)
  * Show solution folder names to disambiguate
  * Allow multi-project configurations to unambiguously refer to projects by path
* Bugfix: Support multi-project configurations with same name as project (issue #45)

## Version 3.1
* Less smart, less GUI, more stability, more power for the user
  * Removed smart mode: It was not smart enough and often failed with newer project types. (issue #30)
  * Removed MRU mode: It was not really useful and made it hard to share configurations. (issue #29)
  * Removed GUI: Visual Studio and the GUI frequently caused problems when storing the configurations. (issues #26, #28)
  * Configuration file is now read only. (issue #31)
  * Immediately apply configuration file changes to currently active startup projects. (issue #32)
* New configuration file extension: <SolutionName>.sln.startup.json
* Reorganize readme, license and release notes files

## Version 3.0
* Add support for the upcoming Visual Studio "15"
* Continuous Integration builds on AppVeyor (issue #34)

## Version 2.8
* Add support for F# Exe/WinExe projects in Smart Mode (thanks to TeaDrivenDev)
* Add support for command line arguments (issue #21)
* Add button to clone multi-project startup configuration (issue #18)
* Store single project mode for each solution (issue #22)
* Fix issue #11: Select the current startup project in the dropdown when a project has been (re-)loaded
* Fix issue #20: Select last used multi-project configuration in dropdown box when re-opening a solution
* Fix issue #23: Make sure multi-project configurations of last opened solution are not deleted when creating a new project/solution
* Fix issue #24: Handle renaming of projects

## Version 2.7
* Fix issue #17: Prevent exceptions when creating a new website.
* Re-enable support for Visual Studio 2014 (see issue #13).

## Version 2.6
* Fix issue #16: Fix MRU mode: Most recently used projects should only show projects that are in the solution
* Fix issue #14: Add support for visual fortran projects (and other project types that don't implement IVsAggregatableProject)

## Version 2.5
* Fix issue #12: Fix web site projects support

## Version 2.4
* Track configuration file and reload settings upon change
* Save configuration file whenever solution gets saved
* Smart mode: Improve detection of startable projects
* Support for Visual Studio 2014
* Migrate source code to Visual Studio 2013

## Version 2.3
* Fix issue #10: Improve behavior with projects that are not in dropdown list
* Fix issue #9: Support Azure projects
* Fix issue #5: Support VS 2013 projects in smart mode

## Version 2.2
* Fix issue #8: Fix multi project configurations in VS 2010

## Version 2.1
* Fix issue #6: Enable dropdown after installation and restart
* Fix issue #7: Support VS 2012 projects loading in background

## Version 2.0
* Support for multiple startup projects.