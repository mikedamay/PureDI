## PureDI

A simple dependency injection mechanism

#### Version
Beta

If anybody wants to take this for a spin I would be grateful and responsive

#### Documentation
Documentation can be found at [http://mikedamay.co.uk/PureDI]

Library Maintainer Notes at [Documentation/LibraryMaintainerNotes.md](Documentation/LibraryMaintainerNotes.md)


#### Usage
To run tests:  
install dotnetcore 2.1  
cd PureDITest

then  
On Windows  
dotnet test

On Linux  
dotnet test -c LinuxTest

On Mac  
dotnet test -c MacOsTest

all case sensitive

Sample programs:

LoadTest, PureDIDemo  
change to directory and do "dotnet run"

PureDIDocumentor  
cd PureDIDocumentor  
dotnet run

browse to http://localhost:60653/




    TODO documentation:
    TODO API reference
    TODO set up remarks and notes as top level section headings in each topic
    TODO FAQ / Workarounds
    TODO embolden key text
    *** DONE how to handle injection state passed to an injector with a different
    *** DONE set of profiles
    TODO explain how inheritance, factory with bean name, a separate base factory with Ignore
    TODO are combined to support inheritance.  Execute must be virtual.
    TODO in gotchas we claim that a dangling bean reference will be assigned
    TODO its default value - is that true?
    TODO implementations a la IPropertyMap
    *** DONE document that it is not possible to have OS.Any along with OS.Specific
    TODO developer guide: policy on diagnostics and variation for constructors
    TODO assert that attribute parameters are non-null and of the correct type.
    TODO document that we can't handle same type from multiple assemblies using aliases - I think this will defeat the IOCC
    TODO change text on ReadOnlyProperty to mention that this can be set by using the constructor
    TODO test global:: and document that it won't work for root type passed as string
    TODO spell check documentation
    TODO prefix diagnostic header messages with "PureDI: "
    TODO allow empty array to be passed as Profiles, or a member with null, or a member with "" maybe
    TODO bean names, constructor names and profiles are case insensitive
    TODO Implementation:
    *** DONE Move UserGuide.xml from PureDI to PureDiDocumentor
    TODO can we handle an object or type from some assembly as root which is not
    TODO a scanned assembly
    TODO Later: implement constructor parameters
    *** DONE allow arbitrary objects to be attached to the tree.  Provide them with
	*** DONE a BeanSpec.  This should handle the case where there is a single implementation
	*** DONE of some bean with multiple varying parameters.
    N/A DONE Later: built-in factories for environement variables, command line arguments, config files
    *** DONE Later: syntax colorisation in documentation
    TODO Code Analysis
    TODO deal with exceptions on nested calls to CreateAndInject...()
    TODO handle extern alias situations
    TODO Later: move documentation site to TheDisappointedProgrammer.com
    TODO Later: diagnostics combination functionality
    TODO Later: Make constructors so that we can inject readonly properties - maybe - probably not
    TODO Later: handle whether external interfaces are included or excluded from typeMap
    *** N/A Later: add tables to markdown
    TODO add references to help to diagnostics
    TODO rudimentary developer guide
    TODO Later: look at MEF implementations - heard on dnr 8-8-17
    TODO Later: testing in untrusted environments
    TODO Later: ninject
    *** DONE spring
    *** DONE Later: ASP.NET
    TODO Later: Mass Test - 2 days
    TODO Later: red team: deep hierarchies
    TODO Later: red team: mix new and CreateAndInject...
    TODO Later: red team: self registering classes - that are also beans
    TODO Later: DependencyInjector or InjectionState should expose active
    TODO Later: profiles, particularly for use by factories.
	TODO Later: InjectionState should allow objects to be queried using BeanSpec.
	TODO Later:  Multiple items will be returned as appropriate
    TODO Later: allow user to pass a flag to the injector constructor to treat warnings as errors
    TODO add an interface for DependencyInjector
    TODO docs: DI-OddsAndEnds examples for object cycles
    TODO docs: DI-Assemblies example of including a reference to DependencyInjector
    TODO docs: in a factory bean
    *** N/A docs: example of assembly exclusion
    TODO docs: connect up summary and details in DesignRationale
    TODO Later: add debug message for Diagnostics to prevent it showing the view string - maybe
	TODO Add "No lazy initialisation" to the list of limitations.
	TODO Limitation Allow multiple profiles for a bean.  The difficulty here is in testing uniqueness and
	TODO allowing the user to reason about it.  Perhaps set as limitation.
	*** DONE Limitation - no init or cleanup methods
	*** DONE Limitation - no aliasing
	*** DONE Limitation - miss out on all the advantages of an external configuration, i.e orchestration.
	*** DONE Limitation - no lookup-method-injection - rules out flyweight pattern.
	*** DONE Limitation - no request, session, application, websocket or custom bean scope
	*** DONE Limitation - no autowiring.  In Spring autowiring allows the inclusion of objects in the
	*** DONE			  that are not explicitly marked as beans.
	*** DONE Limitation - no equivalent of Spring's PropertyOverrideConfigurer
	*** DONE LImitation - no abitrary method autowired injection.
	*** DONE Limitation - no autowired collections
	*** DONE Limitation - no indirect dependencies
	*** DONE Limitation - beans can be associated with a single profile only
	*** limitations are based on the IOC Container functionality of Spring Core Framework
    *** DONE Factory.Execute will not automatically inject the dependencies
    *** DONE of the object it creates.  The library user must call 
    *** DONE CreateAndInjectDependencies if they want that behaviour
    *** DONE a run-time diagnostic should be produced.
    TODO add test to ClassScraper for invalid beans
    TODO exception should be thrown if Execution fails due to type mismatch.
    TODO investigate why catch in outer function does not trap exception
    TODO thrown in inner function.
    TODO document that factories must take care of any dependency
    TODO injection for member variables.
    *** DONE How does tye cycle guard interact with factories
    *** DONE Validator gives false positives in PureDIDocumentor for IPropertyMap etc.
    TODO document that there are exceptions to the ruie that all containers of bean references
    TODO must be beans - check on BeanValidator