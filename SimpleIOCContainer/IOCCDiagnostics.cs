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
        public enum Severity
        {
            /// <summary>
            /// diagnostic groups which have a severity of Info
            /// record
            /// </summary>
            Info,
            /// <summary>
            /// 
            /// </summary>
             Warning,
             /// <summary>
             /// 
             /// </summary>
            Error
        }

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
            internal Group(string topic
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
            /// <summary>
            /// cause code of this error or information
            /// such as "DuplicateBeans" or "MissingBean"
            /// </summary>
            public string topic { get; }
            /// <summary>
            /// only Seveity.Warning and Severity.Info are used
            /// </summary>
            public Severity Severity { get; }
            /// <summary>
            /// text indicating the nature of the records in this section
            /// </summary>
            public string Intro { get; }
            /// <summary>
            /// deprecated
            /// </summary>
            public string UserGuide { get; }
            /// <summary>
            /// defines how the record of each warning or info will
            /// be displayed
            /// </summary>
            public string DiagnosticTemplate { get; }
            /// <summary>
            /// definition of specific data points to be included in
            /// a record of the warning or info
            /// </summary>
            public ISet<string> ArtefactSchema { get; }
            /// <summary>
            /// actual instances of warnings or infos that have been encountered
            /// </summary>
            public List<Diagnostic> Occurrences { get; } = new List<Diagnostic>();
            /// <summary>
            /// instantiates a record of a warning or inro
            /// Add must be called to include it in the diagnostics object
            /// </summary>
            /// <returns>a dynamic object that the library populates.  it
            /// contains a list fields specified by ArtefactSchema</returns>
            public Diagnostic CreateDiagnostic()
            {
                return new DiagnosticImpl(this);
            }
            /// <summary>
            /// usage is typically:
            /// Diagnostic diag = diagnostics.Groups["SomeGroup"].CreateDiagnostic();
            /// diag.SomeField = SomeDataRelatingToTheWarning;
            /// diagnostics.Groups["SomeGroup"].Add(diag);
            /// </summary>
            /// <param name="diag">a record containing data points relating
            /// to some specific occurrence of this warning or info</param>
            public void Add(Diagnostic diag)
            {
                Occurrences.Add(diag);
            }
        }
        /// <summary>
        /// A static set of groups as defined by //diagnosticSchema/group in
        /// the resource DiagnosticSchema.xml
        /// </summary>
        public IDictionary<string, Group> Groups { get; }
          = new Dictionary<string, Group>();
        /// <summary>
        /// true indicates that some problems have occurred during
        /// the dependency injection process.  These may well
        /// prevent the application operating correctly.
        /// </summary>
        public bool HasWarnings => Groups.Values.Where(
          g => g.Severity == Severity.Warning).Any(g => g.Occurrences.Count > 0);

        private class DiagnosticImpl : Diagnostic
        {
            public DiagnosticImpl(Group group) : base(group)
            {
                
            }
        }

        private string MakeSubstitutions(string diagnosticTemplate, Diagnostic diag)
        {
            string str = diagnosticTemplate;
            foreach (var key in diag.Members.Keys)
            {
                str = str.Replace("{" + key + "}", diag.Members[key]?.ToString());
            }
            return str;
        }
        /// <summary>
        /// Typically called from the debugger for the library user
        /// to investigate problems and see the status of injection.
        /// It could be called to write to a log.
        /// </summary>
        /// <returns>Details of all warnings and infos.
        /// Infos are: assemblies scanned for beans, profiles set by the user,
        /// bean definitions found in the loaded assemblies and bean injections
        /// </returns>
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
        /// <summary>
        /// Can be called from the debugger in case of error or
        /// may be called as part of normal operation to write
        /// to calling application's log file.
        /// </summary>
        /// <returns>Details of warnings encountered during dependency injection</returns>
        public override string ToString()
        {
            string str = "Diagnostic Report" 
              + Environment.NewLine + Environment.NewLine;
            if (!this.HasWarnings)
            {
                str = "There are no diagnostic warnings to report";
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
