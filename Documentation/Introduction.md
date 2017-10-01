﻿<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <meta http-equiv="cache-control" content="max-age=0" />
    <meta http-equiv="cache-control" content="no-cache" />
    <meta http-equiv="expires" content="0" />
    <meta http-equiv="expires" content="Tue, 01 Jan 1980 1:00:00 GMT" />
    <meta http-equiv="pragma" content="no-cache" />
    <link type="text/png" rel="icon" href="/glyphicons_free/glyphicons/png/glyphicons-30-notes-2.png" />
    <link type="text/css" href="/Content/bootstrap.css" rel="stylesheet" />
</head>
<body>
<div class="container">
    <div class="row">
        <div class="col-sm-12">[Test](/Documentation/Test.md)</div>
    </div>
    <div class="row">
        <div class="col-sm-1"></div>
        <div class="col-sm-10">
            <h3 class="col-sm-10" style="color: blue">Introduction to SimpleIOCContainer</h3>
            <hr class="col-xs-12"/>
        </div>
        <div class="col-sm-1"></div>
    </div>
    <div class="row">
        <div class="col-sm-1"></div>
        <div class="col-sm-10">
            <p>
                The SimpleIOCContainer (Simple Inversion of Control Container) is a
                Dependency Injection (DI) framework promoting simplicity of use and
                rich diagnostics.
            </p>
            <p>
                Usage follows a simple pattern: the caller instantiates some key object
                with a call to <em>SimpleIOCContainer.CreateAndInjectDependnecies</em> and that
                object is instiated together with dependencies to which it refers
                and so forth recursively through the tree of dependencies.
            </p>
            <p>
                The call to <em>CreateAndInjectDependencies</em> is typically made at program
                startup. The framework is principally concerned with singletons that
                have the same life span as the program.
            </p>
            <p>A simple example (Introduction.cs) is as follows:</p>
            <div class="pre-scrollable">
                <pre><code>using com.TheDisappointedProgrammer.IOCC;
[Bean]
public class Program
{
    [BeanReference] private Logger logger = null;
    public static void Main()
    {
        SimpleIOCContainer sic = new SimpleIOCContainer();
        Program prog = sic.CreateAndInjectDependencies&lt;Program&gt;();
        prog.SaySomething();
    }
    private void SaySomething()
    {
        logger.Log(&quot;Hello Simple&quot;);
    }
}
[Bean]
public class Logger
{
    public void Log(string message) =&gt; System.Console.WriteLine(message);
}

</code></pre>
            </div>

            Note:
            <ul>
                <li>Classes to be injected are annotated with the <em>Bean</em> attribute.</li>
                <li>The site of the injection is annotated with the <em>BeanReference</em> attribute</li>
                <li>The member variable to be injected should an interface, a direct or indirect base class of the bean or the same class as the bean</li>
                <li>Once *CreateAndInjectDependencies has been called all beans will have been instantiated and assigned to any matching bean references.</li>
            </ul>
            <p>See Also:</p>
            <p><a href="/diagnosticSchema/MissingBean">Assemblies (or An early gotcha)</a>
            </p>
            <p><a href="/userGuide/Profiles">Profiles (or why bother with DI)</a>
            </p>

        </div>
        <div class="col-sm-1"></div>
    </div>
</div>
</body>
</html>