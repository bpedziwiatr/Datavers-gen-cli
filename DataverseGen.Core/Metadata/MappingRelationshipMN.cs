using DataverseGen.Core.Extensions;
using Microsoft.Xrm.Sdk.Metadata;
using System;

namespace DataverseGen.Core.Metadata
{
    [Serializable]
    public class MappingRelationshipMn : ICloneable,IMappingRelationship
    {
        public CrmRelationshipAttribute Attribute { get; set; }

        public string DisplayName { get; set; }
        public string EntityRole { get; set; }
        public string ForeignKey { get; set; }
        public string HybridName { get; set; }

		

		public bool IsSelfReferenced { get; set; }
        public string PrivateName { get; set; }
        public string SchemaName { get; set; }
        public MappingEntity ToEntity { get; set; }
        public string Type { get; set; }
        public static MappingRelationshipMn Parse(ManyToManyRelationshipMetadata rel, string thisEntityLogicalName)
        {
            MappingRelationshipMn result = new MappingRelationshipMn();
            if (rel.Entity1LogicalName == thisEntityLogicalName)
            {
                result.Attribute = new CrmRelationshipAttribute
                {
                    FromEntity = rel.Entity1LogicalName,
                    FromKey = rel.Entity1IntersectAttribute,
                    ToEntity = rel.Entity2LogicalName,
                    ToKey = rel.Entity2IntersectAttribute,
                    IntersectingEntity = rel.IntersectEntityName
                };
            }
            else
            {
                result.Attribute = new CrmRelationshipAttribute
                {
                    ToEntity = rel.Entity1LogicalName,
                    ToKey = rel.Entity1IntersectAttribute,
                    FromEntity = rel.Entity2LogicalName,
                    FromKey = rel.Entity2IntersectAttribute,
                    IntersectingEntity = rel.IntersectEntityName
                };
            }

            result.EntityRole = "null";
            result.SchemaName = MetadataNamingExtensions.GetProperVariableName(rel.SchemaName);
            result.DisplayName = MetadataNamingExtensions.GetProperVariableName(rel.SchemaName);
            if (rel.Entity1LogicalName == rel.Entity2LogicalName && rel.Entity1LogicalName == thisEntityLogicalName)
            {
                result.DisplayName = "Referenced" + result.DisplayName;
                result.EntityRole = "Microsoft.Xrm.Sdk.EntityRole.Referenced";
                result.IsSelfReferenced = true;
            }
            if (result.DisplayName == thisEntityLogicalName)
            {
                result.DisplayName += "1";   // this is what CrmSvcUtil does
            }

            result.HybridName = MetadataNamingExtensions.GetProperVariableName(rel.SchemaName) + "_NN";
            result.PrivateName = "_nn" + rel.SchemaName.GetEntityPropertyPrivateName();
            result.ForeignKey = MetadataNamingExtensions.GetProperVariableName(result.Attribute.ToKey);
            result.Type = MetadataNamingExtensions.GetProperVariableName(result.Attribute.ToEntity);

            return result;
        }

        public object Clone()
        {
            MappingRelationshipMn newPerson = (MappingRelationshipMn)MemberwiseClone();
            newPerson.Attribute = (CrmRelationshipAttribute)Attribute.Clone();
            return newPerson;
        }
    }
}