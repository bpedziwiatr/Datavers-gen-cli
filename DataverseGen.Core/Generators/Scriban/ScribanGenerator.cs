using DataverseGen.Core.Metadata;
using Scriban;
using Scriban.Parsing;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace DataverseGen.Core.Generators.Scriban
{
    public class ScribanGenerator
    {
        private readonly Context _context;
        private readonly bool _isSingleOutput;
        private readonly string _outPath;
        private readonly string _templateName;

        public ScribanGenerator(string templateName, string outPath, Context context, bool isSingleOutput = false)
        {
            _templateName = templateName;
            _outPath = outPath;
            _context = context;
            _isSingleOutput = isSingleOutput;
        }

        public void GenerateTemplate()
        {
            Console.WriteLine("Welcome to Scriban template generator");
            Stopwatch stopper = Stopwatch.StartNew();
            if (_isSingleOutput)
                SingleFile();
            else
            {
                MultiOutputGenerator();
            }
            Console.WriteLine($"Generating Scriban template '{_templateName}' elapsed in: {stopper.Elapsed:g}");
            stopper.Stop();
        }

        private void MultiOutputGenerator()
        {
            MultipleFileTemplate templates = new MultipleFileTemplate(_templateName);
            string dir = $"{Directory.GetCurrentDirectory()}\\{_outPath}\\";
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            foreach (MappingEntity entity in _context.Entities)
            {
                string entityContent = RemoveEmptyLines(templates.EntityTemplate.Render(new { _context.Namespace,_context.Info, Entity = entity }));
                File.WriteAllText($"{dir}/{entity.HybridName}.cs", entityContent, Encoding.UTF8);

                string fieldsContent = RemoveEmptyLines(templates.FieldsTemplate.Render(new { _context.Namespace,_context.Info, Entity = entity }));
                File.WriteAllText($"{dir}/{entity.HybridName}.Fields.cs", fieldsContent, Encoding.UTF8);

                string enumsContent = RemoveEmptyLines(templates.EnumsTemplate.Render(new { _context.Namespace,_context.Info, Entity = entity }));
                File.WriteAllText($"{dir}/{entity.HybridName}.Enums.cs", enumsContent, Encoding.UTF8);
            }

            string xrmContextContent = RemoveEmptyLines(templates.XrmContextTemplate.Render(_context));
            File.WriteAllText($"{dir}/XrmServiceContext.cs", xrmContextContent, Encoding.UTF8);
        }

        private string RemoveEmptyLines(string lines)
        {
            return Regex.Replace(lines, @"^\s*$\n|\r", string.Empty, RegexOptions.Multiline).TrimEnd();
        }

        private void SingleFile()
        {
            ParserOptions options = new ParserOptions();
            LexerOptions lexerOptions = new LexerOptions() { Mode = ScriptMode.Default };
            var template = Template.Parse(System.Text.Encoding.UTF8.GetString(ScribanTemplates.dataversegen_single)
                , null
                , options
                , lexerOptions);
            string dir = $"{Directory.GetCurrentDirectory()}\\{_outPath}\\";
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            string content = RemoveEmptyLines(template.Render(_context));
            File.WriteAllText($"{dir}XrmServiceContext.cs", content, Encoding.UTF8);
        }
    }
}