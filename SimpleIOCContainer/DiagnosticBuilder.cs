﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using com.TheDisappointedProgrammer.IOCC.Common;

namespace com.TheDisappointedProgrammer.IOCC
{
    internal class DiagnosticBuilder
    {
        public DiagnosticBuilder()
        {
            Diagnostics = CreateDiagnostics();
        }
        internal DiagnosticBuilder(IOCCDiagnostics otherDiagnostics)
        {
            Diagnostics = CreateDiagnostics();
            foreach (var otherGroup in otherDiagnostics.Groups)
            {
                IOCCDiagnostics.Group thisGroup = Diagnostics.Groups[otherGroup.Key];
                foreach (var occurrence in otherGroup.Value.Occurrences)
                {
                    thisGroup.Occurrences.Add(occurrence);
                }
            }
        }

        private IOCCDiagnostics CreateDiagnostics()
        {
            string schemaName
                = $"{Common.Common.ResourcePrefix}.Docs.DiagnosticSchema.xml";
            using (Stream s
                = typeof(PDependencyInjector).Assembly.GetManifestResourceStream(schemaName))
            {
                return CreateDiagnosticsFromSchema(s);
            }
        }

        /// <param name="diagnosticSchema">
        ///     XML Text which populates the diagnostics object
        ///     e.g. typeof(PDependencyInjector).Assembly.GetManifestResourceStream(
        ///          $"{Common.ResourcePrefix}.Docs.DiagnosticSchema.xml")
        /// </param>
        public DiagnosticBuilder(Stream diagnosticSchema)
        {
            Diagnostics = CreateDiagnosticsFromSchema(diagnosticSchema);
        }

        private IOCCDiagnostics CreateDiagnosticsFromSchema(Stream diagnosticSchema)
        {
            IOCCDiagnostics diagnostics = new DiagnosticsImpl();
            XElement groupx = new XElement("no-group-detail-avaialable");
            try
            {
                XDocument schema = XDocument.Load(diagnosticSchema);
                IEnumerable<XElement> groups = schema.Element(Constants.DIAGNOSTIC_SCHEMA_ROOT)
                    .Elements("group");
                foreach (var group in groups)
                {
                    groupx = group;
                    string topic;
                    var dg = new IOCCDiagnostics.Group(
                      topic = group.Element("topic").Value
                      , (IOCCDiagnostics.Severity) Enum.Parse(
                      typeof(IOCCDiagnostics.Severity), group.Element("severity").Value)
                      , group.Element("intro").Value
                      , group.Element("background").Value
                      , group.Element("template").Value
                      , group.Element("artefacts").Elements()
                      .ToHashSet(a => a.Name.ToString())
                    );
                    diagnostics.Groups[topic] = dg;
                }
            }
            catch (ArgumentNullException ane)
            {
                throw new IOException($"The diagnostic schema is not valid. See:{Environment.NewLine}"
                                      + groupx, ane);
            }
            catch (NullReferenceException nre)
            {
                throw new Exception($"The diagnostic schema is not valid. See:{Environment.NewLine}"
                                        + groupx, nre);
            }
            catch (XmlException xe)
            {
                throw new Exception($"The diagnostic schema not correct XML."
                                        , xe);
                
            }
            return diagnostics;
        }
        public IOCCDiagnostics Diagnostics { get; }

        private class DiagnosticsImpl : IOCCDiagnostics
        {
            
        }
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