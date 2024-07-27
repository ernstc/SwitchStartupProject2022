# Configuration File

SwitchStartupProject can be configured with a solution specific configuration file.
It allows to define the available startup project entries in the dropdown and their behavior.

To open the configuration file in Visual Studio, select `Configure...` in the SwitchStartupProject dropdown.
A new configuration file with default values (and some comments) is created if it does not exist yet.

## Location and Filename

The configuration file is located in the same folder as the solution file, it has the same name as the solution file, but uses extension `.sln.startup.json`.

| Example | |
| --- | --- |
| Visual Studio Solution File | `MySolution.sln` |
| SwitchStartupProject Configuration File | `MySolution.sln.startup.json` |

## Format

The configuration file uses [JSON](http://json.org/) syntax with C-style `/* ... */` comments.
See also the [JSON schema](https://heptapod.host/thirteen/switchstartupproject/blob/branch/current/SwitchStartupProject/Configuration/ConfigurationSchema.json) that is used to validate the configuration file.

## Example

```
#!json
{
  "Version": 3,
  "ListAllProjects": false,
  "MultiProjectConfigurations": {
    "A + B (Ext)": {
      "Projects": {
        "MyProjectA": {},
        "MyProjectB": {
          "CommandLineArguments": "1234",
          "WorkingDirectory": "%USERPROFILE%\\test",
          "StartExternalProgram": "c:\\myprogram.exe"
        }
      }
    },
    "A + B": {
      "Projects": {
        "MyProjectA": {},
        "MyProjectB": {
          "CommandLineArguments": "",
          "WorkingDirectory": "",
          "StartProject": true
        }
      }
    },
    "D (Debug x86)": {
      "BeginGroup": true,
      "Projects": {
        "MyProjectD": {}
      },
      "SolutionConfiguration": "Debug",
      "SolutionPlatform": "x86"
    },
    "D (Release x64)": {
      "Projects": {
        "MyProjectD": {}
      },
      "SolutionConfiguration": "Release",
      "SolutionPlatform": "x64"
    }
  }
}
```

## Configuration Properties

### Version
The version of the configuration file format. Must be `3`.

### ListAllProjects
Can be either `true` or `false`.

If set to `true`, SwitchStartupProject creates an item in the dropdown for each project in the solution, which allows each project to be activated individually as startup project.

### MultiProjectConfigurations
A dictionary of named startup configurations, each consisting of one or multiple startup projects with optional parameters like command line arguments and working directory.

Example of a startup configuration:
```
        "A + B (CLA) + C": {                                    /*  Configuration name (appears in the dropdown)  */
            "Projects": {
                "MyProjectA": {},                               /*  Starting project A  */
                "MyProjectB": {                                 /*  and project B ...   */
                    "CommandLineArguments": "1234",             /*  ... with command line arguments "1234"  */
                    "WorkingDirectory": "%USERPROFILE%\\test",  /*  ... with working directory %USERPROFILE%\test  */
                    "StartExternalProgram": "c:\\myprogram.exe" /*  ... using c:\myprogram.exe as the debugging host  */
                },
                "Path\\To\\ProjectC.csproj": {}                 /*  and project C (specified by path)  */
            },
            "SolutionConfiguration": "Release",                 /*  Activating solution configuration "Release"  */
            "SolutionPlatform": "x64"                           /*  and solution platform "x64"  */
        }
```
Startup projects can be specified in two ways:

* Either by project name as shown in the solution explorer of Visual Studio.
* Or by the path to the project file, relative to the solution file. This allows to unambiguously refer to projects with same name. Note that backslashes have to be escaped (duplicated) in JSON.

SwitchStartupProject creates an item in the dropdown for each startup configuration, allowing the corresponding startup projects (and parameters) to be activated.

| Parameter | Type | Example Value | Explanation |
| --- | --- | --- | --- |
| `"CommandLineArguments"` | string | `"--doSomething 123"` | Passes the given string as command line arguments to the started project/program |
| `"WorkingDirectory"` | string | `"F:\\Project\\Test"` | Starts the project/program in the given working directory. Remember to escape (double) backslashes. |
| `"StartExternalProgram"` | string | `"C:\\Windows\\System32\\cmd.exe"`| Starts the specified program instead of the project. Remember to escape (double) backslashes. |
| `"StartBrowserWithURL"` | string | `"https://localhost:1234/api/test"` | Starts the default browser and opens the given URL. |
| `"StartProject"` | boolean | `true` | Starts the project. (Resets a `"StartExternalProgram"` or `"StartBrowserWithURL"` parameter specified in another configuration.) |
| `"EnableRemoteDebugging"` | boolean | `true` | Use remote machine for debugging. |
| `"RemoteDebuggingMachine"` | string | `"\\\\MyDomain\\MyTestMachine"` | Specify the machine name used for remote debugging. Remember to escape (double) backslashes. |
| `"ProfileName"` | string | `"MyLaunchProfile"` | Activates the launch profile with the given name for [CPS projects](https://github.com/Microsoft/VSProjectSystem/blob/master/doc/Index.md). (VS 2017 and later only) |
| `"TargetFramework"` | string | `"netcoreapp3.1"` | Activates the target framework with the given Target Framework Moniker (TFM) for multi-targeting projects. (VS 2019 and later only) |

String parameters may contain [build macros](https://docs.microsoft.com/en-us/cpp/ide/common-macros-for-build-commands-and-properties).

Note:
If a startup project specifies a parameter (like command line arguments or a working directory), that parameter is (persistently) set when the configuration gets activated.
If a startup project does not specify a parameter, the existing parameter value won't change when the configuration gets activated.
To reset a parameter set by another configuration, specify the parameter with an empty string `""` value.
Use `"StartProject": true` to reset a `"StartExternalProgram"` or `"StartBrowserWithURL"` parameter set by another configuration.

Besides startup projects and their parameters, a startup configuration may contain the following parameters:

| Parameter | Type | Example Value | Explanation |
| --- | --- | --- | --- |
| `"SolutionConfiguration"` | string | `"Release"` | Activates the solution configuration with the given name. (VS 2017 and later only) |
| `"SolutionPlatform"` | string | `"Any CPU"` | Activates the solution platform with the given name. (VS 2017 and later only) |


## Default Values

If SwitchStartupProject finds no configuration file, it uses the following defaults:

```
#!json
{
    "Version": 3,
    "ListAllProjects": true,
    "MultiProjectConfigurations": {}
}
```

## Configuration Updates

SwitchStartupProject reads and applies the configuration file of a solution

* when the solution is opened
* whenever the configuration file is changed while the solution is open

## Older Versions

Up to SwitchStartupProject version `3.0` the configuration file was named `MySolution.startup.suo`.
