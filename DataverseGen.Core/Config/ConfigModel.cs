using System.ComponentModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace DataverseGen.Core.Config;

[Serializable]
[DataContract]
public class ConfigModel
{
	[JsonProperty]
	public string ConnectionString { get; set; }
	[JsonProperty]

	public string[] Entities { get; set; }
	[JsonProperty]

	public string Namespace { get; set; }
	[JsonProperty]

	public string OutDirectory { get; set; }
	[JsonProperty]

	public TemplateEngineModel TemplateEngine { get; set; }
	[JsonProperty]

	public string TemplateName { get; set; }
	[JsonProperty]
	[DefaultValue("Templates")]
	public string TemplateDirectoryName { get; set; }
	[JsonProperty]
	[DefaultValue(true)]
	public bool ThrowOnEntityNotFound { get; set; }
	[JsonProperty]
	[DefaultValue(true)]
	public bool EnableConnectionStringValidation { get; set; }
}

[Serializable]
[DataContract]
public class TemplateEngineModel
{
	[JsonProperty]

	public bool IsSingleOutput { get; set; }

	/// <summary>
	///  scriban/t4
	/// </summary>
	[JsonProperty]
	public string Name { get; set; }

	/// <summary>
	///  TS,C#
	/// </summary>
	[JsonProperty]
	public string Type { get; set; }
}