namespace DataverseGen.Core.Metadata;

[Serializable]
[AttributeUsage(AttributeTargets.Class)]
public class CrmEntityAttribute : Attribute
{
	public string LogicalName { get; set; }

	public string PrimaryKey { get; set; }
}

[Serializable]
[AttributeUsage(AttributeTargets.Property)]
public class CrmRelationshipAttribute : Attribute, ICloneable
{
	public string FromEntity { get; set; }

	public string ToEntity { get; set; }

	public string FromKey { get; set; }

	public string ToKey { get; set; }

	public string IntersectingEntity { get; set; }

	public object Clone()
	{
		return MemberwiseClone();
	}
}

[Serializable]
[AttributeUsage(AttributeTargets.Property)]
public class CrmPropertyAttribute : Attribute
{
	public string LogicalName { get; set; }

	public bool IsLookup { get; set; }

	public bool IsEntityReferenceHelper { get; set; }
}

[Serializable]
[AttributeUsage(AttributeTargets.Field)]
public class CrmPicklistAttribute : Attribute
{
	public string DisplayName { get; set; }

	public int Value { get; set; }
}