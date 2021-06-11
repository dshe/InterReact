﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.UnitTests.Experimental
{
    public interface T1
    {


    }
    public class T2 : T1
    {

    }
    public class T3 : T1
    {
        public string Message;
        public T3(Alert alert)
        {
            Message = alert.Message;
        }

    }




    public class TestClass : BaseUnitTest
    {
        public TestClass(ITestOutputHelper output) : base(output) { }


        // return an instance of the subclass of T which is constructed with the inputParameter.
        private static T? GetSubclassOf<T>(object inputParameter)
        {
            Type[] inputParameterTypes = new[] { inputParameter.GetType() };

            ConstructorInfo? ctor = typeof(T).Assembly.GetTypes()
                //.Where(t => t.IsSubclassOf(typeof(T)))
                //.Where(t => t.IsAssignableFrom(typeof(T)))
                .Where(t => t.IsAssignableTo(typeof(T)))
                .Where(t => t.IsPublic && !t.IsAbstract)
                .Select(t => t.GetConstructor(inputParameterTypes))
                .Where(c => c != null)
                .Cast<ConstructorInfo>()
                .SingleOrDefault();

            if (ctor == default)
                return default;

            object instance = ctor.Invoke(new object[] { inputParameter });
            
            return (T)instance;
        }

        [Fact]
        public void Test1()
        {
            //var xxx = GetSubclassOf<ITick>(new Alert(1,1,"message"));
            //var typ = xxx.GetType();

            //var xx1 = typeof(T2).IsSubclassOf(typeof(T1));
            //var xx2 = typeof(T2).IsAssignableFrom(typeof(T1));
            //var xx3 = typeof(T2).IsAssignableTo(typeof(T1));
            ;
        }

    }
}
