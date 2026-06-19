using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace DataverseGen.Core.CustomApi;

[Serializable]
[DataContract]
public class CustomApiParameterModel
{
	[JsonProperty]
	public string Id { get; set; }

	[JsonProperty]
	public string CustomApiId { get; set; }

	[JsonProperty]
	public string UniqueName { get; set; }

	[JsonProperty]
	public string DisplayName { get; set; }

	[JsonProperty]
	public string Description { get; set; }

	[JsonProperty]
	public string Type { get; set; }

	[JsonProperty]
	public string LogicalEntityName { get; set; }

	[JsonProperty]
	public bool IsOptional { get; set; }

	[JsonProperty]
	public int Position { get; set; }

	[JsonProperty]
	public string PropertyName { get; set; }

	[JsonProperty]
	public string RequestPropertyName { get; set; }

	[JsonProperty]
	public string ConstructorParameterName { get; set; }

	[JsonProperty]
	public string TypeScriptType { get; set; }

	[JsonProperty]
	public string CSharpTypeName { get; set; }

	[JsonProperty]
	public string WebApiTypeName { get; set; }

	[JsonProperty]
	public string WebApiStructuralProperty { get; set; }
}
