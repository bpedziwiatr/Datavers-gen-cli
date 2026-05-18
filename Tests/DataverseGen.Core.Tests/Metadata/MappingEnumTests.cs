using DataverseGen.Core.Metadata;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;

namespace DataverseGen.Core.Tests.Metadata;

[TestClass]
public class MappingEnumTests
{
	// Label(string, int) does NOT set UserLocalizedLabel — must be set explicitly
	private static OptionMetadata BuildOption(int value, string labelText)
	{
		var locLabel = new LocalizedLabel(labelText, 1033);
		var label = new Label { UserLocalizedLabel = locLabel };
		return new OptionMetadata(label, value);
	}

	private static PicklistAttributeMetadata BuildPicklist(string schemaName, params (int value, string label)[] options)
	{
		var optionSet = new OptionSetMetadata();
		foreach ((int value, string label) in options)
		{
			optionSet.Options.Add(BuildOption(value, label));
		}

		return new PicklistAttributeMetadata { SchemaName = schemaName, OptionSet = optionSet };
	}

	private static BooleanAttributeMetadata BuildBoolean(string schemaName, string trueLabel, string falseLabel)
	{
		return new BooleanAttributeMetadata
		{
			SchemaName = schemaName,
			OptionSet = new BooleanOptionSetMetadata(BuildOption(1, trueLabel), BuildOption(0, falseLabel))
		};
	}

	// ── Parse(EnumAttributeMetadata) ─────────────────────────────────────────

	[TestMethod]
	public void Parse_Picklist_SetsDisplayName()
	{
		var attr = BuildPicklist("StatusCode", (1, "Active"), (2, "Inactive"));

		MappingEnum result = MappingEnum.Parse((EnumAttributeMetadata)attr);

		Assert.AreEqual("StatusCode", result.DisplayName);
	}

	[TestMethod]
	public void Parse_Picklist_ItemsAreSortedByValue()
	{
		var attr = BuildPicklist("StatusCode", (3, "Pending"), (1, "Active"), (2, "Inactive"));

		MappingEnum result = MappingEnum.Parse((EnumAttributeMetadata)attr);

		Assert.AreEqual(1, result.Items[0].Value);
		Assert.AreEqual(2, result.Items[1].Value);
		Assert.AreEqual(3, result.Items[2].Value);
	}

	[TestMethod]
	public void Parse_Picklist_ItemsHaveCorrectNames()
	{
		var attr = BuildPicklist("StatusCode", (1, "Active"), (2, "Inactive"));

		MappingEnum result = MappingEnum.Parse((EnumAttributeMetadata)attr);

		Assert.AreEqual("Active",   result.Items[0].Attribute.DisplayName);
		Assert.AreEqual("Inactive", result.Items[1].Attribute.DisplayName);
	}

	[TestMethod]
	public void Parse_Picklist_DuplicateNames_AreRenamedWithSuffix()
	{
		var attr = BuildPicklist("StatusCode", (1, "Active"), (2, "Active"));

		MappingEnum result = MappingEnum.Parse((EnumAttributeMetadata)attr);

		Assert.AreNotEqual(result.Items[0].Name, result.Items[1].Name);
		StringAssert.EndsWith(result.Items[1].Name, "_2");
	}

	[TestMethod]
	public void Parse_OptionWithNullLabel_IsExcluded()
	{
		var optionSet = new OptionSetMetadata();
		optionSet.Options.Add(BuildOption(1, "Active"));
		// Option without UserLocalizedLabel — filtered by Where clause
		optionSet.Options.Add(new OptionMetadata(new Label(), 2));

		var attr = new PicklistAttributeMetadata { SchemaName = "StatusCode", OptionSet = optionSet };

		MappingEnum result = MappingEnum.Parse((EnumAttributeMetadata)attr);

		Assert.AreEqual(1, result.Items.Length);
	}

	// ── Parse(BooleanAttributeMetadata) ──────────────────────────────────────

	[TestMethod]
	public void Parse_Boolean_SetsDisplayName()
	{
		var attr = BuildBoolean("IsActive", "Yes", "No");

		MappingEnum result = MappingEnum.Parse(attr);

		Assert.AreEqual("IsActive", result.DisplayName);
	}

	[TestMethod]
	public void Parse_Boolean_HasTwoItems()
	{
		var attr = BuildBoolean("IsActive", "Yes", "No");

		MappingEnum result = MappingEnum.Parse(attr);

		Assert.AreEqual(2, result.Items.Length);
	}

	[TestMethod]
	public void Parse_Boolean_TrueOptionIsFirst()
	{
		var attr = BuildBoolean("IsActive", "Yes", "No");

		MappingEnum result = MappingEnum.Parse(attr);

		Assert.AreEqual("Yes", result.Items[0].Attribute.DisplayName);
		Assert.AreEqual(1, result.Items[0].Value);
	}

	[TestMethod]
	public void Parse_Boolean_FalseOptionIsSecond()
	{
		var attr = BuildBoolean("IsActive", "Yes", "No");

		MappingEnum result = MappingEnum.Parse(attr);

		Assert.AreEqual("No", result.Items[1].Attribute.DisplayName);
		Assert.AreEqual(0, result.Items[1].Value);
	}

	// ── Parse(object) dispatcher ──────────────────────────────────────────────

	[TestMethod]
	public void Parse_Object_WithPicklist_ReturnsEnum()
	{
		var attr = BuildPicklist("StatusCode", (1, "Active"));

		MappingEnum result = MappingEnum.Parse((object)attr);

		Assert.IsNotNull(result);
	}

	[TestMethod]
	public void Parse_Object_WithBoolean_ReturnsEnum()
	{
		var attr = BuildBoolean("IsActive", "Yes", "No");

		MappingEnum result = MappingEnum.Parse((object)attr);

		Assert.IsNotNull(result);
	}

	[TestMethod]
	public void Parse_Object_WithUnknownType_ReturnsNull()
	{
		MappingEnum result = MappingEnum.Parse("not_an_attribute");

		Assert.IsNull(result);
	}

	// ── MapBoolOption null value guard ────────────────────────────────────────

	[TestMethod]
	public void Parse_Boolean_NullValue_ThrowsInvalidOperationException()
	{
		var locLabel = new LocalizedLabel("Yes", 1033);
		var label = new Label { UserLocalizedLabel = locLabel };

		var attr = new BooleanAttributeMetadata
		{
			SchemaName = "IsActive",
			OptionSet = new BooleanOptionSetMetadata(
				new OptionMetadata(label, null), // null value
				BuildOption(0, "No"))
		};

		Assert.ThrowsException<InvalidOperationException>(() => MappingEnum.Parse(attr));
	}
}
