using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.VisualStudio.TextTemplating;

namespace DataverseGen.Core.Generators.T4
{
    [Serializable]
    public class TextTemplatingSession : Dictionary<string, object>, ITextTemplatingSession
    {
        public TextTemplatingSession() : this(Guid.NewGuid())
        {
        }

        public TextTemplatingSession(Guid id)
        {
            Id = id;
        }

        public TextTemplatingSession(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Id = (Guid)info.GetValue("Id", typeof(Guid));
        }

        public Guid Id { get; }

        public override bool Equals(object obj)
        {
            return obj is TextTemplatingSession o && o.Equals(this);
        }

        public bool Equals(ITextTemplatingSession other)
        {
            return other != null && other.Id == Id;
        }

        public bool Equals(Guid other)
        {
            return other.Equals(Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Id", Id);
        }
    }
}