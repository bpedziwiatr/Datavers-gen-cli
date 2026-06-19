using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace DataverseGen.Core.CustomApi;

[Serializable]
[DataContract]
public class CustomApiModel
{
	[JsonProperty]
	public string Id { get; set; }

	[JsonProperty]
	public string UniqueName { get; set; }

	[JsonProperty]
	public string DisplayName { get; set; }

	[JsonProperty]
	public string Description { get; set; }

	[JsonProperty]
	public string OperationName { get; set; }

	[JsonProperty]
	public string BindingType { get; set; }

	[JsonProperty]
	public string BoundEntityLogicalName { get; set; }

	[JsonProperty]
	public string ExecutePrivilegeName { get; set; }

	[JsonProperty]
	public bool IsFunction { get; set; }

	[JsonProperty]
	public bool IsPrivate { get; set; }

	[JsonProperty]
	public string RequestClassName { get; set; }

	[JsonProperty]
	public CustomApiParameterModel[] RequestParameters { get; set; } = Array.Empty<CustomApiParameterModel>();

	[JsonProperty]
	public CustomApiParameterModel[] ResponseProperties { get; set; } = Array.Empty<CustomApiParameterModel>();
}
