# sharp-ver

A command line tool for incrementing the version number in a C# visual studio project in a semver style. This will perform the changes to the AssemblyInfo.cs lines that contain version numbers. In the case of the version containing  four digits in the version, it interpret from the leftmost three values and leave the fourth untouched. 

For using this, the idea is that you put this on your **$PATH** and call it in the base **Solution** directory of a project, afterward changes will be done and automatically staged on git.

---

### Usage

**Arguments:**  
````
sv.exe version-tier [action]

--version-tier  = patch | minor | major           ->  the semvar version tier to update  
--action        = add/increase | reduce/decrease  ->  the action to perform on that tier
                  [by default will add]   
````

**Examples:**  

````
sv major
# Before: [assembly: AssemblyVersion("0.0.1.*")]
# After:  [assembly: AssemblyVersion("1.0.0.*")]
sv minor 
# Before: [assembly: AssemblyVersion("1.0.0.*")]
# After:  [assembly: AssemblyVersion("1.1.0.*")]
sv patch
# Before: [assembly: AssemblyVersion("1.1.0.*")]
# After:  [assembly: AssemblyVersion("1.1.1.*")]
sv minor dec
# Before: [assembly: AssemblyVersion("1.1.1.*")]
# After:  [assembly: AssemblyVersion("1.0.0.*")]
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
