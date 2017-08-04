using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Schema;

namespace com.TheDisappointedProgrammer.IOCC
{
    internal class DiagnosticBuilder
    {
        /// <param name="diagnosticSchema">XML Text which populates the diagnostics object</param>
        public DiagnosticBuilder(Stream diagnosticSchema)
        {
            CreateDiagnosticsFromSchema(diagnosticSchema);
        }

        private void CreateDiagnosticsFromSchema(Stream diagnosticSchema)
        {
            XDocument schema = XDocument.Load(diagnosticSchema);
            IEnumerable<XName> groups = (IEnumerable<XName>)schema.Element("diagnosticSchema")
              .Elements("group").Select( el => el.Name);
            foreach (var group in groups)
            {
                System.Diagnostics.Debug.WriteLine(group);
            }
        }
    }
}