/* Copyright (C) 2019 Interactive Brokers LLC. All rights reserved. This code is subject to the terms
 * and conditions of the IB API Non-Commercial License or the IB API Commercial License, as applicable. */

#pragma warning disable CA1012, CA1307, CA1309, CA1031, CA1310, CA1305

namespace InterReact;

/**
* @class MarginCondition
* @brief This class represents a condition requiring the margin cushion reaching a given percent to be fulfilled.
* Orders can be activated or canceled if a set of given conditions is met. A MarginCondition is met whenever the margin penetrates the given percent.
*/
public sealed class MarginCondition : OperatorCondition
{
    const string header = "the margin cushion percent";

    protected override string Value
    {
        get
        {
            return Percent.ToString();
        }
        set
        {
            Percent = int.Parse(value);
        }
    }

    public override string ToString()
    {
        return header + base.ToString();
    }

    /**
    * @brief Margin percent to trigger condition.
    */
    public int Percent { get; set; }

    protected override bool TryParse(string cond)
    {
        if (!cond.StartsWith(header))
            return false;

        cond = cond.Replace(header, "");

        return base.TryParse(cond);
    }
}

#pragma warning restore CA1012, CA1307, CA1309, CA1031, CA1310, CA1305