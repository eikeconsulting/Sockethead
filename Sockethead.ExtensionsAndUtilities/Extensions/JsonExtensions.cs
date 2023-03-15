using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Sockethead.ExtensionsAndUtilities.Extensions
{
    /// <summary>
    /// One line serialization/deserialization using Newtonsoft.Json library
    /// </summary>
    public static class JsonExtensions
    {
        /// <summary>
        /// Converts an input object to its JSON representation using the Newtonsoft.Json library, with indentation and
        /// camel-cased property names. If the input object is null, the method returns null.
        /// </summary>
        public static string ToJson(this object val)
        {
            return val == null
                ? null
                : JsonConvert.SerializeObject(val, Formatting.Indented,
                    new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver(),
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        }

        /// <summary>
        /// Deserializes a JSON string input to an object of type T using the Newtonsoft.Json library.
        /// If the input string is null, the method returns the default value of type T.
        /// </summary>
        public static T Deserialize<T>(this string str)
        {
            return str == null ? default : JsonConvert.DeserializeObject<T>(str);
        }
    }
}