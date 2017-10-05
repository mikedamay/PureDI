Miek May - October 2017

Documnetation of the library user is at http://mikedamay.co.uk/PureDI

Code Status
==============
* No Warnings except in Sandcastle Help Builder
* All Tests Pass
* Useable (except for the absence of real-world hardening)

Prerequisites
=============
VS2017 or environment for dotnet core.
Sandcastle build helper extension (2017.5.15)


Build Documentation
===================

Add sandDoc project (when the solution is persisted with this removed from the .sln file
as the sandDoc project requires the full .NET framework.  It also slows down the build)

Change the targetFramework of the PureDI.csproj from nestandard2.0 to netcoreapp2.0 before building
docs and back afterwards.  Do Rebuild Solution - but note the issue below.

The Help Build barfs (unable to find the System assembly) when attempting to build
with a target of netstandard2.0.

The Sandcastle Help Builder is version 2017.5.15

Help Builder Gotcha: make sure you change the text of Version History
when you add version.  Simply adding the veersion using ContentLayout.content
is not sufficient.  

Sometimes on _Rebuild Solution_ the SandDoc project will fail to find PureDI.dll - to fix
follow _Rebuild Solution_ with _Build Solution_ and it should build OK.
Presumably this relates to concurrent builds in some way.

Nuspec
======

nuspec pack fails
dotnet pack & msbuild /t:pack produce a file but ignore our 
  nuspec package which has to be merged manually

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
17. merge branch into masterr
18. push master
19. Upload latest help files to mikedamay.co.uk
20. Test nuget install on Mac or Linux or Windows
