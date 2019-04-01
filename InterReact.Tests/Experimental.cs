using System;
using System.Collections.Generic;
using System.Text;

namespace InterReact.Tests
{
    public class Class1
    {

        public void Test()
        {
            var list = new List<string>();
            var array = new string[] { "a" };
            EFcn(array);

        }


        // Array, []: does not support list
        // IList: supports List, array

        public void PFcn(IList<string> xxx)
        {
        }


        public void Fcn(IList<string> xxx)
        {

        }

        public void EFcn(IEnumerable<string> xxx)
        {

        }


    }
}
