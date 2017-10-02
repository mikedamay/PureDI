using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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
            PDependencyInjector pdi = new PDependencyInjector(Assemblies : new []{assembly});
            (object root, InjectionState InjectionState) = pdi.CreateAndInjectDependencies("Level1");
            Diagnostics diagnostics = InjectionState.Diagnostics;
            sw.Stop();
            WriteLine(sw.Elapsed);
            WriteLine(diagnostics);
        }

        private Assembly BuildAssembly()
        {
            var tree = MakeTree();
            var refAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic)
                .Where(a => !string.IsNullOrWhiteSpace(a?.Location)).Select(
                    a => MetadataReference.CreateFromFile(a.Location))
                .ToArray();
            CSharpCompilation cmp = CSharpCompilation.Create(
                    "LoadTest").AddReferences(refAssemblies)
                .AddSyntaxTrees(tree).WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            MemoryStream ms = new MemoryStream();
            cmp.Emit(ms);
            Assembly assembly = Assembly.Load(ms.GetBuffer());
            return assembly;
        }

        private SyntaxTree MakeTreeMinor()
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
                    .WithMembers(MakeVanillaWithBeanClass()
                        //List<MemberDeclarationSyntax>(new MemberDeclarationSyntax[]
                        //    {
                        //        MakeClass("Level1", "Level2a", "Level2b")
                        //        ,MakeLeafClass("Level2a")
                        //        ,MakeLeafClass("Level2b")
                        //    })

                        )
                    .NormalizeWhitespace()
                ;
            SyntaxTree tree = CSharpSyntaxTree.Create(cus);
            return tree;
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
                        List<MemberDeclarationSyntax>(MakeClassList().ToArray()

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
            WriteLine($"{list.Count} classes created");
            return list;

        }

        private SyntaxList<MemberDeclarationSyntax> MakeVanillaClass()
        {
            return SingletonList<MemberDeclarationSyntax>(
                ClassDeclaration("Vanilla")
                    .WithModifiers(
                        TokenList(
                            Token(SyntaxKind.PublicKeyword))));
        }
        private SyntaxList<MemberDeclarationSyntax> MakeVanillaWithBeanClass()
        {
            return 
                SingletonList<MemberDeclarationSyntax>(
                ClassDeclaration("Vanilla")
                    .WithModifiers(
                        TokenList(
                            Token(SyntaxKind.PublicKeyword)))
                    .WithAttributeLists(
                        SingletonList<AttributeListSyntax>(
                            AttributeList(
                                SingletonSeparatedList<AttributeSyntax>(
                                    Attribute(
                                        IdentifierName("Bean")))))))
                            ;
        }
    }
}
