using System;
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
            var tree = MakeTree();
            //byte[] by = File.ReadAllBytes(@"c:\projects\SimpleIOCContainer\SimpleIOCContainerTest\Test1.cs");
            //string str = new String(Encoding.UTF8.GetChars(by));
            //tree = CSharpSyntaxTree.ParseText(str);
            CSharpCompilation cmp = CSharpCompilation.Create(
                "LoadTest").AddReferences(new []
                {
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
                    ,MetadataReference.CreateFromFile(typeof(SimpleIOCContainer).Assembly.Location)
                })
                .AddSyntaxTrees(tree).WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            var ms = new MemoryStream();
            cmp.Emit(ms);
            Assembly.Load(ms.GetBuffer());
            System.Diagnostics.Debug.WriteLine(tree);
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
                List<MemberDeclarationSyntax>(
                    new MemberDeclarationSyntax[]{
            //ClassDeclaration("Level1")
            //.WithAttributeLists(
            //    SingletonList<AttributeListSyntax>(
            //        AttributeList(
            //            SingletonSeparatedList<AttributeSyntax>(
            //                Attribute(
            //                    IdentifierName("Bean"))))))
            //.WithModifiers(
            //    TokenList(
            //        Token(SyntaxKind.PublicKeyword)))
            //.WithMembers(
            //    List<MemberDeclarationSyntax>(
            //        new MemberDeclarationSyntax[]{
            //            FieldDeclaration(
            //                VariableDeclaration(
            //                    IdentifierName("Level1a"))
            //                .WithVariables(
            //                    SingletonSeparatedList<VariableDeclaratorSyntax>(
            //                        VariableDeclarator(
            //                            Identifier("childOne")))))
            //            .WithAttributeLists(
            //                SingletonList<AttributeListSyntax>(
            //                    AttributeList(
            //                        SingletonSeparatedList<AttributeSyntax>(
            //                            Attribute(
            //                                IdentifierName("BeanReference"))))))
            //            .WithModifiers(
            //                TokenList(
            //                    Token(SyntaxKind.PrivateKeyword))),
            //            FieldDeclaration(
            //                VariableDeclaration(
            //                    IdentifierName("Level1b"))
            //                .WithVariables(
            //                    SingletonSeparatedList<VariableDeclaratorSyntax>(
            //                        VariableDeclarator(
            //                            Identifier("childTwo")))))
            //            .WithAttributeLists(
            //                SingletonList<AttributeListSyntax>(
            //                    AttributeList(
            //                        SingletonSeparatedList<AttributeSyntax>(
            //                            Attribute(
            //                                IdentifierName("BeanReference"))))))
            //            .WithModifiers(
            //                TokenList(
            //                    Token(SyntaxKind.PrivateKeyword)))})),
            GetClass("Level1", "Level2a", "Level2b")
            ,GetClass("Level2a","Level2a3a", "Level2a3b")
            ,GetClass("Level2b","Level2b3a", "Level2b3b")
                        ,ClassDeclaration("Level2a3a")
                            .WithAttributeLists(
                                SingletonList<AttributeListSyntax>(
                                    AttributeList(
                                        SingletonSeparatedList<AttributeSyntax>(
                                            Attribute(
                                                IdentifierName("Bean"))))))
                            .WithModifiers(
                                TokenList(
                                    Token(SyntaxKind.PublicKeyword))),
                        ClassDeclaration("Level2a3b")
                            .WithAttributeLists(
                                SingletonList<AttributeListSyntax>(
                                    AttributeList(
                                        SingletonSeparatedList<AttributeSyntax>(
                                            Attribute(
                                                IdentifierName("Bean"))))))
                            .WithModifiers(
                                TokenList(
                                    Token(SyntaxKind.PublicKeyword))),
                        ClassDeclaration("Level2b3a")
                            .WithAttributeLists(
                                SingletonList<AttributeListSyntax>(
                                    AttributeList(
                                        SingletonSeparatedList<AttributeSyntax>(
                                            Attribute(
                                                IdentifierName("Bean"))))))
                            .WithModifiers(
                                TokenList(
                                    Token(SyntaxKind.PublicKeyword))),
                        ClassDeclaration("Level2b3b")
                            .WithAttributeLists(
                                SingletonList<AttributeListSyntax>(
                                    AttributeList(
                                        SingletonSeparatedList<AttributeSyntax>(
                                            Attribute(
                                                IdentifierName("Bean"))))))
                            .WithModifiers(
                                TokenList(
                                    Token(SyntaxKind.PublicKeyword)))


                    }))
            .NormalizeWhitespace()
            ;
            SyntaxTree tree = CSharpSyntaxTree.Create(cus);
            return tree;
        }

        private ClassDeclarationSyntax GetClass(string className, string childOneName, string ChildTwoName)
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
    }
}
