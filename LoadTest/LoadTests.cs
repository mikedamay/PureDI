﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using PureDI;
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
            ReadLine();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            DependencyInjector pdi = new DependencyInjector();
            (object root, InjectionState InjectionState)
                = pdi.CreateAndInjectDependencies("Level1", assemblies: new Assembly[] { assembly });
            Diagnostics diagnostics = InjectionState.Diagnostics;
            sw.Stop();
            WriteLine(sw.Elapsed);
            WriteLine(diagnostics);
            TestAssembly(assembly);
            ReadLine();
        }

        private static void TestAssembly(Assembly assembly)
        {
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
        //
        // used https://roslynquoter.azurewebsites.net/ to generate syntax tree builder code fragments below
        //
        private SyntaxTree MakeTreeMinor()
        {
            CompilationUnitSyntax cus = CompilationUnit()
                    .WithUsings(
                        SingletonList<UsingDirectiveSyntax>(
                            UsingDirective(IdentifierName("PureDI.Attributes"))))
                    .WithMembers(MakeVanillaWithBeanClass())
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
                                        IdentifierName("PureDI"),
                                        IdentifierName("Attributes")))))
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
