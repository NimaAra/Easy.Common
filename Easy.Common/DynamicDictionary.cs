namespace Easy.Common
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;

    /// <summary>
    /// Provides an abstraction for an object to be used dynamically as a key value pair
    /// where the property is the key and value is an object.
    /// </summary>
    public sealed class DynamicDictionary : DynamicObject
    {
        private readonly Dictionary<string, object> _dictionary;

        /// <summary>
        /// Creates a new instance of <see cref="DynamicDictionary"/>.
        /// </summary>
        /// <param name="ignoreCase">The flag indicating whether keys should be treated regardless of the case.</param>
        public DynamicDictionary(bool ignoreCase = true)
        {
            _dictionary = new Dictionary<string, object>(
                ignoreCase ? StringComparer.InvariantCultureIgnoreCase : StringComparer.InvariantCulture);
        }

        /// <summary>
        /// Gets the number of elements in the instance.
        /// </summary>
        public int Count => _dictionary.Count;

        /// <summary>
        /// Gets the keys in the instance.
        /// </summary>
        public string[] Keys => _dictionary.Keys.ToArray();

        /// <summary>
        /// Gets the values in the instance.
        /// </summary>
        public object[] Values => _dictionary.Values.ToArray();

        /// <summary>
        /// Gets or sets the object stored against the given <paramref name="key"/>.
        /// </summary>
        public object this[string key]
        {
            get
            {
                object result;
                _dictionary.TryGetValue(key, out result);
                return result;
            }

            set { _dictionary[key] = value; }
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (!_dictionary.TryGetValue(binder.Name, out result))
            {
                return true;
            }

            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            _dictionary[binder.Name] = value;
            return true;
        }
    }
}