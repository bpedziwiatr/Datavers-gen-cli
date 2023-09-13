using System.Text;
using Scriban;

namespace DataverseGen.Core.Generators.Scriban.Templates.TemplateFileManager;

public class MultipleFileTemplateTypescript : MultipleFileTemplateBase
{
	private string _relationshipsTemplateFilePath;

	public MultipleFileTemplateTypescript(string templateName, string templateDir) : base(templateName, templateDir) { }

	internal Template RelationshipTemplate { get; private set; }

	protected override void CheckFiles()
	{
		base.CheckFiles();
		_relationshipsTemplateFilePath = $"{DirectoryPath}\\entity-relationship.sbncs";

		if (!File.Exists(_relationshipsTemplateFilePath))
		{
			throw new ArgumentException(
				$"Template missing file for entity-relationship '{_relationshipsTemplateFilePath}'");
		}
	}

	protected override void SetMainTemplate()
	{
		EntityTemplate =
			Template.Parse(Encoding.UTF8.GetString(ScribanTemplates.Main_ts_entity));

		EnumsTemplate =
			Template.Parse(Encoding.UTF8.GetString(ScribanTemplates.Main_ts_enums));

		RelationshipTemplate =
			Template.Parse(Encoding.UTF8.GetString(ScribanTemplates.Main_ts_entity_relationship));
	}

	protected override void SetTemplateByFiles()
	{
		base.SetTemplateByFiles();
		RelationshipTemplate = Template.Parse(File.ReadAllText(_relationshipsTemplateFilePath));
	}
}