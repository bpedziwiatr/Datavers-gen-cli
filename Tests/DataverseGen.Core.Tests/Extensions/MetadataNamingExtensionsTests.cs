using DataverseGen.Core.Extensions;

namespace DataverseGen.Core.Tests.Extensions;

[TestClass]
public class MetadataNamingExtensionsTests
{
	// ── GetPluralName ──────────────────────────────────────────────────────────

	[TestMethod]
	[DataRow("Category", "Categories")]
	[DataRow("Entity",   "Entities")]
	public void GetPluralName_EndsWithY_ReplacesWithIes(string input, string expected)
	{
		Assert.AreEqual(expected, input.GetPluralName());
	}

	[TestMethod]
	[DataRow("Status",  "Status")]
	[DataRow("Address", "Address")]
	public void GetPluralName_EndsWithS_ReturnsUnchanged(string input, string expected)
	{
		Assert.AreEqual(expected, input.GetPluralName());
	}

	[TestMethod]
	[DataRow("Account",      "Accounts")]
	[DataRow("Contact",      "Contacts")]
	[DataRow("Opportunity",  "Opportunities")]
	public void GetPluralName_RegularWord_AppendsS(string input, string expected)
	{
		Assert.AreEqual(expected, input.GetPluralName());
	}

	// ── GetProperVariableName(string) ──────────────────────────────────────────

	[TestMethod]
	[DataRow("")]
	[DataRow("   ")]
	[DataRow(null)]
	public void GetProperVariableName_EmptyOrWhitespace_ReturnsEmpty(string input)
	{
		Assert.AreEqual("Empty", MetadataNamingExtensions.GetProperVariableName(input));
	}

	[TestMethod]
	public void GetProperVariableName_ClosedDeprecated_ReturnsClosed()
	{
		Assert.AreEqual("Closed", MetadataNamingExtensions.GetProperVariableName("Closed (deprecated)"));
	}

	[TestMethod]
	[DataRow("AccountName", "AccountName")]
	[DataRow("new_contact", "new_contact")]
	public void GetProperVariableName_NormalName_ReturnsCleaned(string input, string expected)
	{
		Assert.AreEqual(expected, MetadataNamingExtensions.GetProperVariableName(input));
	}

	[TestMethod]
	[DataRow("public",   "__public")]
	[DataRow("private",  "__private")]
	[DataRow("new",      "__new")]
	[DataRow("partial",  "__partial")]
	[DataRow("readonly", "__readonly")]
	[DataRow("object",   "__object")]
	[DataRow("true",     "__true")]
	[DataRow("false",    "__false")]
	public void GetProperVariableName_ReservedKeyword_PrependsDoubleUnderscore(string input, string expected)
	{
		Assert.AreEqual(expected, MetadataNamingExtensions.GetProperVariableName(input));
	}

	[TestMethod]
	public void GetProperVariableName_StartsWithDigit_PrependsUnderscore()
	{
		string result = MetadataNamingExtensions.GetProperVariableName("123Account");

		StringAssert.StartsWith(result, "_");
	}

	// ── XmlEscape ─────────────────────────────────────────────────────────────

	[TestMethod]
	public void XmlEscape_NoSpecialChars_ReturnsUnchanged()
	{
		Assert.AreEqual("normal text", "normal text".XmlEscape());
	}

	[TestMethod]
	public void XmlEscape_NewLine_ReplacedWithSemicolon()
	{
		Assert.AreEqual("line1;line2", "line1\nline2".XmlEscape());
	}

	[TestMethod]
	public void XmlEscape_CarriageReturn_ReplacedWithSemicolon()
	{
		Assert.AreEqual("line1;line2", "line1\rline2".XmlEscape());
	}

	[TestMethod]
	public void XmlEscape_EmptyString_ReturnsEmpty()
	{
		Assert.AreEqual("", "".XmlEscape());
	}

	// ── GetEntityPropertyPrivateName ──────────────────────────────────────────

	[TestMethod]
	public void GetEntityPropertyPrivateName_StartsWithUnderscore()
	{
		string result = "AccountId".GetEntityPropertyPrivateName();

		StringAssert.StartsWith(result, "_");
	}

	[TestMethod]
	public void GetEntityPropertyPrivateName_FirstLetterLowercased()
	{
		string result = "AccountId".GetEntityPropertyPrivateName();

		Assert.AreEqual("_accountId", result);
	}
}
