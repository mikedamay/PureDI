using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using com.TheDisappointedProgrammer.IOCC.Tree;

namespace com.TheDisappointedProgrammer.IOCC
{
    internal class BeanValidator
    {
        public void ValidateAssemblies(IList<Assembly> assemblies, IOCCDiagnostics diagnostics)
        {
            DetectUnreachableMembers(assemblies, diagnostics);
            DetectUnreachableConstructors(assemblies, diagnostics);
            DetectNonBeanWithFactoryInterface(assemblies, diagnostics);
            DetectUnreachableStructs(assemblies, diagnostics);
        }

        public void DetectUnreachableMembers(IList<Assembly> assemblies, IOCCDiagnostics diagnostics)
        {
            var typesAndMembers = assemblies.SelectMany(a =>
              a.GetTypes().SelectMany(
              t => t.GetMembers(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance)
              .Select(m => new { type = t, member = m}))).Where(tm => tm.member.IsPropertyOrField());
            var nonBeanAndMembers =
              typesAndMembers.Where(tm => tm.type.IsAbstract || !tm.type.GetCustomAttributes<BeanAttribute>().Any());
            var nonBeanTypesWithBeanReferences =
                nonBeanAndMembers.Where(tm => tm.member.GetCustomAttributes<BeanReferenceAttribute>().Any());
            IOCCDiagnostics.Group group = diagnostics.Groups["UnreachableReference"];
            foreach (var typeAndMember in nonBeanTypesWithBeanReferences)
            {
                dynamic diag = group.CreateDiagnostic();
                diag.Type = typeAndMember.type;
                diag.MemberName = typeAndMember.member.Name;
                diag.MemberType = typeAndMember.member.GetPropertyOrFieldType();
                group.Add(diag);
            }
        }

        public void DetectUnreachableConstructors(IList<Assembly> assemblies, IOCCDiagnostics diagnostics)
        {
            var typesAndConstructors = assemblies.SelectMany(a =>
                a.GetTypes().SelectMany(
                    t => t.GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance)
                        .Select(c => new { type = t, constructor = c })));
            var nonBeanAndConstructors =
                typesAndConstructors.Where(tm => tm.type.IsAbstract || !tm.type.GetCustomAttributes<BeanAttribute>().Any());
            var nonBeanTypesWithBeanConstructors =
                nonBeanAndConstructors.Where(tm => tm.constructor.GetCustomAttributes<ConstructorAttribute>().Any());
            IOCCDiagnostics.Group group = diagnostics.Groups["UnreachableConstructor"];
            foreach (var typeAndMember in nonBeanTypesWithBeanConstructors)
            {
                dynamic diag = group.CreateDiagnostic();
                diag.Type = typeAndMember.type;
                group.Add(diag);
            }
        }
        private void DetectNonBeanWithFactoryInterface(IList<Assembly> assemblies, IOCCDiagnostics diagnostics)
        {
            var classesWithFactoryInterface
                = assemblies.SelectMany(a => a.GetTypes())
                    .Where(t => t.GetInterfaces()
                        .Any(i => i.FullName == typeof(IFactory).FullName));
            var nonBeanFactories
                = classesWithFactoryInterface.Where(c => !c.GetCustomAttributes<BeanAttribute>().Any());
            IOCCDiagnostics.Group group = diagnostics.Groups["NonBeanFactory"];
            foreach (var type in nonBeanFactories)
            {
                dynamic diag = group.CreateDiagnostic();
                diag.Type = type.FullName;
                group.Add(diag);
            }
        }
        private void DetectUnreachableStructs(IList<Assembly> assemblies, IOCCDiagnostics diagnostics)
        {
            var nonBeanStructMembers
                = assemblies.SelectMany(a => a.GetTypes()).SelectMany(t => t.GetMembers().Select(m => new {type = t, member = m}))
                    .Where(tm => tm.member is FieldInfo || tm.member is PropertyInfo)
                    .Where(tm => tm.member.GetPropertyOrFieldType().IsStruct())
                    .Where(tm => !tm.member.GetCustomAttributes<BeanReferenceAttribute>().Any())
                    .Select(tm => new {declaration = tm, structType = tm.member.GetPropertyOrFieldType()})
                    .Where(ds => ds.structType.GetCustomAttributes<BeanAttribute>().Any());
            IOCCDiagnostics.Group group = diagnostics.Groups["UnreachableStruct"];
            foreach (var ds in nonBeanStructMembers)
            {
                dynamic diag = group.CreateDiagnostic();
                diag.DeclaringType = ds.declaration.type.FullName;
                diag.MemberType = ds.structType.FullName;
                diag.MemberName = ds.declaration.member.Name;
                group.Add(diag);
            }
        }


    }
}