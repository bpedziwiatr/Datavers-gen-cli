using DataverseGen.Core.Extensions;
using Microsoft.Xrm.Sdk.Metadata;

namespace DataverseGen.Core.Metadata;

[Serializable]
public class MappingBool
{
	public string DisplayName { get; set; }

	public MapperEnumItem[] Items { get; set; }

	public static MappingBool Parse(BooleanAttributeMetadata twoOption)
	{
		MappingBool enm = new()
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

	private static void RenameDuplicates(MappingBool enm)
	{
		Dictionary<string, int> duplicates = new();

		foreach (MapperEnumItem i in enm.Items)
		{
			if (duplicates.ContainsKey(i.Name))
			{
				duplicates[i.Name] += 1;
				i.Name += "_" + duplicates[i.Name];
			}
			else
			{
				duplicates[i.Name] = 1;
			}
		}
	}

	private static MapperEnumItem MapBoolOption(OptionMetadata option)
	{
		if (option is null)
		{
			throw new ArgumentNullException(nameof(option));
		}

		MapperEnumItem results = new()
		{
			Attribute = new CrmPicklistAttribute
			{
				DisplayName = option.Label.UserLocalizedLabel.Label,
				Value = option.Value ?? throw new InvalidProgramException(
					$"BoolOptions[{option.MetadataId}] - {option.Label.UserLocalizedLabel.Label} has empty Value property.")
			},
			Name = MetadataNamingExtensions.GetProperVariableName(option.Label.UserLocalizedLabel.Label)
		};

		return results;
	}
}