using DataverseGen.Core.DataConverter;
using Microsoft.Xrm.Sdk;
using System.Reflection;

namespace DataverseGen.Core.Tests.DataConverter;

[TestClass]
public class DataverseCustomApiConverterTests
{
	[TestMethod]
	public void FilterCustomApiEntities_ReturnsOnlySelectedUniqueNames()
	{
		Entity first = CreateCustomApiEntity("11111111-1111-1111-1111-111111111111", "newfirstapi");
		Entity second = CreateCustomApiEntity("22222222-2222-2222-2222-222222222222", "newsecondapi");
		Entity third = CreateCustomApiEntity("33333333-3333-3333-3333-333333333333", "newthirdapi");

		List<Entity> filtered = DataverseCustomApiConverter.FilterCustomApiEntities(
			new[] { first, second, third },
			new[] { "  NEWSECONDAPI  " });

		Assert.AreEqual(1, filtered.Count);
		Assert.AreEqual(second.Id, filtered[0].Id);
		Assert.AreEqual("newsecondapi", filtered[0].GetAttributeValue<string>("uniquename"));
	}

	[TestMethod]
	public void FilterCustomApiEntities_KeepsSelectionOrder()
	{
		Entity first = CreateCustomApiEntity("11111111-1111-1111-1111-111111111111", "newfirstapi");
		Entity second = CreateCustomApiEntity("22222222-2222-2222-2222-222222222222", "newsecondapi");
		Entity third = CreateCustomApiEntity("33333333-3333-3333-3333-333333333333", "newthirdapi");

		List<Entity> filtered = DataverseCustomApiConverter.FilterCustomApiEntities(
			new[] { first, second, third },
			new[] { "newthirdapi", "newfirstapi" });

		Assert.AreEqual(2, filtered.Count);
		Assert.AreEqual(third.Id, filtered[0].Id);
		Assert.AreEqual(first.Id, filtered[1].Id);
	}

	[TestMethod]
	public void FilterCustomApiEntities_ReturnsAllWhenSelectionIsEmpty()
	{
		Entity first = CreateCustomApiEntity("11111111-1111-1111-1111-111111111111", "newfirstapi");
		Entity second = CreateCustomApiEntity("22222222-2222-2222-2222-222222222222", "newsecondapi");

		List<Entity> filtered = DataverseCustomApiConverter.FilterCustomApiEntities(
			new[] { first, second },
			Array.Empty<string>());

		Assert.AreEqual(2, filtered.Count);
		Assert.AreEqual(first.Id, filtered[0].Id);
		Assert.AreEqual(second.Id, filtered[1].Id);
	}

	[TestMethod]
	public void FilterCustomApiEntities_IgnoresDuplicateAndBlankSelections()
	{
		Entity first = CreateCustomApiEntity("11111111-1111-1111-1111-111111111111", "newfirstapi");
		Entity second = CreateCustomApiEntity("22222222-2222-2222-2222-222222222222", "newsecondapi");

		List<Entity> filtered = DataverseCustomApiConverter.FilterCustomApiEntities(
			new[] { first, second },
			new[] { " ", "newsecondapi", "NEWSECONDAPI", null! });

		Assert.AreEqual(1, filtered.Count);
		Assert.AreEqual(second.Id, filtered[0].Id);
	}

	[DataTestMethod]
	[DataRow("Collection of strings", "", "string[]", "string[]", "Collection(Edm.String)", "WebApiRequestStructuralProperty.Collection")]
	[DataRow("Entity", "account", "any", "EntityReference", "mscrm.account", "WebApiRequestStructuralProperty.EntityType")]
	[DataRow("Whole Number", "", "number", "int", "Edm.Int32", "WebApiRequestStructuralProperty.PrimitiveType")]
	[DataRow("Date and Time", "", "Date", "DateTime", "Edm.DateTimeOffset", "WebApiRequestStructuralProperty.PrimitiveType")]
	[DataRow("", "", "string", "string", "Edm.String", "WebApiRequestStructuralProperty.PrimitiveType")]
		[DataRow(null!, "", "string", "string", "Edm.String", "WebApiRequestStructuralProperty.PrimitiveType")]
	[DataRow("Unknown Type", "", "string", "string", "Edm.String", "WebApiRequestStructuralProperty.PrimitiveType")]
	public void NormalizeCustomApiParameterType_MapsCommonTypes(
		string typeLabel,
		string logicalEntityName,
		string expectedTypeScriptType,
		string expectedCSharpTypeName,
		string expectedWebApiTypeName,
		string expectedStructuralProperty)
	{
		MethodInfo? method = typeof(DataverseCustomApiConverter).GetMethod(
			"NormalizeCustomApiParameterType",
			BindingFlags.NonPublic | BindingFlags.Static);

		Assert.IsNotNull(method);

		object[] parameters =
		{
			typeLabel,
			logicalEntityName,
			null!,
			null!,
			null!,
			null!
		};

		method!.Invoke(null, parameters);

		Assert.AreEqual(expectedTypeScriptType, parameters[2]);
		Assert.AreEqual(expectedCSharpTypeName, parameters[3]);
		Assert.AreEqual(expectedWebApiTypeName, parameters[4]);
		Assert.AreEqual(expectedStructuralProperty, parameters[5]);
	}

	private static Entity CreateCustomApiEntity(string id, string uniqueName)
	{
		Entity entity = new("customapi")
		{
			Id = Guid.Parse(id)
		};

		entity["uniquename"] = uniqueName;
		entity["name"] = uniqueName;

		return entity;
	}
}
