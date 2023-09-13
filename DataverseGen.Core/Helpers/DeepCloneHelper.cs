using System.Runtime.Serialization;

namespace DataverseGen.Core.Helpers;

internal static class DeepCloneExtensions
{
	internal static T CreateDeepCopy<T>(T obj)
	{
		using MemoryStream stream = new();

		DataContractSerializer serializer = new(typeof(T));
		serializer.WriteObject(stream, obj);
		stream.Position = 0;

		return (T)serializer.ReadObject(stream);
	}
}