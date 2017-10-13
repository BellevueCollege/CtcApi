# CtcApi

The CtcApi is a standardized, reusable, open source code library developed by and for the Community and Technical Colleges of Washington State in the creation of their own software applications. For more detailed information, see the [repository wiki](https://github.com/BellevueCollege/CtcApi/wiki).

The CtcApi is written in C# for the .NET 4 platform using Visual Studio 2015. [NuGet](http://www.nuget.org/) is used for all 3rd-party dependencies as well as for release deployments.

For more details about the projects themselves, see [Structure and examples](https://github.com/BellevueCollege/CtcApi/wiki#structure-and-examples) in the wiki.

### Current NuGet package versions (as published on BC NuGet server)

 - CtcApi 
 	- 0.9.16 - for Commons Logging v3.3.1
 	- 0.9.15-legacy - for Commons Logging v2.1.2 (see branch legacy-commonslogging2.1.2
 - CtcApi.Ods
	 - 0.9.15.1 (there was an error with 0.9.15 so this is merely a rebuilt package)  

### To create NuGet packages

Each project (CtcApi and CtcApi.Ods) has its own .nuspec file for use in building NuGet packages which are then deployed to the Bellevue College NuGet server. To build NuGet packages, you will need a local copy of the NuGet CLI. [See the NuGet CLI reference](https://docs.microsoft.com/en-us/nuget/tools/nuget-exe-cli-reference) for download instructions. Put the `nuget.exe` file in an easy to use location like `C:\Nuget`. You can then build each project into a NuGet package (.nupkg) using the Package Manager Console in Visual Studio.

Example:

```
PM> C:\Nuget\nuget.exe pack CtcApi\CtcApi.csproj -properties Configuration=Release
```

This will build the .nupkg and put it in the root folder of the CtcApi solution.

### Files you may not be familiar with

In addition to the standard Visual Studio files, these projects include additional files defined in the list below:

* CtcApi.Ods

    * **CtcOdsApi.shfbproj** - project file for the [Sandcastle documentation compiler](https://sandcastle.codeplex.com/). See also these [notes on setting up and using Sandcastle](http://bit.ly/projdoc).


