using Tomlyn;
using CrispyWaffle.Serialization.Adapters;

namespace CrispyWaffle.Serialization.Adapters
{
    /// <summary>
    /// A serializer for Toml.
    /// </summary>
    /// <seealso cref="ISerializerAdapter" />
    public sealed class TomlSerializerAdapter : BaseSerializerAdapter
    {
        /// <summary>
        /// Serializes an object to a Toml string.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>A Toml string representation of the object.</returns>
        public override string Serialize(object obj)
        {
            return Toml.ToTomlString(obj);
        }

        /// <summary>
        /// Deserializes a Toml string to an object of type T.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <param name="data">The Toml string to deserialize.</param>
        /// <returns>An object of type T.</returns>
        public override T Deserialize<T>(string data)
        {
            return Toml.ToModel<T>(data);
        }
    }
}
