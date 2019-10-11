using System;
using System.Reflection;
using System.Linq;
using Xunit;

namespace InterReact.Tests.Utility
{
    /* Moq requires: 
     *  InternalsVisibleTo
     *  non-sealed class
     *  virtual properties
     *  parameterles constructor
    */

    public static class SetReadOnlyEx
    {
        public static void SetProperty<T>(this T instance, string propertyName, object value) where T : class
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            if (string.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentException(nameof(propertyName));

            int i = propertyName.LastIndexOf(".");
            if (i >= 0)
                propertyName = propertyName.Remove(0, i + 1);

            var type = typeof(T).GetTypeInfo();
            var propertyInfo = type.GetDeclaredProperty(propertyName);
            if (propertyInfo == null)
                throw new Exception($"Property not found: '{propertyName}'.");
            try
            {
                if (propertyInfo.CanWrite)
                    propertyInfo.SetValue(instance, value);
                else
                {
                    var fieldName = "<" + propertyName + ">";
                    type.DeclaredFields
                        .Where(f => f.Name.StartsWith(fieldName))
                        .Single()
                        .SetValue(instance, value);
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Could not set property '{propertyName}'.", e);
            }
        }
    }

    public class Class1
    {
        public int RequestId { get; } = -2;
    }

    public class SetReadOnlyExTest
    {
        [Fact]
        public void Test1()
        {
            var instance = new Class1();
            instance.SetProperty(nameof(Class1.RequestId), 123);
            Assert.Equal(123, instance.RequestId);
        }
    }
  



}
