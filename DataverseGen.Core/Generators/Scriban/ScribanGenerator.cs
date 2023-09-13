using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using DataverseGen.Core.Config;
using DataverseGen.Core.Generators.Scriban.Templates;
using DataverseGen.Core.Generators.Scriban.Templates.TemplateFileManager;
using DataverseGen.Core.Metadata;
using Scriban;
using Scriban.Parsing;

namespace DataverseGen.Core.Generators.Scriban;

public partial class ScribanGenerator : BaseGenerator
{
	private string _fullOutputPath;

	public ScribanGenerator(
		string templateName,
		string templateDirName,
		string outPath,
		Context context,
		TemplateEngineModel templateEngineModel)
		: base(templateName,
			templateDirName,
			outPath,
			context,
			templateEngineModel) { }

	public override void GenerateTemplate()
	{
		Console.WriteLine(@"Welcome to Scriban template generator");
		Stopwatch stopper = Stopwatch.StartNew();
		_fullOutputPath = $"{Directory.GetCurrentDirectory()}\\{OutPath}\\";

		if (!Directory.Exists(_fullOutputPath))
		{
			Directory.CreateDirectory(_fullOutputPath);
		}

		switch (TemplateEngineModel.Type.ToLowerInvariant())
		{
			case "c#":
				RunCSharpGenerator();

				break;

			case "ts":
				RunTypeScriptGenerator();

				break;

			default:
				throw new ArgumentOutOfRangeException("_templateEngineModel.Type",
					$@"type {TemplateEngineModel.Type} not supported use C# or TS");
		}

		Console.WriteLine($@"Generating Scriban template '{TemplateName}' elapsed in: {stopper.Elapsed:g}");
		stopper.Stop();
	}

	private static string RemoveEmptyLines(string lines)
	{
		return RemoveEmptyLinesRegexCompiled()
		   .Replace(lines, string.Empty)
		   .TrimEnd();
	}

	private void MultiOutputGeneratorCSharp()
	{
		MultipleFileTemplateCsharp templates = new(TemplateName, TemplateDirName);
		templates.Init();
		string dir = $"{Directory.GetCurrentDirectory()}\\{OutPath}\\";

		if (!Directory.Exists(dir))
		{
			Directory.CreateDirectory(dir);
		}

		foreach (MappingEntity entity in Context.Entities)
		{
			WriteEntityTemplateToFile(templates.EntityTemplate, entity, "cs");
			WriteEntityTemplateToFile(templates.FieldsTemplate, entity, "Fields.cs");
			WriteEntityTemplateToFile(templates.EnumsTemplate, entity, "Enums.cs");
		}

		string xrmContextContent =
			RemoveEmptyLines(templates.XrmContextTemplate.Render(Context));

		File.WriteAllText($"{dir}/XrmServiceContext.cs", xrmContextContent, Encoding.UTF8);
	}

	private void MultiOutputGeneratorTypeScript()
	{
		MultipleFileTemplateTypescript templates = new(TemplateName, TemplateDirName);
		templates.Init();

		foreach (MappingEntity entity in Context.Entities)
		{
			WriteEntityTemplateToFile(templates.EntityTemplate, entity, "attributes.ts");
			WriteEntityTemplateToFile(templates.EnumsTemplate, entity, "enums.ts");
			WriteEntityTemplateToFile(templates.RelationshipTemplate,
				entity,
				"relationships.ts");
		}
	}

	private void RunCSharpGenerator()
	{
		if (TemplateEngineModel.IsSingleOutput)
		{
			SingleFileCSharp();

			return;
		}

		MultiOutputGeneratorCSharp();
	}

	private void RunTypeScriptGenerator()
	{
		if (TemplateEngineModel.IsSingleOutput)
		{
			SingleFileTypescript();

			return;
		}

		MultiOutputGeneratorTypeScript();
	}

	private void SingleFileCSharp()
	{
		ParserOptions options = new();
		LexerOptions lexerOptions = new()
		{
			Mode = ScriptMode.Default
		};

		Template template = Template.Parse(Encoding.UTF8.GetString(ScribanTemplates.dataversegen_single),
			null,
			options,
			lexerOptions);

		string content = RemoveEmptyLines(template.Render(Context));
		File.WriteAllText($"{_fullOutputPath}XrmServiceContext.cs", content, Encoding.UTF8);
	}

	private void SingleFileTypescript()
	{
		ParserOptions options = new();
		LexerOptions lexerOptions = new()
		{
			Mode = ScriptMode.Default
		};

		Template template = Template.Parse(Encoding.UTF8.GetString(ScribanTemplates.dataversegen_single_ts),
			null,
			options,
			lexerOptions);

		string content = RemoveEmptyLines(template.Render(Context));
		File.WriteAllText($"{_fullOutputPath}dataverse.metadata.ts", content, Encoding.UTF8);
	}

	private void WriteEntityTemplateToFile(
		Template template,
		MappingEntity entity,
		string outputFileSuffixWithExtension)
	{
		string entityContent =
			RemoveEmptyLines(template.Render(new
				{
					Context.Namespace,
					Context.Info,
					Entity = entity
				}))
			   .Replace("\n", "\r\n");

		if (outputFileSuffixWithExtension.Contains("ts"))
		{
			entityContent = $"{entityContent}\r\n";
		}

		File.WriteAllText($"{_fullOutputPath}/{entity.HybridName.ToLower()}.{outputFileSuffixWithExtension}",
			entityContent,
			Encoding.UTF8);
	}

	[GeneratedRegex("^\\s*$\\n|\\r", RegexOptions.Multiline)]
	private static partial Regex RemoveEmptyLinesRegexCompiled();
}