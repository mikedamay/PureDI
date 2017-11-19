using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using PureDI;

namespace SimpleIOCCDocumentor
{
    internal interface ICodeFetcher
    {
        string SubstituteCode(string doc);
    }
    [Bean(Profile = "noop-codefetcher")]
    internal class NoopCodeFetcher : ICodeFetcher
    {
        public string SubstituteCode(string doc)
        {
            return doc;
        }
    }
    [Bean]
    internal class CodeFetcher : ICodeFetcher
    {
        private const string samplePath = @"../../../../SampleCode";
        public string SubstituteCode(string doc)
        {
            string resultDoc = null;
            void GetCodeText(string codefile, string pre, string post)
            {
                string codeText = PrepareCode(GetCodeFromFile(codefile));
                resultDoc = string.IsNullOrWhiteSpace(codefile) ? doc 
                  : new StringBuilder().Append(pre).Append(codeText).Append( post).ToString();
            }
            ExtractCodeToken(doc, GetCodeText);
            return resultDoc;
        }

        private string PrepareCode(string codeText)
        {
            Regex regex = new Regex("(?<displaycode>.*)namespace.*$", RegexOptions.Singleline);
            Match match = regex.Match(codeText);
            string str 
              = match.Groups.Where(g => g.Name == "displaycode")
              .Select(g => g.Value).FirstOrDefault();
            return string.IsNullOrWhiteSpace(str) ? codeText : $"{str}";
        }

        /// <summary>
        /// this depends on the documentor and sample code must retain the positions
        /// that are conventional in the project status
        /// </summary>
        /// <param name="codefile"></param>
        /// <returns></returns>
        private string GetCodeFromFile(string codefile)
        {
            if (codefile == null)
            {
                return string.Empty;
            }
            string codeFilePath 
              = Path.Combine(Path.Combine(
              Path.GetDirectoryName(this.GetType().Assembly.Location), samplePath
              ), codefile);
            String str = File.ReadAllText(codeFilePath);
            return str;
        }
        
        /// <summary>
        /// currently used only as a subroutine of SubstituteCode
        /// uses regex to analyse a document potentially containing a token identifying some sample code
        /// the token looks like this:
        ///   
        ///     {code-introduction.cs}
        /// 
        /// The contents of the file identified by the token are substituted for the token
        /// </summary>
        /// <param name="doc">the doc can contain at most 1 instance of "{code-xxx.cs}"</param>
        /// <param name="stitchTogether">the function passed in stitches together the 3 strings
        /// identified by this method and makes the resulting string available within the closure.
        /// Before concatenating the strings it massages the actual code bit.</param>
        private void ExtractCodeToken(string doc, Action<string, string, string> stitchTogether)
        {
            string codeToken = string.Empty;
            Regex regex 
              = new Regex(@"(?<pre>.*)\{code-(?<codefile>[A-Za-z\.]*)\}(?<post>.*)"
              , RegexOptions.Singleline);
            Match match = regex.Match(doc);
            stitchTogether(
                match.Groups.Where(g => g.Name == "codefile").Select(g => g.Value).FirstOrDefault()
                ,match.Groups.Where(g => g.Name == "pre").Select(g => g.Value).FirstOrDefault()
                ,match.Groups.Where(g => g.Name == "post").Select(g => g.Value).FirstOrDefault()
                );
        }
    }
}