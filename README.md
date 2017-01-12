# sharp-ver

A command line tool for incrementing the version number in a C# WPF project in a semver style. This will perform the changes to the AssemblyInfo.cs that contain the version numbers. In the case of there being four digits in the version, it interpret from the leftmost value.

**Arguments**: {solution-path} {version-tier} {action}  
--solution-path = A path to a solution directory  
--version-tier  = patch|minor|major, the semvar version VersionTier to update  
--action        = add/increase | reduce/decrease, the action to perform on that tier  

---

### Build & Run

**Requirements:** [nuget.exe](https://dist.nuget.org/win-x86-commandline/latest/nuget.exe) on PATH, Visual Studio 2015 and/or C# 6.0 Roslyn Compiler  
**Optional:** Devenv (Visual Studio 2015) on PATH  

```
git clone https://github.com/dukemiller/sharp-ver
cd sharp-ver
nuget install sharp-ver\packages.config -OutputDirectory packages
```  

**Building with Devenv (CLI):** ``devenv sharp-ver.sln /Build``  
**Building with Visual Studio:**  Open (Ctrl+Shift+O) "sharp-ver.sln", Build Solution (Ctrl+Shift+B)

A "sharp-ver.exe" artifact will be created in the parent sharp-ver folder.
