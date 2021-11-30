/* Copyright (C) 2019 Interactive Brokers LLC. All rights reserved. This code is subject to the terms
 * and conditions of the IB API Non-Commercial License or the IB API Commercial License, as applicable. */

#pragma warning disable CA1012, CA1307, CA1309, CA1031, CA1310, CA1305

namespace InterReact;

/**
* @brief Used with conditional orders to submit or cancel an order based on a specified volume change in a security. 
*/
public sealed class VolumeCondition : ContractCondition
{
    protected override string Value
    {
        get
        {
            return Volume.ToString();
        }
        set
        {
            Volume = long.Parse(value);
        }
    }

    public long Volume { get; set; }
}

#pragma warning restore CA1012, CA1307, CA1309, CA1031, CA1310, CA1305