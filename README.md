# sharp-ver

A command line tool for incrementing the version number in a C# visual studio project in a semver style. This will perform the changes to the AssemblyInfo.cs lines that contain version numbers. In the case of the version containing four positions, it will interpret from the leftmost three values and leave the fourth untouched. 

For using this, the idea is that you put this on your **$PATH** and call it in the base **Solution** directory of a project, afterward changes will be done and automatically staged on git.

---

### Usage

````
> ./sv --help

-- Usage 1: sv
Display the current version.

-- Usage 2: sv {tier} {action}
{tier}     = patch | minor | major     -> the semvar tier to change
{action}   = increase | decrease       -> the action to perform on the given tier

Apply {action} on {tier}. If no {action} is given, by default use increase.

-- Usage 3: sv {version}
{version}  = (x.y.z)                   -> a numeric version

Change the current version to the given {version}.
````

**Examples:**  

````
> sv
1.0.0

> sv 2.0.0
1.0.0 -> 2.0.0

> sv major
2.0.0 -> 3.0.0

> sv minor 
3.0.0 -> 3.1.0

> sv patch
3.1.0 -> 3.1.1

> sv minor dec
3.1.1 -> 3.0.0
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
