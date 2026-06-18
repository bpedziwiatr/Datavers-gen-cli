namespace DataverseGen.Core.Metadata;

[Serializable]
public class Context
{
	public MappingEntity[] Entities { get; set; }

	public GeneratorInfo Info { get; set; } = new();

	public string Namespace { get; set; }

	public CustomApi.CustomApiModel[] CustomApis { get; set; } = Array.Empty<CustomApi.CustomApiModel>();
}
