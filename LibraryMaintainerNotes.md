Miek May - October 2017

Documnetation of the library user is at http://mikedamay.co.uk/PureDI

Release Procedure
releases are saved to branches:
v0_1_2_0


Build Documentation
===================

Add sandDoc project (the solution is persisted with this removed from the solution
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

