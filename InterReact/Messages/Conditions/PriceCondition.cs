/* Copyright (C) 2019 Interactive Brokers LLC. All rights reserved. This code is subject to the terms
 * and conditions of the IB API Non-Commercial License or the IB API Commercial License, as applicable. */

using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace InterReact
{
    public static class CTriggerMethod
    {
        public static readonly string[] friendlyNames = new string[] { "default", "double bid/ask", "last", "double last", "bid/ask", "", "", "last of bid/ask", "mid-point" };


        public static string ToFriendlyString(this TriggerMethod th)
        {
            return friendlyNames[(int)th];
        }

        public static TriggerMethod FromFriendlyString(string friendlyName)
        {
            return (TriggerMethod)Array.IndexOf(friendlyNames, friendlyName);
        }
    }

/** 
 *  @brief Used with conditional orders to cancel or submit order based on price of an instrument. 
 */

    public sealed class PriceCondition : ContractCondition
    {
        protected override string Value
        {
            get
            {
                return Price.ToString();
            }
            set
            {
                Price = double.Parse(value, NumberFormatInfo.InvariantInfo);
            }
        }

        public override string ToString()
        {
            return TriggerMethod.ToFriendlyString() + " " + base.ToString();
        }

        public override bool Equals(object? obj)
        {
            var other = obj as PriceCondition;

            if (other == null)
                return false;

            return base.Equals(obj)
                && TriggerMethod == other.TriggerMethod;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() + TriggerMethod.GetHashCode();
        }

        public double Price { get; set; }
        public TriggerMethod TriggerMethod { get; set; }

        internal override void Deserialize(ResponseReader c)
        {
            base.Deserialize(c);
            TriggerMethod = c.ReadEnum<TriggerMethod>();
        }

        internal override void Serialize(RequestMessage message)
        {
            base.Serialize(message);
            message.Write(TriggerMethod);
        }

        protected override bool TryParse(string cond)
        {
            var fName = CTriggerMethod.friendlyNames.Where(n => cond.StartsWith(n)).OrderByDescending(n => n.Length).FirstOrDefault();

            if (fName == null)
                return false;

            try
            {
                TriggerMethod = CTriggerMethod.FromFriendlyString(fName);
                cond = cond.Substring(cond.IndexOf(fName) + fName.Length + 1);

                return base.TryParse(cond);
            }
            catch
            {
                return false;
            }
        }
    }
}
