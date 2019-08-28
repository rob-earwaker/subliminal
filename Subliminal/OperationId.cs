using System;
using System.Linq;

namespace Subliminal
{
    internal static class OperationId
    {
        private const string Alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        private static readonly Random Random = new Random();

        public static string New()
        {
            lock (Random)
            {
                return new string(new int[8].Select(_ => Alphabet[Random.Next(Alphabet.Length)]).ToArray());
            }
        }
    }
}
