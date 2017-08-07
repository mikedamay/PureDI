using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace com.TheDisappointedProgrammer.IOCC
{
    internal class DiagnosticBuilder
    {
        public DiagnosticBuilder()
        {
            string schemaName
                = "com.TheDisappointedProgrammer.IOCC.Docs.DiagnosticSchema.xml";
            using (Stream s
                = typeof(IOCC).Assembly.GetManifestResourceStream(schemaName))
            {
                Diagnostics = CreateDiagnosticsFromSchema(s);
            }
        }
        /// <param name="diagnosticSchema">
        ///     XML Text which populates the diagnostics object
        ///     e.g. typeof(IOCC).Assembly.GetManifestResourceStream(
        ///          "com.TheDisappointedProgrammer.IOCC.Docs.DiagnosticSchema.xml")
        /// </param>
        public DiagnosticBuilder(Stream diagnosticSchema)
        {
            Diagnostics = CreateDiagnosticsFromSchema(diagnosticSchema);
        }

        private IOCCDiagnostics CreateDiagnosticsFromSchema(Stream diagnosticSchema)
        {
            IOCCDiagnostics diagnostics = new IOCCDiagnostics();
            XElement groupx = new XElement("no-group-detail-avaialable");
            try
            {
                XDocument schema = XDocument.Load(diagnosticSchema);
                IEnumerable<XElement> groups = schema.Element("diagnosticSchema")
                    .Elements("group");
                foreach (var group in groups)
                {
                    groupx = group;
                    string causeCode;
                    var dg = new IOCCDiagnostics.Group(
                      causeCode = group.Element("causeCode").Value
                      , (IOCCDiagnostics.Severity) Enum.Parse(
                      typeof(IOCCDiagnostics.Severity), group.Element("severity").Value)
                      , group.Element("intro").Value
                      , group.Element("userGuide").Value
                      , group.Element("template").Value
                      , group.Element("artefacts").Elements()
                      .ToHashSet(a => a.Name.ToString())
                    );
                    diagnostics.Groups[causeCode] = dg;
                }
            }
            catch (ArgumentNullException ane)
            {
                throw new IOException($"The diagnostic schema is not valid. See:{Environment.NewLine}"
                                      + groupx, ane);
            }
            catch (NullReferenceException nre)
            {
                throw new IOCCException($"The diagnostic schema is not valid. See:{Environment.NewLine}"
                                        + groupx, nre);
            }
            catch (XmlException xe)
            {
                throw new IOCCException($"The diagnostic schema not correct XML."
                                        , xe);
                
            }
            return diagnostics;
        }
        public IOCCDiagnostics Diagnostics { get; }
    }

    internal static class DiagnosticExtensions
    {
        public static HashSet<TKey> 
          ToHashSet<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)            
        {
            if (source == null) throw new ArgumentNullException("source");
            if (keySelector == null) throw new ArgumentNullException("keySelector");
            HashSet<TKey> d = new HashSet<TKey>();
            foreach (TSource element in source) d.Add(keySelector(element));
            return d;
        }
        
    }
}