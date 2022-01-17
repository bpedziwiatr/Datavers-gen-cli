using System;
using System.Collections.Generic;
using System.Linq;
using DataverseGen.Core.Extensions;
using Microsoft.Xrm.Sdk.Metadata;

//using Microsoft.Xrm.Sdk.Metadata;
//using CrmCodeGenerator.VSPackage.Helpers;

namespace DataverseGen.Core.Metadata
{
    [Serializable]
    public class MappingEntity
    {
        public MappingEntity()
        {
            Description = "";
        }

        public string DescriptionXmlSafe => Description.XmlEscape();
        public string LogicalName => Attribute.LogicalName;
        public string Plural => DisplayName.GetPluralName();

        public CrmEntityAttribute Attribute { get; set; }
        public string Description { get; set; }
        public string DisplayName { get; set; }
        public MappingEnum[] Enums { get; set; }
        public MappingField[] Fields { get; set; }
        public string HybridName { get; set; }
        public bool IsIntersect { get; set; }
        public MappingField PrimaryKey { get; set; }
        public string PrimaryKeyProperty { get; set; }
        public string PrimaryNameAttribute { get; set; }
        public MappingRelationshipMn[] RelationshipsManyToMany { get; set; }
        public MappingRelationshipN1[] RelationshipsManyToOne { get; set; }
        public MappingRelationship1N[] RelationshipsOneToMany { get; set; }
        public string StateName { get; set; }
        public MappingEnum States { get; set; }
        public int? TypeCode { get; set; }

        public static MappingEntity Parse(EntityMetadata entityMetadata)
        {
            MappingEntity entity = new MappingEntity
            {
                Attribute = new CrmEntityAttribute(),
                TypeCode = entityMetadata.ObjectTypeCode
            };

            entity.Attribute.LogicalName = entityMetadata.LogicalName;
            if (entityMetadata.IsIntersect != null) entity.IsIntersect = (bool)entityMetadata.IsIntersect;
            entity.Attribute.PrimaryKey = entityMetadata.PrimaryIdAttribute;

            entity.DisplayName = entityMetadata.GetProperEntityName();
            entity.HybridName = entityMetadata.GetProperHybridName();
            entity.StateName = entity.HybridName + "State";

            if (entityMetadata.Description?.UserLocalizedLabel != null)
                entity.Description = entityMetadata.Description.UserLocalizedLabel.Label;

            List<MappingField> fields = MapFieldsFromEntityMetadata(entityMetadata, entity).ToList();

            fields.ForEach(f =>
                {
                    if (f.DisplayName == entity.DisplayName)
                        f.DisplayName += "1";
                }
            );

            AddEntityImageCrm2013(fields);
            AddLookupFields(fields);

            entity.Fields = fields.ToArray();
            entity.States = entityMetadata.Attributes.Where(a => a is StateAttributeMetadata)
                .Select(a => MappingEnum.Parse(a as EnumAttributeMetadata)).FirstOrDefault();

            entity.Enums = entityMetadata.Attributes
                .Where(a => a is PicklistAttributeMetadata || a is StateAttributeMetadata ||
                            a is StatusAttributeMetadata || a is BooleanAttributeMetadata ||
                            a is MultiSelectPicklistAttributeMetadata)
                .Select(a => MappingEnum.Parse(a)).ToArray();

            entity.PrimaryKey = entity.Fields.First(f => f.Attribute.LogicalName == entity.Attribute.PrimaryKey);
            entity.PrimaryKeyProperty = entity.PrimaryKey.DisplayName;
            entity.PrimaryNameAttribute = entityMetadata.PrimaryNameAttribute;

            entity.RelationshipsOneToMany = entityMetadata.OneToManyRelationships.Select(r =>
                MappingRelationship1N.Parse(r, entity.Fields)).ToArray();

            entity.RelationshipsOneToMany.ToList().ForEach(r =>
            {
                string newName = r.DisplayName;

                if (newName == entity.DisplayName || newName == entity.HybridName)
                    newName = r.DisplayName += "1";

                if (entity.Fields.Any(e => e.DisplayName == newName))
                {
                    r.DisplayName += "2";
                }
            });

            entity.RelationshipsManyToOne = entityMetadata.ManyToOneRelationships.Select(r =>
                MappingRelationshipN1.Parse(r, entity.Fields)).ToArray();

            entity.RelationshipsManyToOne.ToList().ForEach(r =>
            {
                string newName = r.DisplayName;

                if (newName == entity.DisplayName || newName == entity.HybridName)
                    newName = r.DisplayName += "1";

                if (entity.Fields.Any(e => e.DisplayName == newName))
                {
                    r.DisplayName += "2";
                }
            });

            List<MappingRelationshipMn> relationshipsManyToMany = entityMetadata.ManyToManyRelationships
                .Select(r => MappingRelationshipMn.Parse(r, entity.LogicalName)).ToList();
            List<MappingRelationshipMn> selfReferenced =
                relationshipsManyToMany.Where(r => r.IsSelfReferenced).ToList();
            foreach (MappingRelationshipMn referenced in selfReferenced)
            {
                MappingRelationshipMn referencing = (MappingRelationshipMn)referenced.Clone();
                referencing.DisplayName =
                    "Referencing" + MetadataNamingExtensions.GetProperVariableName(referenced.SchemaName);
                referencing.EntityRole = "Microsoft.Xrm.Sdk.EntityRole.Referencing";
                relationshipsManyToMany.Add(referencing);
            }

            relationshipsManyToMany.ForEach(r =>
            {
                string newName = r.DisplayName;

                if (newName == entity.DisplayName || newName == entity.HybridName)
                    newName = r.DisplayName += "1";

                if (entity.Fields.Any(e => e.DisplayName == newName))
                {
                    r.DisplayName += "2";
                }
            });
            entity.RelationshipsManyToMany = relationshipsManyToMany.OrderBy(r => r.DisplayName).ToArray();

            return entity;
        }

        private static void AddEntityImageCrm2013(ICollection<MappingField> fields)
        {
            if (!fields.Any(f => f.DisplayName.Equals("EntityImageId")))
                return;

            MappingField image = new MappingField
            {
                Attribute = new CrmPropertyAttribute
                {
                    IsLookup = false,
                    LogicalName = "entityimage",
                    IsEntityReferenceHelper = false
                },
                DisplayName = "EntityImage",
                HybridName = "EntityImage",
                TargetTypeForCrmSvcUtil = "byte[]",
                IsValidForUpdate = true,
                Description =
                    "", // TODO there is an Description for this entityimage, Need to figure out how to read it from the server
                GetMethod = ""
            };
            SafeAddField(fields, image);

            MappingField imageTimestamp = new MappingField
            {
                Attribute = new CrmPropertyAttribute
                {
                    IsLookup = false,
                    LogicalName = "entityimage_timestamp",
                    IsEntityReferenceHelper = false
                },
                DisplayName = "EntityImage_Timestamp",
                HybridName = "EntityImage_Timestamp",
                TargetTypeForCrmSvcUtil = "System.Nullable<long>",
                FieldType = AttributeTypeCode.BigInt,
                IsValidForUpdate = false,
                IsValidForCreate = false,
                Description = " ", // CrmSvcUtil provides an empty description for this EntityImage_TimeStamp
                GetMethod = ""
            };
            SafeAddField(fields, imageTimestamp);

            MappingField imageUrl = new MappingField
            {
                Attribute = new CrmPropertyAttribute
                {
                    IsLookup = false,
                    LogicalName = "entityimage_url",
                    IsEntityReferenceHelper = false
                },
                DisplayName = "EntityImage_URL",
                HybridName = "EntityImage_URL",
                TargetTypeForCrmSvcUtil = "string",
                FieldType = AttributeTypeCode.String,
                IsValidForUpdate = false,
                IsValidForCreate = false,
                Description = " ", // CrmSvcUtil provides an empty description for this EntityImage_URL
                GetMethod = ""
            };
            SafeAddField(fields, imageUrl);
        }

        private static void AddLookupFields(ICollection<MappingField> fields)
        {
            MappingField[] fieldsIterator = fields.Where(e => e.Attribute.IsLookup).ToArray();
            foreach (MappingField lookup in fieldsIterator)
            {
                MappingField nameField = new MappingField
                {
                    Attribute = new CrmPropertyAttribute
                    {
                        IsLookup = false,
                        LogicalName = lookup.Attribute.LogicalName + "Name",
                        IsEntityReferenceHelper = true
                    },
                    DisplayName = lookup.DisplayName + "Name",
                    HybridName = lookup.HybridName + "Name",
                    FieldType = AttributeTypeCode.EntityName,
                    IsValidForUpdate = false,
                    GetMethod = "",
                    PrivatePropertyName = lookup.PrivatePropertyName + "Name"
                };

                if (fields.Count(f => f.DisplayName == nameField.DisplayName) == 0)
                    fields.Add(nameField);

                if (!string.IsNullOrEmpty(lookup.LookupSingleType))
                    continue;

                MappingField typeField = new MappingField
                {
                    Attribute = new CrmPropertyAttribute
                    {
                        IsLookup = false,
                        LogicalName = lookup.Attribute.LogicalName + "Type",
                        IsEntityReferenceHelper = true
                    },
                    DisplayName = lookup.DisplayName + "Type",
                    HybridName = lookup.HybridName + "Type",
                    FieldType = AttributeTypeCode.EntityName,
                    IsValidForUpdate = false,
                    GetMethod = "",
                    PrivatePropertyName = lookup.PrivatePropertyName + "Type"
                };

                if (fields.Count(f => f.DisplayName == typeField.DisplayName) == 0)
                    fields.Add(typeField);
            }
        }

        private static IEnumerable<MappingField> MapFieldsFromEntityMetadata(EntityMetadata entityMetadata,
            MappingEntity entity)
        {
            foreach (AttributeMetadata attribute in entityMetadata.Attributes.Where(a => a.AttributeOf == null))
            {
                MappingField result = MappingField.Parse(attribute, entity);
                yield return result;
                if (attribute.AttributeTypeName == "FileType" || attribute.AttributeType == AttributeTypeCode.Lookup)
                {
                    yield return result.CreateFileNameField(result);
                }
                
                if (attribute.AttributeType == AttributeTypeCode.Lookup)
                {
                    yield return result.CreateLookupNameField(result);
                }
            }
        }

        private static void SafeAddField(ICollection<MappingField> fields, MappingField image)
        {
            if (fields.All(f => f.DisplayName != image.DisplayName))
                fields.Add(image);
        }
    }
}