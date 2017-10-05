Documnetation of the library user is at http://mikedamay.co.uk/PureDI

The Sandcastle Help Builder is version 2017.5.15

Help Builder Gotcha: make sure you change the text of Version History
when you add version.  Simply adding the veersion using ContentLayout.content
is not sufficient.  

Sometimes on _Rebuild Solution_ the SandDoc project will fail to find PureDI.dll - to fix
follow _Rebuild Solution_ with _Build Solution_ and it should build OK.
Presumably this relates to concurrent builds in some way.