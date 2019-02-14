# CtcApi

The CtcApi is a standardized, reusable, open source code library developed by and for the Community and Technical Colleges of Washington State in the creation of their own software applications. For more detailed information, see the [repository wiki](https://github.com/BellevueCollege/CtcApi/wiki).

The CtcApi is written in C# for the .NET 4 platform using Visual Studio 2015. [NuGet](http://www.nuget.org/) is used for all 3rd-party dependencies as well as for release deployments.

For more details about the projects themselves, see [Structure and examples](https://github.com/BellevueCollege/CtcApi/wiki#structure-and-examples) in the wiki.

### Current NuGet package versions (as published on BC NuGet server)

 - CtcApi 
 	- 0.9.16.2/1.0.0
 - CtcApi.Ods
	 - 0.9.16.2/1.0.0  

> Note: In an attempt to end the versioning madness (and to get closer to SemVer specs), the version has arbitrarily been moved forward to 1.0.0. As such, the 0.9.16.2 and 1.0.0 versions are the same.

### To create NuGet packages

Nuget packages for CtcApi and CtcApi.Ods are now created using build processes in Azure DevOps.  In the CtcApi project, queue a new build of CtcApi-CI and CtcOdsApi-CI. When queueing the build, add a variable `version.current` and then specify the build version. This will be used by Azure DevOps as the package version, i.e. `1.0.0`. This should match the assembly version. Once the builds successfully queue and run, you can check the Artifacts list to verify the new Nuget package is there.

If you want to locally build a Nuget package of CtcApi or CtcApi.Ods for your own purposes, you can do so using the Nuget CLI. [See the NuGet CLI reference](https://docs.microsoft.com/en-us/nuget/tools/nuget-exe-cli-reference) for download instructions. Put the `nuget.exe` file in an easy to use location like `C:\Nuget`. You can then build each project into a NuGet package (.nupkg) using the Package Manager Console in Visual Studio.

Example:

```
PM> C:\Nuget\nuget.exe pack CtcApi\CtcApi.csproj -properties Configuration=Release
```

This will build the .nupkg and put it in the root folder of the CtcApi solution.

### Files you may not be familiar with

In addition to the standard Visual Studio files, these projects include additional files defined in the list below:

* CtcApi.Ods

    * **CtcOdsApi.shfbproj** - project file for the [Sandcastle documentation compiler](https://sandcastle.codeplex.com/). See also these [notes on setting up and using Sandcastle](http://bit.ly/projdoc).


