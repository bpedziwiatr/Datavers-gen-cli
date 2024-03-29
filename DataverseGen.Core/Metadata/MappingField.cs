﻿using DataverseGen.Core.Extensions;
using DataverseGen.Core.Helpers;
using Microsoft.Xrm.Sdk.Metadata;

namespace DataverseGen.Core.Metadata;

[Serializable]
public class MappingField
{
	public string DescriptionXmlSafe => Description.XmlEscape();

	public CrmPropertyAttribute Attribute { get; set; }

	public string AttributeOf { get; set; }

	public string AttributeTypeName { get; private set; }

	public string DeprecatedVersion { get; set; }

	public string Description { get; set; } = "";

	public string DisplayName { get; set; }

	public MappingEntity Entity { get; set; }

	public MappingEnum EnumData { get; set; }

	public AttributeTypeCode FieldType { get; set; }

	public string FieldTypeString { get; set; }

	public string GetMethod { get; set; }

	public string HybridName { get; set; }

	public bool IsActivityParty { get; set; }

	public bool IsDeprecated { get; set; }

	public bool IsOptionSet { get; private set; }

	public bool IsRequired { get; set; }

	public bool IsStateCode { get; set; }

	public bool IsTwoOption { get; private set; }

	public bool IsValidForCreate { get; set; }

	public bool IsValidForRead { get; set; }

	public bool IsValidForUpdate { get; set; }

	public string Label { get; set; }

	public string LogicalName { get; set; }

	public string LookupSingleType { get; set; }

	public decimal? Max { get; set; }

	public int? MaxLength { get; set; }

	public decimal? Min { get; set; }

	public string PrivatePropertyName { get; set; }

	public string TargetTypeForCrmSvcUtil { get; set; }

	private bool IsPrimaryKey { get; set; }

	public static MappingField Parse(AttributeMetadata attribute, MappingEntity entity)
	{
		MappingField result = new()
		{
			Entity = entity,
			AttributeOf = attribute.AttributeOf
		};

		if (attribute.IsValidForCreate != null)
		{
			result.IsValidForCreate = (bool)attribute.IsValidForCreate;
		}

		if (attribute.IsValidForRead != null)
		{
			result.IsValidForRead = (bool)attribute.IsValidForRead;
		}

		if (attribute.IsValidForUpdate != null)
		{
			result.IsValidForUpdate = (bool)attribute.IsValidForUpdate;
		}

		result.IsActivityParty = attribute.AttributeType == AttributeTypeCode.PartyList;
		result.IsStateCode = attribute.AttributeType == AttributeTypeCode.State;
		result.IsOptionSet = attribute.AttributeType == AttributeTypeCode.Picklist;
		result.IsTwoOption = attribute.AttributeType == AttributeTypeCode.Boolean;
		result.DeprecatedVersion = attribute.DeprecatedVersion;
		result.IsDeprecated = !string.IsNullOrWhiteSpace(attribute.DeprecatedVersion);

		result.EnumData = attribute switch
		{
			PicklistAttributeMetadata pickList => MappingEnum.Parse(pickList),
			MultiSelectPicklistAttributeMetadata multiSelectPickListAttributeMetadata => MappingEnum.Parse(
				multiSelectPickListAttributeMetadata),
			_ => result.EnumData
		};

		LookupAttributeMetadata lookup = attribute as LookupAttributeMetadata;

		if (lookup?.Targets.Length == 1)
		{
			result.LookupSingleType = lookup.Targets[0];
		}

		ParseMinMaxValues(attribute, result);

		if (attribute.AttributeType != null)
		{
			result.FieldType = attribute.AttributeType.Value;
		}

		if (attribute.AttributeTypeName != null)
		{
			result.AttributeTypeName = attribute.AttributeTypeName.Value;
		}

		result.IsPrimaryKey = attribute.IsPrimaryId == true;

		result.LogicalName = attribute.LogicalName;
		result.DisplayName = attribute.GetProperVariableName();
		result.PrivatePropertyName = attribute.SchemaName.GetEntityPropertyPrivateName();
		result.HybridName = result.GetProperHybridFieldName();

		if (attribute.Description?.UserLocalizedLabel != null)
		{
			result.Description = attribute.Description.UserLocalizedLabel.Label;
		}

		if (attribute.DisplayName?.UserLocalizedLabel != null)
		{
			result.Label = attribute.DisplayName.UserLocalizedLabel.Label;
		}

		result.IsRequired = attribute.RequiredLevel != null &&
			attribute.RequiredLevel.Value == AttributeRequiredLevel.ApplicationRequired;

		result.Attribute =
			new CrmPropertyAttribute
			{
				LogicalName = attribute.LogicalName,
				IsLookup = attribute.AttributeType is AttributeTypeCode.Lookup or AttributeTypeCode.Customer
			};

		result.TargetTypeForCrmSvcUtil = GetTargetType(result);
		result.FieldTypeString = result.TargetTypeForCrmSvcUtil;

		return result;
	}

	public MappingField CreateFileNameField(MappingField field)
	{
		MappingField fieldCopy = DeepCloneExtensions.CreateDeepCopy(field);
		fieldCopy.TargetTypeForCrmSvcUtil = "string";
		fieldCopy.DisplayName = $"{fieldCopy.DisplayName}Name";
		fieldCopy.Attribute.LogicalName = $"{fieldCopy.Attribute.LogicalName}_name";

		return fieldCopy;
	}

	private static string GetTargetType(MappingField field)
	{
		if (field.IsPrimaryKey)
		{
			return "Guid?";
		}

		switch (field.FieldType)
		{
			case AttributeTypeCode.Picklist:
				return "OptionSetValue";

			case AttributeTypeCode.BigInt:
				return "long?";

			case AttributeTypeCode.Integer:
				return "int?";

			case AttributeTypeCode.Boolean:
				return "bool?";

			case AttributeTypeCode.DateTime:
				return "DateTime?";

			case AttributeTypeCode.Decimal:
				return "decimal?";

			case AttributeTypeCode.Money:
				return "Money";

			case AttributeTypeCode.Double:
				return "double?";

			case AttributeTypeCode.Uniqueidentifier:
				return "Guid?";

			case AttributeTypeCode.Lookup:
			case AttributeTypeCode.Owner:
			case AttributeTypeCode.Customer:
				return "EntityReference";

			case AttributeTypeCode.State:
				return field.Entity.StateName + "?";

			case AttributeTypeCode.Status:
				return "OptionSetValue";

			case AttributeTypeCode.Memo:
			case AttributeTypeCode.Virtual:
			case AttributeTypeCode.EntityName:
			case AttributeTypeCode.String:
			{
				return field.AttributeTypeName switch
				{
					"FileType" => "Guid?",
					"MultiSelectPicklistType" => "OptionSetValueCollection",
					_ => "string"
				};
			}

			case AttributeTypeCode.PartyList:
				return "IEnumerable<ActivityParty>";

			case AttributeTypeCode.ManagedProperty:
				return "BooleanManagedProperty";

			default:
				return "object";
		}
	}

	private static void ParseMinMaxValues(AttributeMetadata attribute, MappingField result)
	{
		switch (attribute)
		{
			case StringAttributeMetadata stringAttributeMetadata:
				result.MaxLength = stringAttributeMetadata.MaxLength ?? -1;

				break;

			case MemoAttributeMetadata memoAttributeMetadata:
				result.MaxLength = memoAttributeMetadata.MaxLength ?? -1;

				break;

			case IntegerAttributeMetadata integerAttributeMetadata:
			{
				result.Min = integerAttributeMetadata.MinValue ?? -1;
				result.Max = integerAttributeMetadata.MaxValue ?? -1;

				break;
			}

			case DecimalAttributeMetadata decimalAttributeMetadata:
			{
				result.Min = decimalAttributeMetadata.MinValue ?? -1;
				result.Max = decimalAttributeMetadata.MaxValue ?? -1;

				break;
			}

			case MoneyAttributeMetadata moneyAttributeMetadata:
			{
				result.Min = moneyAttributeMetadata.MinValue != null
					? (decimal)moneyAttributeMetadata.MinValue.Value
					: -1;

				result.Max = moneyAttributeMetadata.MaxValue != null
					? (decimal)moneyAttributeMetadata.MaxValue.Value
					: -1;

				break;
			}

			case DoubleAttributeMetadata doubleAttributeMetadata:
			{
				result.Min = doubleAttributeMetadata.MinValue != null
					? (decimal)doubleAttributeMetadata.MinValue.Value
					: -1;

				result.Max = doubleAttributeMetadata.MaxValue != null
					? (decimal)doubleAttributeMetadata.MaxValue.Value
					: -1;

				break;
			}
		}
	}
}