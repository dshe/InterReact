/* Copyright (C) 2019 Interactive Brokers LLC. All rights reserved. This code is subject to the terms
 * and conditions of the IB API Non-Commercial License or the IB API Commercial License, as applicable. */

using System.Globalization;

namespace InterReact;

/**
* @brief Used with conditional orders to place or submit an order based on a percentage change of an instrument to the last close price.
*/
public sealed class PercentChangeCondition : ContractCondition
{

    protected override string Value
    {
        get
        {
#pragma warning disable CA1305 // Specify IFormatProvider
            return ChangePercent.ToString();
#pragma warning restore CA1305 // Specify IFormatProvider
        }
        set
        {
            ChangePercent = double.Parse(value, NumberFormatInfo.InvariantInfo);
        }
    }

    public double ChangePercent { get; set; }
}
