# CtcApi

The CtcApi is a standardized, reusable, open source code library developed by and for the Community & Technical Colleges of Washington State in the creation of their own software applications. For more detailed information, see the [repository wiki](https://github.com/BellevueCollege/CtcApi/wiki).

## Projects

The CtcApi is written in C# for the .NET 4 platform using Visual Studio 2010. [NuGet](http://www.nuget.org/) is used for all 3rd-party dependencies as well as for release deployments.

For more details about the projects themselves, see [Structure and examples](https://github.com/BellevueCollege/CtcApi/wiki#structure-and-examples) in the wiki.

### Files you may not be familiar with

In addition to the standard Visual Studio files, these projects include additional files defined in the list below:

* CtcApi

    * **CtcApi.nuspec** - settings for [NuGet package creation](docs.nuget.org/docs/creating-packages/creating-and-publishing-a-package).
    * **mkpkg.cmd** - command-line script to create a NuGet release package (using custom MSBuild target).

* CtcApi.Ods

    * nuget/ - files used in the creation of the NuGet deployment package.
    * **CtcODS.nuspec** - settings for [NuGet package creation](docs.nuget.org/docs/creating-packages/creating-and-publishing-a-package).
    * **CtcOdsApi.shfbproj** - project file for the [Sandcastle documentation compiler](https://sandcastle.codeplex.com/). See also these [notes on setting up and using Sandcastle](http://bit.ly/projdoc).
    * **mkpkg.cmd** - command-line script to create a NuGet release package (using custom MSBuild target).
