using DataverseGen.Core.DataConverter;
using Microsoft.Xrm.Sdk;

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
