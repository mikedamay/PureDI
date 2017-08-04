using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.TheDisappointedProgrammer.IOCC
{
   public class IOCCDiagnostics
    {
        public enum Severity { Info, Warning, Error }

        public class Group
        {
            public Group(string causeCode
              , Severity severity
              , string intro, string userGuide)
            {
                this.CauseCode = causeCode;
                this.Severity = severity;
                this.Intro = intro;
                this.UserGuide = userGuide;
            }

            public string CauseCode { get; }
            public Severity Severity { get; }
            public string Intro { get; }
            public string UserGuide { get; }
            public IList<string> Occurrences { get; } = new List<string>();
        }

        public IList<Group> Groups { get; } = new List<Group>
        {
        };
    }
}
