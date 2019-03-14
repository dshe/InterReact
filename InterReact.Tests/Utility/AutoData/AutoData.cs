using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

// AutoFixture is not used because it:
//   cannot set internal or private properties
//   has a confusing API
//   cannot create observables
//   would add another dependency to this solution

namespace InterReact.Tests.Utility.AutoData
{
    public static class AutoData
    {
        private static readonly Random Rand = new Random();

        public static List<T> Create<T>(int count)
        {
            var list = new List<T>();
            for (var i = 0; i < count; i++)
                list.Add(Create<T>());
            return list;
        }

        public static T Create<T>() => (T)Create(typeof(T));

        public static List<T> Create<T>(int count, Action<T> setAction)
        {
            var list = new List<T>();
            for (var i = 0; i < count; i++)
                list.Add(Create(setAction));
            return list;
        }

        public static T Create<T>(Action<T> setAction)
        {
            var instance = (T)Create(typeof(T));
            setAction?.Invoke(instance);
            return instance;
        }

        public static object Create(Type type)
        {
            if (type == typeof(string))
                return Guid.NewGuid().ToString("N"); // 32 character hex

            var utype = Nullable.GetUnderlyingType(type);
            if (utype != null)
                type = utype;

            if (type == typeof(int))
                return Rand.Next(1, 100);
            if (type == typeof(long))
                return (long) Rand.Next(1, 100);

            if (type == typeof(double))
                return Rand.NextDouble() * 10;
            if (type == typeof(bool))
                return true;
            if (type == typeof(DateTime))
                return DateTime.UtcNow.AddMinutes(Rand.Next(0, int.MaxValue));

            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsEnum)
            {
                var values = typeInfo.GetEnumValues();
                var count = values.Length;
                var i = Rand.Next(1, count);
                return values.GetValue(i);
            }

            if (typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(typeInfo))
            {
                var genericTypes = typeInfo.GenericTypeArguments;
                var listType = typeof(List<>).MakeGenericType(genericTypes);
                var list = Activator.CreateInstance(listType);
                AddItemsToList(list);
                return list;
            }

            if (typeInfo.IsClass)
            {
                var classInstance = Activator.CreateInstance(type);
                SetClassProperties(classInstance);
                return classInstance;
            }

            throw new InvalidDataException($"Cannot create type '{typeInfo.Name}'.");
        }

        private static void SetClassProperties(object classInstance)
        {
            var type = classInstance.GetType().GetTypeInfo();
            Debug.Assert(type.IsClass);

            foreach (var property in type.DeclaredProperties)
            {
                var propertyType = property.PropertyType;
                var propertyValue = property.GetValue(classInstance);
                var defaultPropertyValue = DefaultValueOfType(propertyType);

                if (Equals(propertyValue, defaultPropertyValue))
                    property.SetValue(classInstance, Create(propertyType));
                else if (propertyValue is IEnumerable enumerable)
                {
                    if (!enumerable.GetEnumerator().MoveNext())
                        AddItemsToList(enumerable);
                }
                else if (propertyType.GetTypeInfo().IsClass)
                    SetClassProperties(propertyValue);
            }
        }

        private static void AddItemsToList(object obj)
        {
            var enumerable = (IEnumerable)obj;
            var type = enumerable.GetType();
            var genericTypes = type.GenericTypeArguments;
            var listType = typeof(List<>).MakeGenericType(genericTypes);
            var addMethod = listType.GetMethod("Add", genericTypes);
            if (addMethod == null)
                throw new Exception("Cannot find Add method.");
            // use the Add method to add 3 items to the list
            for (var i = 0; i < 3; i++)
            {
                var item = Create(genericTypes[0]);
                addMethod.Invoke(enumerable, new[] { item });
            }
        }

        private static object? DefaultValueOfType(Type type)
        {
            if (type != typeof(string) && type.GetTypeInfo().IsValueType)
                return Activator.CreateInstance(type);
            return null;
        }
    }
}
