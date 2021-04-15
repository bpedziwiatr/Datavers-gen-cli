using System.Text.RegularExpressions;
using DataverseGen.Core.Metadata;
using Scriban;
using Scriban.Parsing;

namespace DataverseGen.Core.Generators.Scriban
{
    public class ScribanGenerator
    {
        private readonly Context _context;
        private readonly string _outPath;
        private readonly string _t4TemplateFile;

        public ScribanGenerator(string t4TemplateFile, string outPath, Context context)
        {
            _t4TemplateFile = t4TemplateFile;
            _outPath = outPath;
            _context = context;
        }

        public void GenerateTemplate()
        {

            ParserOptions options = new ParserOptions();
            LexerOptions lexerOptions = new LexerOptions() {Mode = ScriptMode.Default};
            var template = Template.Parse(System.Text.Encoding.UTF8.GetString(ScribanTemplates.dataversegen_single)
                ,null
            ,options
            , lexerOptions);

            var result = RemoveEmptyLines(template.Render(_context));
        }

        private string RemoveEmptyLines(string lines)
        {
            return Regex.Replace(lines, @"^\s*$\n|\r", string.Empty, RegexOptions.Multiline).TrimEnd();
        }
    }
}