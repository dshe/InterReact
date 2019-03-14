using System;
using System.Globalization;
using System.Linq;
using InterReact.Core;
using InterReact.Enums;

namespace InterReact.Messages.Conditions
{
/** 
 *  @brief Used with conditional orders to cancel or submit order based on price of an instrument. 
 */
    public class PriceCondition : ContractCondition
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
            var name = Enum.GetName(typeof(TriggerMethod), TriggerMethod);
            return name + " " + base.ToString();
        }

        public override bool Equals(object obj)
        {
            var other = obj as PriceCondition;
            if (other == null)
                return false;
            return base.Equals(obj) && this.TriggerMethod == other.TriggerMethod;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() + TriggerMethod.GetHashCode();
        }

        public double Price { get; set; }
        public TriggerMethod TriggerMethod { get; set; }

        internal override void Deserialize(ResponseReader reader)
        {
            base.Deserialize(reader);
            TriggerMethod = reader.Read<TriggerMethod>();
        }

        internal override void Serialize(RequestMessage message)
        {
            base.Serialize(message);
            message.Write(TriggerMethod);
        }

        protected override bool TryParse(string cond)
        {
            var fName = Enum.GetNames(typeof(TriggerMethod))
                .Where(n => cond.StartsWith(n)).OrderByDescending(n => n.Length).FirstOrDefault();

            if (fName == null)
                return false;

            try
            {
                TriggerMethod  = (TriggerMethod) Enum.Parse(typeof(TriggerMethod), fName);
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
