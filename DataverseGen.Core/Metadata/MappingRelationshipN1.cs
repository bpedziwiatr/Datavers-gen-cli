using System;
using System.Collections.Generic;
using System.Linq;
using DataverseGen.Core.Extensions;
using Microsoft.Xrm.Sdk.Metadata;

namespace DataverseGen.Core.Metadata
{
	[Serializable]
	public class MappingRelationshipN1 : IMappingRelationship
	{
		public CrmRelationshipAttribute Attribute { get; set; }

		public string LogicalName { get; set; }

		public MappingField Property { get; set; }

		public MappingEntity ToEntity { get; set; }

		public string Type { get; set; }

		public string DisplayName { get; set; }

		public string EntityRole { get; set; }

		public string ForeignKey { get; set; }

		public string HybridName { get; set; }

		public string PrivateName { get; set; }

		public string SchemaName { get; set; }

		public static MappingRelationshipN1 Parse(
			OneToManyRelationshipMetadata rel,
			IList<MappingField> properties)
		{
			MappingField property = properties.First(p => string.Equals(p.Attribute.LogicalName,
				rel.ReferencingAttribute,
				StringComparison.CurrentCultureIgnoreCase));

			string propertyName = property.DisplayName;

			MappingRelationshipN1 result = new MappingRelationshipN1
			{
				Attribute = new CrmRelationshipAttribute
				{
					ToEntity = rel.ReferencedEntity,
					ToKey = rel.ReferencedAttribute,
					FromEntity = rel.ReferencingEntity,
					FromKey = rel.ReferencingAttribute,
					IntersectingEntity = ""
				},

				DisplayName = MetadataNamingExtensions.GetProperVariableName(rel.SchemaName),
				SchemaName = MetadataNamingExtensions.GetProperVariableName(rel.SchemaName),
				LogicalName = rel.ReferencingAttribute,
				HybridName = MetadataNamingExtensions.GetProperVariableName(rel.SchemaName) + "_N1",
				PrivateName = "_n1" + rel.SchemaName.GetEntityPropertyPrivateName(),
				ForeignKey = propertyName,
				Type = MetadataNamingExtensions.GetProperVariableName(rel.ReferencedEntity),
				Property = property,
				EntityRole = "null"
			};

			if (rel.ReferencedEntity != rel.ReferencingEntity)
			{
				return result;
			}

			result.EntityRole = "Microsoft.Xrm.Sdk.EntityRole.Referencing";
			result.DisplayName = "Referencing" + result.DisplayName;

			return result;
		}
	}
}