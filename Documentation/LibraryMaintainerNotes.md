Mike May - October 2017, Updated March 2018

Documnetation of the *library user* is at http://mikedamay.co.uk/PureDI

Code Status
==============
* No Warnings except in Sandcastle Help Builder
* All Tests Pass
* Useable (except for the absence of real-world hardening)

Prerequisites
=============
VS2017.
Sandcastle build helper extension (2017.5.15).

Alternatively:
If you don't need to build User Guide and API docs 
or you are building on Linux or MacOs then Rider or VSCode can be used


Building User Guide and API Docs
================================

The documentation can be built only on Windows (not Linux or MacOs) and requires VS2017.

The build process takes the files in sandDoc/Contents and generates sandDoc/GeneratedHelp as 
HTML, js, css etc.  The contents of sandDoc/GeneratedHelp can be copied into a web folder,
say /PureDI for publication.  Currently http://mikedamay.co.uk/PureDI contains the latest published docs.
A reference to PureDI.csproj under "Documentation Sources" is sufficient to drag in the
binary from which API details are extracted.

The following steps are requred to build the User Guide and API documentation:

1) Add sandDoc VS project to the PureDI solution.  (The sandDoc project bulds that User Guide and API Docs.)
(Note that the PureDI solution is persisted with the sandBox project removed from the .sln file
as the sandDoc project requires the full .NET framework so its inclusion by
default would break non-Windows builds.  It also slows down the build).  The sandBox
project is included in the PureDI git repo under the PureDI solution directory.

2) Change the targetFramework of the PureDI.csproj from nestandard2.0 to netcoreapp2.0 before building
docs and back afterwards.  (See Note A, below and Step 5)

3) Make changes to the Content folder if necessary.  (See Note C, below)

4) Do Rebuild Solution - but note the issue (Note D) below.

5) Reverse steps 1) and 2).

6) Copy contents of sandDoc/GeneratedHelp to the document directory of some website.

Note A. The Help Build barfs (unable to find the System assembly) when attempting to build
with a target of netstandard2.0.

Note B. The Sandcastle Help Builder is version 2017.5.15

Note C. Help Builder Gotcha: make sure you change the text of Version History
when you add version.  Simply adding the version using ContentLayout.content
is not sufficient.  

Note D. Sometimes on _Rebuild Solution_ the SandDoc project will fail to find PureDI.dll - to fix
follow _Rebuild Solution_ with _Build Solution_ and it should build OK.
Presumably this relates to concurrent builds in some way.

Nuspec
======

nuspec pack fails
dotnet pack & msbuild /t:pack produce a file but ignore our 
  nuspec package which has to be merged manually (See Release Activity 9, below)

Release Activities
==================
-1 Remove SandDoc project from solution
0. Ensure PureDI targets netstandard2.0 (not netcoreapp)
1. Build & Test on Windows
2. Build & Test on Linux
3. Build & Test on Mac
4. Check that PureDI AssemblyInfo AssemblyVersion is correct
5. Check that PureDI AssemblyInfo AssemblyFileVersion is correct
6. Update version in PureDI.nuspec
7. Generate nupkg
8. Change name of nupkg to reflect version
9. Merge in nuspec file into nupkg manually
10. Create branch for release
11. track branch in remote
12. change version number of file in publish.bat
13. substitute secret in publish.bat
14. run publish.bat
15. unsubstitute secret in publish.bat
16. final check in on branch
17. merge branch into master
18. push master
19. Upload latest help files to mikedamay.co.uk/PureDI
20. Test nuget install on Mac or Linux or Windows
