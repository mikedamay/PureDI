using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using com.TheDisappointedProgrammer.IOCC;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace IOCCTest.LoadTest
{
    [TestClass]
    public class LoadTest
    {
        [TestMethod]
        public void TestLoad()
        {
            var assembly = BuildAssembly();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            SimpleIOCContainer sic = new SimpleIOCContainer();
            sic.SetAssemblies(assembly.GetName().Name);
            object root = sic.CreateAndInjectDependencies("Level1", out var diagnostics);
            sw.Stop();
            System.Diagnostics.Debug.WriteLine(sw.Elapsed);
            System.Diagnostics.Debug.WriteLine(diagnostics);
            Assert.IsNotNull(root);
        }

        private Assembly BuildAssembly()
        {
            var tree = MakeTree();
            //byte[] by = File.ReadAllBytes(@"c:\projects\SimpleIOCContainer\SimpleIOCContainerTest\Test1.cs");
            //string str = new String(Encoding.UTF8.GetChars(by));
            //tree = CSharpSyntaxTree.ParseText(str);
            CSharpCompilation cmp = CSharpCompilation.Create(
                    "LoadTest").AddReferences(new[]
                {
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(SimpleIOCContainer).Assembly.Location)
                })
                .AddSyntaxTrees(tree).WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            var ms = new MemoryStream();
            cmp.Emit(ms);
            Assembly assembly = Assembly.Load(ms.GetBuffer());
            System.Diagnostics.Debug.WriteLine(tree);
            return assembly;
        }

        private SyntaxTree MakeTree()
        {
            CompilationUnitSyntax cus = CompilationUnit()
            .WithUsings(
                SingletonList<UsingDirectiveSyntax>(
                    UsingDirective(
                        QualifiedName(
                            QualifiedName(
                                IdentifierName("com"),
                                IdentifierName("TheDisappointedProgrammer")),
                            IdentifierName("IOCC")))))
            .WithMembers(
                List<MemberDeclarationSyntax>( MakeClassList().ToArray()

                    //}
        ))
            .NormalizeWhitespace()
            ;
            SyntaxTree tree = CSharpSyntaxTree.Create(cus);
            return tree;
        }

        private ClassDeclarationSyntax MakeClass(string className, string childOneName, string ChildTwoName)
        {
            return ClassDeclaration(className)
                .WithAttributeLists(
                    SingletonList<AttributeListSyntax>(
                        AttributeList(
                            SingletonSeparatedList<AttributeSyntax>(
                                Attribute(
                                    IdentifierName("Bean"))))))
                .WithModifiers(
                    TokenList(
                        Token(SyntaxKind.PublicKeyword)))
                .WithMembers(
                    List<MemberDeclarationSyntax>(
                        new MemberDeclarationSyntax[]
                        {
                            FieldDeclaration(
                                    VariableDeclaration(
                                            IdentifierName(childOneName))
                                        .WithVariables(
                                            SingletonSeparatedList<VariableDeclaratorSyntax>(
                                                VariableDeclarator(
                                                    Identifier("childOne")))))
                                .WithAttributeLists(
                                    SingletonList<AttributeListSyntax>(
                                        AttributeList(
                                            SingletonSeparatedList<AttributeSyntax>(
                                                Attribute(
                                                    IdentifierName("BeanReference"))))))
                                .WithModifiers(
                                    TokenList(
                                        Token(SyntaxKind.PrivateKeyword))),
                            FieldDeclaration(
                                    VariableDeclaration(
                                            IdentifierName(ChildTwoName))
                                        .WithVariables(
                                            SingletonSeparatedList<VariableDeclaratorSyntax>(
                                                VariableDeclarator(
                                                    Identifier("childTwo")))))
                                .WithAttributeLists(
                                    SingletonList<AttributeListSyntax>(
                                        AttributeList(
                                            SingletonSeparatedList<AttributeSyntax>(
                                                Attribute(
                                                    IdentifierName("BeanReference"))))))
                                .WithModifiers(
                                    TokenList(
                                        Token(SyntaxKind.PrivateKeyword)))
                        }));
        }

        private ClassDeclarationSyntax MakeLeafClass(string className)
        {
            return ClassDeclaration(className)
                .WithAttributeLists(
                    SingletonList<AttributeListSyntax>(
                        AttributeList(
                            SingletonSeparatedList<AttributeSyntax>(
                                Attribute(
                                    IdentifierName("Bean"))))))
                .WithModifiers(
                    TokenList(
                        Token(SyntaxKind.PublicKeyword)));
        }

        private List<ClassDeclarationSyntax> MakeClassList()
        {
            List<ClassDeclarationSyntax> list = new List<ClassDeclarationSyntax>
            {
                MakeClass("Level1", "Level2a", "Level2b"),
                MakeClass("Level2a", "Level2a3a", "Level2a3b"),
                MakeClass("Level2b", "Level2b3a", "Level2b3b"),
                MakeLeafClass("Level2a3a"),
                MakeLeafClass("Level2a3b"),
                MakeLeafClass("Level2b3a"),
                MakeLeafClass("Level2b3b")
            };
            return list;

        }
    }
}
