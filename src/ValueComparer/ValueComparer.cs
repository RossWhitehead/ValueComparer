using System;
using System.Collections;
using System.Reflection;

namespace ValueComparer
{
    public static class ValueComparer
    {
        const int DefaultDepth = 5;

        /// <summary>
        /// Compare the value of object1 with object2
        /// </summary>
        /// <param name="object1">Object 1</param>
        /// <param name="object2">Object 2</param>
        public static void AssertEqual(object object1, object object2)
        {
            AssertEqual(object1, object2, DefaultDepth);
        }

        /// <summary>
        /// Compare the value of object1 with object2
        /// </summary>
        /// <param name="object1">Object 1</param>
        /// <param name="object2">Object 2</param>
        /// <param name="depth">Depth of object graph to compare. Required to accommodate cyclical dependencies.</param>
        public static void AssertEqual(object object1, object object2, int depth)
        {
            if (depth < 1)
            {
                throw new ArgumentOutOfRangeException("depth", depth, "Depth must be greater than 0.");
            }

            Compare(object1, object2, depth, level: 0);
        }

        private static void Compare(object object1, object object2, int depth, int level)
        {
            if (level <= depth)
            {
                // Nulls are equal
                if (object1 == null && object2 == null)
                {
                    return;
                }

                // Either object1 or object2 is null
                if (object1 == null ^ object2 == null)
                {
                    throw new EqualException();
                }

                // Same object
                if (object.ReferenceEquals(object1, object2))
                {
                    return;
                }

                Type type = object1.GetType();

                // Objects must be of the same type
                if (object2.GetType() != type)
                {
                    throw new EqualException();
                }

                // Compare value types and classes implementing IComparable
                if (typeof(IComparable).IsAssignableFrom(type))
                {
                    if (object.Equals(object1, object2))
                    {
                        return;
                    }
                    else
                    {
                        throw new EqualException();
                    }
                }

                if (typeof(IDictionary).IsAssignableFrom(type))
                {
                    CompareDictionaries((IDictionary)object1, (IDictionary)object2);
                    return;
                }

                // Compare collections
                if (typeof(IEnumerable).IsAssignableFrom(type))
                {
                    CompareCollections((IEnumerable)object1, (IEnumerable)object2, depth, level);
                    return;
                }

                // Compare classes
                if (type.GetTypeInfo().IsClass)
                {
                    CompareClasses(type, object1, object2, depth, level);
                    return;
                }

                throw new TypeNotSupportedException() { Type = type };
            }
        }

        /// <summary>
        /// Iterate through the properties, and compare each
        /// </summary>
        /// <param name="type"></param>
        /// <param name="object1"></param>
        /// <param name="object2"></param>
        /// <param name="depth"></param>
        /// <param name="level"></param>
        private static void CompareClasses(Type type, object object1, object object2, int depth, int level)
        {
            level++;

            foreach (PropertyInfo propertyInfo in type.GetProperties())
            {
                var value1 = propertyInfo.GetValue(object1);
                var value2 = propertyInfo.GetValue(object2);

                // Recursively call CompareObject for the full object graph
                Compare(value1, value2, depth, level);
            }
        }

        /// <summary>
        /// Compares dictionaries
        /// </summary>
        /// <param name="dictionary1">Dictionary 1</param>
        /// <param name="dictionary2">Dictionary 2</param>
        private static void CompareDictionaries(IDictionary dictionary1, IDictionary dictionary2)
        {
            if (dictionary1.Count != dictionary2.Count) // Require equal count.
            {
                throw new EqualException();
            }

            // check keys are the same
            foreach (object key in dictionary1.Keys)
            {
                if (!dictionary2.Contains(key) || !dictionary1[key].Equals(dictionary2[key]))
                {
                    throw new EqualException();
                }
            }
        }

        /// <summary>
        /// Compares collections
        /// </summary>
        /// <param name="collection1">Collection 1</param>
        /// <param name="collection2">Collection 2</param>
        /// <returns></returns>
        private static void CompareCollections(IEnumerable collection1, IEnumerable collection2, int depth, int level)
        {
            var enumerator1 = collection1.GetEnumerator();
            var enumerator2 = collection2.GetEnumerator();

            while (enumerator1.MoveNext())
            {
                if (enumerator2.MoveNext())
                {
                    Compare(enumerator1.Current, enumerator2.Current, depth, level);
                }
                else
                {
                    // More items in collection 1
                    throw new EqualException();
                }
            }

            if (enumerator2.MoveNext())
            {
                // More items in collection 2
                throw new EqualException();
            }
        }
    }
}
