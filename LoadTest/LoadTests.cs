using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using com.TheDisappointedProgrammer.IOCC;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static System.Console;

namespace IOCCTest.LoadTest
{
    public class LoadTest
    {
        public void TestLoad()
        {
            var assembly = BuildAssembly();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int ii = 0; ii < 1; ii++)
            {
                SimpleIOCContainer sic = new SimpleIOCContainer();
                sic.SetAssemblies(assembly.GetName().Name);
                object root = sic.CreateAndInjectDependencies("Level1", out var diagnostics);
                
            }
            sw.Stop();
            WriteLine(sw.Elapsed);
            //WriteLine(diagnostics);
            //Assert.IsNotNull(root);
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
            //System.Diagnostics.Debug.WriteLine(tree);
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
                MakeClass("Level1", "Level12a", "Level12b"),
            };

            void ExtendList(string classNameRoot, int level)
            {
                if (level == 13)
                {
                    string newRoot = $"{classNameRoot}{level}a";
                    list.Add(MakeLeafClass(newRoot));
                    newRoot = $"{classNameRoot}{level}b";
                    list.Add(MakeLeafClass(newRoot));
                }
                else
                {
                    string newRoot = $"{classNameRoot}{level}a";
                    list.Add(MakeClass(newRoot, $"{newRoot}{level + 1}a", $"{newRoot}{level + 1}b"));
                    ExtendList(newRoot, level + 1);
                    newRoot = $"{classNameRoot}{level}b";
                    list.Add(MakeClass(newRoot, $"{newRoot}{level + 1}a", $"{newRoot}{level + 1}b"));
                    ExtendList(newRoot, level + 1);
                }
            }

            ExtendList("Level1", 2);
            return list;

        }
    }
}
