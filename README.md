# sharp-ver

A command line tool for incrementing the version number in a C# WPF project in a semver style. This will perform the changes to the AssemblyInfo.cs that contain version numbers. In the case of there being four digits in the version, it interpret from the leftmost three values. The idea is that you put this on your **$PATH** and call it in the **Solution** directory, where changes will be done and then staged on git.

**Arguments**: {solution-path} {version-tier} {action}  
--solution-path = A path to a solution directory  
--version-tier  = patch|minor|major, the semvar version VersionTier to update  
--action        = add/increase | reduce/decrease, the action to perform on that tier  
 
### Basic example 

````
sv minor
# Before: [assembly: AssemblyVersion("0.0.1.*")]
# After:  [assembly: AssemblyVersion("0.1.0.*")]
````

---

### Build & Run

**Requirements:** Visual Studio 2015 and/or C# 6.0 Roslyn Compiler  
**Optional:** Devenv (Visual Studio 2015) on PATH  

```
git clone https://github.com/dukemiller/sharp-ver
cd sharp-ver
```  

**Building with Devenv (CLI):** ``devenv sharp-ver.sln /Build``  
**Building with Visual Studio:**  Open (Ctrl+Shift+O) "sharp-ver.sln", Build Solution (Ctrl+Shift+B)

A "sv.exe" artifact will be created in the parent sharp-ver folder.
