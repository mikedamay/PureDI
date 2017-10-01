using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.TheDisappointedProgrammer.IOCC
{
    /// <summary>
    /// <see cref="Diagnostic"/>
    /// </summary>
    /// <usage>
    /// call ToString() to view warnings or AllToString() to view info
    /// </usage>
    public class IOCCDiagnostics
    {
        /// <summary>
        /// severity is applied to groups rather than individual
        /// occurrences.
        /// `Severity.Error` is not currently supported
        /// </summary>
        public enum Severity { Info, Warning, Error }

        /// <summary>
        /// strong hint to library users is that there is no
        /// need to instantiate the IOCCDiagnostics object
        /// </summary>
        protected IOCCDiagnostics()
        {
            
        }
        /// <summary>
        /// diagnostics are separated into groups by cause.
        /// duplicate bean errors go into one group. missing beans are reported in
        /// another.
        /// </summary>
        /// <remarks>The groups can be inspected in the library resource, DiagnosticSchema.xml</remarks>
        public class Group
        {
            public Group(string topic
              , Severity severity
              , string intro, string userGuide
              , string diagnosticTemplate
              , ISet<string> artefactSchema)
            {
                this.topic = topic;
                this.Severity = severity;
                this.Intro = intro;
                this.UserGuide = userGuide;
                this.DiagnosticTemplate = diagnosticTemplate;
                this.ArtefactSchema = artefactSchema;
            }

            public string topic { get; }
            public Severity Severity { get; }
            public string Intro { get; }
            public string UserGuide { get; }
            public string DiagnosticTemplate { get; }
            public ISet<string> ArtefactSchema { get; }
            public List<Diagnostic> Occurrences { get; } = new List<Diagnostic>();

            public Diagnostic CreateDiagnostic()
            {
                return new DiagnosticImpl(this);
            }

            public void Add(Diagnostic diag)
            {
                Occurrences.Add(diag);
            }
        }

        public IDictionary<string, Group> Groups { get; }
          = new Dictionary<string, Group>();

        public bool HasErrors => Groups.Values.Where(
          g => g.Severity == Severity.Error).Any(g => g.Occurrences.Count > 0);
        public bool HasWarnings => Groups.Values.Where(
          g => g.Severity == Severity.Warning).Any(g => g.Occurrences.Count > 0);

        private class DiagnosticImpl : Diagnostic
        {
            public DiagnosticImpl(Group group) : base(group)
            {
                
            }
        }

        string MakeSubstitutions(string diagnosticTemplate, Diagnostic diag)
        {
            string str = diagnosticTemplate;
            foreach (var key in diag.Members.Keys)
            {
                str = str.Replace("{" + key + "}", diag.Members[key]?.ToString());
            }
            return str;
        }
        public string AllToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Diagnostic Report");
            sb.AppendLine();
            sb.AppendLine();
            sb.Append(GetStringForSeverity(Severity.Warning));
            sb.Append(GetStringForSeverity(Severity.Info));
            return sb.ToString();
        }
        public override string ToString()
        {
            string str = "Diagnostic Report" 
              + Environment.NewLine + Environment.NewLine;
            if (!this.HasWarnings)
            {
                str = "There are no diangostic warnings to report";
            }
            else
            {
                str = str + GetStringForSeverity(Severity.Warning);
            }
            str = str + Environment.NewLine + Environment.NewLine
              + "Note that to see information as well as warnings you should call IOCCDiagnostics.AllToString()";
            return str;
        }

        private string GetStringForSeverity(Severity severity)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Group group in Groups.Values.Where(v => v.Severity == severity))
            {
                if (@group.Occurrences.Count == 0)
                {
                    continue;
                }
                sb.Append(@group.topic);
                sb.Append(Environment.NewLine);
                sb.Append(@group.Severity);
                sb.Append(Environment.NewLine);
                sb.Append(@group.Intro);
                sb.Append(Environment.NewLine);
                foreach (dynamic diag in @group.Occurrences)
                {
                    sb.Append(MakeSubstitutions(@group.DiagnosticTemplate, diag));
                    sb.Append(Environment.NewLine);
                }
                sb.Append(@group.UserGuide);
            }
            return sb.ToString();
        }
    }
}
