using DataverseGen.Core.Metadata;
using Microsoft.Xrm.Sdk.Metadata;

namespace DataverseGen.Core.Tests.Metadata;

[TestClass]
public class MappingRelationshipMnTests
{
	private static ManyToManyRelationshipMetadata BuildRel(
		string schema,
		string entity1,
		string entity1Attr,
		string entity2,
		string entity2Attr,
		string intersect = "intersect_table") => new()
	{
		SchemaName = schema,
		Entity1LogicalName = entity1,
		Entity1IntersectAttribute = entity1Attr,
		Entity2LogicalName = entity2,
		Entity2IntersectAttribute = entity2Attr,
		IntersectEntityName = intersect
	};

	// ── Attribute direction when thisEntity == entity1 ────────────────────────

	[TestMethod]
	public void Parse_ThisEntityIsEntity1_ToEntityIsEntity2()
	{
		var rel = BuildRel("new_account_contact", "account", "accountid", "contact", "contactid");

		var result = MappingRelationshipMn.Parse(rel, "account");

		Assert.AreEqual("contact", result.Attribute.ToEntity);
		Assert.AreEqual("account", result.Attribute.FromEntity);
	}

	// ── Attribute direction when thisEntity == entity2 ────────────────────────

	[TestMethod]
	public void Parse_ThisEntityIsEntity2_ToEntityIsEntity1()
	{
		var rel = BuildRel("new_account_contact", "account", "accountid", "contact", "contactid");

		var result = MappingRelationshipMn.Parse(rel, "contact");

		Assert.AreEqual("account", result.Attribute.ToEntity);
		Assert.AreEqual("contact", result.Attribute.FromEntity);
	}

	// ── IntersectingEntity is always set ─────────────────────────────────────

	[TestMethod]
	public void Parse_SetsIntersectingEntity()
	{
		var rel = BuildRel("new_account_contact", "account", "accountid", "contact", "contactid", "account_contact");

		var result = MappingRelationshipMn.Parse(rel, "account");

		Assert.AreEqual("account_contact", result.Attribute.IntersectingEntity);
	}

	// ── Self-referenced relationship ──────────────────────────────────────────

	[TestMethod]
	public void Parse_SelfReferencedEntity_IsSelfReferencedIsTrue()
	{
		var rel = BuildRel("account_account", "account", "accountid1", "account", "accountid2");

		var result = MappingRelationshipMn.Parse(rel, "account");

		Assert.IsTrue(result.IsSelfReferenced);
	}

	[TestMethod]
	public void Parse_SelfReferencedEntity_DisplayNameStartsWithReferenced()
	{
		var rel = BuildRel("account_account", "account", "accountid1", "account", "accountid2");

		var result = MappingRelationshipMn.Parse(rel, "account");

		StringAssert.StartsWith(result.DisplayName, "Referenced");
	}

	[TestMethod]
	public void Parse_SelfReferencedEntity_EntityRoleIsReferenced()
	{
		var rel = BuildRel("account_account", "account", "accountid1", "account", "accountid2");

		var result = MappingRelationshipMn.Parse(rel, "account");

		Assert.AreEqual("Microsoft.Xrm.Sdk.EntityRole.Referenced", result.EntityRole);
	}

	// ── Non-self-referenced relationship ──────────────────────────────────────

	[TestMethod]
	public void Parse_NonSelfReferenced_IsSelfReferencedIsFalse()
	{
		var rel = BuildRel("new_account_contact", "account", "accountid", "contact", "contactid");

		var result = MappingRelationshipMn.Parse(rel, "account");

		Assert.IsFalse(result.IsSelfReferenced);
		Assert.AreEqual("null", result.EntityRole);
	}

	// ── DisplayName collision with entity name ────────────────────────────────

	[TestMethod]
	public void Parse_DisplayNameCollidesWithEntityName_AppendsSuffix()
	{
		// SchemaName after Clean == thisEntityLogicalName → gets "1" appended
		var rel = BuildRel("account", "account", "accountid", "contact", "contactid");

		var result = MappingRelationshipMn.Parse(rel, "account");

		StringAssert.EndsWith(result.DisplayName, "1");
	}

	// ── HybridName and PrivateName ────────────────────────────────────────────

	[TestMethod]
	public void Parse_HybridNameContainsNNSuffix()
	{
		var rel = BuildRel("new_account_contact", "account", "accountid", "contact", "contactid");

		var result = MappingRelationshipMn.Parse(rel, "account");

		StringAssert.EndsWith(result.HybridName, "_NN");
	}

	[TestMethod]
	public void Parse_PrivateNameStartsWithNnPrefix()
	{
		var rel = BuildRel("new_account_contact", "account", "accountid", "contact", "contactid");

		var result = MappingRelationshipMn.Parse(rel, "account");

		StringAssert.StartsWith(result.PrivateName, "_nn");
	}

	// ── Clone creates independent copy ────────────────────────────────────────

	[TestMethod]
	public void Clone_ModifyingClone_DoesNotAffectOriginal()
	{
		var rel = BuildRel("new_account_contact", "account", "accountid", "contact", "contactid");
		var original = MappingRelationshipMn.Parse(rel, "account");

		var clone = (MappingRelationshipMn)original.Clone();
		clone.Attribute.FromEntity = "modified";

		Assert.AreNotEqual("modified", original.Attribute.FromEntity);
	}

	[TestMethod]
	public void Clone_ProducesIndependentInstance()
	{
		var rel = BuildRel("new_account_contact", "account", "accountid", "contact", "contactid");
		var original = MappingRelationshipMn.Parse(rel, "account");

		var clone = (MappingRelationshipMn)original.Clone();

		Assert.AreNotSame(original, clone);
		Assert.AreNotSame(original.Attribute, clone.Attribute);
	}
}
