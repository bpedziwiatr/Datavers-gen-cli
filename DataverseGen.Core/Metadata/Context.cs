namespace DataverseGen.Core.Metadata;

[Serializable]
public class Context
{
	public MappingEntity[] Entities { get; set; }

	public GeneratorInfo Info { get; set; } = new();

	public string Namespace { get; set; }
}