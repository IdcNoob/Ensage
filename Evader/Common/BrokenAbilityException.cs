namespace Evader.Common
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [Serializable]
    internal class BrokenAbilityException : Exception
    {
        public BrokenAbilityException()
            : base()
        {
        }

        public BrokenAbilityException(string message)
            : base(message)
        {
        }

        public BrokenAbilityException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected BrokenAbilityException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public override IDictionary Data { get; } = new Dictionary<string, object>();
    }
}