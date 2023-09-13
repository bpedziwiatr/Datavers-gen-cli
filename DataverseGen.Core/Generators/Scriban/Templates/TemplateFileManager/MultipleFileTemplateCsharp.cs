using System.Text;
using Scriban;

namespace DataverseGen.Core.Generators.Scriban.Templates.TemplateFileManager;

public class MultipleFileTemplateCsharp : MultipleFileTemplateBase
{
	private string _fieldsTemplateFilePath;
	private string _xrmServiceContextTemplateFilePath;

	public MultipleFileTemplateCsharp(string templateName, string templateDirName) :
		base(templateName, templateDirName) { }

	internal Template XrmContextTemplate { get; private set; }

	internal Template FieldsTemplate { get; private set; }

	protected override void CheckFiles()
	{
		base.CheckFiles();
		_xrmServiceContextTemplateFilePath = $"{DirectoryPath}\\xrmservicecontext.sbncs";
		_fieldsTemplateFilePath = $"{DirectoryPath}\\fields.sbncs";

		if (!File.Exists(_xrmServiceContextTemplateFilePath))
		{
			throw new ArgumentException(
				$"Template missing file for xrmServiceContext '{_xrmServiceContextTemplateFilePath}'");
		}

		if (!File.Exists(_fieldsTemplateFilePath))
		{
			throw new ArgumentException($"Template missing file for fields '{_fieldsTemplateFilePath}'");
		}
	}

	protected override void SetMainTemplate()
	{
		base.SetMainTemplate();
		XrmContextTemplate =
			Template.Parse(Encoding.UTF8.GetString(ScribanTemplates.Main_xrmservicecontext));

		FieldsTemplate =
			Template.Parse(Encoding.UTF8.GetString(ScribanTemplates.Main_fields));
	}

	protected override void SetTemplateByFiles()
	{
		base.SetTemplateByFiles();
		XrmContextTemplate = Template.Parse(File.ReadAllText(_xrmServiceContextTemplateFilePath));
		FieldsTemplate = Template.Parse(File.ReadAllText(_fieldsTemplateFilePath));
	}
}