using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using DataverseGen.Core.Metadata;
using Microsoft.Xrm.Sdk.Metadata;

namespace DataverseGen.Core.Extensions;

public static partial class MetadataNamingExtensions
{
	public static string GetEntityPropertyPrivateName(this string name)
	{
		return "_" + Clean(Capitalize(name, false));
	}

	public static string GetPluralName(this string name)
	{
		if (name.EndsWith('y'))
		{
			return name[..^1] + "ies";
		}

		if (name.EndsWith('s'))
		{
			return name;
		}

		return name + "s";
	}

	public static string GetProperEntityName(this EntityMetadata entityMetadata)
	{
		return Clean(Capitalize(entityMetadata.SchemaName, true));
	}

	public static string GetProperHybridFieldName(this MappingField mappingField)
	{
		return mappingField.Attribute != null &&
			mappingField.Attribute.LogicalName.Contains('_')
				? mappingField.Attribute.LogicalName
				: mappingField.DisplayName;
	}

	public static string GetProperHybridName(this EntityMetadata entityMetadata)
	{
		return !entityMetadata.LogicalName.Contains('_')
			? Clean(Capitalize(entityMetadata.SchemaName, true))
			: entityMetadata.SchemaName;
	}

	public static string GetProperVariableName(this AttributeMetadata attribute)
	{
		return attribute.LogicalName switch
		{
			// Normally we want to use the SchemaName as it has the capitalized names (Which is what CrmSvcUtil.exe does).
			// HOWEVER, If you look at the 'annual' attributes on the annualfiscalcalendar you see it has schema name of Period1
			// So if the logicalname & schema name don't match use the logical name and try to capitalize it
			// EXCEPT,  when it's RequiredAttendees/From/To/Cc/Bcc/SecondHalf/FirstHalf  (i have no idea how CrmSvcUtil knows to make those upper case)
			"requiredattendees" => "RequiredAttendees",
			"from" => "From",
			"to" => "To",
			"cc" => "Cc",
			"bcc" => "Bcc",
			"firsthalf" => "FirstHalf",
			"secondhalf" => "SecondHalf",
			"firsthalf_base" => "FirstHalf_Base",
			"secondhalf_base" => "SecondHalf_Base",
			"attributes" => "Attributes1",
			"id" => "__Id" // LocalConfigStore has an attribute named Id, the template will already add and Id
			,
			"entitytypecode" =>
				"__EntityTypeCode" // PrincipalSyncAttributeMap has a field called EntityTypeCode, the template will already have a property called EntityTypeCode which refers to the entity's type code.
			,
			_ => Clean(attribute.LogicalName != null
				&& attribute.LogicalName.Equals(attribute.SchemaName, StringComparison.InvariantCultureIgnoreCase)
					? attribute.SchemaName
					: Capitalize(attribute.LogicalName, true))
		};
	}

	public static string GetProperVariableName(string schemaName)
	{
		if (string.IsNullOrWhiteSpace(schemaName))
		{
			const string emptyVariableName = "Empty";

			return emptyVariableName;
		}

		const string closedDeprecated = "Closed (deprecated)";
		const string closedVariableName = "Closed";

		return schemaName == closedDeprecated
			? closedVariableName
			: Clean(schemaName);
	}

	public static string XmlEscape(this string unescaped)
	{
		return unescaped
		   .Replace("\n", ";")
		   .Replace("\r", ";");
	}

	private static string Capitalize(string name, bool capitalizeFirstWord)
	{
		string[] parts = name.Split(' ', '_', StringSplitOptions.RemoveEmptyEntries);

		for (int i = 0; i < parts.Length; i++)
		{
			parts[i] = i != 0 || capitalizeFirstWord
				? CapitalizeWord(parts[i])
				: DecapitalizeWord(parts[i]);
		}

		return string.Join("_", parts);
	}

	private static string CapitalizeWord(string word)
	{
		return string.IsNullOrWhiteSpace(word)
			? string.Empty
			: word[..1].ToUpper() + word[1..];
	}

	private static string Clean(string text)
	{
		string result = "";

		if (string.IsNullOrEmpty(text))
		{
			return result;
		}

		text = text.Trim();
		text = Normalize(text);

		if (!string.IsNullOrEmpty(text))
		{
			StringBuilder sb = new();
			const int start = 0;

			if (!char.IsLetter(text[0]))
			{
				sb.Append('_');
			}

			for (int i = start; i < text.Length; i++)
			{
				if ((char.IsDigit(text[i]) || char.IsLetter(text[i]) || text[i] == '_') &&
					!string.IsNullOrEmpty(text[i].ToString()))
				{
					sb.Append(text[i]);
				}
			}

			result = sb.ToString();
		}

		result = ReplaceKeywords(result);

		result = CleanRegexCompiled().Replace(result, "");

		return result;
	}

	private static string DecapitalizeWord(string word)
	{
		if (string.IsNullOrWhiteSpace(word))
		{
			return "";
		}

		return word[..1].ToLower() + word[1..];
	}

	private static string Normalize(string regularString)
	{
		string normalizedString = regularString.Normalize(NormalizationForm.FormD);

		StringBuilder sb = new(normalizedString);

		for (int i = 0; i < sb.Length; i++)
		{
			if (CharUnicodeInfo.GetUnicodeCategory(sb[i]) == UnicodeCategory.NonSpacingMark)
			{
				sb.Remove(i, 1);
			}
		}

		regularString = sb.ToString();

		return regularString.Replace("æ", "");
	}

	private static string ReplaceKeywords(string keyword)
	{
		return keyword.Equals("public", StringComparison.InvariantCultureIgnoreCase)
			|| keyword.Equals("private", StringComparison.InvariantCultureIgnoreCase)
			// || name.Equals("event", StringComparison.InvariantCultureIgnoreCase)
			|| keyword.Equals("single", StringComparison.InvariantCultureIgnoreCase)
			|| keyword.Equals("new", StringComparison.InvariantCultureIgnoreCase)
			|| keyword.Equals("partial", StringComparison.InvariantCultureIgnoreCase)
			|| keyword.Equals("to", StringComparison.InvariantCultureIgnoreCase)
			|| keyword.Equals("error", StringComparison.InvariantCultureIgnoreCase)
			|| keyword.Equals("readonly", StringComparison.InvariantCultureIgnoreCase)
			|| keyword.Equals("case", StringComparison.InvariantCultureIgnoreCase)
			|| keyword.Equals("object", StringComparison.InvariantCultureIgnoreCase)
			|| keyword.Equals("global", StringComparison.InvariantCultureIgnoreCase)
			|| keyword.Equals("true", StringComparison.InvariantCultureIgnoreCase)
			|| keyword.Equals("false", StringComparison.InvariantCultureIgnoreCase)
				? "__" + keyword
				: keyword;
	}

	[GeneratedRegex("[^A-Za-z0-9_]")]
	private static partial Regex CleanRegexCompiled();
}