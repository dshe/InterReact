using System;
using System.Globalization;
using System.Linq;

namespace InterReact
{
    /** 
     *  @brief Used with conditional orders to cancel or submit order based on price of an instrument. 
     */
    public class PriceCondition : ContractCondition
    {
        protected override string Value
        {
            get => Price.ToString();
            set => Price = double.Parse(value, NumberFormatInfo.InvariantInfo);
        }
        public override string ToString()
        {
            var name = Enum.GetName(typeof(TriggerMethod), TriggerMethod);
            return name + " " + base.ToString();
        }
        public override bool Equals(object obj)
        {
            return (obj is PriceCondition other)
                && base.Equals(obj)
                && TriggerMethod == other.TriggerMethod;
        }

        public override int GetHashCode() => base.GetHashCode() + TriggerMethod.GetHashCode();
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
            var fName = Enum.GetNames(typeof(TriggerMethod))
                .Where(n => cond.StartsWith(n)).OrderByDescending(n => n.Length).FirstOrDefault();

            if (fName == null)
                return false;

            try
            {
                TriggerMethod = (TriggerMethod)Enum.Parse(typeof(TriggerMethod), fName);
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
