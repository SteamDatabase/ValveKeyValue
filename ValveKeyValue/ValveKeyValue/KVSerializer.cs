﻿using System.IO;
using System.Runtime.Serialization;

namespace ValveKeyValue
{
    /// <summary>
    /// Helper class to easily deserialize KeyValue objects.
    /// </summary>
    public static class KVSerializer
    {
        /// <summary>
        /// Deserializes a KeyValue object from a stream.
        /// </summary>
        /// <param name="stream">The stream to deserialize from.</param>
        /// <returns>A <see cref="KVObject"/> representing the KeyValues structure in the stream.</returns>
        public static KVObject Deserialize(Stream stream)
        {
            return Deserialize(stream, null);
        }

        /// <summary>
        /// Deserializes a KeyValue object from a stream.
        /// </summary>
        /// <param name="stream">The stream to deserialize from.</param>
        /// <param name="conditions">A list of conditions to use to match conditional values.</param>
        /// <returns>A <see cref="KVObject"/> representing the KeyValues structure in the stream.</returns>
        public static KVObject Deserialize(Stream stream, string[] conditions)
        {
            Require.NotNull(stream, nameof(stream));

            using (var reader = new KVTextReader(stream, conditions ?? new string[0]))
            {
                return reader.ReadObject();
            }
        }

        /// <summary>
        /// Deserializes an object from a KeyValues representation in a stream.
        /// </summary>
        /// <param name="stream">The stream to deserialize from.</param>
        /// <returns>A <typeparamref name="TObject" /> instance representing the KeyValues structure in the stream.</returns>
        /// <typeparam name="TObject">The type of object to deserialize.</typeparam>;
        public static TObject Deserialize<TObject>(Stream stream)
        {
            Require.NotNull(stream, nameof(stream));

            var @object = Deserialize(stream);

            object[] enumerableValues;
            if (ObjectCopier.IsArray(@object, out enumerableValues))
            {
                object enumerable;
                if (ObjectCopier.CreateTypedEnumerable(typeof(TObject), enumerableValues, out enumerable))
                {
                    return (TObject)enumerable;
                }
            }

            var typedObject = (TObject)FormatterServices.GetSafeUninitializedObject(typeof(TObject));
            ObjectCopier.CopyObject(@object, typedObject);
            return typedObject;
        }
    }
}