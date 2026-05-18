using DataverseGen.Core.Metadata;

namespace DataverseGen.Core.Tests.Metadata;

[TestClass]
public class CrmAttributeExtensionsTests
{
	[AttributeUsage(AttributeTargets.All)]
	private sealed class SampleAttribute : Attribute
	{
		public string Name { get; set; }
		public bool IsActive { get; set; }
		public int Priority { get; set; }
	}

	[AttributeUsage(AttributeTargets.All)]
	private sealed class NoSuffixAttr : Attribute
	{
		public string Value { get; set; }
	}

	// ── Type name formatting ───────────────────────────────────────────────────

	[TestMethod]
	public void MergeCode_TypeNameEndsWithAttribute_StripsAttributeSuffix()
	{
		var attr = new SampleAttribute { Name = "test" };

		string result = attr.MergeCode();

		StringAssert.StartsWith(result, "[Sample(");
	}

	[TestMethod]
	public void MergeCode_TypeNameDoesNotEndWithAttribute_KeepsFullName()
	{
		var attr = new NoSuffixAttr { Value = "x" };

		string result = attr.MergeCode();

		StringAssert.StartsWith(result, "[NoSuffixAttr(");
	}

	// ── Value filtering ────────────────────────────────────────────────────────

	[TestMethod]
	public void MergeCode_DefaultValues_AreExcluded()
	{
		// IsActive=false (default bool) and Priority=0 (default int) should not appear
		var attr = new SampleAttribute { Name = "test" };

		string result = attr.MergeCode();

		Assert.IsFalse(result.Contains("IsActive"), "Default bool should be excluded");
		Assert.IsFalse(result.Contains("Priority"), "Default int should be excluded");
	}

	[TestMethod]
	public void MergeCode_NullStringValue_IsExcluded()
	{
		var attr = new SampleAttribute(); // Name is null

		string result = attr.MergeCode();

		Assert.IsFalse(result.Contains("Name"), "Null string property should be excluded");
	}

	[TestMethod]
	public void MergeCode_EmptyStringValue_IsExcluded()
	{
		var attr = new SampleAttribute { Name = "" };

		string result = attr.MergeCode();

		Assert.IsFalse(result.Contains("Name"), "Empty string property should be excluded");
	}

	// ── Value formatting ───────────────────────────────────────────────────────

	[TestMethod]
	public void MergeCode_StringValue_IsQuoted()
	{
		var attr = new SampleAttribute { Name = "hello" };

		string result = attr.MergeCode();

		StringAssert.Contains(result, "Name = \"hello\"");
	}

	[TestMethod]
	public void MergeCode_TrueBoolValue_IsLowercaseTrue()
	{
		var attr = new SampleAttribute { IsActive = true };

		string result = attr.MergeCode();

		StringAssert.Contains(result, "IsActive = true");
	}

	[TestMethod]
	public void MergeCode_NonDefaultIntValue_IsIncluded()
	{
		var attr = new SampleAttribute { Priority = 42 };

		string result = attr.MergeCode();

		StringAssert.Contains(result, "Priority = 42");
	}

	// ── Full output ────────────────────────────────────────────────────────────

	[TestMethod]
	public void MergeCode_AllNonDefaultValues_ProducesCorrectSyntax()
	{
		var attr = new SampleAttribute { Name = "test", IsActive = true, Priority = 5 };

		string result = attr.MergeCode();

		StringAssert.StartsWith(result, "[");
		StringAssert.EndsWith(result, "]");
		StringAssert.Contains(result, "Name = \"test\"");
		StringAssert.Contains(result, "IsActive = true");
		StringAssert.Contains(result, "Priority = 5");
	}

	[TestMethod]
	public void MergeCode_AllDefaultValues_ProducesEmptyParens()
	{
		var attr = new SampleAttribute();

		string result = attr.MergeCode();

		Assert.AreEqual("[Sample()]", result);
	}
}
