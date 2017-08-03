using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.TheDisappointedProgrammer.IOCC
{
   public class IOCCDiagnostics
    {
        public enum DiagnosticType { DuplicateDependency = 99001}
        public enum DiagnosticSeverity { Info, Warning, Error }

        public class Group
        {
            public Group(DiagnosticType diagnosticType
              , DiagnosticSeverity diagnosticSeverity
              , string header, string footer)
            {
                this.DiagnosticType = diagnosticType;
                this.DiagnosticSeverity = diagnosticSeverity;
                this.Header = header;
                this.Footer = footer;
            }

            public DiagnosticType DiagnosticType { get; }
            public DiagnosticSeverity DiagnosticSeverity { get; }
            public string Header { get; }
            public string Footer { get; }
            public IList<string> Occurrences { get; } = new List<string>();
        }

        public IList<Group> Groups { get; } = new List<Group>
        {
            new Group(DiagnosticType.DuplicateDependency, DiagnosticSeverity.Info, "", "")
        };
    }
}
