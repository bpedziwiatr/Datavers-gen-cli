using System.Diagnostics;
using System.Globalization;
using System.Text;
using DataverseGen.Core.CustomApi;
using DataverseGen.Core.Extensions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using static DataverseGen.Core.ColorConsole;

namespace DataverseGen.Core.DataConverter;

public class DataverseCustomApiConverter
{
	private const string CustomApiTableName = "customapi";
	private const string CustomApiRequestParameterTableName = "customapirequestparameter";
	private const string CustomApiResponsePropertyTableName = "customapiresponseproperty";

	private readonly DataverseConnector _dataverseConnector;

	public DataverseCustomApiConverter(DataverseConnector dataverseConnector)
	{
		_dataverseConnector = dataverseConnector;
	}

	public CustomApiModel[] GetCustomApis()
	{
		Stopwatch stopper = Stopwatch.StartNew();
		WriteInfo(@"Loading custom APIs");

		List<Entity> customApiEntities = RetrieveCustomApiEntities();
		List<Entity> requestParameterEntities = RetrieveRelatedEntities(CustomApiRequestParameterTableName);
		List<Entity> responsePropertyEntities = RetrieveRelatedEntities(CustomApiResponsePropertyTableName);

		Dictionary<string, List<CustomApiParameterModel>> requestParametersByApiId =
			GroupParametersByApiId(requestParameterEntities);
		Dictionary<string, List<CustomApiParameterModel>> responsePropertiesByApiId =
			GroupParametersByApiId(responsePropertyEntities);

		List<CustomApiModel> result = customApiEntities
		   .Select(entity => ParseCustomApi(entity,
				requestParametersByApiId.GetValueOrDefault(GetEntityId(entity)) ?? new List<CustomApiParameterModel>(),
				responsePropertiesByApiId.GetValueOrDefault(GetEntityId(entity)) ?? new List<CustomApiParameterModel>()))
		   .OrderBy(api => api.RequestClassName)
		   .ToList();

		stopper.Stop();
		WriteInfo($@"Loaded custom APIs in: {stopper.Elapsed:g}");

		return result.ToArray();
	}

	private static Dictionary<string, List<CustomApiParameterModel>> GroupParametersByApiId(IEnumerable<Entity> entities)
	{
		Dictionary<string, List<CustomApiParameterModel>> grouped = new(StringComparer.InvariantCultureIgnoreCase);

		foreach (Entity entity in entities)
		{
			string apiId = GetLookupId(entity, "customapiid");

			if (string.IsNullOrWhiteSpace(apiId))
			{
				continue;
			}

			CustomApiParameterModel parameter = ParseParameter(entity);

			if (!grouped.TryGetValue(apiId, out List<CustomApiParameterModel> parameters))
			{
				parameters = new List<CustomApiParameterModel>();
				grouped[apiId] = parameters;
			}

			parameters.Add(parameter);
		}

		return grouped;
	}

	private List<Entity> RetrieveCustomApiEntities()
	{
		QueryExpression query = new(CustomApiTableName)
		{
			ColumnSet = new ColumnSet(
				"customapiid",
				"uniquename",
				"name",
				"description",
				"bindingtype",
				"boundentitylogicalname",
				"executeprivilegename",
				"isfunction",
				"isprivate")
		};

		EntityCollection response = _dataverseConnector.OrganizationService.RetrieveMultiple(query);
		WriteInfo($@"Found custom APIs: {response.Entities.Count}");
		return response.Entities.ToList();
	}

	private List<Entity> RetrieveRelatedEntities(string tableName)
	{
		QueryExpression query = new(tableName)
		{
			ColumnSet = new ColumnSet(
				$"{tableName}id",
				"customapiid",
				"uniquename",
				"name",
				"description",
				"type",
				"logicalentityname",
				"isoptional",
				"position")
		};

		EntityCollection response = _dataverseConnector.OrganizationService.RetrieveMultiple(query);
		WriteInfo($@"Found {tableName}: {response.Entities.Count}");
		return response.Entities.ToList();
	}

	private static CustomApiModel ParseCustomApi(
		Entity entity,
		IReadOnlyCollection<CustomApiParameterModel> requestParameters,
		IReadOnlyCollection<CustomApiParameterModel> responseProperties)
	{
		string id = GetEntityId(entity);
		string displayName = GetString(entity, "name");
		string uniqueName = GetString(entity, "uniquename");
		string classNameSource = string.IsNullOrWhiteSpace(displayName) ? uniqueName : displayName;
		string className = ToPascalCase(classNameSource);
		if (!className.EndsWith("Request", StringComparison.InvariantCultureIgnoreCase))
		{
			className += "Request";
		}

		return new CustomApiModel
		{
			Id = id,
			UniqueName = uniqueName,
			DisplayName = displayName,
			Description = GetString(entity, "description"),
			OperationName = string.IsNullOrWhiteSpace(uniqueName) ? className : uniqueName,
			BindingType = GetFormattedOrRawValue(entity, "bindingtype"),
			BoundEntityLogicalName = GetString(entity, "boundentitylogicalname"),
			ExecutePrivilegeName = GetString(entity, "executeprivilegename"),
			IsFunction = GetBool(entity, "isfunction"),
			IsPrivate = GetBool(entity, "isprivate"),
			RequestClassName = className,
			RequestParameters = requestParameters
			   .OrderBy(p => p.Position)
			   .ThenBy(p => p.PropertyName)
			   .ToArray(),
			ResponseProperties = responseProperties
			   .OrderBy(p => p.Position)
			   .ThenBy(p => p.PropertyName)
			   .ToArray()
		};
	}

	private static CustomApiParameterModel ParseParameter(Entity entity)
	{
		string displayName = GetString(entity, "name");
		string uniqueName = GetString(entity, "uniquename");
		string sourceName = string.IsNullOrWhiteSpace(uniqueName) ? displayName : uniqueName;
		string propertyName = ToPascalCase(sourceName, false);
		string typeLabel = GetFormattedOrRawValue(entity, "type");
		string logicalEntityName = GetString(entity, "logicalentityname");

		NormalizeCustomApiParameterType(typeLabel, logicalEntityName, out string tsType, out string webApiTypeName, out string structuralProperty);

		return new CustomApiParameterModel
		{
			Id = GetEntityId(entity),
			CustomApiId = GetLookupId(entity, "customapiid"),
			UniqueName = uniqueName,
			DisplayName = displayName,
			Description = GetString(entity, "description"),
			Type = typeLabel,
			LogicalEntityName = logicalEntityName,
			IsOptional = GetBool(entity, "isoptional"),
			Position = GetInt(entity, "position"),
			PropertyName = propertyName,
			TypeScriptType = tsType,
			WebApiTypeName = webApiTypeName,
			WebApiStructuralProperty = structuralProperty
		};
	}

	private static string GetEntityId(Entity entity)
	{
		return entity.Id.ToString("D");
	}

	private static string GetFormattedOrRawValue(Entity entity, string attributeName)
	{
		if (entity.FormattedValues.TryGetValue(attributeName, out string formattedValue) &&
			!string.IsNullOrWhiteSpace(formattedValue))
		{
			return formattedValue;
		}

		object rawValue = entity.Contains(attributeName) ? entity[attributeName] : null;
		return rawValue switch
		{
			OptionSetValue optionSetValue => optionSetValue.Value.ToString(CultureInfo.InvariantCulture),
			Money money => money.Value.ToString(CultureInfo.InvariantCulture),
			_ => rawValue?.ToString() ?? string.Empty
		};
	}

	private static string GetLookupId(Entity entity, string attributeName)
	{
		EntityReference reference = entity.GetAttributeValue<EntityReference>(attributeName);
		return reference?.Id.ToString("D") ?? string.Empty;
	}

	private static bool GetBool(Entity entity, string attributeName)
	{
		object value = entity.Contains(attributeName) ? entity[attributeName] : null;
		return value switch
		{
			bool b => b,
			OptionSetValue optionSetValue => optionSetValue.Value != 0,
			_ => false
		};
	}

	private static int GetInt(Entity entity, string attributeName)
	{
		object value = entity.Contains(attributeName) ? entity[attributeName] : null;
		return value switch
		{
			int i => i,
			OptionSetValue optionSetValue => optionSetValue.Value,
			decimal decimalValue => (int)decimalValue,
			double doubleValue => (int)doubleValue,
			_ => 0
		};
	}

	private static string GetString(Entity entity, string attributeName)
	{
		return entity.GetAttributeValue<string>(attributeName) ?? string.Empty;
	}

	private static void NormalizeCustomApiParameterType(
		string typeLabel,
		string logicalEntityName,
		out string typeScriptType,
		out string webApiTypeName,
		out string structuralProperty)
	{
		string normalizedType = typeLabel.Trim().ToLowerInvariant();

		if (normalizedType.Contains("collection"))
		{
			typeScriptType = "string[]";
			webApiTypeName = string.IsNullOrWhiteSpace(logicalEntityName)
				? "Collection(Edm.String)"
				: $"Collection(mscrm.{logicalEntityName})";
			structuralProperty = "WebApiRequestStructuralProperty.Collection";
			return;
		}

		if (normalizedType.Contains("entity"))
		{
			typeScriptType = "any";
			webApiTypeName = string.IsNullOrWhiteSpace(logicalEntityName)
				? "mscrm.crmbaseentity"
				: $"mscrm.{logicalEntityName}";
			structuralProperty = "WebApiRequestStructuralProperty.EntityType";
			return;
		}

		if (normalizedType.Contains("bool"))
		{
			typeScriptType = "boolean";
			webApiTypeName = "Edm.Boolean";
			structuralProperty = "WebApiRequestStructuralProperty.PrimitiveType";
			return;
		}

		if (normalizedType.Contains("int") || normalizedType.Contains("whole"))
		{
			typeScriptType = "number";
			webApiTypeName = "Edm.Int32";
			structuralProperty = "WebApiRequestStructuralProperty.PrimitiveType";
			return;
		}

		if (normalizedType.Contains("decimal") || normalizedType.Contains("double") || normalizedType.Contains("money") || normalizedType.Contains("float"))
		{
			typeScriptType = "number";
			webApiTypeName = "Edm.Decimal";
			structuralProperty = "WebApiRequestStructuralProperty.PrimitiveType";
			return;
		}

		if (normalizedType.Contains("date"))
		{
			typeScriptType = "Date";
			webApiTypeName = "Edm.DateTimeOffset";
			structuralProperty = "WebApiRequestStructuralProperty.PrimitiveType";
			return;
		}

		if (normalizedType.Contains("guid") || normalizedType.Contains("unique"))
		{
			typeScriptType = "string";
			webApiTypeName = "Edm.Guid";
			structuralProperty = "WebApiRequestStructuralProperty.PrimitiveType";
			return;
		}

		typeScriptType = "string";
		webApiTypeName = "Edm.String";
		structuralProperty = "WebApiRequestStructuralProperty.PrimitiveType";
	}

	private static string ToPascalCase(string value, bool capitalizeFirstWord = true)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			return string.Empty;
		}

		char[] separators = [' ', '_', '-', '.', '/', '\\'];
		string[] parts = value.Split(separators, StringSplitOptions.RemoveEmptyEntries);
		StringBuilder builder = new();

		for (int i = 0; i < parts.Length; i++)
		{
			string part = parts[i].Trim();

			if (part.Length == 0)
			{
				continue;
			}

			if (i == 0 && !capitalizeFirstWord)
			{
				builder.Append(char.ToLowerInvariant(part[0]));
				if (part.Length > 1)
				{
					builder.Append(part[1..]);
				}
			}
			else
			{
				builder.Append(char.ToUpperInvariant(part[0]));
				if (part.Length > 1)
				{
					builder.Append(part[1..]);
				}
			}
		}

		return builder.ToString();
	}
}
