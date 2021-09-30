using DataverseGen.Core.Config;
using DataverseGen.Core.Generators.Scriban.Templates;
using DataverseGen.Core.Generators.Scriban.Templates.TemplateFileManager;
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
    public class ScribanGenerator : BaseGenerator
    {
        private string _fullOutputPath;

        public ScribanGenerator(string templateName, string outPath, Context context, TemplateEngineModel templateEngineModel)
            : base(templateName, outPath, context, templateEngineModel)
        {
        }

        public override void GenerateTemplate()
        {
            Console.WriteLine(@"Welcome to Scriban template generator");
            Stopwatch stopper = Stopwatch.StartNew();
            _fullOutputPath = $"{Directory.GetCurrentDirectory()}\\{_outPath}\\";
            if (!Directory.Exists(_fullOutputPath))
                Directory.CreateDirectory(_fullOutputPath);
            switch (_templateEngineModel.Type.ToLowerInvariant())
            {
                case "c#":
                    RunCSharpGenerator();

                    break;

                case "ts":
                    RunTypeScriptGenerator();
                    break;

                default:
                    throw new ArgumentOutOfRangeException($"_templateEngineModel.Type",
                        $@"type {_templateEngineModel.Type} not supported use C# or TS");
            }

            Console.WriteLine($@"Generating Scriban template '{_templateName}' elapsed in: {stopper.Elapsed:g}");
            stopper.Stop();
        }

        private void MultiOutputGeneratorCSharp()
        {
            MultipleFileTemplateCsharp templates = new MultipleFileTemplateCsharp(_templateName);
            templates.Init();
            string dir = $"{Directory.GetCurrentDirectory()}\\{_outPath}\\";
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            foreach (MappingEntity entity in _context.Entities)
            {
                WriteEntityTemplateToFile(templates.EntityTemplate, entity, "cs");
                WriteEntityTemplateToFile(templates.FieldsTemplate, entity, "Fields.cs");
                WriteEntityTemplateToFile(templates.EnumsTemplate, entity, "Enums.cs");
            }

            string xrmContextContent = RemoveEmptyLines(templates.XrmContextTemplate.Render(_context));
            File.WriteAllText($"{dir}/XrmServiceContext.cs", xrmContextContent, Encoding.UTF8);
        }

        private void MultiOutputGeneratorTypeScript()
        {
            MultipleFileTemplateTypescript templates = new MultipleFileTemplateTypescript(_templateName);
            templates.Init();

            foreach (MappingEntity entity in _context.Entities)
            {
                WriteEntityTemplateToFile(templates.EntityTemplate, entity, "attributes.ts");
                WriteEntityTemplateToFile(templates.EnumsTemplate, entity, "enums.ts");
                WriteEntityTemplateToFile(templates.RelationshipTemplate, entity, "relationships.ts");
            }
        }

        private static string RemoveEmptyLines(string lines)
        {
            return Regex.Replace(lines, @"^\s*$\n|\r", string.Empty, RegexOptions.Multiline).TrimEnd();
        }

        private void RunCSharpGenerator()
        {
            if (_templateEngineModel.IsSingleOutput)
            {
                SingleFileCSharp();
                return;
            }
            MultiOutputGeneratorCSharp();
        }

        private void RunTypeScriptGenerator()
        {
            if (_templateEngineModel.IsSingleOutput)
            {
                SingleFileTypescript();
                return;
            }
            MultiOutputGeneratorTypeScript();
        }
        private void SingleFileCSharp()
        {
            ParserOptions options = new ParserOptions();
            LexerOptions lexerOptions = new LexerOptions() { Mode = ScriptMode.Default };
            var template = Template.Parse(Encoding.UTF8.GetString(ScribanTemplates.dataversegen_single)
                , null
                , options
                , lexerOptions);
            string content = RemoveEmptyLines(template.Render(_context));
            File.WriteAllText($"{_fullOutputPath}XrmServiceContext.cs", content, Encoding.UTF8);
        }

        private void SingleFileTypescript()
        {
            ParserOptions options = new ParserOptions();
            LexerOptions lexerOptions = new LexerOptions() { Mode = ScriptMode.Default };
            var template = Template.Parse(System.Text.Encoding.UTF8.GetString(ScribanTemplates.dataversegen_single_ts)
                , null
                , options
                , lexerOptions);
            string content = RemoveEmptyLines(template.Render(_context));
            File.WriteAllText($"{_fullOutputPath}dataverse.metadata.ts", content, Encoding.UTF8);
        }

        private void TypescriptOutput()
        {
            Template template = Template.Parse(System.Text.Encoding.UTF8.GetString(ScribanTemplates.Main_ts_entity
                ));
            string dir = $"{Directory.GetCurrentDirectory()}\\{_outPath}\\";
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            foreach (MappingEntity entity in _context.Entities)
            {
                string entityContent = RemoveEmptyLines(template.Render(new { _context.Namespace, _context.Info, Entity = entity }));
                File.WriteAllText($"{dir}/{entity.LogicalName}.ts", entityContent, Encoding.UTF8);
            }
        }

        private void WriteEntityTemplateToFile(Template template, MappingEntity entity, string outputFileSufixWithExtension)
        {
            string entityContent =
                RemoveEmptyLines(template.Render(new { _context.Namespace, _context.Info, Entity = entity }));
            File.WriteAllText($"{_fullOutputPath}/{entity.HybridName}.{outputFileSufixWithExtension}", entityContent, Encoding.UTF8);
        }
    }
}