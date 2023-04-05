namespace DataverseGen.Core.Metadata
{
	public interface IMappingRelationship
	{
		string DisplayName { get; set; }

		string EntityRole { get; set; }

		string ForeignKey { get; set; }

		string HybridName { get; set; }

		string PrivateName { get; set; }

		string SchemaName { get; set; }
	}
}