﻿using System.Reflection;

namespace DataverseGen.Core.Metadata;

public static class CrmAttributeExtensions
{
	public static string MergeCode(this Attribute attribute)
	{
		Type type = attribute.GetType();

		PropertyInfo[] properties =
			type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

		Dictionary<string, object> values =
			properties.ToDictionary(p => p.Name,
				p => p.GetValue(attribute, Array.Empty<object>()));

		string typeName = type.Name;

		if (typeName.EndsWith("Attribute"))
		{
			typeName = typeName[..^9];
		}

		string[] valuesString = values.Where(v =>
				!(v.Value == null ||
					Equals(v.Value, "") ||
					v.Value.Equals(GetDefaultValue(v.Value.GetType()))))
		   .Select(v =>
				$"{v.Key} = {FormatValue(v.Value)}")
		   .ToArray();

		return
			$"[{typeName}({string.Join(", ", valuesString)})]";
	}

	private static object GetDefaultValue(Type t)
	{
		return t.IsValueType ? Activator.CreateInstance(t) : null;
	}

	private static string FormatValue(object value)
	{
		if (value is bool valueBool)
		{
			return valueBool ? "true" : "false";
		}

		return value is string ? $"\"{value}\"" : value.ToString();
	}
}