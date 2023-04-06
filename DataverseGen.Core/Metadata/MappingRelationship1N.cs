using System;
using System.Collections.Generic;
using System.Linq;
using DataverseGen.Core.Extensions;
using Microsoft.Xrm.Sdk.Metadata;

namespace DataverseGen.Core.Metadata
{
	[Serializable]
	public class MappingRelationship1N : IMappingRelationship
	{
		public CrmRelationshipAttribute Attribute { get; set; }

		public string LogicalName { get; set; }

		public string Type { get; set; }

		public MappingEntity ToEntity { get; set; }

		public string EntityRoleSchema => EntityRole == "null" ? "" : ", " + EntityRole;

		public string DisplayName { get; set; }

		public string ForeignKey { get; set; }

		public string SchemaName { get; set; }

		public string HybridName { get; set; }

		public string PrivateName { get; set; }

		public string EntityRole { get; set; }

		public static MappingRelationship1N Parse(
			OneToManyRelationshipMetadata rel,
			IList<MappingField> properties)
		{
			string propertyName =
				properties.First(p => string.Equals(p.Attribute.LogicalName,
							   rel.ReferencedAttribute,
							   StringComparison.CurrentCultureIgnoreCase))
						  .DisplayName;

			MappingRelationship1N result = new MappingRelationship1N
			{
				Attribute = new CrmRelationshipAttribute
				{
					FromEntity = rel.ReferencedEntity,
					FromKey = rel.ReferencedAttribute,
					ToEntity = rel.ReferencingEntity,
					ToKey = rel.ReferencingAttribute,
					IntersectingEntity = ""
				},
				ForeignKey = propertyName,
				DisplayName = MetadataNamingExtensions.GetProperVariableName(rel.SchemaName),
				SchemaName = MetadataNamingExtensions.GetProperVariableName(rel.SchemaName),
				LogicalName = rel.ReferencingAttribute,
				PrivateName = rel.SchemaName.GetEntityPropertyPrivateName(),
				HybridName = MetadataNamingExtensions.GetProperVariableName(rel.SchemaName)
													 .GetPluralName(),
				EntityRole = "null",
				Type = MetadataNamingExtensions.GetProperVariableName(rel.ReferencingEntity)
			};

			if (rel.ReferencedEntity != rel.ReferencingEntity)
			{
				return result;
			}

			result.DisplayName = "Referenced" + result.DisplayName;
			result.EntityRole = "Microsoft.Xrm.Sdk.EntityRole.Referenced";

			return result;
		}
	}
}