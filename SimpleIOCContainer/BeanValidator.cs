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
    }
}