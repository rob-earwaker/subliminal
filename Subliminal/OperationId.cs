using System;

namespace Subliminal
{
    public class OperationId
    {
        private static readonly Random Random = new Random();

        public OperationId(string value)
        {
            Value = value;
        }

        public static OperationId New()
        {
            var buffer = new byte[8];

            lock (Random)
            {
                Random.NextBytes(buffer);
            }

            return new OperationId(BitConverter.ToString(buffer).Replace("-", "").ToLowerInvariant());
        }

        public string Value { get; }

        public override string ToString()
        {
            return Value;
        }
    }
}
