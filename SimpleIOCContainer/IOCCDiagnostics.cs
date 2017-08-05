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
    public class IOCCDiagnostics
    {
        public enum Severity { Info, Warning, Error }

        public class Group
        {
            public Group(string causeCode
              , Severity severity
              , string intro, string userGuide
              , string diagnosticTemplate
              , ISet<string> artefactSchema)
            {
                this.CauseCode = causeCode;
                this.Severity = severity;
                this.Intro = intro;
                this.UserGuide = userGuide;
                this.DiagnosticTemplate = diagnosticTemplate;
                this.ArtefactSchema = artefactSchema;
            }

            public string CauseCode { get; }
            public Severity Severity { get; }
            public string Intro { get; }
            public string UserGuide { get; }
            public string DiagnosticTemplate { get; }
            public ISet<string> ArtefactSchema { get; }
            public List<Diagnostic> Errors { get; } = new List<Diagnostic>();

            public Diagnostic CreateDiagnostic()
            {
                return new DiagnosticImpl(this);
            }

            public void Add(Diagnostic diag)
            {
                Errors.Add(diag);
            }
        }

        public IDictionary<string, Group> Groups { get; }
          = new Dictionary<string, Group>();

        private class DiagnosticImpl : Diagnostic
        {
            public DiagnosticImpl(Group group) : base(group)
            {
                
            }
        }

        public override string ToString()
        {
            string MakeSubstitutions(string diagnosticTemplate, Diagnostic diag)
            {
                string str = diagnosticTemplate;
                foreach (var key in diag.Members.Keys)
                {
                    str = str.Replace("{" + key + "}", diag.Members[key].ToString());
                }
                return str;
            }
            StringBuilder sb = new StringBuilder();
            foreach (Group group in Groups.Values)
            {
                if (group.Errors.Count == 0)
                {
                    continue;
                }
                sb.Append(group.CauseCode);
                sb.Append(Environment.NewLine);
                sb.Append(group.Severity);
                sb.Append(Environment.NewLine);
                sb.Append(group.Intro);
                sb.Append(Environment.NewLine);
                foreach (dynamic diag in group.Errors)
                {
                    sb.Append(MakeSubstitutions(group.DiagnosticTemplate, diag));
                    sb.Append(Environment.NewLine);
                }
            }
            return sb.ToString();
        }
    }
}
