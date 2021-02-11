using System;
using System.Linq;
using DataverseGen.Core.Extensions;
using Microsoft.Xrm.Sdk.Metadata;

namespace DataverseGen.Core.Metadata
{
    [Serializable]
    public class MappingRelationship1N
    {
        public CrmRelationshipAttribute Attribute { get; set; }
        public string DisplayName { get; set; }
        public string ForeignKey { get; set; }
        public string LogicalName { get; set; }
        public string SchemaName { get; set; }
        public string HybridName { get; set; }
        public string PrivateName { get; set; }
        public string EntityRole { get; set; }
        public string Type { get; set; }
        public MappingEntity ToEntity { get; set; }

        public static MappingRelationship1N Parse(OneToManyRelationshipMetadata rel, MappingField[] properties)
        {
            string propertyName =
                properties.First(p => p.Attribute.LogicalName.ToLower() == rel.ReferencedAttribute.ToLower()).DisplayName;

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
                DisplayName = Naming.GetProperVariableName(rel.SchemaName),
                SchemaName = Naming.GetProperVariableName(rel.SchemaName),
                LogicalName = rel.ReferencingAttribute,
                PrivateName = Naming.GetEntityPropertyPrivateName(rel.SchemaName),
                HybridName = Naming.GetPluralName(Naming.GetProperVariableName(rel.SchemaName)),
                EntityRole = "null",
                Type = Naming.GetProperVariableName(rel.ReferencingEntity),
            };

            if (rel.ReferencedEntity == rel.ReferencingEntity)
            {
                result.DisplayName = "Referenced" + result.DisplayName;
                result.EntityRole = "Microsoft.Xrm.Sdk.EntityRole.Referenced";
            }

            return result;
        }
    }
}
