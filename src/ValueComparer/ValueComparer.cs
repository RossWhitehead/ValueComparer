using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

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
        /// <returns>True if value is identical, False otherwise</returns>
        public static bool Compare(object object1, object object2)
        {
            return Compare(object1, object2, DefaultDepth);
        }

        public static bool Compare(object object1, object object2, int depth)
        {
            if (depth < 1)
            {
                throw new ArgumentOutOfRangeException("depth", depth, "Depth must be greater than 0.");
            }

            return CompareObject(object1, object2, depth, 0);
        }

        private static bool CompareObject(object object1, object object2, int depth, int level)
        {
            if (level <= depth)
            {
                // Nulls are equal
                if (object1 == null && object2 == null)
                {
                    return true;
                }

                // Either object1 or object2 is null
                if (object1 == null ^ object2 == null)
                {
                    return false;
                }

                Type type = object1.GetType();

                // Objects must be of the same type
                if (object2.GetType() != type)
                {
                    return false;
                }

                // Compare value types and classes implementing IComparable
                if (typeof(IComparable).IsAssignableFrom(type))
                {
                    return object.Equals(object1, object2);
                }

                // Compare collections
                if (typeof(IEnumerable).IsAssignableFrom(type))
                {
                    return CollectionsAreaEqual((IEnumerable)object1, (IEnumerable)object2, depth, level);
                }

                // Compare classes
                if (type.GetTypeInfo().IsClass)
                {
                    return CompareProperties(type, object1, object2, depth, level);
                }

                throw new TypeNotSupportedException() { Type = type };
            }
            else
            {
                return true;
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
        /// <returns></returns>
        private static bool CompareProperties(Type type, object object1, object object2, int depth, int level)
        {
            level++;

            foreach (PropertyInfo propertyInfo in type.GetProperties())
            {
                var value1 = propertyInfo.GetValue(object1);
                var value2 = propertyInfo.GetValue(object2);

                // Recursively call CompareObject for the full object graph
                if (!CompareObject(value1, value2, depth, level))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Compares collections
        /// </summary>
        /// <param name="collection1">Collection 1</param>
        /// <param name="collection2">Collection 2</param>
        /// <returns></returns>
        private static bool CollectionsAreaEqual(IEnumerable collection1, IEnumerable collection2, int depth, int level)
        {
            var enumerator1 = collection1.GetEnumerator();
            var enumerator2 = collection2.GetEnumerator();

            while (enumerator1.MoveNext())
            {
                if (enumerator2.MoveNext())
                {
                    if (!CompareObject(enumerator1.Current, enumerator2.Current, depth, level))
                    {
                        return false;
                    }
                }
                else
                {
                    // More items in collection 1
                    return false;
                }
            }

            if (enumerator2.MoveNext())
            {
                // More items in collection 2
                return false;
            }

            return true;
        }
    }
}
