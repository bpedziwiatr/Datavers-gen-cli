using System.Diagnostics;
using DataverseGen.Core.Extensions;
using Microsoft.Xrm.Sdk.Metadata;

namespace DataverseGen.Core.Metadata;

[Serializable]
public class MappingEnum
{
	public string DisplayName { get; set; }

	public MapperEnumItem[] Items { get; set; }

	public static MappingEnum Parse(object attribute)
	{
		return attribute switch
		{
			EnumAttributeMetadata attributeMetadata => Parse(attributeMetadata),
			BooleanAttributeMetadata booleanAttributeMetadata => Parse(booleanAttributeMetadata),
			_ => null
		};
	}

	public static MappingEnum Parse(EnumAttributeMetadata pickList)
	{
		MappingEnum enm = new()
		{
			DisplayName =
				MetadataNamingExtensions.GetProperVariableName(
					MetadataNamingExtensions.GetProperVariableName(pickList.SchemaName)),
			Items =
				pickList.OptionSet.Options
				   .Where(p => p.Label.UserLocalizedLabel != null)
				   .Select(o => new MapperEnumItem
					{
						Attribute = new CrmPicklistAttribute
						{
							DisplayName = o.Label.UserLocalizedLabel.Label,
							Value = o.Value ?? 1
						},
						Name = MetadataNamingExtensions.GetProperVariableName(o.Label.UserLocalizedLabel.Label)
					})
				   .ToArray()
		};

		RenameDuplicates(enm);

		return enm;
	}

	public static MappingEnum Parse(BooleanAttributeMetadata twoOption)
	{
		MappingEnum enm = new()
		{
			DisplayName =
				MetadataNamingExtensions.GetProperVariableName(
					MetadataNamingExtensions.GetProperVariableName(twoOption.SchemaName)),
			Items = new MapperEnumItem[2]
		};

		enm.Items[0] = MapBoolOption(twoOption.OptionSet.TrueOption);
		enm.Items[1] = MapBoolOption(twoOption.OptionSet.FalseOption);
		RenameDuplicates(enm);

		return enm;
	}

	private static void RenameDuplicates(MappingEnum enm)
	{
		Dictionary<string, int> duplicates = new();

		foreach (MapperEnumItem enumItem in enm.Items)
		{
			if (duplicates.ContainsKey(enumItem.Name))
			{
				duplicates[enumItem.Name] += 1;
				enumItem.Name = $@"{enumItem.Name}_{duplicates[enumItem.Name]}";
			}
			else
			{
				duplicates[enumItem.Name] = 1;
			}
		}
	}

	private static MapperEnumItem MapBoolOption(OptionMetadata option)
	{
		Debug.Assert(option.Value != null, "option.Value != null ");
		MapperEnumItem results = new()
		{
			Attribute = new CrmPicklistAttribute
			{
				DisplayName = option.Label.UserLocalizedLabel.Label,
				Value = (int)option.Value
			},
			Name = MetadataNamingExtensions.GetProperVariableName(option.Label.UserLocalizedLabel.Label)
		};

		return results;
	}
}

[Serializable]
public class MapperEnumItem
{
	public CrmPicklistAttribute Attribute { get; set; }

	public string Name { get; set; }

	public int Value => Attribute.Value;
}